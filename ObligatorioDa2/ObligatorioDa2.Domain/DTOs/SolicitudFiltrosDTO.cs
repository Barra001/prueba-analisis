using ObligatorioDa2.Domain.Util;
using System;

namespace ObligatorioDa2.Domain.DTOs
{
    public class SolicitudFiltrosDTO
    {
        public SolicitudFiltrosDTO(string tokenDeEmpleado, DateTime? desde, DateTime? hasta, string codigoMedicamento, Enumeradores.EstadoDeSolicitud? estadoSolicitud)
        {
            this.TokenDeEmpleado = tokenDeEmpleado;
            this.Desde = desde;
            this.Hasta = hasta;
            this.CodigoMedicamento = codigoMedicamento;
            this.EstadoSolicitud = estadoSolicitud;
        }

        public string TokenDeEmpleado { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public string CodigoMedicamento { get; set; }
        public Enumeradores.EstadoDeSolicitud? EstadoSolicitud { get; set; }
    }
}
