using System.ComponentModel.DataAnnotations;

namespace ProyectoCxP_MVC.Models
{
	public class ConceptoPago
	{
		public int IdConcepto { get; set; }

		[Required]
		[StringLength(100)]
		public string Descripcion { get; set; } = string.Empty;

		[Required]
		public string Estado { get; set; } = "Activo";
	}
}
