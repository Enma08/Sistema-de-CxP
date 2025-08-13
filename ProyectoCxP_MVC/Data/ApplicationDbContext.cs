using Microsoft.EntityFrameworkCore;
using ProyectoCxP_MVC.Models;
using System.Collections.Generic;

namespace ProyectoCxP_MVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AsientoContable> AsientosContables { get; set; }
        public DbSet<DocumentoPorPagar> DocumentosPorPagar { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }

        public DbSet<Contador> Contadores { get; set; }


    }
}
