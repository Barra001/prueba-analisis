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
using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.Api.Test
{
    [TestClass]
    public class InvitacionControllerTest
    {

        private Mock<IInvitacionServicio> mock;
        private InvitacionesController api;
        private Invitacion invitacion;
        private Farmacia farmacia;
        private IEnumerable<Invitacion> invitaciones;
        private readonly string nombreUsuario = "nombreUsuario";
        private string tokenValido = "ToktnValido";

        [TestInitialize]
        public void InitTest()
        {
            mock = new Mock<IInvitacionServicio>(MockBehavior.Strict);
            api = new InvitacionesController(mock.Object);
            farmacia = new Farmacia();
            invitacion = new Invitacion()
            {
                NombreDeUsuario = "palgom",
                RolInvitado = Domain.Util.Enumeradores.Rol.Empleado,
                Farmacia = farmacia
            };
            invitaciones = new List<Invitacion>() { invitacion };
        }

      

      

        [TestMethod]
        public void GetInvitacionOk()
        {
            mock.Setup(x => x.GetByNombreYcodigo(It.IsAny<string>(), It.IsAny<int>())).Returns(invitacion);

            int codigo = 56565;
            IActionResult result = api.Get(nombreUsuario, codigo, null, null, It.IsAny<string>());
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            Invitacion body = objectResult.Value as Invitacion;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.AreEqual(invitacion, body);
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void GetInvitacionBadRequest()
        {

            mock.Setup(x => x.GetByNombreYcodigo(nombreUsuario, invitacion.Codigo)).Throws(new BusinessLogicException());

            api.Get(nombreUsuario, invitacion.Codigo, null, null, It.IsAny<string>());
           
            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void GetInvitacionBadRequestAccesoDatos()
        {
            mock.Setup(x => x.GetByNombreYcodigo(nombreUsuario, invitacion.Codigo)).Throws(new AccesoDatosException(""));

            api.Get(nombreUsuario, invitacion.Codigo, null, null, It.IsAny<string>());


            mock.VerifyAll();
           
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetInvitacionFail()
        {
            mock.Setup(x => x.GetByNombreYcodigo(nombreUsuario, invitacion.Codigo)).Throws(new Exception());

            api.Get(nombreUsuario, invitacion.Codigo, null, null, It.IsAny<string>());
            
            mock.VerifyAll();
        }


        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void PostInvitacionBadRequest()
        {
            mock.Setup(x => x.InsertInvitacion(invitacion, tokenValido)).Throws(new BusinessLogicException());
            api.Post(invitacion, tokenValido);
            
            mock.VerifyAll();

        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void PostInvitacionBadRequestInDataAccess()
        {
            mock.Setup(x => x.InsertInvitacion(invitacion, tokenValido)).Throws(new AccesoDatosException(""));
            api.Post(invitacion, tokenValido);

            mock.VerifyAll();
   
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PostInvitacionFail()
        {
            mock.Setup(x => x.InsertInvitacion(invitacion, tokenValido)).Throws(new Exception());
            api.Post(invitacion, tokenValido);

            mock.VerifyAll();
        }

        [TestMethod]
        public void PostInvitacionOk()
        {
            mock.Setup(x => x.InsertInvitacion(It.IsAny<Invitacion>(), tokenValido)).Returns(invitacion);
            IActionResult result = api.Post(It.IsAny<Invitacion>(), tokenValido);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            Invitacion body = objectResult.Value as Invitacion;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(invitacion.Equals(body));
        }

        [TestMethod]
        public void GetInvitacionesFiltradasOk()
        {
            int farmaciaId = 1;
            string nombreUsuario = "nombreValido";
            Enumeradores.Rol rolValido = Enumeradores.Rol.Empleado;
            string tokenValido = "ireohjtioer8t97e8r7t";
 
            List<Invitacion> invitacions = new List<Invitacion>(){invitacion};
  
            mock.Setup(x => x.ConseguirInvitacionesFiltradas(It.IsAny<InvitacionFiltroDTO>())).Returns(invitacions);

            IActionResult result = api.Get(nombreUsuario, 0, farmaciaId, rolValido, tokenValido);

            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            List<Invitacion> body = objectResult.Value as List<Invitacion>;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.AreEqual(invitacions, body);
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void GetInvitacionesFiltradasBadRequest()
        {
            mock.Setup(x => x.ConseguirInvitacionesFiltradas(It.IsAny<InvitacionFiltroDTO>())).Throws(new BusinessLogicException());
            api.Get(nombreUsuario, 0, It.IsAny<int>(), Enumeradores.Rol.Empleado, It.IsAny<string>());
            mock.VerifyAll();
        }
        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void GetInvitacionesFiltradasBadRequestAccesoDatos()
        {
            mock.Setup(x => x.ConseguirInvitacionesFiltradas(It.IsAny<InvitacionFiltroDTO>())).Throws(new AccesoDatosException(""));
            api.Get(nombreUsuario, 0, It.IsAny<int>(), Enumeradores.Rol.Empleado, It.IsAny<string>());
            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void EditarInvitacionNotFound()
        {
            mock.Setup(x => x.EditarInvitacion(invitacion, false)).Throws(new AccesoDatosException(""));
            api.Put(invitacion, false);
            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void EditarInvitacionFalla()
        {
            mock.Setup(x => x.EditarInvitacion(invitacion, false)).Throws(new Exception());
            api.Put(invitacion, false);
            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void EditarInvitacionRequestInvalida()
        {
            mock.Setup(x => x.EditarInvitacion(invitacion, false)).Throws(new BusinessLogicException());
            api.Put(invitacion, false);
            mock.VerifyAll();
        }

        [TestMethod]
        public void EditarInvitacionOk()
        {
            Invitacion invitacionModificada = invitacion;
            invitacionModificada.NombreDeUsuario = "jupa";
            mock.Setup(x => x.EditarInvitacion(invitacion, false)).Returns(invitacionModificada);
            IActionResult result = api.Put(invitacion, false);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            Invitacion body = objectResult.Value as Invitacion;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(invitacionModificada.NombreDeUsuario.Equals(body.NombreDeUsuario));
        }
    }
}
