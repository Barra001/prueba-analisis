using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Domain.Util;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;
using ObligatorioDa2.ILogicaDeNegocio;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ObligatorioDa2.LogicaDeNegocio
{
    public class SolicitudDeReposicionServicio : ISolicitudDeReposicionServicio
    {

        private readonly ISolicitudDeReposicionRepository solicitudDeReposicionRepository;
        private readonly IProductoServicio productoServicio;
        private readonly ISesionServicio sesionServicio;

        public SolicitudDeReposicionServicio(ISolicitudDeReposicionRepository solicitudDeReposicionRepository, IProductoServicio productoServicio, ISesionServicio sesionServicio)
        {
            this.solicitudDeReposicionRepository = solicitudDeReposicionRepository;
            this.productoServicio = productoServicio;
            this.sesionServicio = sesionServicio;
        }

        public IEnumerable<SolicitudDeReposicion> GetSolicitudes(string token)
        {
            Usuario elUser = sesionServicio.GetUsuarioByToken(token);
            if (elUser.ConseguirTipo() == Enumeradores.Rol.Empleado)
            {
                List<SolicitudDeReposicion>
                    laList = (List<SolicitudDeReposicion>)solicitudDeReposicionRepository.GetSolicitudesDe((Empleado) elUser);
                List<SolicitudDeReposicion>
                    laListReturn = new List<SolicitudDeReposicion>();
                foreach (SolicitudDeReposicion solicitud in laList)
                {
                   
                        laListReturn.Add(solicitud);
                    
                }
                return laListReturn;
            }
            else if (elUser.ConseguirTipo() == Enumeradores.Rol.Dueño)
            {
                Farmacia farmaciaDelDueno =  sesionServicio.GetFarmaciaByToken(token);
                List<SolicitudDeReposicion>
                    laList = farmaciaDelDueno.SolicitudesDeReposicion;
                List<SolicitudDeReposicion>
                    laListReturn = new List<SolicitudDeReposicion>();
                foreach (SolicitudDeReposicion solicitud in laList)
                {
                    if (solicitud.EstadoDeSolicitud == Enumeradores.EstadoDeSolicitud.Pendiente)
                    {
                        laListReturn.Add(solicitud);
                    }
                }
                return laListReturn; 
            }
            
            throw new NotEnoughPrivilegesException("Solo los dueños y empleados pueden ver solicitudes");
            
        }

        public SolicitudDeReposicion InsertarSolicitud(SolicitudDeReposicion solicitud, string token)
        {
            if (EsValidaSolicitud(solicitud))
            {
                try
                { 
                  
                    Empleado unEmpleado = (Empleado) sesionServicio.GetUsuarioByToken(token);
                    solicitud.Solicitante = unEmpleado.Id;
                }
                catch (InvalidCastException)
                {
                    
                    throw new NotEnoughPrivilegesException("Solo los Empleados pueden hacer solicitudes");
                }
             
                Farmacia empleadoFarmacia = sesionServicio.GetFarmaciaByToken(token);

                foreach (CantidadProductos cantProd in solicitud.Productos)
                {
                    Producto elProducto = empleadoFarmacia.Productos.Find(x => x.Codigo == cantProd.Producto.Codigo);
                    if (elProducto==null)
                    {
                        throw new BusinessLogicException("La solicitud tiene producto/s no registrado/s en tu farmacia.");
                    }
                    else
                    {
                        cantProd.Producto = new Producto() {Id = elProducto.Id};
                    }
                }
              


                solicitudDeReposicionRepository.InsertarSolicitud(solicitud, empleadoFarmacia);
            }
            return solicitud;
        }

        public SolicitudDeReposicion ActualizarSolicitud(int id, Enumeradores.EstadoDeSolicitud estado)
        {

            SolicitudDeReposicion solicitud = solicitudDeReposicionRepository.GetById(id);
            if (Enumeradores.EstadoDeSolicitud.Pendiente != solicitud.EstadoDeSolicitud)
            {
                throw new BusinessLogicException("Solo se puede modificar el estado de una solicitud pendiente");
            }
            if (estado == Enumeradores.EstadoDeSolicitud.Aceptada)
            {
                solicitud.EstadoDeSolicitud = Enumeradores.EstadoDeSolicitud.Aceptada;
                solicitudDeReposicionRepository.Update(solicitud);
                RellenarStock(solicitud.Productos);
            }
            else if (estado == Enumeradores.EstadoDeSolicitud.Rechazada)
            {
                solicitud.EstadoDeSolicitud = Enumeradores.EstadoDeSolicitud.Rechazada;
                solicitudDeReposicionRepository.Update(solicitud);
            }
            else
            {
                throw new BusinessLogicException("Solo se le puede poner estado Aceptada o Rechazada");
            }

            return solicitud;
        }
        public List<SolicitudDeReposicion> GetSolicitudesConFiltros(SolicitudFiltrosDTO infoFiltros)
        {
            Empleado elEmpleado;
            try
            {
                elEmpleado = (Empleado)sesionServicio.GetUsuarioByToken(infoFiltros.TokenDeEmpleado);
            }
            catch (InvalidCastException)
            {
                throw new NotEnoughPrivilegesException("Solo los empleados tienen acceso a estos filtros.");
            }

            Farmacia laFarmacia = sesionServicio.GetFarmaciaByToken(infoFiltros.TokenDeEmpleado);
            List<SolicitudDeReposicion> respuesta = new List<SolicitudDeReposicion>();
            foreach (SolicitudDeReposicion solicitudDeReposicion in laFarmacia.SolicitudesDeReposicion)
            {
                if (elEmpleado.Id != solicitudDeReposicion.Solicitante)
                    continue;
                if (infoFiltros.Desde != null && infoFiltros.Desde > solicitudDeReposicion.FechaDeEmision)
                    continue;
                if (infoFiltros.Hasta != null && infoFiltros.Hasta < solicitudDeReposicion.FechaDeEmision)
                    continue;
                if (infoFiltros.CodigoMedicamento != null)
                {
                    if (solicitudDeReposicion.Productos == null)
                        continue;
                    bool codigoCoincide = solicitudDeReposicion
                        .Productos
                        .Any(x => x.Producto.Codigo == infoFiltros.CodigoMedicamento);
                    if (!codigoCoincide)
                        continue;
                }
                if (infoFiltros.EstadoSolicitud != null && infoFiltros.EstadoSolicitud != solicitudDeReposicion.EstadoDeSolicitud)
                    continue;

                respuesta.Add(solicitudDeReposicion);
            }
            return respuesta;
        }

        private bool EsValidaSolicitud(SolicitudDeReposicion solicitud)
        {
            if(solicitud == null)
            {
                throw new NullReferenceException("La solicitud no puede ser null");
            }
            if (solicitud.Productos == null || solicitud.Productos.Count()<1)
            {
                throw new NullReferenceException("La lista de productos no puede ser nula o vacía");

            }
            foreach (CantidadProductos cantidadPorProducto in solicitud.Productos)
            {
                if (cantidadPorProducto.Producto == null)
                {
                    throw new NullReferenceException("El producto no puede ser null");
                }
                if(cantidadPorProducto.Cantidad < 1)
                {
                    throw new BusinessLogicException("La cantidad de ningun producto a solicitar puede ser menor a 1");

                }
            }
            return true;
        }

      
        
        private void RellenarStock(List<CantidadProductos> cantidadProductos)
        {
            foreach (CantidadProductos par in cantidadProductos)
            {
                productoServicio.CambiarStock(par.Producto.Id, par.Cantidad);
            }
        }


        
    }
}
