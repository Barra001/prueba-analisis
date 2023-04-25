using System.Collections.Generic;

using System.Linq;
using Microsoft.EntityFrameworkCore;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;

namespace ObligatorioDa2.AccesoDatos.Repositorys
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DbSet<Usuario> usuarioEntities;
        private readonly DbContext context;

        public UsuarioRepository(DbContext context)
        {
            usuarioEntities = context.Set<Usuario>();
            this.context = context;
        }


        public bool Exists(int id)
        {

            return usuarioEntities.Any(unUsuario => unUsuario.Id == id);

        }

        public bool ExisteUsuarioConMail(string mail)
        {

            return usuarioEntities.Any(user => user.Mail == mail);

        }

        public Trabajador GetByIdTrabajador(int id)
        {

            if (!Exists(id))
            {
                throw new NotFoundException($"Trabajador {id} no encontrado");
            }
            return usuarioEntities
                .OfType<Trabajador>()
                .Include(user => user.Farmacia)
                .FirstOrDefault(unUsuario => unUsuario.Id == id);

        }
   

        public Administrador GetByIdAdministrador(int id)
        {

            if (!Exists(id))
            {
                throw new NotFoundException($"Usuario {id} no encontrado");
            }
            return usuarioEntities.OfType<Administrador>().FirstOrDefault(unUsuario => unUsuario.Id == id);

        }

    
        public IEnumerable<Usuario> GetAll()
        {


            return usuarioEntities.ToList(); ;

        }

        public Usuario GetById(int id)
        {

            if (!Exists(id))
            {
                throw new NotFoundException($"Usuario no encontrado");
            }
            return usuarioEntities.FirstOrDefault(unUsuario => unUsuario.Id == id);

        }

        public Usuario GetByNombre(string nombre)
        {

            Usuario elUsuario =
                usuarioEntities.FirstOrDefault(unUsuario => unUsuario.NombreDeUsuario == nombre);
            if (elUsuario == null)
            {
                throw new NotFoundException($"Usuario o contraseña incorrectas");
            }
            return elUsuario;

        }

        public void Insertar(Usuario unUsuario)
        {

            if (Exists(unUsuario.Id))
            {
                throw new AccesoDatosException($"Usuario ya existe");
            }
            usuarioEntities.Attach(unUsuario);
            context.SaveChanges();


        }

        public void Update(Usuario unUsuario)
        {

            if (!Exists(unUsuario.Id))
            {
                throw new NotFoundException($"Usuario no existe");
            }

            context.Entry(unUsuario).State = EntityState.Modified;
            context.SaveChanges();

        }
        public void Save()
        {
            context.SaveChanges();
        }
    }
}

