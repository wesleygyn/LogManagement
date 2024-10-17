using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace LogManagement.Models
{
    public class Empresa
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo CNPJ é obrigatório.")]
        [StringLength(19, ErrorMessage = "O campo CNPJ não pode ter mais de 19 caracteres.")]
        [DisplayName("CNPJ")]
        public string cnpj { get; set; }

        [StringLength(30, ErrorMessage = "O campo Porte não pode ter mais de 30 caracteres.")]
        [DisplayName("Porte")]
        public string? porte { get; set; }

        [StringLength(15, ErrorMessage = "O campo Situação não pode ter mais de 15 caracteres.")]
        [Required(ErrorMessage = "O campo Situação é obrigatório.")]
        [DisplayName("Situação")]
        public string situacao { get; set; }

        [Required(ErrorMessage = "O campo Razão Social é obrigatório.")]
        [StringLength(60, ErrorMessage = "O campo Razão Social não pode ter mais de 60 caracteres.")]
        [DisplayName("Razão Social")]
        public string nome { get; set; }

        [DisplayName("Fantasia")]
        [StringLength(60, ErrorMessage = "O campo Fantasia não pode ter mais de 60 caracteres.")]
        public string? fantasia { get; set; }

        [Required(ErrorMessage = "O campo Telefone é obrigatório.")]
        [Phone(ErrorMessage = "Informe um Telefone válido.")]
        [StringLength(16, MinimumLength = 10, ErrorMessage = "O campo Telefone não pode ter menos de 10 e mais de 16 caracteres.")]
        [DisplayName("Telefone")]
        public string telefone { get; set; }

        [DisplayName("E-mail")]
        [StringLength(50, ErrorMessage = "O campo e-mail não pode ter mais de 50 caracteres.")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Digite o CEP")]
        [DisplayName("CEP")]
        [StringLength(12)]
        public string cep { get; set; }

        [Required(ErrorMessage = "Digite o logradouro")]
        [StringLength(50, ErrorMessage = "O campo logradouro não pode ter mais de 50 caracteres.")]
        [DisplayName("Logradouro")]
        public string logradouro { get; set; }

        [DisplayName("Complemento")]
        [StringLength(40, ErrorMessage = "O campo complemento não pode ter mais de 40 caracteres.")]
        public string? complemento { get; set; }

        [DisplayName("Numero")]
        [Required(ErrorMessage = "O campo Número é obrigatório.")]
        [MaxLength(10, ErrorMessage = "O campo Numero não pode ter mais de 10 caracteres.")]
        public string numero { get; set; }

        [Required(ErrorMessage = "O campo Bairro é obrigatório.")]
        [StringLength(30, ErrorMessage = "O campo bairro não pode ter mais de 30 caracteres.")]
        [DisplayName("Bairro")]
        public string bairro { get; set; }

        [Required(ErrorMessage = "O campo Município é obrigatório.")]
        [StringLength(30, ErrorMessage = "O campo municipio não pode ter mais de 30 caracteres.")]
        [DisplayName("Municipio")]
        public string municipio { get; set; }

        [Required(ErrorMessage = "O campo UF é obrigatório.")]
        [StringLength(2, ErrorMessage = "O campo UF não pode ter mais de 2 caracteres.")]
        [DisplayName("UF")]
        public string uf { get; set; }

        [DisplayName("CNAE")]
        [StringLength(15, ErrorMessage = "O campo CNAE não pode ter mais de 15 caracteres.")]
        public string? cnae { get; set; }

        [StringLength(100, ErrorMessage = "O campo ATIVIDADE não pode ter mais de 100 caracteres.")]
        [DisplayName("ATIVIDADE")]
        public string? atividade { get; set; }
    }
}
