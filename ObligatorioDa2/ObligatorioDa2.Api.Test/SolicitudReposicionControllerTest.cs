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

namespace ObligatorioDa2.Api.Test
{
    [TestClass]
    public class SolicitudReposicionControllerTest
    {
        private Mock<ISolicitudDeReposicionServicio> mock;
        private SolicitudesReposicionController api;
        private SolicitudDeReposicion solicitud;
        private List<SolicitudDeReposicion> solicitudes;
        private readonly string token = "token";
        private readonly Domain.Util.Enumeradores.EstadoDeSolicitud estado = Domain.Util.Enumeradores.EstadoDeSolicitud.Aceptada;
        private string validToken="lidsgjoidsjfdg5464fdg65fd";

        [TestInitialize]
        public void InitTest()
        {
            mock = new Mock<ISolicitudDeReposicionServicio>(MockBehavior.Strict);
            api = new SolicitudesReposicionController(mock.Object);
            solicitud = new SolicitudDeReposicion();
            solicitudes = new List<SolicitudDeReposicion>() { solicitud };
        }

        [TestMethod]
        public void GetSolicitudDeReposicionOk()
        {
            mock.Setup(x => x.GetSolicitudes(token)).Returns(solicitudes);

            IActionResult result = api.Get(token, null, null, null, null);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            IEnumerable<SolicitudDeReposicion> body = objectResult.Value as IEnumerable<SolicitudDeReposicion>;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(body.SequenceEqual(solicitudes));
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void GetSolicitudDeReposicionFail()
        {
            mock.Setup(x => x.GetSolicitudes(token)).Throws(new BusinessLogicException());

            api.Get(token, null, null, null, null);


            mock.VerifyAll();
     
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void PostSolicitudDeReposicionBadRequest()
        {
            mock.Setup(x => x.InsertarSolicitud(solicitud, token)).Throws(new BusinessLogicException());
            api.Post(solicitud, token);
            
            mock.VerifyAll();
     
        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void PostSolicitudDeReposicionBadRequestInDataAccess()
        {
            mock.Setup(x => x.InsertarSolicitud(solicitud, token)).Throws(new AccesoDatosException(""));
            api.Post(solicitud, token);
            
            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PostSolicitudDeReposicionFail()
        {
            mock.Setup(x => x.InsertarSolicitud(solicitud, token)).Throws(new Exception());
            api.Post(solicitud, token);
       

            mock.VerifyAll();

        }

        [TestMethod]
        public void PostSolicitudDeReposicionOk()
        {
            mock.Setup(x => x.InsertarSolicitud(solicitud, token)).Returns(solicitud);
            IActionResult result = api.Post(solicitud, token);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            SolicitudDeReposicion body = objectResult.Value as SolicitudDeReposicion;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(solicitud.Equals(body));
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void PutSolicitudDeReposicionBadRequest()
        {
            mock.Setup(x => x.ActualizarSolicitud(solicitud.Id, estado)).Throws(new BusinessLogicException());
            api.Put(solicitud.Id, estado);

            
            mock.VerifyAll();
       
        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void PutSolicitudDeReposicionNotFound()
        {
            mock.Setup(x => x.ActualizarSolicitud(solicitud.Id, estado)).Throws(new AccesoDatosException(""));
            api.Put(solicitud.Id, estado);
        

            mock.VerifyAll();
         
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PutSolicitudDeReposicionFail()
        {
            mock.Setup(x => x.ActualizarSolicitud(solicitud.Id, estado)).Throws(new Exception());
            api.Put(solicitud.Id, estado);


            mock.VerifyAll();
         
        }

        [TestMethod]
        public void PutSolicitudDeReposicionOk()
        {
            SolicitudDeReposicion solicitudModificada = solicitud;
            solicitudModificada.EstadoDeSolicitud = estado;
            mock.Setup(x => x.ActualizarSolicitud(It.IsAny<int>(), estado)).Returns(solicitudModificada);
            IActionResult result = api.Put(It.IsAny<int>(), estado);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            SolicitudDeReposicion body = objectResult.Value as SolicitudDeReposicion;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(solicitudModificada.EstadoDeSolicitud.Equals(body.EstadoDeSolicitud));
        }

        [TestMethod]
        public void GetSolicitudesConFiltorsOk()
        {
            mock.Setup(x => x.GetSolicitudesConFiltros(It.IsAny<SolicitudFiltrosDTO>())).Returns(solicitudes);

            IActionResult result = api.Get(It.IsAny<string>(), null, null, "codigo", null);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            List<SolicitudDeReposicion> body = objectResult.Value as List<SolicitudDeReposicion>;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.AreEqual(solicitudes, body);
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void GetSolicitudesConFiltorsBadRequest()
        {
            string codigoValido = "78634324kjh";
            mock.Setup(x => x.GetSolicitudesConFiltros(It.IsAny<SolicitudFiltrosDTO>())).Throws(new BusinessLogicException());

            api.Get(validToken, null, null, codigoValido, null);


            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void GetSolicitudesConFiltorsBadRequestAccesoDatos()
        {
            mock.Setup(x => x.GetSolicitudesConFiltros(It.IsAny<SolicitudFiltrosDTO>())).Throws(new AccesoDatosException(""));

            api.Get(validToken, null, null, "codigo", null);

            mock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetSolicitudesConFiltorsFail()
        {
            mock.Setup(x => x.GetSolicitudesConFiltros(It.IsAny<SolicitudFiltrosDTO>())).Throws(new Exception());

            api.Get(validToken, null, null, "codigo", null);

            mock.VerifyAll();
        }
    }
}
