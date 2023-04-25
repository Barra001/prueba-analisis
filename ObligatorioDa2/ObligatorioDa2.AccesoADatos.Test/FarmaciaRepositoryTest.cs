using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using ObligatorioDa2.Domain.Entidades;
using System.Linq;
using ObligatorioDa2.AccesoDatos;
using System.Collections.Generic;
using ObligatorioDa2.AccesoDatos.Repositorys;
using ObligatorioDa2.Exceptions;

namespace ObligatorioDa2.AccesoADatos.Test
{
    [TestClass]
    public class FarmaciaRepositoryTest
    {

        private DbContext context;
        private Farmacia unaFarmacia;
        private FarmaciaRepository repository;


        [TestInitialize]
        public void InitTest()
        {
            

            unaFarmacia = new Farmacia()
            {
                Nombre = "Farmashop",
                Direccion = "21 de Septiembre 1900",
                SolicitudesDeReposicion = new List<SolicitudDeReposicion> { },
                Compras = new List<Compra> { },
                Productos = new List<Producto> { }
            };
        }
        private void CreateDataBase(string name)
        {
            

            DbContextOptions<ProyectDbContext> options = new DbContextOptionsBuilder<ProyectDbContext>()
            .UseInMemoryDatabase(databaseName : name)
            .Options;

            context = new ProyectDbContext(options);
            context.Database.EnsureDeleted();
            context.Set<Farmacia>().Add(unaFarmacia);
            
            context.SaveChanges();
            

            repository = new FarmaciaRepository(context);
            
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
            Farmacia otraFarmacia = new Farmacia()
            {
                Nombre = "Farmacity",
                Direccion = "Ellauri 525",
                SolicitudesDeReposicion = new List<SolicitudDeReposicion> { },
                Compras = new List<Compra> { },
                Productos = new List<Producto> { }
            };
            CreateDataBase("InsertTestDB");
            repository.Insertar(otraFarmacia);
            repository.Save();
            int size = repository.GetAll().ToList().Count;
            Assert.AreEqual(2, size);
        }


        [ExpectedException(typeof(AccesoDatosException))]
        [TestMethod]
        public void InsertFarmaciaYaExistente()
        {
            CreateDataBase("InsertYaExistenteFarmaciaDB");
            repository.Insertar(unaFarmacia);
        }


        [TestMethod]
        public void GetById()
        {
            CreateDataBase("GetByIdFarmaciaDB");           
            Farmacia farmaciaEnElRepo= repository.GetById(unaFarmacia.Id);
            Assert.AreEqual(unaFarmacia.Id, farmaciaEnElRepo.Id);
        }


        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetByIdFarmaciaNoExistente()
        {
            CreateDataBase("GetByIdFarmaciaNoExistenteDB");
            Farmacia farmaciaEnElRepo = repository.GetById(20);       
        }

        [TestMethod]
        public void ExisteFarmaciaConNombre()
        {
            CreateDataBase("ExisteFarmaciaConNombreDB");
            bool farmaciaEnElRepo = repository.ExisteFarmaciaConNombre(unaFarmacia.Nombre);
            Assert.IsTrue(farmaciaEnElRepo);
        }




        [TestMethod]
        public void Update()
        {
            CreateDataBase("UpdateTestDB");
            unaFarmacia.Direccion = "Solano Garcia 2546";
            unaFarmacia.SolicitudesDeReposicion= new List<SolicitudDeReposicion> {new SolicitudDeReposicion()};
            repository.Update(unaFarmacia);
            repository.Save();
            Farmacia farmaciaEnElRepo = repository.GetById(unaFarmacia.Id);
            Assert.AreEqual(unaFarmacia.Direccion, farmaciaEnElRepo.Direccion);
        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void UpdateFarmaciaNoExistente()
        {
            CreateDataBase("UpdateTestNoExistenteDB");
            Farmacia farmaciaNoExistenteEnDb = new Farmacia();
            repository.Update(farmaciaNoExistenteEnDb);
           
        }



    }
}
