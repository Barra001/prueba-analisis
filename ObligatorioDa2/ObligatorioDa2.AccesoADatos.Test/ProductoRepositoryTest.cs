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
    public class ProductoRepositoryTest
    {
        private DbContext context;
        private Producto unProducto;
        private ProductoRepository repository;
        private Farmacia unaFarmacia;

        [TestInitialize]
        public void InitTest()
        {


            unProducto = new Medicamento()
            {
                Nombre = "Paracetamol",
                Codigo = "123",
                CantidadPorPresentacion = 8,
                Unidad = Domain.Util.Enumeradores.Unidad.Comprimidos,
                Precio = 200,
                Presentacion = Domain.Util.Enumeradores.Presentacion.Comprimidos,
                Receta = false,
                Sintomas = "Dolor de cabeza",
                DadoDeBaja = false,
            };

            unaFarmacia = new Farmacia()
            {
                Nombre = "Farmacity",
                Direccion = "Ellauri 525",
                SolicitudesDeReposicion = new List<SolicitudDeReposicion> { },
                Compras = new List<Compra> { },
                Productos = new List<Producto> { unProducto }
            };
        }


        private void CreateDataBase(string name)
        {

           
            DbContextOptions<ProyectDbContext> options = new DbContextOptionsBuilder<ProyectDbContext>()
            .UseInMemoryDatabase(databaseName: name)
            .Options;

            context = new ProyectDbContext(options);
            context.Database.EnsureDeleted();
            context.Set<Producto>().Add(unProducto);

            context.Set<Farmacia>().Add(unaFarmacia);

            context.SaveChanges();


            repository = new ProductoRepository(context);
        }


        [TestMethod]
        public void GetAll()
        {

            CreateDataBase("GetTestDB");

            int size = repository.GetAll().ToList().Count;

            
            Assert.AreEqual(1, size);
        }



        [TestMethod]
        public void Insert()
        {
            Producto otroProducto = new Medicamento()
            {
                Nombre = "Paracetamol",
                Codigo = "587",
                CantidadPorPresentacion = 8,
                Unidad = Domain.Util.Enumeradores.Unidad.Comprimidos,
                Precio = 200,
                Presentacion = Domain.Util.Enumeradores.Presentacion.Comprimidos,
                Receta = false,
                Sintomas = "Dolor de cabeza"
            };
            CreateDataBase("InsertTestDB");
            repository.Insertar(otroProducto);
            repository.Save();
            int size = repository.GetAll().ToList().Count;
            Assert.AreEqual(2, size);
        }


        [ExpectedException(typeof(AccesoDatosException))]
        [TestMethod]
        public void InsertProductoYaExistente()
        {
            CreateDataBase("InsertYaExistente");
            repository.Insertar(unProducto);
        }

        [TestMethod]
        public void GetById()
        {
            CreateDataBase("GetByIdTestDB");
            Producto productoEnElRepo = repository.GetById(unProducto.Id);
            Assert.AreEqual(unProducto, productoEnElRepo);
        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetByIdProductoNoExistente()
        {
            CreateDataBase("GetByIdSProductoNoExistenteTestDB");
            Producto sesionEnElRepo = repository.GetById(2);
        }

        [TestMethod]
        public void InsertarEnFarmacia()
        {
            Producto otroProducto = new Medicamento()
            {
                Nombre = "Paracetamol",
                Codigo = "587",
                CantidadPorPresentacion = 8,
                Unidad = Domain.Util.Enumeradores.Unidad.Comprimidos,
                Precio = 200,
                Presentacion = Domain.Util.Enumeradores.Presentacion.Comprimidos,
                Receta = false,
                Sintomas = "Dolor de cabeza"
            };
            CreateDataBase("InsertarEnFarmaciaTestDB");
            int sizeAntesProductos = unaFarmacia.Productos.Count();
            int sizeFarmacias = repository.GetAll().ToList().Count;
            repository.InsertarEnFarmacia(otroProducto, unaFarmacia);
            repository.Save();

            Assert.AreEqual(sizeFarmacias + 1, repository.GetAll().ToList().Count);
            Assert.AreEqual(sizeAntesProductos + 1, unaFarmacia.Productos.Count());
        }



        [ExpectedException(typeof(AccesoDatosException))]
        [TestMethod]
        public void InsertarEnFarmaciaProductoYaExistente()
        {

            CreateDataBase("InsertarEnFarmaciaProductoYaExistenteTestDB");
            repository.InsertarEnFarmacia(unProducto, unaFarmacia);

        }

        [TestMethod]
        public void GetAllMedicamentos()
        {

            CreateDataBase("GetMedicamentosTestDB");
            int size = repository.GetAllMedicamentos().ToList().Count;
            Assert.AreEqual(1, size);
        }

        [TestMethod]
        public void Update()
        {
            CreateDataBase("UpdateTestDB");
            unProducto.DadoDeBaja = true;
            repository.Update(unProducto);
            repository.Save();
            Producto productoEnElRepo = repository.GetById(unProducto.Id);
            Assert.AreEqual(true, productoEnElRepo.DadoDeBaja);
        }


        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void UpdateProductoIdNoExistente()
        {
            CreateDataBase("UpdateProductoIdNoExistenteTestDB");
            Producto otroProducto = new Medicamento();
            repository.Update(otroProducto);
        }

        [TestMethod]
        public void GetAllByNombre()
        {
            CreateDataBase("GetAllByNombreTestDB");
            List<Medicamento> medicamentos = repository.GetAllByNombre(unProducto.Nombre);
            Assert.AreEqual(unProducto.Id, medicamentos[0].Id);
        }

        [TestMethod]
        public void GetAllByNombreInexistente()
        {
            CreateDataBase("GetAllByNombreInexistenteTestDB");
            string nombreInExistente = "ksduhdsi";
            List<Medicamento> medicamentos = repository.GetAllByNombre(nombreInExistente);
            Assert.AreEqual(0, medicamentos.Count);
        }

    }
}
