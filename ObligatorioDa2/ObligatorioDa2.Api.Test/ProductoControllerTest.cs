using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObligatorioDa2.ILogicaDeNegocio;
using Moq;
using ObligatorioDa2.Api.Controllers;
using ObligatorioDa2.Domain.Entidades;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ObligatorioDa2.Exceptions;
using System;

namespace ObligatorioDa2.Api.Test
{
    [TestClass]
    public class ProductoControllerTest
    {
        private Mock<IProductoServicio> mock;
        private ProductosController api;
        private Medicamento paracetamol;
        private IEnumerable<Medicamento> medicamentos;
        private readonly int id = 1;
        private string validToken = "lkusdifhoiudfshlids545";

        [TestInitialize]
        public void InitTest()
        {
            mock = new Mock<IProductoServicio>(MockBehavior.Strict);
            api = new ProductosController(mock.Object);
            paracetamol = new Medicamento()
            {
                Nombre = "Paracetamol",
                Codigo = "123",
                CantidadPorPresentacion = 8,
                Unidad = Domain.Util.Enumeradores.Unidad.Comprimidos,
                Precio = 200,
                Presentacion = Domain.Util.Enumeradores.Presentacion.Comprimidos,
                Receta = false,
                Sintomas = "Dolor de cabeza"
            };
            medicamentos = new List<Medicamento>() { paracetamol };
        }

        [TestMethod]
        public void GetMedicamentosOk()
        {
            mock.Setup(x => x.GetMedicamentos()).Returns(medicamentos);

            IActionResult result = api.Get(null, null, null, "anonimo");
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            IEnumerable<Medicamento> body = objectResult.Value as IEnumerable<Medicamento>;

            
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(body.SequenceEqual(medicamentos));
        }
    

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void GetMedicamentoFail()
        {
            mock.Setup(x => x.GetMedicamentos()).Throws(new BusinessLogicException());

            api.Get(null,null, null,"anonimo");

            mock.VerifyAll();
        }

        [TestMethod]
        public void GetMedicamentosEmpleadoOk()
        {
            string validToken = "ldfgfdg58df4g564fg524fg453gf";
            mock.Setup(x => x.GetMedicamentosDeEmpleado(validToken)).Returns((List<Medicamento>) medicamentos);

            IActionResult result = api.Get(null, null, validToken, "empleado");
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            IEnumerable<Medicamento> body = objectResult.Value as List<Medicamento>;

            
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(body.SequenceEqual(medicamentos));
        }


        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void GetMedicamentoEmpleadoFail()
        {
            
            string validToken = "ldfgfdg58df4g564fg524fg453gf";
            mock.Setup(x => x.GetMedicamentosDeEmpleado(validToken)).Throws(new BusinessLogicException());

            api.Get(null, null, validToken, "empleado");


            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void GetMedicamentosFiltradosFail()
        {
            string nombreMedicamento = "un nombe";
            string nombreFarmacia = "otro nombe";
            mock.Setup(x => x.FiltrarPorMedicamentoYFarmacia(nombreMedicamento, nombreFarmacia)).Throws(new BusinessLogicException());

            
            api.Get(nombreMedicamento, nombreFarmacia, null, null);

            mock.VerifyAll();

        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void PostMedicamentoBadRequest()
        {
            mock.Setup(x => x.InsertarProducto(paracetamol, validToken)).Throws(new BusinessLogicException());
            api.Post(paracetamol, validToken);
      

            mock.VerifyAll();
          
        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void PostMedicamentoBadRequestInDataAccess()
        {
            mock.Setup(x => x.InsertarProducto(paracetamol, validToken)).Throws(new AccesoDatosException(""));
            api.Post(paracetamol, validToken);


            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PostMedicamentoFail()
        {
            mock.Setup(x => x.InsertarProducto(paracetamol, validToken)).Throws(new Exception());
            api.Post(paracetamol, validToken);


            mock.VerifyAll();

        }

        [TestMethod]
        public void PostMedicamentoOk()
        {
            mock.Setup(x => x.InsertarProducto(It.IsAny<Medicamento>(), It.IsAny<String>())).Returns(paracetamol);
            IActionResult result = api.Post(It.IsAny<Medicamento>(), It.IsAny<String>());
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            Medicamento body = objectResult.Value as Medicamento;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(paracetamol.Equals(body));
        }

       

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void DarDeBajaMedicamentoBadRequest()
        {
            mock.Setup(x => x.DarDeBajaProducto(id)).Throws(new BusinessLogicException());
            api.DarDeBaja(id);


            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void DarDeBajaMedicamentoNotFound()
        {
            mock.Setup(x => x.DarDeBajaProducto(id)).Throws(new AccesoDatosException(""));
            api.DarDeBaja(id);


            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DarDeBajaMedicamentoFail()
        {
            mock.Setup(x => x.DarDeBajaProducto(id)).Throws(new Exception());
            api.DarDeBaja(id);


            mock.VerifyAll();
        }

        [TestMethod]
        public void DarDeBajaMedicamentoOk()
        {
            Medicamento medicamentoModificado = paracetamol;
            medicamentoModificado.DadoDeBaja = true;
            mock.Setup(x => x.DarDeBajaProducto(id)).Returns(medicamentoModificado);
            IActionResult result = api.DarDeBaja(id);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            Medicamento body = objectResult.Value as Medicamento;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(medicamentoModificado.DadoDeBaja.Equals(body.DadoDeBaja));
        }
    }
}
