using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Domain.Util;
using ObligatorioDa2.IAccesoDatos;
using ObligatorioDa2.ILogicaDeNegocio;
using ObligatorioDa2.Exceptions;

namespace ObligatorioDa2.LogicaDeNegocio
{
    public class SesionServicio : ISesionServicio
    {

        private readonly ISesionRepository sesionRepository;
        private readonly IUsuarioRepository usuarioRepository;

        public SesionServicio(ISesionRepository sesionRepository, IUsuarioRepository usuarioRepository)
        {
            this.sesionRepository = sesionRepository;
            this.usuarioRepository = usuarioRepository;
        }

        public Usuario GetUsuarioByToken(string token)
        {
            return sesionRepository.GetUsuarioByToken(token);
        }

        public Farmacia GetFarmaciaByToken(string token)
        {
            return sesionRepository.GetFarmaciaByToken(token);
        }

        
        public TokenDTO LogIn(string nombreUsuario, string contrasena)
        {
            Usuario elUsuario;
            
            BusinessLogicException theException = new BusinessLogicException("Usuario o contraseña incorrectas");
            try
            {
                elUsuario = usuarioRepository.GetByNombre(nombreUsuario);
            }
            catch (AccesoDatosException)
            {
                throw theException;
            }
            if (sesionRepository.ExistsUsuario(nombreUsuario))
            {
                if (contrasena == elUsuario.Contrasena)
                {
                    return new TokenDTO() { Token = sesionRepository.GetTokenFromUserName(nombreUsuario), Rolusuario = (Enumeradores.Rol) elUsuario.ConseguirTipo() };
                }
                else
                {
                    throw theException;
                }
            }

          
            
            if (contrasena == elUsuario.Contrasena)
            {
                Sesion nuevaSesion = new Sesion(elUsuario);
                sesionRepository.Insertar(nuevaSesion);
                return new TokenDTO() {Token = nuevaSesion.Token, Rolusuario = (Enumeradores.Rol)elUsuario.ConseguirTipo() };
            }
            else
            {
                throw theException;
            }

        }

        public MensajeLogOutDTO LogOut(string token)
        {
            Sesion laSesion = sesionRepository.GetSesionByToken(token);
            sesionRepository.Delete(laSesion.Id);
            return new MensajeLogOutDTO {Mensaje="LogOut exitoso" };
        }
    }
}
