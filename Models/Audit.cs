using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using LogManagement.Data.Enuns;

namespace LogManagement.Models
{
    public class Audit
    {
        [DisplayName("Id")]
        public Guid Id { get; set; }
        [StringLength(100)]
        [DisplayName("Usuário")]
        public string AuthenticatedUser { get; set; }
        [StringLength(100)]
        [DisplayName("Operação")]
        public string Type { get; set; }
        [StringLength(100)]
        [DisplayName("Tabela")]
        public string TableName { get; set; }
        [DisplayName("Data/Hora")]
        public DateTime DateTime { get; set; }
        [DisplayName("Valor antigo")]
        public string OldValues { get; set; }
        [DisplayName("Valor novo")]
        public string NewValues { get; set; }
        [DisplayName("Campo")]
        public string AffectedColumns { get; set; }
        [DisplayName("Chave")]
        public string PrimaryKey { get; set; }
        [StringLength(100)]
        [DisplayName("Rotina")]
        public string? ControllerName { get; set; }
    }

    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry) => Entry = entry;

        public EntityEntry Entry { get; }
        public string AuthenticatedUser { get; set; }
        public string TableName { get; set; }
        public string PrimaryKey { get; set; } // Adicionado posteriormente para deixar o PrimaryKey sem o json
        //public Dictionary<string, object> KeyValues { get; } = new();
        public Dictionary<string, object> OldValues { get; } = new();
        public Dictionary<string, object> NewValues { get; } = new();
        public AuditType AuditType { get; set; }
        public List<string> ChangedColumns { get; } = new();
        public string? ControllerName { get; set; }

        public Audit ToAudit()
        {
            var audit = new Audit
            {
                Id = Guid.NewGuid(),
                AuthenticatedUser = AuthenticatedUser,
                Type = AuditType.ToString(),
                TableName = TableName,
                DateTime = DateTime.Now,
                PrimaryKey = PrimaryKey, // Adicionado posteriormente para deixar o PrimaryKey sem o json
                //PrimaryKey = JsonSerializer.Serialize(KeyValues),
                OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues),
                NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues),
                AffectedColumns = ChangedColumns.Count == 0 ? null : JsonSerializer.Serialize(ChangedColumns),
                ControllerName = ControllerName
            };

            return audit;
        }
    }
}
