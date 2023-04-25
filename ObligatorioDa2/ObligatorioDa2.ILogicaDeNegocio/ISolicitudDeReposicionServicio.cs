using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Domain.Util;
using System.Collections.Generic;

namespace ObligatorioDa2.ILogicaDeNegocio
{
    public interface ISolicitudDeReposicionServicio
    {
        IEnumerable<SolicitudDeReposicion> GetSolicitudes(string token);
        SolicitudDeReposicion InsertarSolicitud(SolicitudDeReposicion solicitud, string token);
        SolicitudDeReposicion ActualizarSolicitud(int id, Enumeradores.EstadoDeSolicitud estado);
        public List<SolicitudDeReposicion> GetSolicitudesConFiltros(SolicitudFiltrosDTO infoFiltros);
    }
}
