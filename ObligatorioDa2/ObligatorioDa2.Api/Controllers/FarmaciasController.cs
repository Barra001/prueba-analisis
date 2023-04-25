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
    public class FarmaciasController : ControllerBase
    {
        private readonly IFarmaciaServicio servicioFarmacia;

        public FarmaciasController(IFarmaciaServicio sr)
        {
            this.servicioFarmacia = sr;
        }
        
        [HttpGet]
        [AuthorizationWithParameterFilter("Administrador")]
        public IActionResult Get()
        {
             return Ok(servicioFarmacia.GetFarmacias());
        }

        [HttpGet("{id}")]
        [AuthorizationWithParameterFilter("Administrador")]
        public IActionResult GetById(int id)
        {
            return Ok(servicioFarmacia.GetFarmaciaById(id));
        }

        [HttpPost]
        [AuthorizationWithParameterFilter("Administrador")]
        public IActionResult Post([FromBody] Farmacia value)
        {
            return Ok(servicioFarmacia.InsertFarmacia(value));
        }

    }
}
