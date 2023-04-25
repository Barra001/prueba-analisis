using Microsoft.AspNetCore.Mvc;
using ObligatorioDa2.Api.Filters;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.ILogicaDeNegocio;

namespace ObligatorioDa2.Api.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ExceptionFilter]
    public class SesionesController : ControllerBase
    {
        private readonly ISesionServicio sesionServicio;

        public SesionesController(ISesionServicio sr)
        {
            this.sesionServicio = sr;
        }

        [HttpPost]
        public IActionResult LogIn([FromBody] Usuario value)
        {
            return Ok(sesionServicio.LogIn(value.NombreDeUsuario, value.Contrasena));
        }

        [HttpDelete]
        [AuthorizationWithParameterFilter("Administrador, Dueño, Empleado")]
        public IActionResult LogOut([FromHeader] string token)
        {
            return Ok(sesionServicio.LogOut(token));
        }

     

    }
}
