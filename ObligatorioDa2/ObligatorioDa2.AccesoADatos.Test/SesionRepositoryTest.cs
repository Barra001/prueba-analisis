using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObligatorioDa2.AccesoDatos.Repositorys;
using ObligatorioDa2.AccesoDatos;
using ObligatorioDa2.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using ObligatorioDa2.Exceptions;

namespace ObligatorioDa2.AccesoADatos.Test
{
    [TestClass]
    public class SesionRepositoryTest
    {
        private DbContext context;
        private Sesion unaSesion;
        private SesionRepository repository;
        private Usuario unEmpleado;
        private Farmacia unaFarmacia;

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

            unEmpleado = new Empleado()
            {
               
                    NombreDeUsuario = "Palgom",
                    Mail = "jpnecasek@gmail.com",
                    Contrasena = "Juanjuan!",
                    Direccion = "Ellauri 245",
                    FechaDeRegistro = DateTime.Now,
                    Farmacia = unaFarmacia,
                
        };

            unaSesion = new Sesion()
            {
               UsuarioSesion= unEmpleado,
            };
        }
        private void CreateDataBase(string name)
        {


            DbContextOptions<ProyectDbContext> options = new DbContextOptionsBuilder<ProyectDbContext>()
            .UseInMemoryDatabase(databaseName: name)
            .Options;

            context = new ProyectDbContext(options);

            context.Set<Sesion>().Add(unaSesion);

            context.SaveChanges();


            repository = new SesionRepository(context);
        }



        [TestMethod]
        public void GetAllSesiones()
        {
            CreateDataBase("GetSesionesTestDB");
            int size = repository.GetAll().ToList().Count;
            Assert.AreEqual(1, size);
        }

        [TestMethod]
        public void Insert()
        {
            Sesion otraSesion = new Sesion()
            {
                UsuarioSesion = null,
            };

            CreateDataBase("InsertTestDB");
            int sizeAntes = repository.GetAll().ToList().Count;
            repository.Insertar(otraSesion);
            repository.Save();
           
            Assert.AreEqual(sizeAntes+1, repository.GetAll().ToList().Count);
        }

        [ExpectedException(typeof(AccesoDatosException))]
        [TestMethod]
        public void InsertSesionYaExistente()
        {
            CreateDataBase("InsertTestDB");
            repository.Insertar(unaSesion);                     
        }

        [TestMethod]
        public void GetById()
        {
            CreateDataBase("UpdateTestDB");
            Sesion sesionEnElRepo = repository.GetById(unaSesion.Id);
            Assert.AreEqual(unaSesion, sesionEnElRepo);
        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetByIdSesionNoExistente()
        {
            CreateDataBase("GetByIdSesionNoExistenteTestDB");
            Sesion sesionEnElRepo = repository.GetById(3);
        }





        [TestMethod]
        public void ExistUsuario()
        {
            CreateDataBase("ExistUsuarioTestDB");
            bool existUsuario = repository.ExistsUsuario(unaSesion.UsuarioSesion.NombreDeUsuario);
            Assert.IsTrue(existUsuario);
        }


        [TestMethod]
        public void ExistToken()
        {
            CreateDataBase("ExistTokenTestDB");
            bool existToken = repository.ExistsToken(unaSesion.Token);
            Assert.IsTrue(existToken);
        }

        [TestMethod]
        public void GetFarmaciaByToken()
        {
            CreateDataBase("GetFarmaciaByTokenTestDB");
            Farmacia farmaciaByToken = repository.GetFarmaciaByToken(unaSesion.Token);
            Assert.AreEqual(unaFarmacia.Nombre, farmaciaByToken.Nombre);
        }


        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetFarmaciaByTokenNoExistente()
        {
            CreateDataBase("GetFarmaciaByTokenTestDB");
            Farmacia farmaciaByToken = repository.GetFarmaciaByToken(Guid.NewGuid().ToString());
           
        }


        [TestMethod]
        public void GetSesionByToken()
        {
            CreateDataBase("GetSesionByTokenTestDB");
            Sesion sesionByToken = repository.GetSesionByToken(unaSesion.Token);
            Assert.AreEqual(unaSesion, sesionByToken);
        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetSesionByTokenNoExistente()
        {
            CreateDataBase("GetSesionByTokenNoExistenteTestDB");
            Sesion sesionByToken = repository.GetSesionByToken(Guid.NewGuid().ToString());
    
        }

        [TestMethod]
        public void GetTokenFromUserName()
        {
            CreateDataBase("GetTokenFromUserNameNoExistenteTestDB");
            string tokenFromUserName = repository.GetTokenFromUserName(unaSesion.UsuarioSesion.NombreDeUsuario);
            Assert.AreEqual(unaSesion.Token , tokenFromUserName);
        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetTokenFromUserNameNoExistente()
        {
            CreateDataBase("GetTokenFromUserNameTestDB");
            string tokenFromUserName = repository.GetTokenFromUserName("Juan");
            
        }


        [TestMethod]
        public void GetUsuarioByToken()
        {
            CreateDataBase("GetUsuarioByTokenTestDB");
            Usuario usuarioByToken = repository.GetUsuarioByToken(unaSesion.Token);
            Assert.AreEqual(unEmpleado, usuarioByToken);
        }

        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void GetUsuarioByTokenNoExistente()
        {
            CreateDataBase("GetUsuarioByTokenNoExistenteDB");
            Usuario usuarioByToken = repository.GetUsuarioByToken(Guid.NewGuid().ToString());
        }

        [TestMethod]
        public void Delete()
        {
            CreateDataBase("DeleteTestDB");
            repository.Delete(unaSesion.Id);
            repository.Save();
            int size = repository.GetAll().ToList().Count;
            Assert.AreEqual(0, size);
        }


        [ExpectedException(typeof(NotFoundException))]
        [TestMethod]
        public void DeleteSesionNoExistente()
        {
            CreateDataBase("DeleteSesionNoExistenteDB");
            repository.Delete(20);       
        }






    }
}
