using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;

namespace ObligatorioDa2.AccesoDatos.Repositorys
{
    public class InvitacionRepository : IInvitacionRepository
    {
        private readonly DbSet<Invitacion> invitacionEntities;
        private readonly DbContext context;

        public InvitacionRepository(DbContext context)
        {
            invitacionEntities = context.Set<Invitacion>();
            this.context = context;
        }
        public bool Exists(int key)
        {


            return invitacionEntities.Any(unaInvitacion => unaInvitacion.Id == key);

        }

        public IEnumerable<Invitacion> GetAll()
        {

            List<Invitacion> invitaciones = invitacionEntities.Include(x=>x.Farmacia)
                .ToList();
            return invitaciones;

        }



        public Invitacion GetById(int id)
        {

            if (!Exists(id))
            {
                throw new NotFoundException($"Invitación {id} no encontrada");
            }
            return invitacionEntities.AsNoTracking()
                .Include(invitacion => invitacion.Farmacia).ThenInclude(unaFermacia => unaFermacia.SolicitudesDeReposicion).ThenInclude(soli => soli.Productos).ThenInclude(cantProd => cantProd.Producto)
                
                .Include(invitacion => invitacion.Farmacia).ThenInclude(unaFermacia => unaFermacia.Productos)
                .Include(invitacion => invitacion.Farmacia).ThenInclude(unaFermacia => unaFermacia.Compras).ThenInclude(comp => comp.Productos).ThenInclude(cantProd => cantProd.Producto)
                .FirstOrDefault(unaInvitacion => unaInvitacion.Id == id);

        }

        public void Insertar(Invitacion unaInvitacion)
        {

            if (Exists(unaInvitacion.Id))
            {
                throw new AccesoDatosException($"Invitación ya existe");
            }

            invitacionEntities.Attach(unaInvitacion);
            context.SaveChanges();


        }

        public void Update(Invitacion unaInvitacion)
        {

            if (!Exists(unaInvitacion.Id))
            {
                throw new NotFoundException($"Invitación no existe");
            }

            context.Entry(unaInvitacion).State = EntityState.Modified;
            if (unaInvitacion.Farmacia != null)
            {
                context.Entry(unaInvitacion.Farmacia).State = EntityState.Modified;
            }
            context.SaveChanges();

        }

        public bool ExisteInvitacionConNombre(string nombre)
        {

            return invitacionEntities.Any(unaInvitacion => unaInvitacion.NombreDeUsuario == nombre);

        }


        public Invitacion GetByNombre(string nombre)
        {

            if (!ExisteInvitacionConNombre(nombre))
            {
                return null;
            }
            return invitacionEntities.Include(invi=>invi.Farmacia).FirstOrDefault(unaInvitacion => unaInvitacion.NombreDeUsuario == nombre);


        }

        public void Save()
        {
            context.SaveChanges();
        }

        public bool ExisteOtraInvitacionConEsteNombre(Invitacion invitacion)
        {
            return invitacionEntities.Any(unaInvitacion => unaInvitacion.NombreDeUsuario == invitacion.NombreDeUsuario && 
                unaInvitacion.Id != invitacion.Id);
        }
    }
}
