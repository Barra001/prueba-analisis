using Microsoft.AspNetCore.Mvc;
using ObligatorioDa2.Api.Filters;
using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.ILogicaDeNegocio;
using ObligatorioDa2.Reflection;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ObligatorioDa2.Api.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ExceptionFilter]
    public class ExportadoresController : ControllerBase
    {
        private readonly IImporterServicio importerServicio;
        private readonly ISesionServicio sesionServicio;
        public ExportadoresController(IImporterServicio importerServicio, ISesionServicio sesionServicio)
        {
            this.importerServicio = importerServicio;
            this.sesionServicio = sesionServicio;
        }

        [HttpGet]
        [AuthorizationWithParameterFilter("Empleado")]
        public IActionResult Get()
        {
            return Ok(importerServicio.GetExportadores());
        }

        [HttpPost]
        [AuthorizationWithParameterFilter("Empleado")]
        public IActionResult Post([FromBody] ExportarDTO exportador, [FromHeader] string token)
        {
            return Ok(importerServicio.Exportar(exportador, sesionServicio.GetFarmaciaByToken(token)));
        }

    }
}
