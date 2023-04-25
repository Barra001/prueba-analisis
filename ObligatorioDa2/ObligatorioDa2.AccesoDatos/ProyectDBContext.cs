
using ObligatorioDa2.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace ObligatorioDa2.AccesoDatos
{
   public class ProyectDbContext : DbContext
    {
        public ProyectDbContext()  { }
        public ProyectDbContext(DbContextOptions options) : base(options)
        {

        }



        public DbSet<Farmacia> FarmaciaEntities { get; set; }
        public DbSet<SolicitudDeReposicion> SolicitudDeReposicionEntities { get; set; }
        public DbSet<Producto> ProductoEntities { get; set; }
        public DbSet<Medicamento> MedicamentoEntities { get; set; }
        public DbSet<Usuario> UsuarioEntities { get; set; }
        public DbSet<Trabajador> TrabajadorEntities { get; set; }
        public DbSet<Empleado> EmpleadoEntities { get; set; }
        public DbSet<Administrador> AministradorEntities { get; set; }
        public DbSet<Dueño> DueñoEntities { get; set; }
        public DbSet<Compra> CompraEntities { get; set; }
        public DbSet<Invitacion> InvitacionEntities { get; set; }
        public DbSet<CantidadProductos> CantidadProductosEntities { get; set; }
        public DbSet<Sesion>  SesionEntities { get; set; }
        public DbSet<CantidadProductosCompra> CantidadProductosCompraEntities { get; set; }
    }
}
