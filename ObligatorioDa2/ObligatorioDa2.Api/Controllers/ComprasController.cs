using System;
using Microsoft.AspNetCore.Mvc;
using ObligatorioDa2.Api.Filters;
using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Util;
using ObligatorioDa2.ILogicaDeNegocio;

namespace ObligatorioDa2.Api.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ExceptionFilter]
    public class ComprasController : ControllerBase
    {
        private readonly ICompraServicio compraServicio;

        public ComprasController(ICompraServicio compraServicio)
        {
            this.compraServicio = compraServicio;
        }

        [HttpGet]
     
        public IActionResult Get([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromHeader] string token, [FromHeader] int codigoSeguimiento)
        {
            if ((desde == null || hasta == null) && (codigoSeguimiento == 0 || codigoSeguimiento == null))
            {
               return Ok(compraServicio.GetComprasPorFarmaciaDeEmpleado(token));
            
            }
            else if(codigoSeguimiento == 0 || codigoSeguimiento == null)
            {
                return Ok(compraServicio.GetComprasPorRangoEnFarmacia(token, desde, hasta));

            }
            else
            { 
                return Ok(compraServicio.GetCompraPorCodigoDeSeguimiento(codigoSeguimiento));
            }

        }


        [HttpPost]
        public IActionResult Post([FromBody] CompraDTO value)
        {
            return Ok(compraServicio.InsertarCompra(value));
        }

        [AuthorizationWithParameterFilter("Empleado")]
        [HttpPut("{idCompra}/Medicamentos")]
        public IActionResult Put([FromQuery] string codigo, int idCompra, [FromHeader] Enumeradores.EstadoDeCompraProducto estado, [FromHeader] string token)
        {
            return Ok(compraServicio.AceptarORechazarMedicamento(idCompra, codigo, estado, token));
        }


    }
}
