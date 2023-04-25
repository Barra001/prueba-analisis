using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObligatorioDa2.ILogicaDeNegocio;
using Moq;
using ObligatorioDa2.Api.Controllers;
using ObligatorioDa2.Domain.Entidades;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ObligatorioDa2.Exceptions;
using System;
using ObligatorioDa2.Domain.DTOs;

namespace ObligatorioDa2.Api.Test
{
    [TestClass]
    public class SesionControllerTest
    {
        private Mock<ISesionServicio> mock;
        private SesionesController api;
        private Sesion sesion;
        private IEnumerable<Sesion> sesiones;
        private Usuario usuario;
        private readonly string nombreDeUsuario = "Juan24";
        private readonly string contrasena = "juanjuan!";
        private readonly TokenDTO token = new TokenDTO(){Token = "validToken"};

        [TestInitialize]
        public void InitTest()
        {
            mock = new Mock<ISesionServicio>(MockBehavior.Strict);
            api = new SesionesController(mock.Object);
            sesion = new Sesion()
            {
                UsuarioSesion = usuario
            };
            sesiones = new List<Sesion>() { sesion };
            usuario = new Empleado()
            {
                NombreDeUsuario = nombreDeUsuario,
                Contrasena = contrasena
            };
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void LogInUsuarioBadRequest()
        {
            mock.Setup(x => x.LogIn(nombreDeUsuario, contrasena)).Throws(new BusinessLogicException());
            api.LogIn(usuario);
            
            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void LogInUsuarioBadRequestInDataAccess()
        {
            mock.Setup(x => x.LogIn(nombreDeUsuario, contrasena)).Throws(new AccesoDatosException(""));
            api.LogIn(usuario);


            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void LogInUsuarioFail()
        {
            mock.Setup(x => x.LogIn(nombreDeUsuario, contrasena)).Throws(new Exception());
            api.LogIn(usuario);
    

            mock.VerifyAll();
        }

        [TestMethod]
        public void LogInUsuarioOk()
        {
            mock.Setup(x => x.LogIn(nombreDeUsuario, contrasena)).Returns(token);
            IActionResult result = api.LogIn(usuario);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            Usuario body = objectResult.Value as Usuario;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void LogOutUsuarioNotFound()
        {
            mock.Setup(x => x.LogOut(token.Token)).Throws(new AccesoDatosException(""));
            api.LogOut(token.Token);

            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void LogOutUsuarioFail()
        {
            mock.Setup(x => x.LogOut(token.Token)).Throws(new Exception());
            api.LogOut(token.Token);


            mock.VerifyAll();
        }

        [TestMethod]
        public void LogOutUsuarioOk()
        {
            mock.Setup(x => x.LogOut(token.Token)).Returns(new MensajeLogOutDTO(){Mensaje = "LogOut exitoso" });
            IActionResult result = api.LogOut(token.Token);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
        }
    }
}
