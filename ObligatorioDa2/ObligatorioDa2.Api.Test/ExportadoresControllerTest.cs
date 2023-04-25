using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ObligatorioDa2.Api.Controllers;
using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.ILogicaDeNegocio;
using ObligatorioDa2.Reflection;


namespace ObligatorioDa2.Api.Test
{
    [TestClass]
    public class ExportadoresControllerTest
    {
        private Mock<IImporterServicio> mock;
        private Mock<ISesionServicio> mockSesion;
        private Farmacia farmacia;
        private ExportarDTO exportadorDTO;
        private ExportadoresController api;
        private string validToken ="kdsjhdskjhfkjdsfhkdhs43u5h43";
        private IEnumerable<ExportarDTO> exportadores;


        [TestInitialize]
        public void InitTest()
        {
            mock = new Mock<IImporterServicio>(MockBehavior.Strict);
            mockSesion = new Mock<ISesionServicio>(MockBehavior.Strict);
            api = new ExportadoresController(mock.Object, mockSesion.Object);
            farmacia = new Farmacia();
            exportadorDTO = new ExportarDTO();
            exportadores = new List<ExportarDTO>() {exportadorDTO };
        }

        [TestMethod]
        public void GetExportadoresOk()
        {
            mock.Setup(x => x.GetExportadores()).Returns(exportadores);

            IActionResult result = api.Get();
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            IEnumerable<ExportarDTO> body = objectResult.Value as IEnumerable<ExportarDTO>;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(body.SequenceEqual(exportadores));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetExportadoresFail()
        {
            mock.Setup(x => x.GetExportadores()).Throws(new Exception());

            IActionResult result = api.Get();

            mock.VerifyAll();
        }

        [TestMethod]
        public void ExportarOk()
        {
            mock.Setup(x => x.Exportar(exportadorDTO, farmacia)).Returns(exportadorDTO);
            mockSesion.Setup(x => x.GetFarmaciaByToken(validToken)).Returns(farmacia);
            
            IActionResult result = api.Post(exportadorDTO, validToken);

            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            ExportarDTO body = objectResult.Value as ExportarDTO;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(body.Equals(exportadorDTO));

        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void ExportarBadRequest()
        {
            mock.Setup(x => x.Exportar(exportadorDTO, farmacia)).Throws(new NotFoundException());
            mockSesion.Setup(x => x.GetFarmaciaByToken(validToken)).Returns(farmacia);
            api.Post(exportadorDTO, validToken);
            mock.VerifyAll();

        }
        
    }
}
