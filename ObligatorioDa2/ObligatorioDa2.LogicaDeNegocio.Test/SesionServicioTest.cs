using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ObligatorioDa2.ILogicaDeNegocio;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;

namespace ObligatorioDa2.LogicaDeNegocio.Test
{
    [TestClass]
    public class SesionServicioTest
    {
        private Mock<IAccesoDatos.ISesionRepository> mockSesion;
        private Mock<IUsuarioRepository> mockUsuario;
        private Mock<IAccesoDatos.IInvitacionRepository> mockInvitacion;
        private SesionServicio servicioSesion;
        private UsuarioServicio servicioUsuario;
        private InvitacionServicio servicioInvitacion;
        private Sesion sesion;
        private Empleado usuarioExistente;


        [TestInitialize]
        public void InitTest()
        {
            mockInvitacion = new Mock<IAccesoDatos.IInvitacionRepository>(MockBehavior.Strict);
            mockSesion = new Mock<IAccesoDatos.ISesionRepository>(MockBehavior.Strict);
            mockUsuario = new Mock<IUsuarioRepository>(MockBehavior.Strict);
            servicioSesion = new SesionServicio(mockSesion.Object, mockUsuario.Object);
            servicioInvitacion = new InvitacionServicio(mockInvitacion.Object, new Mock<ISesionServicio>(MockBehavior.Strict).Object);


            usuarioExistente = new Empleado()
            {
                NombreDeUsuario = "Juan24",
                Contrasena = "juanjuan!"
            };
           
            sesion = new Sesion()
            {
                UsuarioSesion = usuarioExistente,
                Id = 2

            };

        }

        [TestMethod]
        public void GetUsuarioPorTokenOk()
        {
            mockSesion.Setup(x => x.GetUsuarioByToken(sesion.Token)).Returns(sesion.UsuarioSesion);
            servicioSesion.GetUsuarioByToken(sesion.Token);
            mockSesion.VerifyAll();
        }



        [ExpectedException(typeof(AccesoDatosException))]
        [TestMethod]
        public void GetUsuarioPorTokenQueNoExiste()
        {
            mockSesion.Setup(x => x.GetUsuarioByToken(sesion.Token)).Throws(new AccesoDatosException("El token no existe"));
            servicioSesion.GetUsuarioByToken(sesion.Token);
            mockSesion.VerifyAll();
        }
       
        [TestMethod]
        public void LogInExistente()
        {
            string validToken = "lisdfgsd45fdsf";
            mockSesion.Setup(x => x.ExistsUsuario(usuarioExistente.NombreDeUsuario)).Returns(true);
            mockSesion.Setup(x => x.GetTokenFromUserName(usuarioExistente.NombreDeUsuario)).Returns(validToken);
            mockUsuario.Setup(x => x.GetByNombre(usuarioExistente.NombreDeUsuario)).Returns(usuarioExistente);

            servicioSesion.LogIn(usuarioExistente.NombreDeUsuario, usuarioExistente.Contrasena);
            mockSesion.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void LogInUsuarioInexistente()
        {
            mockSesion.Setup(x => x.ExistsUsuario(usuarioExistente.NombreDeUsuario)).Returns(false);
            mockUsuario.Setup(x => x.GetByNombre(usuarioExistente.NombreDeUsuario)).Throws(new AccesoDatosException("Usuario inexistente"));
            servicioSesion.LogIn(usuarioExistente.NombreDeUsuario, usuarioExistente.NombreDeUsuario);
            mockSesion.VerifyAll();
        }
        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void LogInContraseñaIncorrecta()
        {
            mockSesion.Setup(x => x.ExistsUsuario(usuarioExistente.NombreDeUsuario)).Returns(false);
            mockUsuario.Setup(x => x.GetByNombre(usuarioExistente.NombreDeUsuario)).Returns(usuarioExistente);
            servicioSesion.LogIn(usuarioExistente.NombreDeUsuario, It.IsAny<string>());
            mockSesion.VerifyAll();
        }

    
        [TestMethod]
        public void LogInValido()
        {
            mockSesion.Setup(x => x.ExistsUsuario(usuarioExistente.NombreDeUsuario)).Returns(false);
            mockSesion.Setup(x => x.Insertar(It.IsAny<Sesion>()));
            mockUsuario.Setup(x => x.GetByNombre(usuarioExistente.NombreDeUsuario)).Returns(usuarioExistente);
            servicioSesion.LogIn(usuarioExistente.NombreDeUsuario, usuarioExistente.Contrasena);
            mockSesion.VerifyAll();
        }
        [TestMethod]
        public void LogOutValido()
        {
            mockSesion.Setup(x => x.GetSesionByToken(It.IsAny<string>())).Returns(sesion);
            mockSesion.Setup(x => x.Delete(sesion.Id));
            Assert.AreEqual(servicioSesion.LogOut(It.IsAny<string>()).Mensaje, "LogOut exitoso"); 
            mockSesion.VerifyAll();
        }



    }




}

