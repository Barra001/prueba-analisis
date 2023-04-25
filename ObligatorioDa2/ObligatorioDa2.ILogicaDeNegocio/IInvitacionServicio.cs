using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using System.Collections.Generic;

namespace ObligatorioDa2.ILogicaDeNegocio
{
    public interface IInvitacionServicio
    {
        IEnumerable<Invitacion> GetInvitaciones();
        public Invitacion InsertInvitacion(Invitacion invitacion, string token);
        Invitacion UsarInvitacion(string nombreUsuario, int codigo);
        public Invitacion GetByNombreYcodigo(string nombreDeUsuario, int codigo);
        Invitacion EditarInvitacion(Invitacion invitacion, bool generarCodigo);
        public List<Invitacion> ConseguirInvitacionesFiltradas(InvitacionFiltroDTO filtros);
    }
}
