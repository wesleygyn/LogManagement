using LogManagement.Data;
using LogManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JsonDiffPatchDotNet;
using Microsoft.AspNetCore.Identity;
using LogManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using cloudscribe.Pagination.Models;
using LogManagement.Data.Enuns;

namespace LogManagement.Controllers
{
    public class AuditoriaController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly ILogger<AuditoriaController> _logger;

        public AuditoriaController(ILogger<AuditoriaController> logger, ApplicationDbContext context, IUserService userService, UserManager<Usuario> userManager)
        {
            _logger = logger;
            _context = context;
            _userService = userService;
            _userManager = userManager;
        }

        // GET: Auditoria
        public async Task<IActionResult> Index(string type, string searchString, string usuario, string rotina, string chave, DateTime dtInicial, DateTime dtFinal, int pageNumber = 1, int pageSize = 20)
        {
            TempData["searchString"] = searchString;
            TempData["chave"] = chave;
            TempData["rotina"] = rotina;
            int ExcludeRecords = (pageSize * pageNumber) - pageSize;

            if (dtInicial == new DateTime() || dtFinal == new DateTime())
            {
                dtFinal = DateTime.Now;
                dtInicial = dtFinal.AddDays(-7);
            }

            dtFinal = dtFinal.AddDays(+1);

            var typeCreate = "valCreate";
            var typeUpdate = "valUpdate";
            var typeDelete = "valDelete";

            if (!string.IsNullOrEmpty(type))
            {
                var subType = type.Split(",");
                foreach (var sub in subType)
                {
                    switch (sub)
                    {
                        case "Inclusão":
                            typeCreate = "Create";
                            break;
                        case "Atualização":
                            typeUpdate = "Update";
                            break;
                        case "Exclusão":
                            typeDelete = "Delete";
                            break;
                    }
                }
            }
            else
            {
                typeCreate = "Create";
                typeUpdate = "Update";
                typeDelete = "Delete";
            }

            var auditType = _context.Audits.Where(x =>
                                    (x.TableName != "RefreshToken") &&
                                    (x.DateTime >= dtInicial && x.DateTime <= dtFinal) &&
                                    (x.Type == typeCreate ||
                                        x.Type == typeUpdate ||
                                        x.Type == typeDelete) &&
                                    (string.IsNullOrEmpty(usuario) || x.AuthenticatedUser == usuario) &&
                                    (string.IsNullOrEmpty(searchString) || x.OldValues.Contains(searchString) || x.NewValues.Contains(searchString)) &&
                                    (string.IsNullOrEmpty(chave) || x.PrimaryKey == chave) &&
                                    (string.IsNullOrEmpty(rotina) || x.ControllerName.Contains(rotina)))
                                    .Skip(ExcludeRecords)
                                    .Take(pageSize);

            var CatCount = _context.Audits.Count();

            var auditTypeTratado = AuditTratada(await auditType.ToListAsync()).Value.ToList();

            var result = new PagedResult<Audit>
            {
                Data = auditTypeTratado,
                TotalItems = CatCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var listaUsuarios = _userManager.Users;

            if (!string.IsNullOrEmpty(usuario))
            {
                var usuarioPesquisa = listaUsuarios.Where(x => x.Id == usuario).First();
                TempData["idUsuario"] = usuario;
                TempData["nomeUsuario"] = usuarioPesquisa.Nome;
            }

            ViewData["listaUsuarios"] = new SelectList(listaUsuarios, "Id", "Nome");

            return View(result);
        }

        private ActionResult<IEnumerable<Audit>> AuditTratada(List<Audit> audits)
        {
            var newlistAudits = new List<Audit>();
            foreach (var newAudits in audits)
            {
                if (newAudits.TableName != "RefreshToken")
                {
                    var listUsuario = _userManager.Users;

                    switch (newAudits.Type)
                    {
                        case "Create":
                            newAudits.Type = "Inclusão";
                            break;
                        case "Update":
                            newAudits.Type = "Atualização";
                            break;
                        case "Delete":
                            newAudits.Type = "Exclusão";
                            break;
                    }

                    if (newAudits.Type == "Atualização")
                    {
                        Audit listaudit = new Audit();

                        var diffObj = new JsonDiffPatch();

                        listaudit.OldValues = diffObj.Diff(newAudits.OldValues, newAudits.NewValues);

                        var responseOldValues = JsonConvert.DeserializeObject<dynamic>(listaudit.OldValues);

                        foreach (var item in responseOldValues)
                        {
                            Audit audit = new Audit();

                            audit.Id = newAudits.Id;
                            audit.DateTime = newAudits.DateTime;
                            audit.ControllerName = newAudits.ControllerName;
                            audit.Type = newAudits.Type;

                            audit.AffectedColumns = item.Name;

                            if (audit.AffectedColumns == "PasswordHash")
                            {
                                audit.OldValues = "**********"; ;
                                audit.NewValues = "**********"; ;
                            }
                            else
                            {
                                audit.OldValues = item.Value.First;
                                audit.NewValues = item.Value.Last;
                            }

                            audit.TableName = newAudits.TableName;

                            if (newAudits.AuthenticatedUser != null && newAudits.AuthenticatedUser.Length > 1)
                            {
                                var usuarioSelecionado = listUsuario.Where(x => x.Id == newAudits.AuthenticatedUser).First();
                                audit.AuthenticatedUser = usuarioSelecionado.Nome;
                            }
                            else
                            {
                                audit.AuthenticatedUser = newAudits.AuthenticatedUser;
                            }

                            audit.PrimaryKey = newAudits.PrimaryKey;

                            if (item.Name != "CreatedAt" && item.Name != "UpdatedAt")
                            {
                                newlistAudits.Add(audit);
                            }
                        }
                    }

                    if (newAudits.Type == "Inclusão")
                    {
                        Audit listaudit = new Audit();

                        listaudit.NewValues = JValue.Parse(newAudits.NewValues).ToString(Newtonsoft.Json.Formatting.Indented);
                        var responseOldValues = JsonConvert.DeserializeObject<dynamic>(listaudit.NewValues);

                        foreach (var item in responseOldValues)
                        {
                            Audit audit = new Audit();

                            audit.Id = newAudits.Id;
                            audit.DateTime = newAudits.DateTime;
                            audit.ControllerName = newAudits.ControllerName;
                            audit.Type = newAudits.Type;
                            audit.AffectedColumns = item.Name;

                            if (audit.AffectedColumns == "PasswordHash")
                            {
                                audit.OldValues = String.Empty;
                                audit.NewValues = "**********"; ;
                            }
                            else
                            {
                                audit.OldValues = String.Empty;
                                audit.NewValues = item.Value.Parent.First;
                            }

                            audit.TableName = newAudits.TableName;

                            if (newAudits.AuthenticatedUser != null && newAudits.AuthenticatedUser.Length > 1)
                            {
                                var usuarioSelecionado = listUsuario.Where(x => x.Id == newAudits.AuthenticatedUser).First();
                                audit.AuthenticatedUser = usuarioSelecionado.Nome;
                            }
                            else
                            {
                                audit.AuthenticatedUser = newAudits.AuthenticatedUser;
                            }

                            audit.PrimaryKey = newAudits.PrimaryKey;

                            if (item.Name != "CreatedAt" && item.Name != "UpdatedAt")
                            {
                                newlistAudits.Add(audit);
                            }
                        }
                    }

                    if (newAudits.Type == "Exclusão")
                    {
                        Audit listaudit = new Audit();

                        listaudit.OldValues = JValue.Parse(newAudits.OldValues).ToString(Newtonsoft.Json.Formatting.Indented);
                        var responseOldValues = JsonConvert.DeserializeObject<dynamic>(listaudit.OldValues);

                        foreach (var item in responseOldValues)
                        {
                            Audit audit = new Audit();

                            audit.Id = newAudits.Id;
                            audit.DateTime = newAudits.DateTime;
                            audit.ControllerName = newAudits.ControllerName;
                            audit.Type = newAudits.Type;

                            audit.AffectedColumns = item.Name;

                            if (audit.AffectedColumns == "PasswordHash")
                            {
                                audit.OldValues = "**********";
                                audit.NewValues = String.Empty;
                            }
                            else
                            {
                                audit.OldValues = item.Value.Parent.First;
                                audit.NewValues = String.Empty;
                            }

                            audit.TableName = newAudits.TableName;

                            if (newAudits.AuthenticatedUser != null && newAudits.AuthenticatedUser.Length > 1)
                            {
                                var usuarioSelecionado = listUsuario.Where(x => x.Id == newAudits.AuthenticatedUser).First();
                                audit.AuthenticatedUser = usuarioSelecionado.Nome;
                            }
                            else
                            {
                                audit.AuthenticatedUser = newAudits.AuthenticatedUser;
                            }

                            audit.PrimaryKey = newAudits.PrimaryKey;

                            if (item.Name != "CreatedAt" && item.Name != "UpdatedAt")
                            {
                                newlistAudits.Add(audit);
                            }
                        }
                    }
                }
            }

            return newlistAudits;
        }

        // GET: Auditoria/5
        public async Task<IActionResult> Details(Guid id)
        {
            var auditoria = await _context.Audits.FindAsync(id);

            if (auditoria == null)
            {
                return NotFound();
            }

            var audit = new Audit();
            {
                var diffObj = new JsonDiffPatch();

                audit.Id = auditoria.Id;
                audit.AuthenticatedUser = auditoria.AuthenticatedUser;
                audit.Type = auditoria.Type;
                audit.TableName = auditoria.TableName;
                audit.DateTime = auditoria.DateTime;

                if (auditoria.Type == "Update")
                {
                    audit.OldValues = diffObj.Diff(auditoria.OldValues, auditoria.NewValues);
                    audit.NewValues = diffObj.Diff(auditoria.NewValues, auditoria.OldValues);
                }
                else
                {
                    if (auditoria.OldValues != null && auditoria.OldValues.Length >= 1)
                    {
                        audit.OldValues = JValue.Parse(auditoria.OldValues).ToString(Newtonsoft.Json.Formatting.Indented);
                    }
                    else
                    {
                        audit.OldValues = auditoria.OldValues;
                    }

                    if (auditoria.NewValues != null && auditoria.NewValues.Length >= 1)
                    {
                        audit.NewValues = JValue.Parse(auditoria.NewValues).ToString(Newtonsoft.Json.Formatting.Indented);
                    }
                    else
                    {
                        audit.NewValues = auditoria.NewValues;
                    }
                }

                audit.AffectedColumns = auditoria.AffectedColumns;
                audit.PrimaryKey = auditoria.PrimaryKey;
                audit.ControllerName = auditoria.ControllerName;
            }

            return View(audit);
        }
    }
}