using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.IAccesoDatos;
using ObligatorioDa2.ILogicaDeNegocio;
using System;

namespace ObligatorioDa2.LogicaDeNegocio
{
   public class TrabajadorServicio :UsuarioServicio,ITrabajadorServicio
   {
   
        public TrabajadorServicio(IUsuarioRepository usuarioRepository, IInvitacionServicio invitacionServicio) :base(usuarioRepository, invitacionServicio)
        {
        }
        public override Usuario InsertUsuario(Usuario usuario, int codigo)
        {
            if (usuario == null)
            {
                throw new NullReferenceException("Debe enviar el usuario");
            }
            AdjuntarFarmaciaATrabajador(usuario, codigo);
            if (UsuarioValido(usuario, codigo))
            {
                InvitacionServicio.UsarInvitacion(usuario.NombreDeUsuario, codigo);
                UsuarioRepository.Insertar(usuario);

            }
            return usuario;
        }
        private void AdjuntarFarmaciaATrabajador(Usuario usuario, int codigo)
        {
            Trabajador unTrabajador = (Trabajador)usuario;
            Invitacion laInvitacion = InvitacionServicio.GetByNombreYcodigo(usuario.NombreDeUsuario, codigo);

            unTrabajador.Farmacia = laInvitacion.Farmacia;
        }

       
    }
}
