using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Domain.Util;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;
using ObligatorioDa2.ILogicaDeNegocio;
using System;
using System.Collections.Generic;

namespace ObligatorioDa2.LogicaDeNegocio.Test
{
    [TestClass]
    public class SolicitudDeReposicionServicioTest
    {
        private Mock<IAccesoDatos.ISolicitudDeReposicionRepository> mockSolicitud;
        private Mock<IAccesoDatos.IProductoRepository> mockProducto;
        private Mock<IAccesoDatos.ISesionRepository> mockSesion;
        private Mock<IUsuarioRepository> mockUsuario;
        private Mock<IFarmaciaServicio> mockServicioFarmacia;

        private ProductoServicio productoServicio;
        private SesionServicio sesionServicio;
        private SolicitudDeReposicionServicio servicio;
        private SolicitudDeReposicion solicitudDeReposicion;
        private CantidadProductos cantidadProductos;
        private Empleado solicitante;
        private List<SolicitudDeReposicion> solicitudesDeReposicion;
        private SolicitudDeReposicion solicitudNula;
        private Producto producto;
        private Farmacia farmacia;
        private Sesion sesion;
        private Dueño unDueño;
        private Administrador unAdmin;

        [TestInitialize]
        public void InitTest()
        {
            
            mockSolicitud = new Mock<IAccesoDatos.ISolicitudDeReposicionRepository>(MockBehavior.Strict);
            mockProducto = new Mock<IAccesoDatos.IProductoRepository>(MockBehavior.Strict);
            mockSesion = new Mock<IAccesoDatos.ISesionRepository>(MockBehavior.Strict);
            mockUsuario= new Mock<IUsuarioRepository>(MockBehavior.Strict);
            mockServicioFarmacia = new Mock<IFarmaciaServicio>(MockBehavior.Strict);
            sesionServicio = new SesionServicio(mockSesion.Object, mockUsuario.Object);
            productoServicio = new ProductoServicio(mockProducto.Object, sesionServicio, mockServicioFarmacia.Object);
            servicio = new SolicitudDeReposicionServicio(mockSolicitud.Object, productoServicio, sesionServicio);
           
            producto = new Medicamento()
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
            cantidadProductos = new CantidadProductos()
            {
                Producto = producto,
                Cantidad = 2

            };
            solicitudDeReposicion = new SolicitudDeReposicion()
            {
                Solicitante = 1,
                Productos = new List<CantidadProductos> { cantidadProductos },
                FechaDeEmision = new DateTime(2022, 11, 1)

            };
            farmacia = new Farmacia()
            {
                Nombre = "Farmashop",
                Direccion = "21 de Septiembre 1900",
                SolicitudesDeReposicion = new List<SolicitudDeReposicion>(){solicitudDeReposicion},
                Productos = new List<Producto>(){producto}
                
            };
           
          
            solicitante = new Empleado()
            {
                Farmacia=farmacia,
                Id = 1
            };

            sesion = new Sesion()
            {
                UsuarioSesion = solicitante,
                
            };

            
            unDueño = new Dueño()
            {
                Contrasena = "contraseÑaValida&",
                Direccion = "Direccion Valida",
                Farmacia = farmacia,
                Id = 2,
                Mail = "mail@valido.com",
                NombreDeUsuario = "Pepe123"
            };
          
            unAdmin = new Administrador()
            {
                Contrasena = "$contraseÑaValida&",
                Direccion = "Direccion Valida",
                Id = 3,
                Mail = "mailAdmin@valido.com",
                NombreDeUsuario = "Admin123"
            };

            solicitudesDeReposicion = new List<SolicitudDeReposicion> { solicitudDeReposicion };
            solicitudNula = null;
        }

        [TestMethod]
        public void GetSolicitudesEmpleado()
        {
            mockSesion.Setup(x => x.GetUsuarioByToken(It.IsAny<string>())).Returns(solicitante);
            mockSolicitud.Setup(x => x.GetSolicitudesDe(solicitante)).Returns(solicitudesDeReposicion);

            servicio.GetSolicitudes(It.IsAny<string>());
            mockSolicitud.VerifyAll();
        }
        [TestMethod]
        public void GetSolicitudesDueño()
        {
            mockSesion.Setup(x => x.GetUsuarioByToken(It.IsAny<string>())).Returns(unDueño);
            mockSesion.Setup(x => x.GetFarmaciaByToken(It.IsAny<string>())).Returns(farmacia);

            List<SolicitudDeReposicion> listaDada = (List<SolicitudDeReposicion>) servicio.GetSolicitudes(It.IsAny<string>());
            Assert.AreEqual(farmacia.SolicitudesDeReposicion[0], listaDada[0]);
           
        }
        [ExpectedException(typeof(NotEnoughPrivilegesException))]
        [TestMethod]
        public void GetSolicitudesAdmin()
        {
            mockSesion.Setup(x => x.GetUsuarioByToken(It.IsAny<string>())).Returns(unAdmin);
            servicio.GetSolicitudes(It.IsAny<string>());
            mockSolicitud.VerifyAll();

        }

        [TestMethod]
        public void InsertarSolicitudOk()
        {
            
            mockSesion.Setup(x => x.GetFarmaciaByToken(sesion.Token)).Returns(farmacia);
            mockSesion.Setup(x => x.GetUsuarioByToken(sesion.Token)).Returns(solicitante);
            mockSolicitud.Setup(x => x.InsertarSolicitud(solicitudDeReposicion,farmacia));
      
            servicio.InsertarSolicitud(solicitudDeReposicion, sesion.Token);
            mockSolicitud.VerifyAll();
        }


        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod]
        public void InsertarSolicitudProductoNull()
        {

            mockSesion.Setup(x => x.GetFarmaciaByToken(sesion.Token)).Returns(farmacia);
            mockSesion.Setup(x => x.GetUsuarioByToken(sesion.Token)).Returns(solicitante);
            solicitudDeReposicion.Productos[0].Producto = null;
            servicio.InsertarSolicitud(solicitudDeReposicion, sesion.Token);
            mockSesion.VerifyAll();
        }




        [ExpectedException(typeof(NotEnoughPrivilegesException))]
        [TestMethod]
        public void InsertarSolicitudDueño()
        {

       
            mockSesion.Setup(x => x.GetUsuarioByToken(sesion.Token)).Returns(unDueño);
        

            servicio.InsertarSolicitud(solicitudDeReposicion, sesion.Token);
            mockSolicitud.VerifyAll();
        }


        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod]
        public void InsertarSolicitudNula()
        {
            mockSolicitud.Setup(x => x.InsertarSolicitud(solicitudNula,solicitante.Farmacia));
            servicio.InsertarSolicitud(solicitudNula, sesion.Token);
            mockSolicitud.VerifyAll();
        }

        

        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod]
        public void InsertarSolicitudSinPeticiones()
        {
            solicitudDeReposicion.Productos = null;
            servicio.InsertarSolicitud(solicitudDeReposicion, sesion.Token);
            mockSolicitud.VerifyAll();
        }
        [TestMethod]
        public void AceptarSolicitud()
        {
            solicitudDeReposicion.EstadoDeSolicitud = Enumeradores.EstadoDeSolicitud.Pendiente;
            mockSolicitud.Setup(x => x.Update(It.IsAny<SolicitudDeReposicion>()));
            producto.Id = 1;
            mockSolicitud.Setup(x => x.GetById(solicitudDeReposicion.Id)).Returns(solicitudDeReposicion);
            mockProducto.Setup(x => x.Update(It.IsAny<Medicamento>()));
            mockProducto.Setup(x => x.GetById(It.IsAny<int>())).Returns(producto);
            producto.Stock = 0;


            servicio.ActualizarSolicitud(solicitudDeReposicion.Id, Enumeradores.EstadoDeSolicitud.Aceptada);
            mockProducto.VerifyAll();
            Assert.AreEqual(Enumeradores.EstadoDeSolicitud.Aceptada, solicitudDeReposicion.EstadoDeSolicitud);
            Assert.AreEqual(solicitudDeReposicion.Productos[0].Cantidad, producto.Stock);
            
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void ActualizarSolicitudEstadoNoPendiente()
        {
            solicitudDeReposicion.EstadoDeSolicitud = Enumeradores.EstadoDeSolicitud.Aceptada;
            mockSolicitud.Setup(x => x.GetById(solicitudDeReposicion.Id)).Returns(solicitudDeReposicion);

            servicio.ActualizarSolicitud(solicitudDeReposicion.Id, Enumeradores.EstadoDeSolicitud.Aceptada);
            mockProducto.VerifyAll();


        }
        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void AceptarSolicitudEstadoErroneo()
        {
            mockSolicitud.Setup(x => x.Update(It.IsAny<SolicitudDeReposicion>()));
            producto.Id = 1;
            mockSolicitud.Setup(x => x.GetById(producto.Id)).Returns(solicitudDeReposicion);
            mockProducto.Setup(x => x.Update(It.IsAny<Medicamento>()));
            mockProducto.Setup(x => x.GetById(It.IsAny<int>())).Returns(producto);
            producto.Stock = 0;


            servicio.ActualizarSolicitud(producto.Id, Enumeradores.EstadoDeSolicitud.Pendiente);
            mockSolicitud.VerifyAll();

        }

        [TestMethod]
        public void RechazarSolicitud()
        {
            mockSolicitud.Setup(x => x.Update(It.IsAny<SolicitudDeReposicion>()));
            producto.Id = 1;
            mockSolicitud.Setup(x => x.GetById(1)).Returns(solicitudDeReposicion);
            servicio.ActualizarSolicitud(producto.Id, Enumeradores.EstadoDeSolicitud.Rechazada);
            Assert.AreEqual(Enumeradores.EstadoDeSolicitud.Rechazada, solicitudDeReposicion.EstadoDeSolicitud);
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertarSolicitudConPeticionInvalida()
        {
            solicitudDeReposicion.Productos[0].Cantidad=0;
            servicio.InsertarSolicitud(solicitudDeReposicion,sesion.Token);
            mockSolicitud.VerifyAll();
        }

        [TestMethod]
        public void GetSolicitudesConFiltrosOk()
        {
            string tokenDeEmpleado = sesion.Token;
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenDeEmpleado)).Returns(solicitante);
            mockSesion.Setup(x => x.GetFarmaciaByToken(tokenDeEmpleado)).Returns(farmacia);

            DateTime desde = solicitudDeReposicion.FechaDeEmision.AddDays(-5);
            DateTime hasta = solicitudDeReposicion.FechaDeEmision.AddDays(5);
            string codigoMedicamento = producto.Codigo;
            Enumeradores.EstadoDeSolicitud estadoSolicitud = solicitudDeReposicion.EstadoDeSolicitud;

            List<SolicitudDeReposicion> respuestaEsperada = new List<SolicitudDeReposicion>() {solicitudDeReposicion};

            SolicitudFiltrosDTO nuevoFiltrosDto =
                new SolicitudFiltrosDTO(tokenDeEmpleado, desde, hasta, codigoMedicamento, estadoSolicitud);

            List<SolicitudDeReposicion> respuestaObtenida = servicio.GetSolicitudesConFiltros(nuevoFiltrosDto);

            Assert.AreEqual(respuestaEsperada.Count, respuestaObtenida.Count);
            Assert.AreEqual(solicitudDeReposicion.Id, respuestaObtenida[0].Id);
        }
        [TestMethod]
        public void GetSolicitudesConAlgunosFiltrosOk()
        {
            string tokenDeEmpleado = sesion.Token;
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenDeEmpleado)).Returns(solicitante);
            mockSesion.Setup(x => x.GetFarmaciaByToken(tokenDeEmpleado)).Returns(farmacia);

            DateTime? desde = null;
            DateTime? hasta = null;

            string codigoMedicamento = producto.Codigo;
            Enumeradores.EstadoDeSolicitud estadoSolicitud = solicitudDeReposicion.EstadoDeSolicitud;

            List<SolicitudDeReposicion> respuestaEsperada = new List<SolicitudDeReposicion>() { solicitudDeReposicion };

            SolicitudFiltrosDTO nuevoFiltrosDto =
                new SolicitudFiltrosDTO(tokenDeEmpleado, desde, hasta, codigoMedicamento, estadoSolicitud);

            List<SolicitudDeReposicion> respuestaObtenida = servicio.GetSolicitudesConFiltros(nuevoFiltrosDto);

            Assert.AreEqual(respuestaEsperada.Count, respuestaObtenida.Count);
            Assert.AreEqual(solicitudDeReposicion.Id, respuestaObtenida[0].Id);
        }

        [TestMethod]
        public void GetSolicitudesConAlgunosOtrosFiltrosOk()
        {
            string tokenDeEmpleado = sesion.Token;
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenDeEmpleado)).Returns(solicitante);
            mockSesion.Setup(x => x.GetFarmaciaByToken(tokenDeEmpleado)).Returns(farmacia);

            DateTime? desde = null;
            DateTime? hasta = null;

            string codigoMedicamento = producto.Codigo;
            Enumeradores.EstadoDeSolicitud? estadoSolicitud = null;

            List<SolicitudDeReposicion> respuestaEsperada = new List<SolicitudDeReposicion>() { solicitudDeReposicion };

            SolicitudFiltrosDTO nuevoFiltrosDto =
                new SolicitudFiltrosDTO(tokenDeEmpleado, desde, hasta, codigoMedicamento, estadoSolicitud);

            List<SolicitudDeReposicion> respuestaObtenida = servicio.GetSolicitudesConFiltros(nuevoFiltrosDto);

            Assert.AreEqual(respuestaEsperada.Count, respuestaObtenida.Count);
            Assert.AreEqual(solicitudDeReposicion.Id, respuestaObtenida[0].Id);
        }

        [TestMethod]
        public void GetSolicitudesSinCodigoFiltrosOk()
        {
            string tokenDeEmpleado = sesion.Token;
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenDeEmpleado)).Returns(solicitante);
            mockSesion.Setup(x => x.GetFarmaciaByToken(tokenDeEmpleado)).Returns(farmacia);

            DateTime desde = solicitudDeReposicion.FechaDeEmision.AddDays(-5);
            DateTime hasta = solicitudDeReposicion.FechaDeEmision.AddDays(5);

            string codigoMedicamento = null;
            Enumeradores.EstadoDeSolicitud estadoSolicitud = solicitudDeReposicion.EstadoDeSolicitud;

            List<SolicitudDeReposicion> respuestaEsperada = new List<SolicitudDeReposicion>() { solicitudDeReposicion };

            SolicitudFiltrosDTO nuevoFiltrosDto =
                new SolicitudFiltrosDTO(tokenDeEmpleado, desde, hasta, codigoMedicamento, estadoSolicitud);

            List<SolicitudDeReposicion> respuestaObtenida = servicio.GetSolicitudesConFiltros(nuevoFiltrosDto);

            Assert.AreEqual(respuestaEsperada.Count, respuestaObtenida.Count);
            Assert.AreEqual(solicitudDeReposicion.Id, respuestaObtenida[0].Id);
        }

        [TestMethod]
        public void GetSolicitudesNoCumplenFiltros()
        {
            string tokenDeEmpleado = sesion.Token;
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenDeEmpleado)).Returns(solicitante);
            mockSesion.Setup(x => x.GetFarmaciaByToken(tokenDeEmpleado)).Returns(farmacia);

            DateTime desde = solicitudDeReposicion.FechaDeEmision.AddDays(-5);
            DateTime hasta = solicitudDeReposicion.FechaDeEmision.AddDays(5);

            string codigoMedicamento = "este es un codigo inexistente";

            Enumeradores.EstadoDeSolicitud estadoSolicitud = solicitudDeReposicion.EstadoDeSolicitud;

            List<SolicitudDeReposicion> respuestaEsperada = new List<SolicitudDeReposicion>();

            SolicitudFiltrosDTO nuevoFiltrosDto =
                new SolicitudFiltrosDTO(tokenDeEmpleado, desde, hasta, codigoMedicamento, estadoSolicitud);

            List<SolicitudDeReposicion> respuestaObtenida = servicio.GetSolicitudesConFiltros(nuevoFiltrosDto);

            Assert.AreEqual(respuestaEsperada.Count, respuestaObtenida.Count);
          
        }

        [TestMethod]
        public void GetSolicitudesNoOtrosFiltros()
        {
            string tokenDeEmpleado = sesion.Token;
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenDeEmpleado)).Returns(solicitante);
            mockSesion.Setup(x => x.GetFarmaciaByToken(tokenDeEmpleado)).Returns(farmacia);

            DateTime desde = solicitudDeReposicion.FechaDeEmision.AddDays(-5);
            DateTime hastaFechaAntes = solicitudDeReposicion.FechaDeEmision.AddDays(-1);

            string codigoMedicamento = producto.Codigo;

            Enumeradores.EstadoDeSolicitud estadoSolicitud = solicitudDeReposicion.EstadoDeSolicitud;

            List<SolicitudDeReposicion> respuestaEsperada = new List<SolicitudDeReposicion>();

            SolicitudFiltrosDTO nuevoFiltrosDto =
                new SolicitudFiltrosDTO(tokenDeEmpleado, desde, hastaFechaAntes, codigoMedicamento, estadoSolicitud);

            List<SolicitudDeReposicion> respuestaObtenida = servicio.GetSolicitudesConFiltros(nuevoFiltrosDto);

            Assert.AreEqual(respuestaEsperada.Count, respuestaObtenida.Count);

        }

        [ExpectedException(typeof(NotEnoughPrivilegesException))]
        [TestMethod]
        public void GetSolicitudesDeDueñoFiltros()
        {
            Sesion sesionDueño = new Sesion() { UsuarioSesion = solicitante, };
          
            mockSesion.Setup(x => x.GetUsuarioByToken(sesionDueño.Token)).Returns(unDueño);
            mockSesion.Setup(x => x.GetFarmaciaByToken(sesionDueño.Token)).Returns(farmacia);

            DateTime desde = solicitudDeReposicion.FechaDeEmision.AddDays(-5);
            DateTime hastaFechaAntes = solicitudDeReposicion.FechaDeEmision.AddDays(5);

            string codigoMedicamento = producto.Codigo;

            Enumeradores.EstadoDeSolicitud estadoSolicitud = solicitudDeReposicion.EstadoDeSolicitud;

            SolicitudFiltrosDTO nuevoFiltrosDto =
                new SolicitudFiltrosDTO(sesionDueño.Token, desde, hastaFechaAntes, codigoMedicamento, estadoSolicitud);

           servicio.GetSolicitudesConFiltros(nuevoFiltrosDto);

           mockSesion.VerifyAll();

        }
    }
}
