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
    public class UsuariosController : ControllerBase
    {
  
        private readonly ITrabajadorServicio servicioTrabajador;
        private readonly IUsuarioServicio servicioUsuario;

        public UsuariosController(IUsuarioServicio servicioUsuario, ITrabajadorServicio servicioTrabajador)
        {
            this.servicioTrabajador = servicioTrabajador;
            this.servicioUsuario = servicioUsuario;
        }


        [HttpPost("Administradores")]
        public IActionResult PostUsuarioAdministrador([FromBody] Administrador usuario, [FromHeader] int codigo)
        {
            return Ok(servicioUsuario.InsertUsuario(usuario, codigo));
        }

        [HttpPost("Dueños")]
        public IActionResult PostUsuarioDueno([FromBody] Dueño dueño, [FromHeader] int codigo)
        {
            return Ok(servicioTrabajador.InsertUsuario(dueño, codigo));
        }

     
        [HttpPost("Empleados")]
        public IActionResult PostUsuarioEmpleado([FromBody] Empleado empleado, [FromHeader] int codigo)
        {
            return Ok(servicioTrabajador.InsertUsuario(empleado, codigo));
        }

    }
}
