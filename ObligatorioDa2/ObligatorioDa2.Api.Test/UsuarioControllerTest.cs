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
    public class UsuarioControllerTest
    {
      
        private Mock<ITrabajadorServicio> mockTrabajador;
        private Mock<IUsuarioServicio> mockUsuario;
        private UsuariosController api;
        private UsuarioDto user;
        private IEnumerable<UsuarioDto> usuarios;
        private Administrador admin;
        private Dueño dueno;
        private Empleado empleado;
        private readonly int codigo = 2;

        [TestInitialize]
        public void InitTest()
        {
           
            mockTrabajador = new Mock<ITrabajadorServicio>(MockBehavior.Strict);
            mockUsuario = new Mock<IUsuarioServicio>(MockBehavior.Strict);
            api = new UsuariosController(mockUsuario.Object, mockTrabajador.Object);
            user = new UsuarioDto(new Usuario())
            {
                NombreDeUsuario = "juan22"
            };
            usuarios = new List<UsuarioDto>() { user };
            admin = new Administrador();
            dueno = new Dueño();
            empleado = new Empleado();
        }

        


        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void PostAdministradorBadRequest()
        {
            mockUsuario.Setup(x => x.InsertUsuario(admin, codigo)).Throws(new BusinessLogicException());
            api.PostUsuarioAdministrador(admin, codigo);
 

            mockUsuario.VerifyAll();
   
        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void PostAdministradorBadRequestInDataAccess()
        {
            mockUsuario.Setup(x => x.InsertUsuario(admin, codigo)).Throws(new AccesoDatosException(""));
            api.PostUsuarioAdministrador(admin, codigo);
 

            mockUsuario.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PostAdministradorFail()
        {
            mockUsuario.Setup(x => x.InsertUsuario(admin, codigo)).Throws(new Exception());
            api.PostUsuarioAdministrador(admin, codigo);


            mockUsuario.VerifyAll();
        }

        [TestMethod]
        public void PostAdministradorOk()
        {
            mockUsuario.Setup(x => x.InsertUsuario(It.IsAny<Administrador>(), codigo)).Returns(admin);
            IActionResult result = api.PostUsuarioAdministrador(It.IsAny<Administrador>(), codigo);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            Administrador body = objectResult.Value as Administrador;

            mockUsuario.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(admin.Equals(body));
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void PostEmpleadoBadRequest()
        {
            mockTrabajador.Setup(x => x.InsertUsuario(empleado, codigo)).Throws(new BusinessLogicException());
            api.PostUsuarioEmpleado(empleado, codigo);
    

            mockTrabajador.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void PostEmpleadoBadRequestInDataAccess()
        {
            mockTrabajador.Setup(x => x.InsertUsuario(empleado, codigo)).Throws(new AccesoDatosException(""));
            api.PostUsuarioEmpleado(empleado, codigo);


            mockTrabajador.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PostEmpleadoFail()
        {
            mockTrabajador.Setup(x => x.InsertUsuario(empleado, codigo)).Throws(new Exception());
            api.PostUsuarioEmpleado(empleado, codigo);
 

            mockTrabajador.VerifyAll();
        }

        [TestMethod]
        public void PostEmpleadoOk()
        {
            mockTrabajador.Setup(x => x.InsertUsuario(It.IsAny<Empleado>(), codigo)).Returns(empleado);
            IActionResult result = api.PostUsuarioEmpleado(It.IsAny<Empleado>(), codigo);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            Empleado body = objectResult.Value as Empleado;

            mockTrabajador.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(empleado.Equals(body));
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void PostDuenoBadRequest()
        {
            mockTrabajador.Setup(x => x.InsertUsuario(dueno, codigo)).Throws(new BusinessLogicException());
            api.PostUsuarioDueno(dueno, codigo);

            mockTrabajador.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void PostDuenoBadRequestInDataAccess()
        {
            mockTrabajador.Setup(x => x.InsertUsuario(dueno, codigo)).Throws(new AccesoDatosException(""));
            api.PostUsuarioDueno(dueno, codigo);
  

            mockTrabajador.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PostDuenoFail()
        {
            mockTrabajador.Setup(x => x.InsertUsuario(dueno, codigo)).Throws(new Exception());
            api.PostUsuarioDueno(dueno, codigo);


            mockTrabajador.VerifyAll();
        }

        [TestMethod]
        public void PostDuenoOk()
        {
            mockTrabajador.Setup(x => x.InsertUsuario(It.IsAny<Dueño>(), codigo)).Returns(dueno);
            IActionResult result = api.PostUsuarioDueno(It.IsAny<Dueño>(), codigo);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            Dueño body = objectResult.Value as Dueño;

            mockTrabajador.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(dueno.Equals(body));
        }

    }
}
