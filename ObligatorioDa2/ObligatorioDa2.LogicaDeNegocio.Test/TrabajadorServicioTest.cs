using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;
using System;
using System.Collections.Generic;
using ObligatorioDa2.ILogicaDeNegocio;
using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.LogicaDeNegocio.Test
{
    [TestClass]
    public class TrabajadorServicioTest
    {
        private Mock<IUsuarioRepository> mockUser;
        private Mock<IInvitacionServicio> mockInvitacionService;
        private TrabajadorServicio servicioTrabajador;
        private InvitacionServicio servicioInvitacion;
        private Trabajador usuarioTrabajadorValido;
        private Farmacia farmaciaValida;
        private Invitacion invitacionValida;
        private int codigoValido;


        [TestInitialize]
        public void InitTest()
        {
       
            mockInvitacionService = new Mock<IInvitacionServicio>(MockBehavior.Strict);
            mockUser = new Mock<IUsuarioRepository>(MockBehavior.Strict);

            servicioTrabajador = new TrabajadorServicio(mockUser.Object, mockInvitacionService.Object);

            farmaciaValida = new Farmacia()
            {
                Nombre = "Farmashop",
                Direccion = "21 de Septiembre 1900",
                SolicitudesDeReposicion = new List<SolicitudDeReposicion> { },
                Compras = new List<Compra> { },
                Productos = new List<Producto> { }
            };
            usuarioTrabajadorValido = new Dueño()
            {
                NombreDeUsuario = "Palgom",
                Mail = "jpnecasek@gmail.com",
                Contrasena = "Juanjuan!",
                Direccion = "Ellauri 245",
                FechaDeRegistro = DateTime.Now,
                Farmacia = farmaciaValida
            };
            codigoValido = 73648;
            invitacionValida = new Invitacion("Palgom", Enumeradores.Rol.Dueño, farmaciaValida);
            invitacionValida.Codigo = codigoValido;

        }

     
        [TestMethod]
        public void InsertTrabajadorOk()
        {
            mockUser.Setup(x => x.ExisteUsuarioConMail(usuarioTrabajadorValido.Mail)).Returns(false);
            mockUser.Setup(x => x.Insertar(usuarioTrabajadorValido));
            mockInvitacionService.Setup(x => x.UsarInvitacion(usuarioTrabajadorValido.NombreDeUsuario, codigoValido)).Returns(invitacionValida);
            mockInvitacionService
                .Setup(x => x.GetByNombreYcodigo(usuarioTrabajadorValido.NombreDeUsuario, codigoValido))
                .Returns(invitacionValida);
            servicioTrabajador.InsertUsuario(usuarioTrabajadorValido, codigoValido);
            mockUser.VerifyAll();
        }



        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void InsertInvalidTrabajadorFarmaciaNula()
        {
            mockUser.Setup(x => x.ExisteUsuarioConMail(usuarioTrabajadorValido.Mail)).Returns(true);
            mockUser.Setup(x => x.Insertar(usuarioTrabajadorValido));
            mockInvitacionService.Setup(x => x.UsarInvitacion(usuarioTrabajadorValido.NombreDeUsuario, codigoValido)).Returns(invitacionValida);
            Trabajador usuarioTrabajadorInvalido = usuarioTrabajadorValido;
            usuarioTrabajadorInvalido.Farmacia = null;
            mockInvitacionService
                .Setup(x => x.GetByNombreYcodigo(usuarioTrabajadorInvalido.NombreDeUsuario, codigoValido))
                .Returns(invitacionValida);
          
            servicioTrabajador.InsertUsuario(usuarioTrabajadorInvalido, codigoValido);
            mockUser.VerifyAll();
        }

        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod]
        public void InsertNullTrabajador()
        {

            servicioTrabajador.InsertUsuario(null, codigoValido);
            mockUser.VerifyAll();
        }

        



    }
}
