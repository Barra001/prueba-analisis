using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;

namespace ObligatorioDa2.AccesoDatos.Repositorys
{
    public class SolicitudDeReposicionRepository : ISolicitudDeReposicionRepository
    {

        private readonly DbSet<SolicitudDeReposicion> solicitudDeReposicionEntities;
        
        private readonly DbContext context;

        public SolicitudDeReposicionRepository(DbContext context)
        {
            solicitudDeReposicionEntities = context.Set<SolicitudDeReposicion>();
            
            this.context = context;
        }

        public bool Exists(int key)
        {

            return solicitudDeReposicionEntities.Any(unaInvitacion => unaInvitacion.Id == key);

        }

        public IEnumerable<SolicitudDeReposicion> GetAll()
        {

            List<SolicitudDeReposicion> solicitudes = solicitudDeReposicionEntities
                .Include(sol => sol.Productos).ThenInclude(producto => producto.Producto)
                .ToList();
            return solicitudes;

        }
        public IEnumerable<SolicitudDeReposicion> GetSolicitudesDe(Empleado unEmpleado)
        {

            List<SolicitudDeReposicion> solicitudes = solicitudDeReposicionEntities
                .Include(sol => sol.Productos).ThenInclude(producto => producto.Producto)
                .Where(sol => sol.Solicitante == unEmpleado.Id)
                .ToList();
            return solicitudes;

        }

        

        public SolicitudDeReposicion GetById(int id)
        {
            if (!Exists(id))
            {
                throw new NotFoundException($"SolicitudDeReposicion {id} no encontrada");
            }
            return solicitudDeReposicionEntities
                .Include(sol => sol.Productos).ThenInclude(producto => producto.Producto)
     
                .FirstOrDefault(unaSolicitudDeReposicion => unaSolicitudDeReposicion.Id == id);

        }

        public void Insertar(SolicitudDeReposicion value)
        {
            solicitudDeReposicionEntities.Attach(value);
            context.SaveChanges();
        }

        public void InsertarSolicitud(SolicitudDeReposicion unaSolicitudDeReposicion, Farmacia unaFarmacia)
        {

            if (Exists(unaSolicitudDeReposicion.Id))
            {
                throw new AccesoDatosException($"Solicitud de reposicion ya existe");
            }

            Insertar(unaSolicitudDeReposicion);
            unaFarmacia.SolicitudesDeReposicion.Add(unaSolicitudDeReposicion);
            context.Entry(unaFarmacia).State = EntityState.Modified;
            context.SaveChanges();
        }



        public void Update(SolicitudDeReposicion unaSolicitudDeReposicion)
        {

            if (!Exists(unaSolicitudDeReposicion.Id))
            {
                throw new NotFoundException($"Solicitud de reposición {unaSolicitudDeReposicion.Id} no existe");
            }

            context.Entry(unaSolicitudDeReposicion).State = EntityState.Modified;
            context.SaveChanges();

        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
