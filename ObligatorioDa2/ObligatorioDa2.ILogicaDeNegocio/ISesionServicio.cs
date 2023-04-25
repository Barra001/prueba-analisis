using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;

namespace ObligatorioDa2.ILogicaDeNegocio
{
    public interface ISesionServicio
    {
        public MensajeLogOutDTO LogOut(string token);
        public TokenDTO LogIn(string nombreUsuario, string contrasena);
        public Farmacia GetFarmaciaByToken(string token);
     

        public Usuario GetUsuarioByToken(string token);
    }
}
