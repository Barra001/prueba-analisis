using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ObligatorioDa2.AccesoDatos;
using ObligatorioDa2.AccesoDatos.Repositorys;
using ObligatorioDa2.IAccesoDatos;
using ObligatorioDa2.ILogicaDeNegocio;
using ObligatorioDa2.LogicaDeNegocio;
using ObligatorioDa2.Reflection;

namespace Fabrica
{
    public class FabricaServicios
    {
        private readonly IServiceCollection coleccionServicios;
        private readonly IConfiguration configuration;

        public FabricaServicios(IServiceCollection coleccionServicios, IConfiguration configuration)
        {
            this.coleccionServicios = coleccionServicios;
            this.configuration = configuration;
        }

        public void AgregarServicios()
        {
            coleccionServicios.AddScoped<IFarmaciaServicio, FarmaciaServicio>();
            coleccionServicios.AddScoped<IFarmaciaRepository, FarmaciaRepository>();

            coleccionServicios.AddScoped<IInvitacionServicio, InvitacionServicio>();
            coleccionServicios.AddScoped<IInvitacionRepository, InvitacionRepository>();

            coleccionServicios.AddScoped<IUsuarioServicio, UsuarioServicio>();
            coleccionServicios.AddScoped<IUsuarioRepository, UsuarioRepository>();

            coleccionServicios.AddScoped<ITrabajadorServicio, TrabajadorServicio>();

           

            coleccionServicios.AddScoped<IProductoServicio, ProductoServicio>();
            coleccionServicios.AddScoped<IProductoRepository, ProductoRepository>();

            coleccionServicios.AddScoped<ISolicitudDeReposicionServicio, SolicitudDeReposicionServicio>();
            coleccionServicios.AddScoped<ISolicitudDeReposicionRepository, SolicitudDeReposicionRepository>();

            coleccionServicios.AddScoped<ISesionServicio, SesionServicio>();
            coleccionServicios.AddScoped<ISesionRepository, SesionRepository>();


            coleccionServicios.AddScoped<ICompraServicio, CompraServicio>();
            coleccionServicios.AddScoped<ICompraRepository, CompraRepository>();

            coleccionServicios.AddScoped<IImporterServicio, ImporterServicio>();

        }

        public void AgregarContexto()
        {
            coleccionServicios.AddDbContext<DbContext, ProyectDbContext>(
                o => o.UseSqlServer(configuration.GetConnectionString("ObligatorioDa2")));
        }

    }
}
