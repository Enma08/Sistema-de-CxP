using System.ComponentModel.DataAnnotations;

namespace ProyectoCxP_MVC.Models
{
    public class Proveedor
    {
        [Key]
        public int IdProveedor { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string TipoPersona { get; set; } = "Física";

        [Required]
        [RegularExpression(@"^[0-9]{9,11}$", ErrorMessage = "Debe tener 9 dígitos si es RNC o 11 si es cédula.")]
        public string CedulaRNC { get; set; } = string.Empty;

        public decimal Balance { get; set; } = 0;

        [Required]
        public string Estado { get; set; } = "Activo";
    }
}
