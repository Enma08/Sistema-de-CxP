using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCxP_MVC.Data;
using ProyectoCxP_MVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ProyectoCxP_MVC.Controllers
{
    public class AsientosContablesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string apiKey = "ak_live_c3e815ea05a0954c35dbe4db12cccb71d9d951a930accfd8";
        private readonly string baseUrl = "http://3.80.223.142:3001/api/";

        public AsientosContablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<List<Cuenta>> ObtenerCatalogoCuentasAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync($"{baseUrl}public/entradas-cuentas");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

          
            var catalogo = JsonSerializer.Deserialize<CatalogoCuentasResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return catalogo?.Data ?? new List<Cuenta>();
        }

        public IActionResult Index(DateTime? desde, DateTime? hasta)
        {
            ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
            ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");

            if (!desde.HasValue || !hasta.HasValue)
            {
                ViewBag.Mensaje = "Selecciona un rango de fechas para continuar.";
                return View(new List<AsientoResumen>());
            }

            var resumen = (from doc in _context.DocumentosPorPagar
                           join prov in _context.Proveedores on doc.IdProveedor equals prov.IdProveedor
                           where doc.FechaDocumento.Date >= desde.Value.Date &&
                                 doc.FechaDocumento.Date <= hasta.Value.Date &&
                                 doc.Estado != "Pagado" 
                           group new { doc, prov } by new { doc.IdProveedor, prov.Nombre } into g
                           select new AsientoResumen
                           {
                               NombreProveedor = g.Key.Nombre,
                               NumeroDocumento = g.OrderByDescending(x => x.doc.IdDocumento).First().doc.NumeroDocumento,
                               NumeroFactura = g.OrderByDescending(x => x.doc.IdDocumento).First().doc.NumeroFactura,
                               FechaDocumento = g.Max(x => x.doc.FechaDocumento),
                               Monto = g.Sum(x => x.doc.Monto),
                               Estado = g.First().doc.Estado
                           }).ToList();


            return View(resumen);
        }

        [HttpPost]
        public async Task<IActionResult> EnviarAContabilidad(DateTime desde, DateTime hasta)
        {
            int cuentaContablePrueba = 82;

            int auxiliarPrueba = 6;

         
            var documentosPendientes = _context.DocumentosPorPagar
                .Where(d => d.FechaDocumento.Date >= desde.Date &&
                            d.FechaDocumento.Date <= hasta.Date &&
                            d.Estado != "Pagado")
                .ToList();

            if (!documentosPendientes.Any())
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, mensaje = "No hay documentos pendientes para enviar." });

                TempData["Error"] = "No hay documentos pendientes para enviar en ese rango.";
                return RedirectToAction(nameof(Index), new { desde, hasta });
            }

      
            var asientoUnico = new
            {
                descripcion = $"Asiento contable {desde:yyyy-MM-dd} a {hasta:yyyy-MM-dd}",
                auxiliar_Id = auxiliarPrueba,
                cuenta_Id = cuentaContablePrueba,
                tipoMovimiento = "CR",
                fechaAsiento = DateTime.Now.ToString("yyyy-MM-dd"),
                montoAsiento = documentosPendientes.Sum(d => d.Monto),
                idsDocumentos = documentosPendientes.Select(d => d.IdDocumento).ToList()
            };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = JsonSerializer.Serialize(asientoUnico);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}public/entradas-contables", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, mensaje = $"Error: {errorContent}" });

                TempData["Error"] = $"Error al enviar asiento: {response.StatusCode} - {errorContent}";
                return RedirectToAction(nameof(Index), new { desde, hasta });
            }


            var documentosActualizar = _context.DocumentosPorPagar
                .Where(d => asientoUnico.idsDocumentos.Contains(d.IdDocumento))
                .ToList();

            foreach (var doc in documentosActualizar)
                doc.Estado = "Pagado";

            _context.SaveChanges();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, mensaje = "Enviado", ids = asientoUnico.idsDocumentos });

            TempData["Success"] = "Asiento enviado a contabilidad y documentos actualizados.";
            return RedirectToAction(nameof(Index), new { desde, hasta });
        }


        private int ObtenerSiguienteCuentaContable()
        {
            var contador = _context.Contadores.FirstOrDefault(c => c.Nombre == "CuentaContable");
            if (contador == null)
            {
                contador = new Contador { Nombre = "CuentaContable", ValorActual = 1 };
                _context.Contadores.Add(contador);
            }
            else
            {
                contador.ValorActual++;
            }
            _context.SaveChanges();
            return contador.ValorActual;
        }


    }
}