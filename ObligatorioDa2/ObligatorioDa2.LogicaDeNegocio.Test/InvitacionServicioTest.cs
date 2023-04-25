using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ObligatorioDa2.Domain.Entidades;
using System.Collections.Generic;
using ObligatorioDa2.IAccesoDatos;
using ObligatorioDa2.Exceptions;
using System;
using ObligatorioDa2.Domain.Util;
using static ObligatorioDa2.Domain.Util.Enumeradores;
using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.ILogicaDeNegocio;

namespace ObligatorioDa2.LogicaDeNegocio.Test
{
    [TestClass]
    public class InvitacionServicioTest
    {
        private Mock<IInvitacionRepository> mock;
        private Mock<ISesionServicio> mockSesion;
        private InvitacionServicio servicio;
        private Invitacion invitacionValida;
        private Invitacion nullInvitacion;
        private IEnumerable<Invitacion> invitaciones;
        private Invitacion invalidNombreUsuarioInvitacion;
        private Invitacion invalidRolInvitacion;
        private Invitacion nombreUsuarioRepetidoInvitacion;
        private Invitacion invitacionSinFarmacia;
        private Invitacion invitacionAdminConFarmacia;
        private Farmacia farmacia;
        private int codigoInvalido;
        private string nombreInvalido;
        private int codigoValido;
        private string nombreValido;
        private Dueño unDueño;
        private Administrador unAdmin;
        private const string tokenvalido = "tokenValido";
        [TestInitialize]
        public void InitTest()
        {
            mock = new Mock<IInvitacionRepository>(MockBehavior.Strict);
            mockSesion = new Mock<ISesionServicio>(MockBehavior.Strict);
            servicio = new InvitacionServicio(mock.Object, mockSesion.Object);
            farmacia = new Farmacia();
            codigoInvalido = 8728372;
            nombreInvalido = "nombre raro";
            nombreValido = "Palgom";
            invitacionValida = new Invitacion(nombreValido, (Enumeradores.Rol)1, farmacia);
            codigoValido = invitacionValida.Codigo;
            nullInvitacion = null;
            invitaciones = new List<Invitacion> { invitacionValida };
            invalidNombreUsuarioInvitacion = new Invitacion() { NombreDeUsuario = "" };
            invalidRolInvitacion = new Invitacion { NombreDeUsuario = "Juan53", RolInvitado = (Domain.Util.Enumeradores.Rol)4 };
            nombreUsuarioRepetidoInvitacion = new Invitacion { NombreDeUsuario = "Palgom", Farmacia = farmacia };
            invitacionSinFarmacia = new Invitacion
            {
                NombreDeUsuario = "Palgom2",
                RolInvitado = (Domain.Util.Enumeradores.Rol)1,
                Farmacia = null
            };
            invitacionAdminConFarmacia = new Invitacion
            {
                NombreDeUsuario = "Palgom2",
                RolInvitado = Rol.Administrador,
                Farmacia = farmacia
            };
            unDueño = new Dueño("mailValido@mail.com", "conTrase&aValida", "Direccion Valida", "NombreValido",
                farmacia);
            unAdmin = new Administrador("mailValido@mail.com", "conTrase&aValida", "Direccion Valida",
                "NombreValidoDiferente");
        }

        [TestMethod]
        public void GetInvitaciones()
        {
            mock.Setup(x => x.GetAll()).Returns(invitaciones);
            servicio.GetInvitaciones();
            mock.VerifyAll();
        }

        [TestMethod]
        public void InsertarInvitacionOk()
        {
            mock.Setup(x => x.Insertar(invitacionValida));
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(unDueño);
            mockSesion.Setup(x => x.GetFarmaciaByToken(tokenvalido)).Returns(farmacia);
            mock.Setup(x => x.ExisteInvitacionConNombre(invitacionValida.NombreDeUsuario)).Returns(false);

            servicio.InsertInvitacion(invitacionValida, tokenvalido);
            mock.VerifyAll();
        }
       
        [TestMethod]
        public void InsertarInvitacionDueñoFarmaciaDistinta()
        {
            mock.Setup(x => x.Insertar(invitacionValida));
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(unDueño);
            mockSesion.Setup(x => x.GetFarmaciaByToken(tokenvalido)).Returns(farmacia);
            mock.Setup(x => x.ExisteInvitacionConNombre(invitacionValida.NombreDeUsuario)).Returns(false);
            invitacionValida.Farmacia = new Farmacia("buen nombre", "otra direccion") {Id = 100};
            Invitacion laInvitacion =  servicio.InsertInvitacion(invitacionValida, tokenvalido);
            Assert.AreEqual(farmacia.Id, laInvitacion.Farmacia.Id );
            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertarInvitacionDueñoADueño()
        {
            mock.Setup(x => x.Insertar(invitacionValida));
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(unDueño);
            mock.Setup(x => x.ExisteInvitacionConNombre(invitacionValida.NombreDeUsuario)).Returns(false);
            invitacionValida.RolInvitado = Rol.Dueño;
            servicio.InsertInvitacion(invitacionValida, tokenvalido);
            mock.VerifyAll();
        }
        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertarInvitacionDueñoAadmin()
        {
            mock.Setup(x => x.Insertar(invitacionValida));
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(unDueño);
            mock.Setup(x => x.ExisteInvitacionConNombre(invitacionValida.NombreDeUsuario)).Returns(false);
            invitacionValida.RolInvitado = Rol.Administrador;
            servicio.InsertInvitacion(invitacionValida, tokenvalido);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertarInvitacionNombreInvalido()
        {
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(unAdmin);
            servicio.InsertInvitacion(invalidNombreUsuarioInvitacion, tokenvalido);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertarInvitacionRolInvalido()
        {
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(unAdmin);
            servicio.InsertInvitacion(invalidRolInvitacion, tokenvalido);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertarInvitacionEmpleadoSinFarmacia()
        {
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(unAdmin);
            invalidRolInvitacion.RolInvitado = Rol.Empleado;
            invalidRolInvitacion.Farmacia = null;
            servicio.InsertInvitacion(invalidRolInvitacion, tokenvalido);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertarInvitacionNombreUsuarioRepetido()
        {
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(unAdmin);
            mock.Setup(x => x.ExisteInvitacionConNombre(nombreUsuarioRepetidoInvitacion.NombreDeUsuario)).Returns(true);
            mock.Setup(x => x.GetAll()).Returns(invitaciones);
            servicio.InsertInvitacion(nombreUsuarioRepetidoInvitacion, tokenvalido);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertarInvitacionSinFarmacia()
        {
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(unAdmin);
            servicio.InsertInvitacion(invitacionSinFarmacia, tokenvalido);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertarInvitacionDeAdminConFarmacia()
        {
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(unAdmin);
            servicio.InsertInvitacion(invitacionAdminConFarmacia, tokenvalido);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void UsarInvitacionNombreIncorrecto()
        {

            mock.Setup(x => x.GetByNombre(It.IsAny<String>())).Returns(nullInvitacion);

            servicio.UsarInvitacion(nombreInvalido, codigoInvalido);
            mock.VerifyAll();
        }


        [TestMethod]
        public void UsarInvitacionValida()
        {

            mock.Setup(x => x.GetByNombre(nombreValido)).Returns(invitacionValida);
            mock.Setup(x => x.Update(invitacionValida));
            servicio.UsarInvitacion(nombreValido, codigoValido);
            Assert.AreEqual(invitacionValida.Usada, true);

        }
        [TestMethod]
        public void GetInvitacionValida()
        {

            mock.Setup(x => x.GetByNombre(nombreValido)).Returns(invitacionValida);

            servicio.GetByNombreYcodigo(nombreValido, codigoValido);
            mock.VerifyAll();

        }
        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetInvitacionInvalida()
        {
            int codigoInvalido = 87888;
            mock.Setup(x => x.GetByNombre(nombreValido)).Returns(invitacionValida);

            servicio.GetByNombreYcodigo(nombreValido, codigoInvalido);
            mock.VerifyAll();

        }
        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void UsarInvitacionCodigoIncorrecto()
        {

            mock.Setup(x => x.GetByNombre(nombreValido)).Returns(invitacionValida);

            servicio.UsarInvitacion(nombreValido, codigoInvalido);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod]
        public void InsertNullInvitacion()
        {
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(unAdmin);
            servicio.InsertInvitacion(nullInvitacion, tokenvalido);
            mock.VerifyAll();
        }

        
        [TestMethod]
        public void ConseguirInvitacionFiltradasAdminOK()
        {
            List<Invitacion> listaInvitaciones = (List<Invitacion>) invitaciones;
          
            listaInvitaciones.Add(new Invitacion("unNombreValido", Rol.Dueño, farmacia));

            mock.Setup(x => x.GetAll()).Returns(listaInvitaciones);
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(new Administrador());

            
            InvitacionFiltroDTO filtros = new InvitacionFiltroDTO(farmacia.Id, nombreValido, Rol.Empleado, tokenvalido);

            List<Invitacion> invitacionesFiltradas = servicio.ConseguirInvitacionesFiltradas(filtros);

            Assert.AreEqual(invitacionValida.Codigo, invitacionesFiltradas[0].Codigo);

            mock.VerifyAll();
        }

        [TestMethod]
        public void ConseguirInvitacionesFiltradasAdminSinNombreOK()
        {
            List<Invitacion> listaInvitaciones = (List<Invitacion>)invitaciones;
        
            Invitacion segundaInvitacion = new Invitacion("unNombreValido", Rol.Empleado, farmacia);
            listaInvitaciones.Add(segundaInvitacion);

            mock.Setup(x => x.GetAll()).Returns(listaInvitaciones);
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(new Administrador());

            InvitacionFiltroDTO filtros = new InvitacionFiltroDTO(farmacia.Id, null, Rol.Empleado, tokenvalido);

            List<Invitacion> invitacionesFiltradas = servicio.ConseguirInvitacionesFiltradas(filtros);

            Assert.AreEqual(invitacionValida.Codigo, invitacionesFiltradas[0].Codigo);
            Assert.AreEqual(segundaInvitacion.Codigo, invitacionesFiltradas[1].Codigo);

            mock.VerifyAll();
        }
        [TestMethod]
        public void ConseguirInvitacionesFiltradasAdminSinRolOK()
        {
            List<Invitacion> listaInvitaciones = (List<Invitacion>)invitaciones;
         
            Invitacion segundaInvitacion = new Invitacion("unNombreValido", Rol.Empleado, farmacia);
            listaInvitaciones.Add(segundaInvitacion);

            mock.Setup(x => x.GetAll()).Returns(listaInvitaciones);
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(new Administrador());

            InvitacionFiltroDTO filtros = new InvitacionFiltroDTO(farmacia.Id, null, null, tokenvalido);

            List<Invitacion> invitacionesFiltradas = servicio.ConseguirInvitacionesFiltradas(filtros);

            Assert.AreEqual(invitacionValida.Codigo, invitacionesFiltradas[0].Codigo);
            Assert.AreEqual(segundaInvitacion.Codigo, invitacionesFiltradas[1].Codigo);

            mock.VerifyAll();
        }
        [TestMethod]
        public void ConseguirInvitacionesFiltradasAdminSinFarmaciaOK()
        {
            List<Invitacion> listaInvitaciones = (List<Invitacion>)invitaciones;
           
            Invitacion segundaInvitacion = new Invitacion("unNombreValido", Rol.Empleado, farmacia);
            listaInvitaciones.Add(segundaInvitacion);

            mock.Setup(x => x.GetAll()).Returns(listaInvitaciones);
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(new Administrador());

            InvitacionFiltroDTO filtros = new InvitacionFiltroDTO(null, "unNombreValido", null, tokenvalido);

            List<Invitacion> invitacionesFiltradas = servicio.ConseguirInvitacionesFiltradas(filtros);

            Assert.AreEqual(segundaInvitacion.Codigo, invitacionesFiltradas[0].Codigo);
            

            mock.VerifyAll();
        }

        [TestMethod]
        public void ConseguirInvitacionesFiltradasAdminNoOK()
        {
            List<Invitacion> listaInvitaciones = (List<Invitacion>)invitaciones;

            Invitacion segundaInvitacion = new Invitacion("unNombreValido", Rol.Empleado, farmacia);
            listaInvitaciones.Add(segundaInvitacion);

            mock.Setup(x => x.GetAll()).Returns(listaInvitaciones);
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(new Administrador());

            InvitacionFiltroDTO filtros = new InvitacionFiltroDTO(farmacia.Id, "unNombreInexistente", Rol.Empleado, tokenvalido);

            List<Invitacion> invitacionesFiltradas = servicio.ConseguirInvitacionesFiltradas(filtros);

            Assert.AreEqual(0, invitacionesFiltradas.Count);

            mock.VerifyAll();
        }
        [ExpectedException(typeof(NotEnoughPrivilegesException))]
        [TestMethod]
        public void ConseguirInvitacionesFiltradasEmpleadoNoOK()
        {
            
            mock.Setup(x => x.GetAll()).Returns(new List<Invitacion>());
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenvalido)).Returns(new Empleado());

            InvitacionFiltroDTO filtros = new InvitacionFiltroDTO(farmacia.Id, "unNombreInexistente", Rol.Empleado, tokenvalido);

            servicio.ConseguirInvitacionesFiltradas(filtros);

            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void ActualizarInvitacionUsada()
        {
            invitacionValida.Usada = true;
            Invitacion invitacionActualizada = invitacionValida;
            invitacionActualizada.NombreDeUsuario = "nuevoNombre";

            mock.Setup(x => x.GetById(It.IsAny<int>())).Returns(invitacionValida);

            servicio.EditarInvitacion(invitacionActualizada, false);

            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void ActualizarInvitacionComoUsada()
        {
            invitacionValida.Usada = false;
            Invitacion invitacionActualizada = invitacionValida;
            invitacionActualizada.Usada = true;

            mock.Setup(x => x.GetById(It.IsAny<int>())).Returns(invitacionValida);

            servicio.EditarInvitacion(invitacionActualizada, false);

            mock.VerifyAll();
        }
        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void ActualizarInvitacionQueEstaUsada()
        {
            invitacionValida.Usada = false;
            Invitacion invitacionActualizada = new Invitacion("nombreValido", Rol.Empleado, farmacia){Id = invitacionValida.Id, Usada = true};

            mock.Setup(x => x.GetById(It.IsAny<int>())).Returns(invitacionValida);

            servicio.EditarInvitacion(invitacionActualizada, false);

            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void ActualizarInvitacionNombreRepetido()
        {
            Invitacion invitacionActualizada = invitacionValida;
            invitacionActualizada.NombreDeUsuario = "NombreRepetido";

            mock.Setup(x => x.GetById(It.IsAny<int>())).Returns(invitacionValida);
            mock.Setup(x => x.ExisteOtraInvitacionConEsteNombre(It.IsAny<Invitacion>())).Returns(true);

            servicio.EditarInvitacion(invitacionActualizada, false);

            mock.VerifyAll();
        }

        [TestMethod]
        public void ActualizarInvitacionOk()
        {
            Invitacion invitacionActualizada = invitacionValida;
            invitacionActualizada.NombreDeUsuario = "OtroNombre";

            mock.Setup(x => x.GetById(It.IsAny<int>())).Returns(invitacionValida);
            mock.Setup(x => x.ExisteOtraInvitacionConEsteNombre(It.IsAny<Invitacion>())).Returns(false);
            mock.Setup(x => x.Update(invitacionActualizada));

            Invitacion invitacion = servicio.EditarInvitacion(invitacionActualizada, false);

            mock.VerifyAll();
            Assert.AreEqual(invitacionActualizada, invitacion);
        }

        [TestMethod]
        public void ActualizarInvitacionOkConNuevoCodigo()
        {
            Invitacion invitacionActualizada = invitacionValida;
            invitacionActualizada.NombreDeUsuario = "OtroNombre";

            mock.Setup(x => x.GetById(It.IsAny<int>())).Returns(invitacionValida);
            mock.Setup(x => x.ExisteOtraInvitacionConEsteNombre(It.IsAny<Invitacion>())).Returns(false);
            mock.Setup(x => x.Update(invitacionActualizada));

            Invitacion invitacion = servicio.EditarInvitacion(invitacionActualizada, true);
            invitacionActualizada.Codigo = invitacion.Codigo;

            mock.VerifyAll();
            Assert.AreEqual(invitacionActualizada, invitacion);
        }
    }
}