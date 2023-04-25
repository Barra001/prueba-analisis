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
    public class SolicitudDeReposicionRepositoryTest
    {
        private DbContext context;
        private SolicitudDeReposicion solicitudDeReposicion;
        private SolicitudDeReposicionRepository repository;
        private Producto producto;
        private CantidadProductos cantidadProductos;
        private Empleado solicitante;
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
                Compras = new List<Compra> { },
                Productos = new List<Producto> { }
            };

            cantidadProductos = new CantidadProductos()
            {
                Producto = producto,
                Cantidad = 2

            };

            solicitante = new Empleado()
            {
                Farmacia = unaFarmacia,
                Id = 1
            };

            solicitudDeReposicion = new SolicitudDeReposicion()
            {
                Solicitante = 1,
                Productos = new List<CantidadProductos> { cantidadProductos }

            };
        }
        private void CreateDataBase(string name)
        {


            DbContextOptions<ProyectDbContext> options = new DbContextOptionsBuilder<ProyectDbContext>()
            .UseInMemoryDatabase(databaseName: name)
            .Options;

            context = new ProyectDbContext(options);

            context.Set<SolicitudDeReposicion>().Add(solicitudDeReposicion);

            context.Set<Farmacia>().Add(unaFarmacia);

            context.SaveChanges();


            repository = new SolicitudDeReposicionRepository(context);
        }


        [TestMethod]
        public void GetAll()
        {

            CreateDataBase("GetTestDB");

            int size = repository.GetAll().ToList().Count;
            Assert.AreEqual(1, size);
        }

        [TestMethod]
        public void InsertarSolicitud()
        {
     
            SolicitudDeReposicion otraSolicitud = new SolicitudDeReposicion()
            {
                Solicitante = 1,
                Productos = new List<CantidadProductos> { cantidadProductos },

            };
            CreateDataBase("InsertDB");
            repository.InsertarSolicitud(otraSolicitud, unaFarmacia);
            repository.Save();
            int size= repository.GetAll().ToList().Count;
            Assert.AreEqual(1, unaFarmacia.SolicitudesDeReposicion.Count());
            Assert.AreEqual(2, size);
        }


        [ExpectedException(typeof(AccesoDatosException))]
        [TestMethod]
        public void InsertarSolicitudYaExistente()
        {
            CreateDataBase("InsertSolicitudYaExistenteDB");         
            repository.InsertarSolicitud(solicitudDeReposicion, unaFarmacia);        
        }

        [TestMethod]
        public void GetById()
        {
            CreateDataBase("GetByIdTestDB");
            SolicitudDeReposicion solicitudEnElRepo = repository.GetById(solicitudDeReposicion.Id);
            Assert.AreEqual(solicitudDeReposicion, solicitudEnElRepo);
        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetByIdProductoNoExistente()
        {
            CreateDataBase("GetByIdSolicitudNoExistenteTestDB");
            SolicitudDeReposicion sesionEnElRepo = repository.GetById(2);
        }
        [TestMethod]
        public void Update()
        {
            CreateDataBase("UpdateTestDB");
            solicitudDeReposicion.EstadoDeSolicitud = Domain.Util.Enumeradores.EstadoDeSolicitud.Aceptada;
            repository.Update(solicitudDeReposicion);
            repository.Save();
            SolicitudDeReposicion solicitudEnElRepo = repository.GetById(solicitudDeReposicion.Id);
            Assert.AreEqual(Domain.Util.Enumeradores.EstadoDeSolicitud.Aceptada, solicitudEnElRepo.EstadoDeSolicitud);
        }


        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void UpdateSolicitudIdNoExistente()
        {
            CreateDataBase("UpdateSolicitudIdNoExistenteTestDB");
            SolicitudDeReposicion otraSolicitud = new SolicitudDeReposicion();
            repository.Update(otraSolicitud);
        }


        [TestMethod]
        public void GetSolicitudesDe()
        {
            SolicitudDeReposicion otraSolicitudDeReposicion = new SolicitudDeReposicion
            {
                Solicitante = 1,
            };
            CreateDataBase("GetSolicitudesDeDB");       
            repository.InsertarSolicitud(otraSolicitudDeReposicion, unaFarmacia);
            repository.Save();
            int size = repository.GetSolicitudesDe(solicitante).Count();
            Assert.AreEqual(2, size);
        }

        [TestMethod]
        public void GetSolicitudesDeEmpleadoSinSolicitudes()
        {
            CreateDataBase("GetSolicitudesDeEmpleadoSinSolicitudesDB");      
            int size = repository.GetSolicitudesDe(new Empleado()).Count();
            Assert.AreEqual(0, size);
        }
    }
}


