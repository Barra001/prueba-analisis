using Microsoft.AspNetCore.Mvc;
using ObligatorioDa2.Api.Filters;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Domain.Util;
using ObligatorioDa2.ILogicaDeNegocio;
using System;
using ObligatorioDa2.Domain.DTOs;


namespace ObligatorioDa2.Api.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ExceptionFilter]
    public class SolicitudesReposicionController : ControllerBase
    {

        private readonly ISolicitudDeReposicionServicio solicitudDeReposicionServicio;

        public SolicitudesReposicionController(ISolicitudDeReposicionServicio sr)
        {
            this.solicitudDeReposicionServicio = sr;
        }

        [HttpGet]
        [AuthorizationWithParameterFilter("Dueño, Empleado")]
        public IActionResult Get([FromHeader] string token, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta,
            [FromQuery] string codigoMedicamento, [FromQuery] Enumeradores.EstadoDeSolicitud? estadoDeSolicitud)
        {
            if (desde == null && hasta == null && codigoMedicamento == null && estadoDeSolicitud == null)
            {
                return Ok(solicitudDeReposicionServicio.GetSolicitudes(token));
            }
            else
            {
                SolicitudFiltrosDTO losFiltros =
                    new SolicitudFiltrosDTO(token, desde, hasta, codigoMedicamento, estadoDeSolicitud);
                return Ok(solicitudDeReposicionServicio.GetSolicitudesConFiltros(losFiltros));
            }
        }


        [HttpPost]
        [AuthorizationWithParameterFilter("Empleado")]
        public IActionResult Post([FromBody] SolicitudDeReposicion value, [FromHeader] string token)
        {
            return Ok(solicitudDeReposicionServicio.InsertarSolicitud(value, token));
        }

        [HttpPut("{id}")]
        [AuthorizationWithParameterFilter("Dueño")]
        public IActionResult Put(int id, [FromHeader] Enumeradores.EstadoDeSolicitud estado)
        {
            return Ok(solicitudDeReposicionServicio.ActualizarSolicitud(id, estado));
        }

    }
}
