using Microsoft.AspNetCore.Mvc;
using ObligatorioDa2.Api.Filters;
using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Domain.Util;
using ObligatorioDa2.ILogicaDeNegocio;

namespace ObligatorioDa2.Api.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ExceptionFilter]
    public class InvitacionesController : ControllerBase
    {
        private readonly IInvitacionServicio servicioInvitacion;

        public InvitacionesController(IInvitacionServicio servicio)
        {
            this.servicioInvitacion = servicio;
        }


        [HttpGet]
        public IActionResult Get([FromQuery] string nombreDeUsuario, [FromQuery] int codigo, 
            [FromQuery] int? farmaciaId, [FromQuery] Enumeradores.Rol? rol, [FromHeader] string token)
        {
            if (codigo != 0)
            {
                return Ok(servicioInvitacion.GetByNombreYcodigo(nombreDeUsuario, codigo));
            }
            else
            {
                InvitacionFiltroDTO filtros = new InvitacionFiltroDTO(farmaciaId, nombreDeUsuario, rol, token);

                return Ok(servicioInvitacion.ConseguirInvitacionesFiltradas(filtros));
            }
            
        }


        [HttpPost]
        [AuthorizationWithParameterFilter("Administrador, Dueño")]
        public IActionResult Post([FromBody] Invitacion value, [FromHeader] string token)
        {
            return Ok(servicioInvitacion.InsertInvitacion(value, token));
        }

        [HttpPut]
        [AuthorizationWithParameterFilter("Administrador")]
        public IActionResult Put([FromBody] Invitacion value, [FromHeader] bool generarCodigo)
        {
            return Ok(servicioInvitacion.EditarInvitacion(value, generarCodigo));
        }


    }
}
