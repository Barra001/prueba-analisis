using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ObligatorioDa2.Domain.Entidades;
using System.Collections.Generic;
using ObligatorioDa2.IAccesoDatos;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.LogicaDeNegocio.Test
{
    [TestClass]
    public class FarmaciaServicioTest
    {
        private Mock<IFarmaciaRepository> mock;
        private Mock<SesionServicio> mockSesionServicio;
        private FarmaciaServicio servicio;
        private Farmacia farmashop;
        private Farmacia farmaciaValidaConNombreNoExistente;
        private Farmacia farmaciaValidaConNombreYaExistente;
        private Farmacia nullFarmacia;
        private IEnumerable<Farmacia> farmacias;
        private Farmacia farmaciaNombreInvalidoVacio;
        private Farmacia farmaciaDireccionInvalidaVacia;
        private Farmacia farmaciaNombreInvalidoMasLargo;
        private IEnumerable<Compra> compras;
        private Compra compra;
        private Empleado empleado;
        private Sesion sesion;
        [TestInitialize]
        public void InitTest()
        {
            mock = new Mock<IFarmaciaRepository>(MockBehavior.Strict);
            mockSesionServicio= new Mock <SesionServicio>(MockBehavior.Strict);
            servicio = new FarmaciaServicio(mock.Object);
            compra = new Compra()
            {
                Monto = 1200,
                Productos = new List<CantidadProductosCompra> { },
            };
            empleado = new Empleado()
            {

            };
            farmashop = new Farmacia()
            {
                Nombre = "Farmashop",
                Direccion = "21 de Septiembre 1900",
                SolicitudesDeReposicion = new List<SolicitudDeReposicion> { },
                Compras = new List<Compra> { },
                Productos = new List<Producto>
                {
                    new Medicamento("NombreValido", "#85468", 50,"Tos",Enumeradores.Presentacion.Comprimidos, Enumeradores.Unidad.Gramos,20,true){Stock = 0}
                }
            };
            nullFarmacia = null;
            farmacias = new List<Farmacia> { farmashop};
            farmaciaValidaConNombreNoExistente = new Farmacia { Nombre = "Farmacity", Direccion = "Solano Garcia" };
            farmaciaValidaConNombreYaExistente = new Farmacia { Nombre = "Farmashop", Direccion = "Solano Garcia" };
            farmaciaNombreInvalidoVacio = new Farmacia { Nombre = "", Direccion= "Solano Garcia" };
            farmaciaNombreInvalidoMasLargo = new Farmacia { Nombre = "Nombre de prueba de mas de 50 caracteres invalido!!!!", Direccion = "Solano Garcia" };
            farmaciaDireccionInvalidaVacia = new Farmacia {Nombre= "El tunel", Direccion = "" };
        }

        [TestMethod]
        public void GetFarmacias()
        {
            mock.Setup(x => x.GetAll()).Returns(farmacias);
            servicio.GetFarmacias();
            mock.VerifyAll();
        }


        [TestMethod]
        public void GetFarmaciaById()
        {
            mock.Setup(x => x.GetById(farmashop.Id)).Returns(farmashop);
            servicio.GetFarmaciaById(farmashop.Id);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod]
        public void InsertNullFarmacia()
        {            
            servicio.InsertFarmacia(nullFarmacia);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertFarmaciaNombreInvalidoVacio()
        {
            servicio.InsertFarmacia(farmaciaNombreInvalidoVacio);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertFarmaciaNombreInvalidoMasLargo()
        {
            servicio.InsertFarmacia(farmaciaNombreInvalidoMasLargo);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertFarmaciaDireccionInvalidaVacia()
        {
            servicio.InsertFarmacia(farmaciaDireccionInvalidaVacia);
            mock.VerifyAll();
        }

        [TestMethod]
        public void InsertFarmaciaOk()
        {   
            mock.Setup(x => x.ExisteFarmaciaConNombre(farmaciaValidaConNombreNoExistente.Nombre)).Returns(false);
            mock.Setup(x => x.Insertar(farmaciaValidaConNombreNoExistente));
            servicio.InsertFarmacia(farmaciaValidaConNombreNoExistente);
            mock.VerifyAll();
        }
       

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertFarmaciaNombreYaExistente()
        {
            mock.Setup(x => x.ExisteFarmaciaConNombre(farmaciaValidaConNombreYaExistente.Nombre)).Returns(true);
            servicio.InsertFarmacia(farmaciaValidaConNombreYaExistente);
            mock.VerifyAll();
        }

       

        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod]
        public void UpdateNullFarmacia()
        {
            servicio.UpdateFarmacia(nullFarmacia);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void UpdateFarmaciaNombreInvalido()
        {
            servicio.UpdateFarmacia(farmaciaNombreInvalidoVacio);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void UpdateFarmaciaDireccionInvalida()
        {
            mock.Setup(x => x.GetAll()).Returns(farmacias);
            servicio.UpdateFarmacia(farmaciaDireccionInvalidaVacia);
            mock.VerifyAll();
        }

        [ExpectedException(typeof(AccesoDatosException))]
        [TestMethod]
        public void UpdateFarmaciaNoExistente()
        {
            mock.Setup(x => x.Update(farmashop)).Throws(new AccesoDatosException($"Farmacia {farmashop.Id} no existe"));
            servicio.UpdateFarmacia(farmashop);
            mock.VerifyAll();
        }
        [TestMethod]
        public void UpdateFarmaciaOk()
        {
            mock.Setup(x => x.Update(It.IsAny<Farmacia>()));
            servicio.UpdateFarmacia(farmashop);
            mock.VerifyAll();
        }

        [TestMethod]
        public void GetFarmaciasConProductosPorNombre()
        {
            List<Farmacia> lista =(List<Farmacia>) farmacias;
            string nombreMedicamento = lista[0].Productos[0].Nombre;
            bool conStock = false;
            mock.Setup(x => x.GetAll()).Returns(farmacias);
            List<Farmacia> porNombre = servicio.GetFarmaciasConProductosPorNombreyStock(nombreMedicamento, conStock);
            Assert.AreEqual(1, porNombre.Count);
       
        }
        [TestMethod]
        public void GetFarmaciasConProductosPorNombreyStock()
        {
            List<Farmacia> lista = (List<Farmacia>)farmacias;
  
            string nombreMedicamento = lista[0].Productos[0].Nombre;
            bool conStock = true;
            mock.Setup(x => x.GetAll()).Returns(farmacias);
            List<Farmacia> porNombre = servicio.GetFarmaciasConProductosPorNombreyStock(nombreMedicamento, conStock);
            Assert.AreEqual(0, porNombre.Count);

        }

        [TestMethod]
        public void GetFarmaciasConProductosPorNombreyStockProfundo()
        {
            List<Farmacia> lista = (List<Farmacia>)farmacias;
            string nombreMedicamento = lista[0].Productos[0].Nombre;
            lista[0].Productos[0].Stock = 99;
            bool conStock = true;
            mock.Setup(x => x.GetAll()).Returns(farmacias);
            List<Farmacia> porNombre = servicio.GetFarmaciasConProductosPorNombreyStock(nombreMedicamento, conStock);
            Assert.AreEqual(1, porNombre.Count);

        }







    }
}
