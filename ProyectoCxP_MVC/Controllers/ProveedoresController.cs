using Microsoft.AspNetCore.Mvc;
using ProyectoCxP_MVC.Models;
using System.Data.SqlClient;
using System.Data;

namespace ProyectoCxP_MVC.Controllers
{
    public class ProveedoresController : Controller
    {
        private readonly IConfiguration _configuration;

        public ProveedoresController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<Proveedor> lista = new List<Proveedor>();

            var connString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(connString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Proveedores", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Proveedor
                    {
                        IdProveedor = (int)dr["IdProveedor"],
                        Nombre = dr["Nombre"].ToString(),
                        TipoPersona = dr["TipoPersona"].ToString(),
                        CedulaRNC = dr["CedulaRNC"].ToString(),
                        Balance = Convert.ToDecimal(dr["Balance"]),
                        Estado = dr["Estado"].ToString()
                    });
                }
            }

            return View(lista);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Proveedor model)
        {
            if (model.Balance < 0)
                ModelState.AddModelError("Balance", "El balance no puede ser menor a cero.");

            if (model.TipoPersona == "Física" && !EsCedulaValida(model.CedulaRNC))
                ModelState.AddModelError("CedulaRNC", "La cédula no es válida.");

            if (model.TipoPersona == "Jurídica" && !EsRncValido(model.CedulaRNC))
                ModelState.AddModelError("CedulaRNC", "El RNC no es válido.");

            if (!ModelState.IsValid)
                return View(model);

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Proveedores (Nombre, TipoPersona, CedulaRNC, Balance, Estado) VALUES (@Nombre, @TipoPersona, @CedulaRNC, @Balance, @Estado)", con);
                cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                cmd.Parameters.AddWithValue("@TipoPersona", model.TipoPersona);
                cmd.Parameters.AddWithValue("@CedulaRNC", model.CedulaRNC);
                cmd.Parameters.AddWithValue("@Balance", model.Balance);
                cmd.Parameters.AddWithValue("@Estado", model.Estado);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Proveedor model = new Proveedor();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Proveedores WHERE IdProveedor = @Id", con);
                cmd.Parameters.AddWithValue("@Id", id);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    model.IdProveedor = (int)dr["IdProveedor"];
                    model.Nombre = dr["Nombre"].ToString();
                    model.TipoPersona = dr["TipoPersona"].ToString();
                    model.CedulaRNC = dr["CedulaRNC"].ToString();
                    model.Balance = Convert.ToDecimal(dr["Balance"]);
                    model.Estado = dr["Estado"].ToString();
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(Proveedor model)
        {
            if (model.Balance < 0)
                ModelState.AddModelError("Balance", "El balance no puede ser menor a cero.");

            if (model.TipoPersona == "Física" && !EsCedulaValida(model.CedulaRNC))
                ModelState.AddModelError("CedulaRNC", "La cédula no es válida.");

            if (model.TipoPersona == "Jurídica" && !EsRncValido(model.CedulaRNC))
                ModelState.AddModelError("CedulaRNC", "El RNC no es válido.");

            if (!ModelState.IsValid)
                return View(model);

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Proveedores SET Nombre=@Nombre, TipoPersona=@TipoPersona, CedulaRNC=@CedulaRNC, Balance=@Balance, Estado=@Estado WHERE IdProveedor=@IdProveedor", con);
                cmd.Parameters.AddWithValue("@IdProveedor", model.IdProveedor);
                cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                cmd.Parameters.AddWithValue("@TipoPersona", model.TipoPersona);
                cmd.Parameters.AddWithValue("@CedulaRNC", model.CedulaRNC);
                cmd.Parameters.AddWithValue("@Balance", model.Balance);
                cmd.Parameters.AddWithValue("@Estado", model.Estado);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Proveedores WHERE IdProveedor = @Id", con);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

  

        private bool EsCedulaValida(string cedula)
        {
            cedula = cedula.Replace("-", "").Trim();
            if (cedula.Length != 11 || !cedula.All(char.IsDigit)) return false;

            int total = 0;
            int[] multiplicadores = { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 };

            for (int i = 0; i < 11; i++)
            {
                int producto = int.Parse(cedula[i].ToString()) * multiplicadores[i];
                total += producto < 10 ? producto : (producto / 10) + (producto % 10);
            }

            return total % 10 == 0;
        }

        private bool EsRncValido(string rnc)
        {
            rnc = rnc.Replace("-", "").Trim();
            if (rnc.Length != 9 || !"145".Contains(rnc[0])) return false;

            int total = 0;
            int[] multiplicadores = { 7, 9, 8, 6, 5, 4, 3, 2 };

            for (int i = 0; i < 8; i++)
            {
                total += int.Parse(rnc[i].ToString()) * multiplicadores[i];
            }

            int digito = int.Parse(rnc[8].ToString());
            int resto = total % 11;
            int resultado = 11 - resto;

            return (resto == 0 && digito == 1) ||
                   (resto == 1 && digito == 1) ||
                   (resultado == digito);
        }
    }
}
