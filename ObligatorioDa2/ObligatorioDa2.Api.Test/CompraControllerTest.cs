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
using ObligatorioDa2.Domain.DTOs;
using static ObligatorioDa2.Domain.Util.Enumeradores;

namespace ObligatorioDa2.Api.Test
{
    [TestClass]
    public class CompraControllerTest
    {
        private Mock<ICompraServicio> mock;
        private ComprasController api;
        private CompraDTO compraDTO;
        private Compra compra;
        private Farmacia farmacia;
        private Producto producto;
        private CantidadProductosCompra cantidadProductos;
        private List<Compra> compras;
        private InfoComprasDto infoComprasDto;
        private int codigoSeguimiento = 123456;
        private Medicamento paracetamol;
        private string validToken = "sjdisdjisd-sdfsdf656sdffhdfh";

        [TestInitialize]
        public void InitTest()
        {
            mock = new Mock<ICompraServicio>(MockBehavior.Strict);
            api = new ComprasController(mock.Object);
            farmacia = new Farmacia();
            compraDTO = new CompraDTO()
            {
                Productos = new List<CantidadProductosCompra> { cantidadProductos },
                MailComprador = "b@h.com"
            };

            compra = new Compra()
            {
                Productos = new List<CantidadProductosCompra> { cantidadProductos },
                FechaCompra = new DateTime(2022, 1, 1),
                MailComprador = "b@h.com"
            };
            cantidadProductos = new CantidadProductosCompra()
            {
                Producto = producto,
                Cantidad = 2

            };
            producto = new Medicamento()
            {
                Nombre = "Paracetamol",
                Codigo = "123",
                CantidadPorPresentacion = 8,
                Unidad = Domain.Util.Enumeradores.Unidad.Comprimidos,
                Precio = 200,
                Presentacion = Domain.Util.Enumeradores.Presentacion.Comprimidos,
                Receta = false,
                Sintomas = "Dolor de cabeza",
                Stock = 100

            };
            compras = new List<Compra>() { compra };
            infoComprasDto = new InfoComprasDto()
            {
                ListaCompras = compras,
                Total = 800
            };
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
        }

        [TestMethod]
        public void GetComprasOk()
        {
            mock.Setup(x => x.GetComprasPorFarmaciaDeEmpleado(validToken)).Returns(compras);

            IActionResult result = api.Get(null, null, validToken, 0);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            IEnumerable<Compra> body = objectResult.Value as IEnumerable<Compra>;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(body.SequenceEqual(compras));
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void GetComprasFail()
        {
            int mesNoValido = 0;
            mock.Setup(x => x.GetComprasPorFarmaciaDeEmpleado(validToken)).Throws(new BusinessLogicException());

            IActionResult result = api.Get(null, null, validToken, 0);

            mock.VerifyAll();
        }

        [TestMethod]
        public void GetComprasPorMesOk()
        {
            mock.Setup(x => x.GetComprasPorRangoEnFarmacia(validToken, It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(infoComprasDto);

            IActionResult result = api.Get(It.IsAny<DateTime>(), It.IsAny<DateTime>(), validToken, 0);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            InfoComprasDto body = objectResult.Value as InfoComprasDto;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.AreEqual(infoComprasDto, body);
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void GetComprasPorMesBadRequest()
        {
            int mesValido = 1;

            mock.Setup(x => x.GetComprasPorRangoEnFarmacia(validToken, It.IsAny<DateTime>(), It.IsAny<DateTime>())).Throws(new BusinessLogicException());

            IActionResult result = api.Get(It.IsAny<DateTime>(), It.IsAny<DateTime>(), validToken, 0);

            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetComprasPorMesFail()
        {
            int mesValido = 1;
            mock.Setup(x => x.GetComprasPorRangoEnFarmacia(validToken, It.IsAny<DateTime>(), It.IsAny<DateTime>())).Throws(new Exception());

            IActionResult result = api.Get(It.IsAny<DateTime>(), It.IsAny<DateTime>(), validToken, 0);

            mock.VerifyAll();
         
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void PostCompraBadRequest()
        {
            mock.Setup(x => x.InsertarCompra(It.IsAny<CompraDTO>())).Throws(new BusinessLogicException());
            api.Post(It.IsAny<CompraDTO>());
            
            mock.VerifyAll();
        
        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void PostCompraBadRequestInDataAccess()
        {
            mock.Setup(x => x.InsertarCompra(It.IsAny<CompraDTO>())).Throws(new AccesoDatosException(""));
            api.Post(It.IsAny<CompraDTO>());
            
            mock.VerifyAll();
         
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PostCompraFail()
        {
            mock.Setup(x => x.InsertarCompra(It.IsAny<CompraDTO>())).Throws(new Exception());
            api.Post(It.IsAny<CompraDTO>());
            
            mock.VerifyAll();
          
        }

        [TestMethod]
        public void PostCompraOk()
        {
            CodigoSeguimientoDTO codigoSeguimientoDto = new CodigoSeguimientoDTO(){ Codigo = compraDTO.CodigoSeguimiento};
            mock.Setup(x => x.InsertarCompra(compraDTO)).Returns(codigoSeguimientoDto);
            IActionResult result = api.Post(compraDTO);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            
            Assert.IsTrue(codigoSeguimientoDto.Equals(objectResult.Value));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AceptarRechazarMedicamentoFalla()
        {
            mock.Setup(x => x.AceptarORechazarMedicamento(It.IsAny<int>(), It.IsAny<string>(), EstadoDeCompraProducto.Aceptada, validToken)).Throws(new Exception());
            api.Put(It.IsAny<string>(), It.IsAny<int>(), EstadoDeCompraProducto.Aceptada, validToken);
            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void AceptarRechazarMedicamentoBadRequest()
        {
            mock.Setup(x => x.AceptarORechazarMedicamento(It.IsAny<int>(), It.IsAny<string>(), EstadoDeCompraProducto.Pendiente, validToken)).Throws(new BusinessLogicException());
            api.Put(It.IsAny<string>(), It.IsAny<int>(), EstadoDeCompraProducto.Pendiente, validToken);
            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void AceptarRechazarMedicamentoNotFound()
        {
            mock.Setup(x => x.AceptarORechazarMedicamento(It.IsAny<int>(), It.IsAny<string>(), EstadoDeCompraProducto.Rechazada, validToken)).Throws(new NotFoundException());
            api.Put(It.IsAny<string>(), It.IsAny<int>(), EstadoDeCompraProducto.Rechazada, validToken);
            mock.VerifyAll();
        }

        [TestMethod]
        public void AceptarRechazarMedicamentoOk()
        {
            mock.Setup(x => x.AceptarORechazarMedicamento(It.IsAny<int>(), It.IsAny<string>(), EstadoDeCompraProducto.Aceptada, validToken)).Returns(paracetamol);
            IActionResult result = api.Put(It.IsAny<string>(), It.IsAny<int>(), EstadoDeCompraProducto.Aceptada, validToken);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            Medicamento body = objectResult.Value as Medicamento;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.AreEqual(paracetamol, body);
        }

        [TestMethod]
        public void GetCompraCodigoSeguimientoOk()
        {
            mock.Setup(x => x.GetCompraPorCodigoDeSeguimiento(codigoSeguimiento)).Returns(compra);

            IActionResult result = api.Get(null, null, null, codigoSeguimiento);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            Compra body = objectResult.Value as Compra;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.AreEqual(compra, body);
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void GetComprasCodigoSeguimientoFail()
        {
            mock.Setup(x => x.GetCompraPorCodigoDeSeguimiento(codigoSeguimiento)).Throws(new BusinessLogicException());

            IActionResult result = api.Get(null, null, null, codigoSeguimiento);

            mock.VerifyAll();
        }

    }
}
