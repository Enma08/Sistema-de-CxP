using System;

namespace ProyectoCxP_MVC.Models
{
    public class AsientoResumen
    {
        public string NumeroDocumento { get; set; } = string.Empty;
        public string NumeroFactura { get; set; } = string.Empty;
        public DateTime FechaDocumento { get; set; }
        public decimal Monto { get; set; }
        public string NombreProveedor { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }

}
