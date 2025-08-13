using System.ComponentModel.DataAnnotations;

namespace ProyectoCxP_MVC.Models
{
    public class AsientoContable
    {
        [Key]
        public int IdAsiento { get; set; }

        public string Descripcion { get; set; }

        public int IdTipoInventario { get; set; }

        public string CuentaContable { get; set; }

        [Required]
        public string TipoMovimiento { get; set; }
        

        public DateTime FechaAsiento { get; set; }

        public decimal MontoAsiento { get; set; }

        [Required]
        public string Estado { get; set; } 
    }


}
