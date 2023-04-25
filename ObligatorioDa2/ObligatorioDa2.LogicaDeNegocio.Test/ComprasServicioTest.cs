using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;
using ObligatorioDa2.ILogicaDeNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.LogicaDeNegocio.Test
{
    [TestClass]
    public class ComprasServicioTest
    {

        private Mock<ICompraRepository> mock;

        private Mock<IProductoServicio> mockProducto;
        private Mock<ISesionServicio> mockSesionServicio;
        private Mock<IFarmaciaServicio> mockFarmaciaServicio;
        private CompraServicio servicio;
        private Compra unaCompra;
        private Compra otraCompra;
        private Empleado unEmpleado;
        private Dueño unDueño;
        private Sesion sesionDueno;
        private Sesion sesionEmpleado;
        private Farmacia unaFarmacia;
        private IEnumerable<Farmacia> farmacias;
        private IEnumerable<Compra> compras;
        private Producto producto;
        private CompraDTO unaCompraDto;
        private CantidadProductosCompra cantidadProductos;
        private CantidadProductosCompra cantidadProductos2;
        private string validToken = "aValidToken";


        [TestInitialize]
        public void InitTest()
        {
            mockProducto = new Mock<IProductoServicio>(MockBehavior.Strict);
            mock = new Mock<ICompraRepository>(MockBehavior.Strict);
            mockSesionServicio = new Mock<ISesionServicio>(MockBehavior.Strict);
            mockFarmaciaServicio = new Mock<IFarmaciaServicio>(MockBehavior.Strict);
            servicio = new CompraServicio(mock.Object, mockSesionServicio.Object, mockFarmaciaServicio.Object, mockProducto.Object);

            cantidadProductos2 = new CantidadProductosCompra()
            {
                Producto = producto,
                Cantidad = 2,
                EstadoDeCompraProducto = Enumeradores.EstadoDeCompraProducto.Pendiente

            };

            otraCompra = new Compra()
            {
                Monto = 800,
                Productos = new List<CantidadProductosCompra> { cantidadProductos2 },
                FechaCompra = new DateTime(2022, 1, 1),
                MailComprador = "b@h.com"
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
                Stock = 100,
                DadoDeBaja= false,

            };
            cantidadProductos = new CantidadProductosCompra()
            {
                Producto = producto,
                Cantidad = 2,
                EstadoDeCompraProducto = Enumeradores.EstadoDeCompraProducto.Pendiente

            };
            unaCompra = new Compra()
            {
                Productos = new List<CantidadProductosCompra> { cantidadProductos },
                MailComprador = "a@h.com"
            };

            unaFarmacia = new Farmacia()
            {
                Nombre = "Farmashop",
                Direccion = "21 de Septiembre 1900",
                SolicitudesDeReposicion = new List<SolicitudDeReposicion> { },
                Compras = new List<Compra> { unaCompra, otraCompra },
                Productos = new List<Producto> { producto }
            };

            unaCompraDto = new CompraDTO()
            {
                Productos = new List<CantidadProductosCompra> { cantidadProductos },
                MailComprador = "a@h.com"

            };

            unEmpleado = new Empleado()
            {
                Farmacia = unaFarmacia,
            };
            sesionEmpleado = new Sesion()
            {
                UsuarioSesion = unEmpleado,
            };
            unDueño = new Dueño() { Farmacia = unaFarmacia };
            sesionDueno = new Sesion()
            {
                UsuarioSesion = unDueño
            };

            farmacias = new List<Farmacia>(){ unaFarmacia };
            

        }



        [TestMethod]
        public void GetComprasPorFarmacia()
        {
            mockSesionServicio.Setup(x => x.GetFarmaciaByToken(sesionEmpleado.Token)).Returns(unaFarmacia);
            mockSesionServicio.Setup(x => x.GetUsuarioByToken(sesionEmpleado.Token)).Returns(sesionEmpleado.UsuarioSesion);
            List<Compra> comprasFarmacia = servicio.GetComprasPorFarmaciaDeEmpleado(sesionEmpleado.Token);
            mockSesionServicio.VerifyAll();
            Assert.AreEqual(unaFarmacia.Compras, comprasFarmacia);

        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetComprasPorFarmaciaSinCompras()
        {
            unaFarmacia.Compras = new List<Compra> { };
            mockSesionServicio.Setup(x => x.GetFarmaciaByToken(sesionEmpleado.Token)).Returns(unaFarmacia);
            mockSesionServicio.Setup(x => x.GetUsuarioByToken(sesionEmpleado.Token)).Returns(sesionEmpleado.UsuarioSesion);
            List<Compra> comprasFarmacia = servicio.GetComprasPorFarmaciaDeEmpleado(sesionEmpleado.Token);
            mockSesionServicio.VerifyAll();

        }


        [TestMethod]
        public void GetComprasPorCodigoDeSeguimientoValido()
        {
            otraCompra.Monto = 400;
            unaCompra.CodigoSeguimiento = 678549;
            otraCompra.CodigoSeguimiento = unaCompra.CodigoSeguimiento;
            List<Compra> comprasRepo = new List<Compra> { unaCompra, otraCompra };
            mock.Setup(x => x.GetComprasPorCodigoDeSeguimiento(unaCompra.CodigoSeguimiento)).Returns(comprasRepo);
            Compra devueltaPorLogica= servicio.GetCompraPorCodigoDeSeguimiento(unaCompra.CodigoSeguimiento);
            int montoCompra = otraCompra.Monto + unaCompra.Monto;
            int cantidadDeProductosDeCompraEsperada = 2;
            Assert.AreEqual(montoCompra, devueltaPorLogica.Monto);
            Assert.AreEqual(cantidadDeProductosDeCompraEsperada, devueltaPorLogica.Productos.Count());
            mock.VerifyAll();

        }


        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetComprasPorCodigoDeSeguimientoValidoPeroInexistente()
        {
            int codigoSeguimientoValidoInexistente = 123456;
            mock.Setup(x => x.GetComprasPorCodigoDeSeguimiento(codigoSeguimientoValidoInexistente)).Throws(new NotFoundException());
           
            servicio.GetCompraPorCodigoDeSeguimiento(codigoSeguimientoValidoInexistente);
            mock.VerifyAll();

        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void GetComprasPorCodigoDeSeguimientoInvalido()
        {
            int codigoSeguimientoInvalido = 123;
            servicio.GetCompraPorCodigoDeSeguimiento(codigoSeguimientoInvalido);

        }



        [TestMethod]
        public void InsertarCompra()
        {
            mockFarmaciaServicio.Setup(x => x.GetFarmacias()).Returns(farmacias);
            mock.Setup(x => x.InsertarCompra(It.IsAny<Compra>(), It.IsAny<Farmacia>()));
            Assert.AreEqual(unaCompraDto.CodigoSeguimiento,servicio.InsertarCompra(unaCompraDto).Codigo);
            mock.VerifyAll();
        }



        [TestMethod]
        public void InsertarCompraMasDeUnProducto()
        {
            Producto nuevoProducto = new Medicamento()
            {
                Id = 1,
                Precio = 100,
                Stock= 150
            };
            CantidadProductosCompra otraCantProductos = new CantidadProductosCompra()
            {
                Producto = nuevoProducto,

                Cantidad = 3
            };

            unaFarmacia.Productos.Add(nuevoProducto);
            unaCompraDto.Productos.Add(otraCantProductos);
            mockFarmaciaServicio.Setup(x => x.GetFarmacias()).Returns(farmacias);
            mock.Setup(x => x.InsertarCompra(It.IsAny<Compra>(), It.IsAny<Farmacia>()));
            servicio.InsertarCompra(unaCompraDto);
            mock.VerifyAll();
        }


        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod]
        public void InsertarCompraNula()
        {
            unaCompraDto = null;
            
            mock.Setup(x => x.InsertarCompra(unaCompra, unaFarmacia));
            servicio.InsertarCompra(unaCompraDto);
            mock.VerifyAll();
            mockFarmaciaServicio.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertarCompraSinProductos()
        {
            unaCompraDto.Productos = null;
            
            mock.Setup(x => x.InsertarCompra(unaCompra, unaFarmacia));
            servicio.InsertarCompra(unaCompraDto);
            mock.VerifyAll();
            mockFarmaciaServicio.VerifyAll();
        }




   


        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertarCompraMailInvalido()
        {
            unaCompraDto.MailComprador = "jpnecasek";
            
            mock.Setup(x => x.InsertarCompra(unaCompra, unaFarmacia));
            servicio.InsertarCompra(unaCompraDto);
            mock.VerifyAll();
            mockFarmaciaServicio.VerifyAll();
        }

        [TestMethod]
        public void ComprasEnEneroFarmacia()
        {
            cantidadProductos.EstadoDeCompraProducto = Enumeradores.EstadoDeCompraProducto.Aceptada;
            cantidadProductos2.EstadoDeCompraProducto = Enumeradores.EstadoDeCompraProducto.Aceptada;
            mockSesionServicio.Setup(x => x.GetFarmaciaByToken(sesionDueno.Token)).Returns(unaFarmacia);
            mockSesionServicio.Setup(x => x.GetUsuarioByToken(sesionDueno.Token)).Returns(sesionDueno.UsuarioSesion);
            InfoComprasDto infoCompra = servicio.GetComprasPorRangoEnFarmacia(sesionDueno.Token, new DateTime(2022,1,1), new DateTime(2022, 2, 1));

            Assert.AreEqual(otraCompra.Monto, infoCompra.Total);
            Assert.AreEqual(1, infoCompra.ListaCompras.Count());
            Mock.VerifyAll();
        }

        [TestMethod]
        public void ComprasEnMesSinComprasFarmacia()
        {
            mockSesionServicio.Setup(x => x.GetFarmaciaByToken(sesionDueno.Token)).Returns(unaFarmacia);
            mockSesionServicio.Setup(x => x.GetUsuarioByToken(sesionDueno.Token)).Returns(sesionDueno.UsuarioSesion);
            InfoComprasDto infoCompra = servicio.GetComprasPorRangoEnFarmacia(sesionDueno.Token, new DateTime(2023, 1, 1), new DateTime(2023, 2, 1));

            Assert.AreEqual(0, infoCompra.Total);
            Assert.AreEqual(0, infoCompra.ListaCompras.Count());
            Mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void ComprasHastayDesdemal()
        {
            mockSesionServicio.Setup(x => x.GetFarmaciaByToken(sesionDueno.Token)).Returns(unaFarmacia);
            mockSesionServicio.Setup(x => x.GetUsuarioByToken(sesionDueno.Token)).Returns(sesionDueno.UsuarioSesion);
            InfoComprasDto infoCompra = servicio.GetComprasPorRangoEnFarmacia(sesionDueno.Token, new DateTime(2022, 5, 1), new DateTime(2022, 2, 1));

            Mock.VerifyAll();
        }

        [ExpectedException(typeof(NotEnoughPrivilegesException))]
        [TestMethod]
        public void RolDiferenteADueñoError()
        {
            mockSesionServicio.Setup(x => x.GetFarmaciaByToken(sesionEmpleado.Token)).Returns(unaFarmacia);
            mockSesionServicio.Setup(x => x.GetUsuarioByToken(sesionEmpleado.Token)).Returns(unEmpleado);
            InfoComprasDto infoCompra = servicio.GetComprasPorRangoEnFarmacia(sesionEmpleado.Token, new DateTime(2022, 1, 1), new DateTime(2022, 2, 1));

            Mock.VerifyAll();
        }




        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertarCompraInvalidaProductoSinStockSuficiente()
        {
            int cantidadSobrepassaStock = 150;
            cantidadProductos.Cantidad = cantidadSobrepassaStock;
            mockFarmaciaServicio.Setup(x => x.GetFarmacias()).Returns(farmacias);
            mock.Setup(x => x.InsertarCompra(unaCompra, unaFarmacia));
            servicio.InsertarCompra(unaCompraDto);
            mock.VerifyAll();
            mockFarmaciaServicio.VerifyAll();

        }




        [TestMethod]
        public void InsertarCompraProductoDeDistintaFarmacia()
        {

            CantidadProductosCompra otraCantProductos = new CantidadProductosCompra()
            {
                Producto = new Medicamento()
                {
                    Id = 1,
                    Precio = 100,
                },

                Cantidad = 3
            };
            unaCompra.Productos.Add(otraCantProductos);
            mockFarmaciaServicio.Setup(x => x.GetFarmacias()).Returns(farmacias);
            mock.Setup(x => x.InsertarCompra(It.IsAny<Compra>(), It.IsAny<Farmacia>()));
            servicio.InsertarCompra(unaCompraDto);
            mock.VerifyAll();
            mockFarmaciaServicio.VerifyAll();

        }


        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertarCompraInvalidaProductoDadoDeBaja()
        {
            producto.DadoDeBaja = true;
            mockFarmaciaServicio.Setup(x => x.GetFarmacias()).Returns(farmacias);
            mockFarmaciaServicio.Setup(x => x.GetFarmaciaById(unaFarmacia.Id)).Returns(unaFarmacia);
            mock.Setup(x => x.InsertarCompra(unaCompra, unaFarmacia));
            servicio.InsertarCompra(unaCompraDto);
            mock.VerifyAll();
            mockFarmaciaServicio.VerifyAll();
        }


        [TestMethod]
        public void AceptarORechazarMedicamentoSinError()
        {
            unaCompra.Id = 5;
            mock.Setup(x => x.GetById(unaCompra.Id)).Returns(unaCompra);
            mockSesionServicio.Setup(x => x.GetFarmaciaByToken(validToken)).Returns(unaFarmacia);
            mockProducto.Setup(x => x.UpdateProducto(unaCompra.Productos[0].Producto));
            mock.Setup(x => x.UpdateEstadoProducuto(unaCompra.Productos[0]));
            int expectedValue = unaCompra.Productos[0].Producto.Stock - unaCompra.Productos[0].Cantidad;
            servicio.AceptarORechazarMedicamento(unaCompra.Id, unaCompra.Productos[0].Producto.Codigo, Enumeradores.EstadoDeCompraProducto.Aceptada, validToken);

            Assert.AreEqual(expectedValue, unaCompra.Productos[0].Producto.Stock);
            Assert.AreEqual(Enumeradores.EstadoDeCompraProducto.Aceptada, unaCompra.Productos[0].EstadoDeCompraProducto);

            mock.VerifyAll();
            mockProducto.VerifyAll();
            mockSesionServicio.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void AceptarORechazarMedicamentoYaAceptado()
        {
            unaCompra.Id = 5;
            unaCompra.Productos[0].EstadoDeCompraProducto = Enumeradores.EstadoDeCompraProducto.Aceptada;

            mock.Setup(x => x.GetById(unaCompra.Id)).Returns(unaCompra);
            mockSesionServicio.Setup(x => x.GetFarmaciaByToken(validToken)).Returns(unaFarmacia);

            servicio.AceptarORechazarMedicamento(unaCompra.Id, unaCompra.Productos[0].Producto.Codigo, Enumeradores.EstadoDeCompraProducto.Aceptada, validToken);

            mock.VerifyAll();
            mockSesionServicio.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void AceptarORechazarMedicamentoAEstadoPendiente()
        {
            unaCompra.Id = 5;

            mock.Setup(x => x.GetById(unaCompra.Id)).Returns(unaCompra);
            mockSesionServicio.Setup(x => x.GetFarmaciaByToken(validToken)).Returns(unaFarmacia);

            servicio.AceptarORechazarMedicamento(unaCompra.Id, unaCompra.Productos[0].Producto.Codigo, Enumeradores.EstadoDeCompraProducto.Pendiente, validToken);

            mock.VerifyAll();
            mockSesionServicio.VerifyAll();
        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void AceptarORechazarMedicamentoInexistenteEnCompra()
        {
            unaCompra.Id = 5;

            mock.Setup(x => x.GetById(unaCompra.Id)).Returns(unaCompra);
            mockSesionServicio.Setup(x => x.GetFarmaciaByToken(validToken)).Returns(unaFarmacia);

            servicio.AceptarORechazarMedicamento(unaCompra.Id, "CodInexistente", Enumeradores.EstadoDeCompraProducto.Aceptada, validToken);

            mock.VerifyAll();
            mockSesionServicio.VerifyAll();
        }


    }


}
