using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.IAccesoDatos;
using ObligatorioDa2.ILogicaDeNegocio;
using System;
using System.Collections.Generic;
using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Exceptions;
using static ObligatorioDa2.Domain.Util.Enumeradores;
using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.LogicaDeNegocio
{
    public class InvitacionServicio : IInvitacionServicio
    {

        private readonly IInvitacionRepository invitacionRepository;
        private readonly ISesionServicio sesionServicio;
   
        public InvitacionServicio(IInvitacionRepository invitacionRepository, ISesionServicio sesionServicio)
        {
            this.invitacionRepository = invitacionRepository;
            this.sesionServicio = sesionServicio;
       
        }
        public IEnumerable<Invitacion> GetInvitaciones()
        {
            return invitacionRepository.GetAll();
        }

        public Invitacion InsertInvitacion(Invitacion invitacion, string token)
        {
            Usuario elUser = sesionServicio.GetUsuarioByToken(token);
            if (elUser.ConseguirTipo() == Enumeradores.Rol.Dueño)
            {
                Dueño elDueño = (Dueño) elUser;
                if (invitacion.RolInvitado != Enumeradores.Rol.Empleado)
                {
                    throw new BusinessLogicException(
                        "Un dueño solo puede crear invitaciones para empleados");
                }

                Farmacia farmaciaDeDueño = sesionServicio.GetFarmaciaByToken(token);
                invitacion.Farmacia = new Farmacia() {Id = farmaciaDeDueño.Id};

            }

            if (EsValidaInvitacion(invitacion) && !ExisteInvitacionConNombre(invitacion.NombreDeUsuario))
            {
                invitacionRepository.Insertar(invitacion);
            }
            return invitacion;
        }
        public Invitacion UsarInvitacion(string nombreDeUsuario, int codigo)
        {
            Invitacion invitacion = invitacionRepository.GetByNombre(nombreDeUsuario);
            if (invitacion != null)
            {
                bool coincideCodigo = codigo == invitacion.Codigo;
                bool invitacionUsada = invitacion.Usada;
                if (coincideCodigo && !invitacionUsada)
                {
                    invitacion.Usada = true;
                    invitacionRepository.Update(invitacion);
                    return invitacion;
                }
            }

            throw new NotFoundException("Usuario y/o código incorrecto");
        }

        public Invitacion GetByNombreYcodigo(string nombreDeUsuario, int codigo)
        {
            Invitacion invitacion = invitacionRepository.GetByNombre(nombreDeUsuario);
            if (invitacion != null)
            {
                bool coincideCodigo = codigo == invitacion.Codigo;
                bool invitacionUsada = invitacion.Usada;
                if (coincideCodigo && !invitacionUsada)
                {
                    return invitacion;
                }
            }

            throw new NotFoundException("La invitación ya fue usada o se ingresaron incorrectamente los datos");
        }
        public List<Invitacion> ConseguirInvitacionesFiltradas(InvitacionFiltroDTO filtros)
        {

            try
            {
                Administrador elAdmin = (Administrador)sesionServicio.GetUsuarioByToken(filtros.token);
            }
            catch (InvalidCastException)
            {
                throw new NotEnoughPrivilegesException("Solo los administradores tienen acceso a estos filtros.");
            }

            List<Invitacion> todasInvitaciones = (List<Invitacion>)invitacionRepository.GetAll();
            List<Invitacion> filtradas = new();

            foreach (Invitacion invitacion in todasInvitaciones)
            {
                if (filtros.FarmaciaId != null && invitacion.Farmacia == null)
                    continue;
                if (filtros.FarmaciaId != null && filtros.FarmaciaId != invitacion.Farmacia.Id)
                    continue;
                if (filtros.NombreDeUsuario != null && filtros.NombreDeUsuario != invitacion.NombreDeUsuario)
                    continue;
                if (filtros.Rol != null && filtros.Rol != invitacion.RolInvitado)
                    continue;
                filtradas.Add(invitacion);
            }

            return filtradas;
        }

        public Invitacion EditarInvitacion(Invitacion invitacion, bool generarCodigo)
        {
            Invitacion invitacionVieja = invitacionRepository.GetById(invitacion.Id);
            if (invitacionVieja.Usada == true)
            {
                throw new BusinessLogicException("No se puede editar una invitacion usada");
            }
            else if (invitacion.Usada == true)
            {
                throw new BusinessLogicException("No se puede modificar el atributo Usada");
            }
            if (EsValidaInvitacion(invitacion) && !ExisteOtraInvitacionConEsteNombre(invitacion))
            {
                if (generarCodigo)
                {
                    invitacion.Codigo = GeneradorCodigoRandom.RandomCode();
                }
                invitacionRepository.Update(invitacion);
            }
            return invitacion;
        }
        private bool EsValidaInvitacion(Invitacion invitacion)
        {
            if (invitacion == null)
            {
                throw new NullReferenceException("La invitacion no puede ser nula");
            }
            if (NombreDeUsuarioInvalido(invitacion.NombreDeUsuario))
            {
                throw new BusinessLogicException("El nombre de usuario no es valido");
            }
            if (!(Enum.IsDefined(typeof(Rol), invitacion.RolInvitado)))
            {
                throw new BusinessLogicException("Rol Inválido");
            }
            if (invitacion.RolInvitado == Rol.Administrador && invitacion.Farmacia != null)
            {
                throw new BusinessLogicException("Si es administrador no puede tener farmacia asociada");
            }
            if (invitacion.RolInvitado != Rol.Administrador && invitacion.Farmacia == null)
            {
                throw new BusinessLogicException("Debe tener una farmacia asociada si es dueño o empleado");
            }

            return true;
        }

        private bool ExisteInvitacionConNombre(string nombre)
        {
            bool nombreUsado = invitacionRepository.ExisteInvitacionConNombre(nombre);
            if (nombreUsado)
            {
                throw new BusinessLogicException("El nombre de usuario ya está ocupado");
            }
            return false;
        }

        private bool NombreDeUsuarioInvalido(string nombre)
        {
            if (string.IsNullOrEmpty(nombre) || nombre.Length >= 20 || nombre.Contains(" "))
            {
                return true;
            }
            return false;
        }


       

        private bool ExisteOtraInvitacionConEsteNombre(Invitacion invitacion)
        {
            bool nombreUsado = invitacionRepository.ExisteOtraInvitacionConEsteNombre(invitacion);
            if (nombreUsado)
            {
                throw new BusinessLogicException("El nombre de usuario ya está ocupado");
            }
            return false;
        }
    }
}
