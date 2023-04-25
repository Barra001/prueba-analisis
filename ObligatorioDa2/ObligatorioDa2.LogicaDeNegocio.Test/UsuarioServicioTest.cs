using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;
using System;
using System.Collections.Generic;
using ObligatorioDa2.Domain.Util;
using ObligatorioDa2.ILogicaDeNegocio;

namespace ObligatorioDa2.LogicaDeNegocio.Test
{
    [TestClass]
    public class UsuarioServicioTest
    {

        private Mock<IUsuarioRepository> mock;
        private UsuarioServicio servicio;
        private Usuario usuarioAdministradorValido;
        private IEnumerable<Usuario> usuarios;
        private Usuario nullUsuario;
        private Invitacion invitacionValida;
        private int codigoValido;
        private Mock<IInvitacionServicio> mockInvitacionService;

        [TestInitialize]
        public void InitTest()
        {
            mockInvitacionService = new Mock<IInvitacionServicio>(MockBehavior.Strict);
            codigoValido = 73648;
            mock = new Mock<IUsuarioRepository>(MockBehavior.Strict);
            servicio = new UsuarioServicio(mock.Object, mockInvitacionService.Object);
            usuarioAdministradorValido = new Administrador()
            {
                NombreDeUsuario = "Palgom",
                Mail= "jpnecasek@gmail.com",
                Contrasena= "Juanjuan!",
                Direccion= "Ellauri 245",
                FechaDeRegistro= DateTime.Now,                  
            };
            invitacionValida = new Invitacion("Palgom", Enumeradores.Rol.Administrador, null);
            invitacionValida.Codigo = codigoValido;
            nullUsuario = null;
            usuarios = new List<Usuario> { usuarioAdministradorValido };
           
        }

        [TestMethod]
        public void GetUsuarios()
        {
            mock.Setup(x => x.GetAll()).Returns(usuarios);
            servicio.GetUsuarios();
            mock.VerifyAll();
        }

        [TestMethod]
        public void GetUsuarioByNombre()
        {
            mock.Setup(x => x.GetByNombre(usuarioAdministradorValido.NombreDeUsuario)).Returns(usuarioAdministradorValido);
            servicio.GetUsuarioByNombre(usuarioAdministradorValido.NombreDeUsuario);
            mock.VerifyAll();
        }


        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod]
        public void InsertUsuarioNulo()
        {
            servicio.InsertUsuario(nullUsuario, codigoValido);
            mock.VerifyAll();
        }

        [TestMethod]
        public void InsertUsuarioOk()
        {
            mockInvitacionService
                .Setup(x => x.GetByNombreYcodigo(usuarioAdministradorValido.NombreDeUsuario, codigoValido))
                .Returns(invitacionValida);
            mock.Setup(x => x.ExisteUsuarioConMail(usuarioAdministradorValido.Mail)).Returns(false);
            mock.Setup(x => x.Insertar(usuarioAdministradorValido));
            mockInvitacionService.Setup(x => x.UsarInvitacion(usuarioAdministradorValido.NombreDeUsuario, codigoValido)).Returns(invitacionValida);
            servicio.InsertUsuario(usuarioAdministradorValido, codigoValido);
            mock.VerifyAll();
        }
     


        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertUsuarioEmailInvalido()
        {
            mockInvitacionService
                .Setup(x => x.GetByNombreYcodigo(usuarioAdministradorValido.NombreDeUsuario, codigoValido))
                .Returns(invitacionValida);
            usuarioAdministradorValido.Mail = "jpnecasekgmail";
            servicio.InsertUsuario(usuarioAdministradorValido, codigoValido);
            mock.VerifyAll();
        }
        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertUsuarioRolInvalido()
        {
            Dueño unDueño = new Dueño("mail@valido.com", "passwordValida%&", "DirecValida", "nombreDueño",
                new Farmacia());
            mockInvitacionService
                .Setup(x => x.GetByNombreYcodigo(unDueño.NombreDeUsuario, codigoValido))
                .Returns(invitacionValida);

            servicio.InsertUsuario(unDueño, codigoValido);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertUsuarioEmailEnUso()
        {
            Usuario usuarioEmpleadoMailRepetido = new Administrador()
            {
                NombreDeUsuario = "Juan",
                Mail = "jpnecasek@gmail.com",
                Contrasena = "Juan5678!",
                Direccion = "Ellauri 250",
                FechaDeRegistro = DateTime.Now,
            };
            mockInvitacionService
                .Setup(x => x.GetByNombreYcodigo(usuarioEmpleadoMailRepetido.NombreDeUsuario, codigoValido))
                .Returns(invitacionValida);
            mock.Setup(x => x.ExisteUsuarioConMail(usuarioEmpleadoMailRepetido.Mail)).Returns(true);
            servicio.InsertUsuario(usuarioEmpleadoMailRepetido, codigoValido);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertUsuarioContrasenaInvalidaSinCaracterEspecial()
        {
            mockInvitacionService
                .Setup(x => x.GetByNombreYcodigo(usuarioAdministradorValido.NombreDeUsuario, codigoValido))
                .Returns(invitacionValida);
            usuarioAdministradorValido.Contrasena = "juanjuan";            
            servicio.InsertUsuario(usuarioAdministradorValido, codigoValido);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertUsuarioContrasenaInvalidaMenosDeOchoCaracteres()
        {
            mockInvitacionService
                .Setup(x => x.GetByNombreYcodigo(usuarioAdministradorValido.NombreDeUsuario, codigoValido))
                .Returns(invitacionValida);

            usuarioAdministradorValido.Contrasena = "juanua!";           
            servicio.InsertUsuario(usuarioAdministradorValido, codigoValido);
            mock.VerifyAll();
        }


        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertUsuarioDireccionInvalidaVacia()
        {
            mockInvitacionService
                .Setup(x => x.GetByNombreYcodigo(usuarioAdministradorValido.NombreDeUsuario, codigoValido))
                .Returns(invitacionValida);
            usuarioAdministradorValido.Direccion = "";
            servicio.InsertUsuario(usuarioAdministradorValido, codigoValido);
            mock.VerifyAll();
        }
        [ExpectedException(typeof(AccesoDatosException))]
        [TestMethod]
        public void InsertUsuarioNoInvitado()
        {
            int codigoInvalido = 3546354;
            mockInvitacionService
                .Setup(x => x.GetByNombreYcodigo(usuarioAdministradorValido.NombreDeUsuario, codigoInvalido))
                .Throws(new AccesoDatosException(""));
            mock.Setup(x => x.ExisteUsuarioConMail(usuarioAdministradorValido.Mail)).Returns(false);
            mockInvitacionService.Setup(x =>
                x.UsarInvitacion(usuarioAdministradorValido.NombreDeUsuario, codigoInvalido)).Throws(new BusinessLogicException());

            servicio.InsertUsuario(usuarioAdministradorValido, codigoInvalido);
            mock.VerifyAll();
        }



    }
}
