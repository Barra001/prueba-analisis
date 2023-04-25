using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObligatorioDa2.AccesoDatos.Repositorys;
using ObligatorioDa2.AccesoDatos;
using ObligatorioDa2.Domain.Entidades;
using System.Collections.Generic;
using System.Linq;
using ObligatorioDa2.Exceptions;

namespace ObligatorioDa2.AccesoADatos.Test
{
    [TestClass]
    public class CompraRepositoryTest
    {


        private DbContext context;
        private Compra unaCompra;
        private CompraRepository repository;
        private Producto producto;
        private CantidadProductosCompra cantidadProductos;
        private Farmacia unaFarmacia;

        [TestInitialize]
        public void InitTest()
        {


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

            unaFarmacia = new Farmacia()
            {
                Nombre = "Farmacity",
                Direccion = "Ellauri 525",
                SolicitudesDeReposicion = new List<SolicitudDeReposicion> { },
                Compras = new List<Compra> {},
                Productos = new List<Producto> { producto }
            };

            cantidadProductos = new CantidadProductosCompra()
            {
                Producto = producto,
                Cantidad = 2

            };

            unaCompra = new Compra()
            {
                Monto = 1000,
                Productos= new List<CantidadProductosCompra> { cantidadProductos},

            };
        }


        private void CreateDataBase(string name)
        {

            DbContextOptions<ProyectDbContext> options = new DbContextOptionsBuilder<ProyectDbContext>()
            .UseInMemoryDatabase(databaseName: name)
            .Options;

            context = new ProyectDbContext(options);
            context.Database.EnsureDeleted();
            context.Set<Compra>().Add(unaCompra);

            context.Set<Farmacia>().Add(unaFarmacia);

            context.SaveChanges();


            repository = new CompraRepository(context);
        }




        [TestMethod]
        public void GetAll()
        {
           CreateDataBase("GetTestDB");
           int size = repository.GetAll().ToList().Count;
           Assert.AreEqual(1, size);
        }


        [TestMethod]
        public void InsertarCompra()
        {

            cantidadProductos.Cantidad = 3;
            Compra otraCompra = new Compra()
            {
                Monto = 1500,
                Productos = new List<CantidadProductosCompra> { cantidadProductos },

            };
            CreateDataBase("InsertDB");
            repository.InsertarCompra(otraCompra, unaFarmacia);
            repository.Save();
            int size = repository.GetAll().ToList().Count;
            Assert.AreEqual(1, unaFarmacia.Compras.Count());
            Assert.AreEqual(2, size);
        }



        [ExpectedException(typeof(AccesoDatosException))]
        [TestMethod]
        public void InsertarCompraYaExistente()
        {
            CreateDataBase("InsertSolicitudYaExistenteDB");
            repository.InsertarCompra(unaCompra, unaFarmacia);
        }

        [TestMethod]
        public void GetById()
        {
            CreateDataBase("GetByIdTestDB");
            Compra unaCompra = repository.GetById(1);
            Assert.AreEqual(1, unaCompra.Id);
        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetByWrongId()
        {
            CreateDataBase("GetByWrongIdTestDB");
            repository.GetById(100);
            
        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void UpdateWrongId()
        {
            CreateDataBase("UpdateWrongIdTestDB");
            unaCompra.Id = 100;
            repository.Update(this.unaCompra);

        }

        [TestMethod]
        public void UpdateOk()
        {
            CreateDataBase("UpdateOkTestDB");
            string nuevoMailComprador = "nuevoMail";
            unaCompra.MailComprador = nuevoMailComprador;
            repository.Update(this.unaCompra);
            Assert.AreEqual(context.Find<Compra>(1).MailComprador, nuevoMailComprador);

        }

        [TestMethod]
        public void GetComprasPorCodigoDeSeguimientoOk()
        {
            CreateDataBase("GetComprasPorCodigoDeSeguimientoOkTestDB");
            int size = repository.GetComprasPorCodigoDeSeguimiento(unaCompra.CodigoSeguimiento).ToList().Count;
            Assert.AreEqual(1, size);
        }


        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetComprasPorCodigoDeSeguimientoNoExistente()
        {
            int codigoNoExistente = 123456;
            CreateDataBase("GetComprasPorCodigoDeSeguimientoNoExistenteTestDB");
            int size = repository.GetComprasPorCodigoDeSeguimiento(codigoNoExistente).ToList().Count;

        }


        [TestMethod]
        public void UpdateEstadoProducto()
        {
            CreateDataBase("UpdateEstadoProductoTestDB");
            CantidadProductosCompra unaCantidadProdCompra = unaCompra.Productos[0];
            unaCantidadProdCompra.EstadoDeCompraProducto = Domain.Util.Enumeradores.EstadoDeCompraProducto.Rechazada;
          
            repository.UpdateEstadoProducuto(unaCantidadProdCompra);
            Assert.AreEqual(unaCantidadProdCompra.EstadoDeCompraProducto, Domain.Util.Enumeradores.EstadoDeCompraProducto.Rechazada);

        }

    }
}
