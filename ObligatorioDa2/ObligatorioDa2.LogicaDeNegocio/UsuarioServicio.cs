using EmailValidation;
using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;
using ObligatorioDa2.ILogicaDeNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ObligatorioDa2.LogicaDeNegocio
{
   
    public class UsuarioServicio : IUsuarioServicio
    {
        protected readonly IUsuarioRepository UsuarioRepository;
        protected readonly IInvitacionServicio InvitacionServicio;


        private readonly int minCaracteresContrasena=8;
        private readonly Regex caracteresEspeciales = new Regex("[!\"#\\$%&'()*+,-./:;=?@\\[\\]^_`{|}~]");

        public UsuarioServicio(IUsuarioRepository usuarioRepository, IInvitacionServicio invitacionServicio)
        {
            this.UsuarioRepository = usuarioRepository;
            this.InvitacionServicio = invitacionServicio;

        }
       
        public List<UsuarioDto> GetUsuarios()
        {
            IEnumerable<Usuario> usuarios = UsuarioRepository.GetAll();
            List<UsuarioDto> usuariosDto = new List<UsuarioDto>();
            foreach (Usuario usuario in usuarios)
            {
                usuariosDto.Add(new UsuarioDto(usuario));
            }
            return usuariosDto;
        }
        public Usuario GetUsuarioByNombre(string nombre)
        {
         
            return UsuarioRepository.GetByNombre(nombre);
        }


        public virtual Usuario InsertUsuario(Usuario usuario, int codigo)
        {
            if (usuario == null)
            {
                throw new NullReferenceException("Debe enviar el usuario");
            }
            if (UsuarioValido(usuario, codigo))
            {
                InvitacionServicio.UsarInvitacion(usuario.NombreDeUsuario, codigo);
                UsuarioRepository.Insertar(usuario);

            }
            return usuario;
        }

        protected bool UsuarioValido(Usuario usuario, int codigo)
        {
            if (!RolHabilitado(usuario, codigo))
            {
                throw new BusinessLogicException("La invitación no es del rol que esta indiciando");
            }

            if (!MailValido(usuario.Mail))
            {
                throw new BusinessLogicException("Formato de mail Inválido");
            }
           
            if (!ContrasenaValida(usuario.Contrasena))
            {
                throw new BusinessLogicException("Contraseña inválida: Debe tenes 8 o mas caracteres y un caracter especial como mínimo");
            }
            if((string.IsNullOrEmpty(usuario.Direccion)))
            {
                throw new BusinessLogicException("El usuario debe tener dirección");
            }
            if (MailEnUso(usuario.Mail))
            {
                throw new BusinessLogicException("El mail ingresado ya está en uso");
            }
        
            return true;
        }



        private bool MailValido (string mail)
        {
            return EmailValidator.Validate(mail.Trim());
        }

        private bool MailEnUso(string mail)
        {
            return UsuarioRepository.ExisteUsuarioConMail(mail);
        }

        private bool ContrasenaValida(string contrasena)
        {
            return (contrasena.Count() >= minCaracteresContrasena && caracteresEspeciales.IsMatch(contrasena));
            
        }
      
        private bool RolHabilitado(Usuario usuario, int codigo)
        {
            Invitacion laInvitacion = InvitacionServicio.GetByNombreYcodigo(usuario.NombreDeUsuario, codigo);
            if (usuario.ConseguirTipo() != laInvitacion.RolInvitado)
            {
                return false;
            }
            return true;
        }

       

    }
}
