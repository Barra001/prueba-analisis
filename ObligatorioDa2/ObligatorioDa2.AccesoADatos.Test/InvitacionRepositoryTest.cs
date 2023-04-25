using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObligatorioDa2.AccesoDatos.Repositorys;
using ObligatorioDa2.AccesoDatos;
using ObligatorioDa2.Domain.Entidades;
using System.Linq;
using ObligatorioDa2.Domain.Util;
using ObligatorioDa2.Exceptions;

namespace ObligatorioDa2.AccesoADatos.Test
{
    [TestClass]
    public class InvitacionRepositoryTest
    {

        private DbContext context;
        private InvitacionRepository repository;
        private Invitacion unaInvitacion;
        private Farmacia unaFarmacia;

        [TestInitialize]
        public void InitTest()
        {


            unaInvitacion = new Invitacion()
            {
                NombreDeUsuario = "Palgom",
                RolInvitado = (Enumeradores.Rol)1,
                Usada = false,
              
            };

            unaFarmacia = new Farmacia();
        }
        private void CreateDataBase(string name)
        {


            DbContextOptions<ProyectDbContext> options = new DbContextOptionsBuilder<ProyectDbContext>()
            .UseInMemoryDatabase(databaseName: name)
            .Options;

            context = new ProyectDbContext(options);

            context.Set<Invitacion>().Add(unaInvitacion);
            context.Set<Farmacia>().Add(unaFarmacia);

            context.SaveChanges();


            repository = new InvitacionRepository(context);
        }


        [TestMethod]
        public void GetAll()
        {

            CreateDataBase("GetTestDB");

            int size = repository.GetAll().ToList().Count;
            Assert.AreEqual(1, size);
        }

        [TestMethod]
        public void Insertar()
        {

            Invitacion otraInvitacion = new Invitacion()
            {
                NombreDeUsuario= "JuanPerez",
                RolInvitado = (Enumeradores.Rol)2

            };
            CreateDataBase("InsertDB");
            repository.Insertar(otraInvitacion);
            repository.Save();
            int size = repository.GetAll().ToList().Count;
            Assert.AreEqual(2, size);
        }

        [ExpectedException(typeof(AccesoDatosException))]
        [TestMethod]
        public void InsertarYaExistente()
        {
   
            CreateDataBase("InsertYaExistenteDB");
            repository.Insertar(unaInvitacion);
        }

        [TestMethod]
        public void GetById()
        {
            CreateDataBase("GetByIdTestDB");
            Invitacion invitacionEnElRepo= repository.GetById(unaInvitacion.Id);
            Assert.AreEqual(unaInvitacion.Id, invitacionEnElRepo.Id);
        }


        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetByIdNoExistente()
        {
            CreateDataBase("GetByIdNoExistenteTestDB");
            Invitacion invitacionEnElRepo = repository.GetById(2);
        }


        [TestMethod]
        public void ExisteInvitacionConNombre()
        {
            CreateDataBase("ExisteInvitacionConNombreDB");
            bool existeInvitacionConNombre = repository.ExisteInvitacionConNombre(unaInvitacion.NombreDeUsuario);
            Assert.IsTrue(existeInvitacionConNombre);
        }

        [TestMethod]
        public void ExisteInvitacionConNombreNoExistente()
        {
            CreateDataBase("ExisteInvitacionConNombreNoExistenteDB");
            bool existeInvitacionConNombre = repository.ExisteInvitacionConNombre("JuanLopez");
            Assert.IsFalse(existeInvitacionConNombre);
        }


        [TestMethod]
        public void GetByNombre()
        {
            CreateDataBase("GetByNombreTestDB");
            Invitacion invitacionEnElRepo = repository.GetByNombre(unaInvitacion.NombreDeUsuario);
            Assert.AreEqual(unaInvitacion, invitacionEnElRepo);
        }


        [TestMethod]
        public void GetByNombreNoExistente()
        {
            CreateDataBase("GetByNombreNoExistenteTestDB");
            Invitacion invitacionEnElRepo = repository.GetByNombre("JuanLopez");
            Assert.AreEqual(null, invitacionEnElRepo);
        }

        [TestMethod]
        public void Update()
        {
            CreateDataBase("UpdateTestDB");
            unaInvitacion.Usada = true;
            repository.Update(unaInvitacion);
            repository.Save();
            Invitacion invitacionEnElRepo = repository.GetById(unaInvitacion.Id);
            Assert.AreEqual(true, invitacionEnElRepo.Usada);
        }

        [TestMethod]
        public void UpdateConFarmacia()
        {
            CreateDataBase("UpdateConFarmaciaDB");
            unaInvitacion.Usada = true;
            unaInvitacion.Farmacia = unaFarmacia;
            unaInvitacion.RolInvitado = Enumeradores.Rol.Dueño;
            repository.Update(unaInvitacion);
            repository.Save();
            Invitacion invitacionEnElRepo = repository.GetById(unaInvitacion.Id);
            Assert.AreEqual(true, invitacionEnElRepo.Usada);
        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void UpdateSolicitudIdNoExistente()
        {
            CreateDataBase("UpdateSolicitudIdNoExistenteTestDB");
            Invitacion otraInvitacion = new Invitacion()
            {
                NombreDeUsuario = "JuanPerez",
                RolInvitado = (Enumeradores.Rol)2

            };
            repository.Update(otraInvitacion);
        }

        [TestMethod]
        public void ExisteOtraInvitacionConEsteNombre()
        {
            CreateDataBase("ExisteOtraInvitacionConEsteNombreDB");
            bool existeInvitacionConNombre = repository.ExisteOtraInvitacionConEsteNombre(unaInvitacion);
            Assert.IsFalse(existeInvitacionConNombre);
        }




    }
}
