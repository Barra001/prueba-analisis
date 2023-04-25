using System;
using System.Collections.Generic;
using static ObligatorioDa2.Domain.Util.Enumeradores;

namespace ObligatorioDa2.Domain.Entidades
{
    public class SolicitudDeReposicion
    {
        public int Id { get; set; }
        public List<CantidadProductos> Productos { get; set; }

        public int Solicitante { get; set; }

        public DateTime FechaDeEmision { get; set; }

        public EstadoDeSolicitud EstadoDeSolicitud { get; set; }

        public SolicitudDeReposicion()
        {
            this.FechaDeEmision = DateTime.Now;
            this.EstadoDeSolicitud = EstadoDeSolicitud.Pendiente;
        }
        public SolicitudDeReposicion(int solicitante, List<CantidadProductos> listaProductos): this()
        {
            this.Productos = listaProductos;
            this.Solicitante = solicitante;
        }
    }
}