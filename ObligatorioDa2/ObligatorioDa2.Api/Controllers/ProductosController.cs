using Microsoft.AspNetCore.Mvc;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.ILogicaDeNegocio;
using ObligatorioDa2.Api.Filters;

namespace ObligatorioDa2.Api.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ExceptionFilter]
    public class ProductosController : ControllerBase
    {

        private readonly IProductoServicio servicioProducto;

        public ProductosController(IProductoServicio servicioProducto)
        {
            this.servicioProducto = servicioProducto;
        }

      
        [HttpGet("Medicamentos")]
        public IActionResult Get([FromQuery] string nombreMedicamento, [FromQuery] string nombreFarmacia, [FromHeader] string token, [FromHeader] string modo)
        {
            if (modo == "anonimo")
            {
                return Ok(servicioProducto.GetMedicamentos());
            }else if (modo == "empleado")
            {
                return Ok(servicioProducto.GetMedicamentosDeEmpleado(token));
            }
            else
            {
                return Ok(servicioProducto.FiltrarPorMedicamentoYFarmacia(nombreMedicamento, nombreFarmacia));
            }
        }

    
       
        [HttpPost("Medicamentos")]
        [AuthorizationWithParameterFilter("Empleado")]
        public IActionResult Post([FromBody] Medicamento value, [FromHeader] string token)
        {
            return Ok(servicioProducto.InsertarProducto(value, token));
        }

        [HttpPut("{id}")]
        [AuthorizationWithParameterFilter("Empleado")]
        public IActionResult DarDeBaja(int id)
        {
            return Ok(servicioProducto.DarDeBajaProducto(id));
        }

    }
}
