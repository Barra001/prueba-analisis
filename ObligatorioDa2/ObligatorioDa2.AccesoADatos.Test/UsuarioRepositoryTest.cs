using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObligatorioDa2.AccesoDatos.Repositorys;
using ObligatorioDa2.AccesoDatos;
using ObligatorioDa2.Domain.Entidades;
using System.Linq;
using ObligatorioDa2.Exceptions;

namespace ObligatorioDa2.AccesoADatos.Test
{
    [TestClass]
    public class UsuarioRepositoryTest
    {

        private DbContext context;
        private UsuarioRepository repository;
        private Usuario unUsuarioAdministrador;
        private Usuario unUsuarioEmpleado;

        [TestInitialize]
        public void InitTest()
        {

            unUsuarioAdministrador= new Administrador()
            {
                NombreDeUsuario = "Palgom",
                Mail = "jpnecasek@gmail.com",
                Contrasena = "Juanjuan!",
                Direccion = "Ellauri 245",
            };

            unUsuarioEmpleado = new Empleado()
            {
                NombreDeUsuario = "Juan123",
                Mail = "juan23@gmail.com",
                Contrasena = "Juanlopez34!",
                Direccion = "Solano 254",
            };
        }
        private void CreateDataBase(string name)
        {


            DbContextOptions<ProyectDbContext> options = new DbContextOptionsBuilder<ProyectDbContext>()
            .UseInMemoryDatabase(databaseName: name)
            .Options;

            context = new ProyectDbContext(options);

            context.Set<Usuario>().Add(unUsuarioAdministrador);
            context.Add(unUsuarioEmpleado);

            context.SaveChanges();


            repository = new UsuarioRepository(context);
        }


        [TestMethod]
        public void GetAll()
        {
            CreateDataBase("GetAllTestDB");
            int size = repository.GetAll().ToList().Count;
            Assert.AreEqual(2, size);
        }

        [TestMethod]
        public void GetById()
        {
            CreateDataBase("GetByIdUsuarioTestDB");
            Usuario usuarioEnElRepo = repository.GetById(unUsuarioAdministrador.Id);
            Assert.AreEqual(unUsuarioAdministrador, usuarioEnElRepo);
        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetByIdUsuarioNoExistente()
        {
            CreateDataBase("GetByIdusuarioNoExistenteTestDB");
            Usuario sesionEnElRepo = repository.GetById(3);
        }

        [TestMethod]
        public void Insertar()
        {

            Usuario otroUsuario = new Administrador()
            {
                NombreDeUsuario = "Lolo234",
                Mail = "lolo@gmail.com",
                Contrasena = "Lolololo!",
                Direccion = "Ellauri 245",
            };
            CreateDataBase("InsertDB");
            repository.Insertar(otroUsuario);
            repository.Save();
            int size = repository.GetAll().ToList().Count;
            Assert.AreEqual(3, size);
        }

        [ExpectedException(typeof(AccesoDatosException))]
        [TestMethod]
        public void InsertarUsuarioYaExistente()
        {
            CreateDataBase("InsertYaExistenteDB");
            repository.Insertar(unUsuarioAdministrador);

        }

        [TestMethod]
        public void GetByIdAdministrador()
        {

            CreateDataBase("GetAdministradorByIdTestDB");
            Administrador getAdministradorById = repository.GetByIdAdministrador(unUsuarioAdministrador.Id);
            Assert.AreEqual(unUsuarioAdministrador, getAdministradorById);
        }



        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetByIdAdministradorNoExistente()
        {
            CreateDataBase("GetAdministradorNoExistenteByIdTestDB");
            Administrador getAdministradorById = repository.GetByIdAdministrador(3);
        }

        [TestMethod]
        public void GetByIdTrabajador()
        {

            CreateDataBase("GetByIdTrabajadorTestDB");
            Trabajador getTrabajadorById = repository.GetByIdTrabajador(unUsuarioEmpleado.Id);
            Assert.AreEqual(unUsuarioEmpleado, getTrabajadorById);
        }



        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetByIdTrabajadorNoExistente()
        {
            CreateDataBase("GetByIdTrabajadorNoExistenteTestDB");
            Trabajador getAdministradorById = repository.GetByIdTrabajador(3);
        }


        [TestMethod]
        public void ExisteUsuarioConMail()
        {

            CreateDataBase("ExisteUsuarioConMailTestDB");

            bool existeUsuarioConMail = repository.ExisteUsuarioConMail(unUsuarioEmpleado.Mail);
            Assert.IsTrue(existeUsuarioConMail);
        }


        [TestMethod]
        public void ExisteUsuarioConMailNoExistente()
        {

            CreateDataBase("ExisteUsuarioConMailNoExistenteTestDB");

            bool existeUsuarioConMail = repository.ExisteUsuarioConMail("juan76@gmail.com");
            Assert.IsFalse(existeUsuarioConMail);
        }


        [TestMethod]
        public void GetByNombre()
        {
            CreateDataBase("GetByNombreTestDB");
            Usuario invitacionEnElRepo = repository.GetByNombre(unUsuarioAdministrador.NombreDeUsuario);
            Assert.AreEqual(unUsuarioAdministrador, invitacionEnElRepo);
        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetByNombreNoExistente()
        {
            CreateDataBase("GetByNombreNoExistenteTestDB");
            Usuario invitacionEnElRepo = repository.GetByNombre("JuanLopez");
        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void UpdateWrongId()
        {
            CreateDataBase("UpdateWrongIdTestDB");
            unUsuarioEmpleado.Id = 100;
            repository.Update(this.unUsuarioEmpleado);

        }

        [TestMethod]
        public void UpdateOk()
        {
            CreateDataBase("UpdateOkTestDB");
            string nuevaDireccion = "nueva direccion";
            unUsuarioEmpleado.Direccion = nuevaDireccion;
            repository.Update(this.unUsuarioEmpleado);
            Assert.AreEqual(context.Find<Usuario>(unUsuarioEmpleado.Id).Direccion, nuevaDireccion);

        }

    }
}
