using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCxP_MVC.Models
{
    public class DocumentoPorPagar
    {
        [Key]
        public int IdDocumento { get; set; }

        public string NumeroDocumento { get; set; } = string.Empty;

        [Required]
        public string NumeroFactura { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(DocumentoPorPagar), nameof(ValidarFechaNoFutura))]
        public DateTime FechaDocumento { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal Monto { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(DocumentoPorPagar), nameof(ValidarFechaNoFutura))]
        public DateTime FechaRegistro { get; set; }

        [Required]
        public int IdProveedor { get; set; }

        [NotMapped]
        public string NombreProveedor { get; set; } = string.Empty;

        [Required]
        public string Estado { get; set; } = "Pendiente";

        public static ValidationResult? ValidarFechaNoFutura(DateTime fecha, ValidationContext context)
        {
            return fecha > DateTime.Today
                ? new ValidationResult("La fecha no puede ser futura.")
                : ValidationResult.Success;
        }
    }
}
