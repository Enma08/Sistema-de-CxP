using Microsoft.AspNetCore.Mvc;

namespace ProyectoCxP_MVC.Models
{
    public class Cuenta
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public int? Auxiliar_Id { get; set; }
    }

    public class CatalogoCuentasResponse
    {
        public List<Cuenta> Data { get; set; }
    }
}
