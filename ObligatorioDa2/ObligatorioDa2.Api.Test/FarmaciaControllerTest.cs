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
    public class FarmaciaControllerTest
    {
        private Mock<IFarmaciaServicio> mock;
        private FarmaciasController api;
        private Farmacia farmashop;
        private IEnumerable<Farmacia> farmacias;

        [TestInitialize]
        public void InitTest()
        {
            mock = new Mock<IFarmaciaServicio>(MockBehavior.Strict);
            api = new FarmaciasController(mock.Object);
            farmashop = new Farmacia()
            {
                Nombre = "Farmashop",
                Direccion = "21 de Septiembre 1900",
                SolicitudesDeReposicion = new List<SolicitudDeReposicion> { },
                Compras = new List<Compra> { },
                Productos = new List<Producto> { }
            };
            farmacias = new List<Farmacia>() { farmashop };
        }

        [TestMethod]
        public void GetFarmaciasOk()
        {
            mock.Setup(x => x.GetFarmacias()).Returns(farmacias);

            IActionResult result = api.Get();
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            IEnumerable<Farmacia> body = objectResult.Value as IEnumerable<Farmacia>;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(body.SequenceEqual(farmacias));
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void GetFarmaciasFail()
        {
            mock.Setup(x => x.GetFarmacias()).Throws(new BusinessLogicException());
            api.Get();
            
            mock.VerifyAll();
           
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void PostFarmaciaBadRequest()
        {
            mock.Setup(x => x.InsertFarmacia(farmashop)).Throws(new BusinessLogicException());
            api.Post(farmashop);
            
            mock.VerifyAll();
          
        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void PostFarmaciaBadRequestInDataAccess()
        {
            mock.Setup(x => x.InsertFarmacia(farmashop)).Throws(new AccesoDatosException(""));
            api.Post(farmashop);

            mock.VerifyAll();
     
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PostFarmaciaFail()
        {
            mock.Setup(x => x.InsertFarmacia(farmashop)).Throws(new Exception());
            api.Post(farmashop);

            mock.VerifyAll();
      
        }

        [TestMethod]
        public void PostFarmaciaOk()
        {
            mock.Setup(x => x.InsertFarmacia(farmashop)).Returns(farmashop);
            IActionResult result = api.Post(farmashop);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            Farmacia body = objectResult.Value as Farmacia;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(farmashop.Equals(body));
        }



        [TestMethod]
        public void GetFarmaciaByIdOk()
        {
            mock.Setup(x => x.GetFarmaciaById(It.IsAny<int>())).Returns(farmashop);

            IActionResult result = api.GetById(1);
            ObjectResult objectResult = result as ObjectResult;
            int? statusCode = objectResult.StatusCode;
            Farmacia body = objectResult.Value as Farmacia;

            mock.VerifyAll();
            Assert.AreEqual(200, statusCode);
            Assert.IsTrue(farmashop.Nombre.Equals(body.Nombre));
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessLogicException))]
        public void GetFarmaciaByIdFail()
        {
            int validId = 1;
            mock.Setup(x => x.GetFarmaciaById(validId)).Throws(new BusinessLogicException());

            api.GetById(validId);
            
            mock.VerifyAll();
         
        }

        [TestMethod]
        [ExpectedException(typeof(AccesoDatosException))]
        public void GetFarmaciaByIdNotFound()
        {
            int validId = 1;
            mock.Setup(x => x.GetFarmaciaById(validId)).Throws(new AccesoDatosException(""));

            api.GetById(validId);
            
            mock.VerifyAll();
          
        }


    }
}
