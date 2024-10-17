using LogManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using LogManagement.Data.Enuns;
using LogManagement.Services.Interfaces;

namespace LogManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        private readonly IUserService _userService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IUserService userService)
            : base(options)
        {
            _userService = userService;
        }

        public DbSet<Models.Audit> Audits { get; set; }
        public DbSet<Models.Empresa> Empresas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public virtual int SaveChanges(string controllerName = null)
        {
            OnBeforeSaveChanges(controllerName).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = base.SaveChanges();

            return result;
        }

        public virtual async Task<int> SaveChangesAsync(string controllerName = null)
        {
            await OnBeforeSaveChanges(controllerName);
            var result = await base.SaveChangesAsync();
            return result;
        }

        private async Task OnBeforeSaveChanges(string controllerName)
        {
            try
            {
                var utcNow = DateTime.UtcNow;
                ChangeTracker.DetectChanges();
                var auditEntries = new List<AuditEntry>();
                foreach (var entry in ChangeTracker.Entries())
                {
                    if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                        continue;
                    var auditEntry = new AuditEntry(entry);
                    auditEntry.TableName = entry.Entity.GetType().Name;
                    auditEntry.AuthenticatedUser = _userService.GetMyId();
                    auditEntries.Add(auditEntry);
                    foreach (var property in entry.Properties)
                    {
                        //var propertyName = property.Metadata.Name;
                        //if (property.Metadata.IsPrimaryKey())
                        //{
                        //    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        //    continue;
                        //}

                        var propertyName = property.Metadata.Name;
                        if (property.Metadata.IsPrimaryKey())
                        {
                            auditEntry.PrimaryKey = property.CurrentValue.ToString();
                            continue;
                        }

                        switch (entry.State)
                        {
                            case EntityState.Added:
                                auditEntry.AuditType = AuditType.Create;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                                auditEntry.ControllerName = controllerName;
                                break;

                            case EntityState.Deleted:
                                auditEntry.AuditType = AuditType.Delete;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.ControllerName = controllerName;
                                break;

                            case EntityState.Modified:
                                if (property.IsModified)
                                {
                                    auditEntry.ChangedColumns.Add(propertyName);
                                    auditEntry.AuditType = AuditType.Update;
                                    auditEntry.OldValues[propertyName] = entry.GetDatabaseValues().GetValue<object>(property.Metadata.Name);
                                    auditEntry.NewValues[propertyName] = property.CurrentValue;
                                    auditEntry.ControllerName = controllerName;

                                }
                                break;
                        }
                    }
                }
                foreach (var auditEntry in auditEntries)
                {
                    await Audits.AddAsync(auditEntry.ToAudit());
                }
            }
            catch (Exception ex)
            {
                //Serilog.Log.Error(ex, "Erro ao salvar auditoria");
            }
        }
    }
}