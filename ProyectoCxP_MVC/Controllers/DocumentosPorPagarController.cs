using Microsoft.AspNetCore.Mvc;
using ProyectoCxP_MVC.Models;
using System.Data.SqlClient;

namespace ProyectoCxP_MVC.Controllers
{
    public class DocumentosPorPagarController : Controller
    {
        private readonly IConfiguration _config;

        public DocumentosPorPagarController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            var lista = new List<DocumentoPorPagar>();
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();

            var cmd = new SqlCommand(@"
        SELECT dp.*, p.Nombre AS NombreProveedor
        FROM DocumentosPorPagar dp
        INNER JOIN Proveedores p ON dp.IdProveedor = p.IdProveedor", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new DocumentoPorPagar
                {
                    IdDocumento = (int)reader["IdDocumento"],
                    NumeroDocumento = reader["NumeroDocumento"].ToString(),
                    NumeroFactura = reader["NumeroFactura"].ToString(),
                    FechaDocumento = (DateTime)reader["FechaDocumento"],
                    Monto = (decimal)reader["Monto"],
                    FechaRegistro = (DateTime)reader["FechaRegistro"],
                    IdProveedor = (int)reader["IdProveedor"],
                    Estado = reader["Estado"].ToString(),

                  
                    NombreProveedor = reader["NombreProveedor"].ToString()
                });
            }
            return View(lista);
        }


        public IActionResult Create()
        {
            ViewBag.Proveedores = ObtenerProveedores();
            return View();
        }

        [HttpPost]
        public IActionResult Create(DocumentoPorPagar model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Proveedores = ObtenerProveedores();
                return View(model);
            }

            var numeroDoc = GenerarNumeroDocumento();

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();

            // Insertar el documento
            var cmdInsert = new SqlCommand(@"
        INSERT INTO DocumentosPorPagar 
        (NumeroDocumento, NumeroFactura, FechaDocumento, Monto, FechaRegistro, IdProveedor, Estado)
        VALUES (@nd, @nf, @fd, @m, @fr, @idp, @e)", conn);
            cmdInsert.Parameters.AddWithValue("@nd", numeroDoc);
            cmdInsert.Parameters.AddWithValue("@nf", model.NumeroFactura);
            cmdInsert.Parameters.AddWithValue("@fd", model.FechaDocumento);
            cmdInsert.Parameters.AddWithValue("@m", model.Monto);
            cmdInsert.Parameters.AddWithValue("@fr", model.FechaRegistro);
            cmdInsert.Parameters.AddWithValue("@idp", model.IdProveedor);
            cmdInsert.Parameters.AddWithValue("@e", model.Estado);
            cmdInsert.ExecuteNonQuery();


            var cmdUpdateBalance = new SqlCommand(@"
        UPDATE Proveedores
        SET Balance = Balance + @monto
        WHERE IdProveedor = @idProveedor", conn);
            cmdUpdateBalance.Parameters.AddWithValue("@monto", model.Monto);
            cmdUpdateBalance.Parameters.AddWithValue("@idProveedor", model.IdProveedor);
            cmdUpdateBalance.ExecuteNonQuery();

            return RedirectToAction("Index");
        }


        public IActionResult Edit(int id)
        {
            DocumentoPorPagar? doc = null;
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM DocumentosPorPagar WHERE IdDocumento = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                doc = new DocumentoPorPagar
                {
                    IdDocumento = (int)reader["IdDocumento"],
                    NumeroDocumento = reader["NumeroDocumento"].ToString(),
                    NumeroFactura = reader["NumeroFactura"].ToString(),
                    FechaDocumento = (DateTime)reader["FechaDocumento"],
                    Monto = (decimal)reader["Monto"],
                    FechaRegistro = (DateTime)reader["FechaRegistro"],
                    IdProveedor = (int)reader["IdProveedor"],
                    Estado = reader["Estado"].ToString()
                };
            }

            ViewBag.Proveedores = ObtenerProveedores();
            return View(doc);
        }

        [HttpPost]
        public IActionResult Edit(DocumentoPorPagar model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Proveedores = ObtenerProveedores();
                return View(model);
            }

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();

            // Obtener monto anterior
            decimal montoAnterior = 0;
            var cmdGetOldMonto = new SqlCommand("SELECT Monto FROM DocumentosPorPagar WHERE IdDocumento = @id", conn);
            cmdGetOldMonto.Parameters.AddWithValue("@id", model.IdDocumento);
            var result = cmdGetOldMonto.ExecuteScalar();
            if (result != null)
                montoAnterior = Convert.ToDecimal(result);

            // Actualizar documento
            var cmdUpdateDoc = new SqlCommand(@"
        UPDATE DocumentosPorPagar 
        SET NumeroFactura = @nf, FechaDocumento = @fd, Monto = @m, 
            FechaRegistro = @fr, IdProveedor = @idp, Estado = @e
        WHERE IdDocumento = @id", conn);
            cmdUpdateDoc.Parameters.AddWithValue("@nf", model.NumeroFactura);
            cmdUpdateDoc.Parameters.AddWithValue("@fd", model.FechaDocumento);
            cmdUpdateDoc.Parameters.AddWithValue("@m", model.Monto);
            cmdUpdateDoc.Parameters.AddWithValue("@fr", model.FechaRegistro);
            cmdUpdateDoc.Parameters.AddWithValue("@idp", model.IdProveedor);
            cmdUpdateDoc.Parameters.AddWithValue("@e", model.Estado);
            cmdUpdateDoc.Parameters.AddWithValue("@id", model.IdDocumento);
            cmdUpdateDoc.ExecuteNonQuery();

            // Actualizar balance con la diferencia
            decimal diferencia = model.Monto - montoAnterior;
            var cmdUpdateBalance = new SqlCommand(@"
        UPDATE Proveedores
        SET Balance = Balance + @diferencia
        WHERE IdProveedor = @idProveedor", conn);
            cmdUpdateBalance.Parameters.AddWithValue("@diferencia", diferencia);
            cmdUpdateBalance.Parameters.AddWithValue("@idProveedor", model.IdProveedor);
            cmdUpdateBalance.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            DocumentoPorPagar? doc = null;
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM DocumentosPorPagar WHERE IdDocumento = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                doc = new DocumentoPorPagar
                {
                    IdDocumento = (int)reader["IdDocumento"],
                    NumeroDocumento = reader["NumeroDocumento"].ToString(),
                    NumeroFactura = reader["NumeroFactura"].ToString(),
                    FechaDocumento = (DateTime)reader["FechaDocumento"],
                    Monto = (decimal)reader["Monto"],
                    FechaRegistro = (DateTime)reader["FechaRegistro"],
                    IdProveedor = (int)reader["IdProveedor"],
                    Estado = reader["Estado"].ToString()
                };
            }

            return View(doc);
        }

        [HttpPost]
        public IActionResult Delete(DocumentoPorPagar model)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();

            // Obtener monto y proveedor
            decimal monto = 0;
            int idProveedor = 0;
            var cmdGet = new SqlCommand("SELECT Monto, IdProveedor FROM DocumentosPorPagar WHERE IdDocumento = @id", conn);
            cmdGet.Parameters.AddWithValue("@id", model.IdDocumento);
            using (var reader = cmdGet.ExecuteReader())
            {
                if (reader.Read())
                {
                    monto = (decimal)reader["Monto"];
                    idProveedor = (int)reader["IdProveedor"];
                }
            }

            // Restar monto al balance del proveedor
            var cmdUpdateBalance = new SqlCommand(@"
        UPDATE Proveedores
        SET Balance = Balance - @monto
        WHERE IdProveedor = @idProveedor", conn);
            cmdUpdateBalance.Parameters.AddWithValue("@monto", monto);
            cmdUpdateBalance.Parameters.AddWithValue("@idProveedor", idProveedor);
            cmdUpdateBalance.ExecuteNonQuery();

            // Borrar documento
            var cmdDelete = new SqlCommand("DELETE FROM DocumentosPorPagar WHERE IdDocumento = @id", conn);
            cmdDelete.Parameters.AddWithValue("@id", model.IdDocumento);
            cmdDelete.ExecuteNonQuery();

            return RedirectToAction("Index");
        }


        private string GenerarNumeroDocumento()
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = new SqlCommand("SELECT COUNT(*) FROM DocumentosPorPagar", conn);
            int count = (int)cmd.ExecuteScalar();
            return (count + 1).ToString("D3"); // 001, 002...
        }

        private List<Proveedor> ObtenerProveedores()
        {
            var lista = new List<Proveedor>();
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM Proveedores WHERE Estado = 'Activo'", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Proveedor
                {
                    IdProveedor = (int)reader["IdProveedor"],
                    Nombre = reader["Nombre"].ToString()
                });
            }
            return lista;
        }
    }
}