using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;

namespace ObligatorioDa2.AccesoDatos.Repositorys
{
    public class SesionRepository : ISesionRepository
    {


        private readonly DbSet<Sesion> sesiones;
        private readonly DbSet<Usuario> usuarios;
        private readonly DbContext context;

        public SesionRepository(DbContext context)
        {
            sesiones = context.Set<Sesion>();
            usuarios = context.Set<Usuario>();
            this.context = context;
        }

        public bool Exists(int key)
        {

            return sesiones.Any(unaSesion => unaSesion.Id == key);

        }

        public IEnumerable<Sesion> GetAll()
        {
            List<Sesion> sesionLista = sesiones
                 .Include(unaSesion => unaSesion.UsuarioSesion)
                 .ToList();

            return sesionLista;
        }

        public Sesion GetById(int id)
        {

            if (!Exists(id))
            {
                throw new NotFoundException($"La sesion no existe.");
            }

            return sesiones
                .Include(sesion => sesion.UsuarioSesion).FirstOrDefault(unaSesion => unaSesion.Id == id);

        }

        public string GetTokenFromUserName(string nombreUsuario)
        {

            if (!ExistsUsuario(nombreUsuario))
            {
                throw new NotFoundException($"La sesion no existe.");
            }

            return sesiones
                .Include(sesion => sesion.UsuarioSesion)
                .FirstOrDefault(unaSesion => unaSesion.UsuarioSesion.NombreDeUsuario == nombreUsuario)
                .Token;


        }
        public Sesion GetSesionByToken(string token)
        {


            if (!ExistsToken(token))
            {
                throw new NotFoundException($"El token no existe.");
            }

            Sesion unaSesion = sesiones
                .Include(sesion => sesion.UsuarioSesion).FirstOrDefault(unaSesion => unaSesion.Token == token);

            return unaSesion;

        }
        public Usuario GetUsuarioByToken(string token)
        {


            if (!ExistsToken(token))
            {
                throw new NotFoundException($"El token no existe");
            }

            Sesion unaSesion = sesiones
                .Include(sesion => sesion.UsuarioSesion).FirstOrDefault(unaSesion => unaSesion.Token == token);

            return unaSesion.UsuarioSesion;

        }
        public Farmacia GetFarmaciaByToken(string token)
        {


            if (!ExistsToken(token))
            {
                throw new NotFoundException($"El token no existe");
            }

            Sesion unaSesion = sesiones
                .Include(sesion => sesion.UsuarioSesion)
                .FirstOrDefault(unaSesion => unaSesion.Token == token);
            Usuario userSinFarmacia = unaSesion.UsuarioSesion;

            Trabajador trabajadoConFarmacia = (Trabajador)
                usuarios.AsNoTracking().OfType<Trabajador>().Include(tra => tra.Farmacia)
                    .ThenInclude(far => far.SolicitudesDeReposicion).ThenInclude(sol=>sol.Productos).ThenInclude(cant => cant.Producto)
                    .Include(tra => tra.Farmacia)
                    .ThenInclude(far => far.Productos)
                    .Include(tra => tra.Farmacia)
                    .ThenInclude(far => far.Compras).ThenInclude(comp=>comp.Productos).ThenInclude(cant => cant.Producto)
                    .FirstOrDefault(u => u.Id == userSinFarmacia.Id);
            return trabajadoConFarmacia.Farmacia;

        }


        public void Insertar(Sesion unaSesion)
        {

            if (Exists(unaSesion.Id))
            {
                throw new AccesoDatosException($"Sesion ya existe");
            }
            sesiones.Attach(unaSesion);
            context.SaveChanges();
            Update(unaSesion);

        }

        public void Update(Sesion value)
        {
            
        }


        public bool ExistsToken(string token)
        {

            return sesiones.Any(unaSesion => unaSesion.Token == token);

        }
        public bool ExistsUsuario(string nombreUsuario)
        {

            return sesiones.Any(unaSesion => unaSesion.UsuarioSesion.NombreDeUsuario == nombreUsuario);

        }

        public void Delete(int id)
        {

            if (!Exists(id))
            {
                throw new NotFoundException($"Sesion no existe");
            }

            sesiones.Remove(GetById(id));
            context.SaveChanges();

        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}

