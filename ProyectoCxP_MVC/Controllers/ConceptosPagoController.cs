using Microsoft.AspNetCore.Mvc;
using ProyectoCxP_MVC.Models;
using System.Data.SqlClient;

namespace ProyectoCxP_MVC.Controllers
{
    public class ConceptosPagoController : Controller
    {
        private readonly IConfiguration _config;

        public ConceptosPagoController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            var lista = new List<ConceptoPago>();
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM ConceptosPago", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new ConceptoPago
                {
                    IdConcepto = (int)reader["IdConcepto"],
                    Descripcion = reader["Descripcion"].ToString(),
                    Estado = reader["Estado"].ToString()
                });
            }
            return View(lista);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(ConceptoPago model)
        {
            if (!ModelState.IsValid) return View(model);

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = new SqlCommand("INSERT INTO ConceptosPago (Descripcion, Estado) VALUES (@d, @e)", conn);
            cmd.Parameters.AddWithValue("@d", model.Descripcion);
            cmd.Parameters.AddWithValue("@e", model.Estado);
            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            ConceptoPago? concepto = null;
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM ConceptosPago WHERE IdConcepto = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                concepto = new ConceptoPago
                {
                    IdConcepto = (int)reader["IdConcepto"],
                    Descripcion = reader["Descripcion"].ToString(),
                    Estado = reader["Estado"].ToString()
                };
            }
            return View(concepto);
        }

        [HttpPost]
        public IActionResult Edit(ConceptoPago model)
        {
            if (!ModelState.IsValid) return View(model);

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = new SqlCommand("UPDATE ConceptosPago SET Descripcion=@d, Estado=@e WHERE IdConcepto=@id", conn);
            cmd.Parameters.AddWithValue("@d", model.Descripcion);
            cmd.Parameters.AddWithValue("@e", model.Estado);
            cmd.Parameters.AddWithValue("@id", model.IdConcepto);
            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = new SqlCommand("DELETE FROM ConceptosPago WHERE IdConcepto = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }
    }
}
