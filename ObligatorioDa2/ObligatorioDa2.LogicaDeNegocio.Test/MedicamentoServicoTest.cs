using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ObligatorioDa2.ILogicaDeNegocio;
using ObligatorioDa2.Domain.Entidades;
using System.Collections.Generic;
using System.Linq;
using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;

namespace ObligatorioDa2.LogicaDeNegocio.Test
{
    [TestClass]
    public class MedicamentoServicoTest
    {
        private Mock<IAccesoDatos.IProductoRepository> mockProducto;
        private Mock<IUsuarioRepository> mockUsuario;
        private Mock<IAccesoDatos.ISesionRepository> mockSesion;
        private Mock<IFarmaciaServicio> mockFarmacia;
        private ProductoServicio servicioProducto;
        private SesionServicio servicioSesion;
        private Medicamento paracetamol;
        private Medicamento nullMedicamento;
        private Medicamento aspirina;
        private IEnumerable<Medicamento> medicamentos;
        private Medicamento codigoRepetidoMedicamento;
        private Farmacia farmaciaNoNull;
        private string tokenNoNull;
        [TestInitialize]
        public void InitTest()
        {
            mockFarmacia = new Mock<IFarmaciaServicio>(MockBehavior.Strict);
            mockProducto = new Mock<IAccesoDatos.IProductoRepository>(MockBehavior.Strict);
            mockSesion = new Mock<IAccesoDatos.ISesionRepository>(MockBehavior.Strict);
            mockUsuario = new Mock<IUsuarioRepository>(MockBehavior.Strict);
            servicioSesion = new SesionServicio(mockSesion.Object, mockUsuario.Object);
            servicioProducto = new ProductoServicio(mockProducto.Object, servicioSesion, mockFarmacia.Object);
            paracetamol = new Medicamento()
            {
                Nombre = "Paracetamol",
                Codigo = "123",
                CantidadPorPresentacion = 8,
                Unidad = Domain.Util.Enumeradores.Unidad.Comprimidos,
                Precio = 200,
                Presentacion = Domain.Util.Enumeradores.Presentacion.Comprimidos,
                Receta = false,
                Sintomas = "Dolor de cabeza",
                Id = 1
            };
            aspirina = new Medicamento()
            {
                Nombre = "Aspirina",
                Codigo = "456",
                CantidadPorPresentacion = 8,
                Unidad = Domain.Util.Enumeradores.Unidad.Comprimidos,
                Precio = 150,
                Presentacion = Domain.Util.Enumeradores.Presentacion.Comprimidos,
                Receta = false,
                Sintomas = "Dolor de cabeza",
                Id = 2
            };
            nullMedicamento = null;
            medicamentos = new List<Medicamento> { paracetamol};
            codigoRepetidoMedicamento = new Medicamento() 
            {
                Nombre = "Neumocort",
                Codigo = "123",
                CantidadPorPresentacion = 1,
                Unidad= Domain.Util.Enumeradores.Unidad.Comprimidos,
                Precio = 100,
                Presentacion = Domain.Util.Enumeradores.Presentacion.Comprimidos,
                Receta = false,
                Sintomas = "Asma",
                Id = 2
            };
            farmaciaNoNull = new Farmacia("Un nombre", "una direccion"){Id = 1, Productos = new List<Producto>(){paracetamol, codigoRepetidoMedicamento}};
            tokenNoNull = "ds354fds354f3ds54f35ds4f35sd";

        }

        [TestMethod]
        public void GetMedicamentos()
        {
            aspirina.DadoDeBaja = true;
         
            mockProducto.Setup(x => x.GetAllMedicamentos()).Returns(medicamentos);
            servicioProducto.GetMedicamentos();
            mockProducto.VerifyAll();
        }

        [TestMethod]
        public void InsertarMedicamentoOk()
        {
            mockProducto.Setup(x => x.InsertarEnFarmacia(aspirina, farmaciaNoNull));
            mockSesion.Setup(x => x.GetFarmaciaByToken(It.IsAny<string>())).Returns(farmaciaNoNull);

            servicioProducto.InsertarProducto(aspirina, tokenNoNull);
            mockProducto.VerifyAll();
        }

        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod]
        public void InsertarMedicamentoSinNombre()
        {
            mockSesion.Setup(x => x.GetFarmaciaByToken(It.IsAny<string>())).Returns(It.IsAny<Farmacia>());
            paracetamol.Nombre = null;

            servicioProducto.InsertarProducto(paracetamol, tokenNoNull);
            mockProducto.VerifyAll();
        }

        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod]
        public void InsertarMedicamentoSinCodigo()
        {
            mockSesion.Setup(x => x.GetFarmaciaByToken(It.IsAny<string>())).Returns(It.IsAny<Farmacia>());
            paracetamol.Codigo = null;

            servicioProducto.InsertarProducto(paracetamol, tokenNoNull);
            mockProducto.VerifyAll();
        }

        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod] 
        public void InsertarMedicamentoCodigoRepetido()
        {
            mockSesion.Setup(x => x.GetFarmaciaByToken(It.IsAny<string>())).Returns(farmaciaNoNull);
            mockProducto.Setup(x => x.InsertarEnFarmacia(paracetamol, It.IsAny<Farmacia>()));
            mockSesion.Setup(x => x.GetFarmaciaByToken(It.IsAny<string>())).Returns(farmaciaNoNull);
            servicioProducto.InsertarProducto(codigoRepetidoMedicamento, tokenNoNull);
            mockProducto.VerifyAll();
        }

        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod]
        public void InsertNullMedicamento()
        {
            mockSesion.Setup(x => x.GetFarmaciaByToken(It.IsAny<string>())).Returns(It.IsAny<Farmacia>());
            servicioProducto.InsertarProducto(nullMedicamento, tokenNoNull);
            mockProducto.VerifyAll();
        }

        [TestMethod]
        public void DarDeBajaMedicamento()
        {
            mockProducto.Setup(x => x.Update(It.IsAny<Medicamento>()));
            mockProducto.Setup(x => x.GetById(paracetamol.Id)).Returns(paracetamol);
            servicioProducto.DarDeBajaProducto(paracetamol.Id);
            Assert.IsTrue(paracetamol.DadoDeBaja);
            paracetamol.DadoDeBaja = false;
        }
        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void DarDeBajaMedicamentoYaBajado()
        {
            paracetamol.DadoDeBaja = true;
            mockProducto.Setup(x => x.Update(It.IsAny<Medicamento>()));
            mockProducto.Setup(x => x.GetById(paracetamol.Id)).Returns(paracetamol);
            servicioProducto.DarDeBajaProducto(paracetamol.Id);
            paracetamol.DadoDeBaja = false;
        }

        [ExpectedException(typeof(AccesoDatosException))]
        [TestMethod]
        public void DarDeBajaMedicamentoInexistente()
        {
        
            mockProducto.Setup(x => x.Update(It.IsAny<Medicamento>()));
            mockProducto.Setup(x => x.GetById(It.IsAny<int>())).Throws(new AccesoDatosException(""));
            servicioProducto.DarDeBajaProducto(paracetamol.Id);
      
        }


        [TestMethod]
        public void AgregarStock()
        {
            mockProducto.Setup(x => x.Update(paracetamol));
            mockProducto.Setup(x => x.GetById(paracetamol.Id)).Returns(paracetamol);
            paracetamol.Stock = 0;
            servicioProducto.CambiarStock(paracetamol.Id, 20);
            Assert.AreEqual(20, paracetamol.Stock);
        }

        [ExpectedException(typeof(AccesoDatosException))]
        [TestMethod]
        public void AgregarStockNullMedicamento()
        {
            mockProducto.Setup(x => x.GetById(It.IsAny<int>())).Throws(new AccesoDatosException(""));
            int idNoExistente = 999;
            servicioProducto.CambiarStock(idNoExistente, 10);
            mockProducto.VerifyAll();
        }
        [ExpectedException(typeof(BusinessLogicException))]
        [TestMethod]
        public void AgregarStockDadoDeBajaMedicamento()
        {
            mockProducto.Setup(x => x.Update(paracetamol));
            mockProducto.Setup(x => x.GetById(paracetamol.Id)).Returns(paracetamol);
            paracetamol.DadoDeBaja = true;
            servicioProducto.CambiarStock(paracetamol.Id, 20);
            mockProducto.VerifyAll();
        }

        [TestMethod]
        public void FiltrarPorMedicamentoYFarmacia()
        {
            paracetamol.Stock = 10;
            mockFarmacia.Setup(x => x.GetFarmaciasConProductosPorNombreyStock(paracetamol.Nombre, true)).Returns(new List<Farmacia>(){farmaciaNoNull});

            List<MedicamentoConFarmaciaDTO> medicamentos = servicioProducto.FiltrarPorMedicamentoYFarmacia(paracetamol.Nombre, farmaciaNoNull.Nombre);

            Assert.AreEqual(medicamentos[0].IdFarmacia, farmaciaNoNull.Id);
            Assert.AreEqual(medicamentos[0].Medicamento.Id, paracetamol.Id);
            mockProducto.VerifyAll();
        }
        [TestMethod]
        public void FiltrarPorMedicamentoSinSock()
        {
            paracetamol.Stock = 0;
            mockFarmacia.Setup(x => x.GetFarmaciasConProductosPorNombreyStock(paracetamol.Nombre, true)).Returns(new List<Farmacia>());

            List<MedicamentoConFarmaciaDTO> medicamentos = servicioProducto.FiltrarPorMedicamentoYFarmacia(paracetamol.Nombre, farmaciaNoNull.Nombre);

            Assert.AreEqual(medicamentos.Count, 0);
            
            mockProducto.VerifyAll();
        }

        [TestMethod]
        public void FiltrarPorMedicamentoYNoFarmacia()
        {
            paracetamol.Stock = 0;
            mockFarmacia.Setup(x => x.GetFarmaciasConProductosPorNombreyStock(paracetamol.Nombre, false)).Returns(new List<Farmacia>() { farmaciaNoNull });

            List<MedicamentoConFarmaciaDTO> medicamentos = servicioProducto.FiltrarPorMedicamentoYFarmacia(paracetamol.Nombre, "");

            Assert.AreEqual(medicamentos[0].IdFarmacia, farmaciaNoNull.Id);
            Assert.AreEqual(medicamentos[0].Medicamento.Id, paracetamol.Id);
            mockProducto.VerifyAll();
        }
        [TestMethod]
        public void GetMedicamentosDeEmpleadoOk()
        {
            aspirina.DadoDeBaja = true;
            farmaciaNoNull.Productos = new List<Producto>() { paracetamol, aspirina };
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenNoNull)).Returns(new Empleado());
            mockSesion.Setup(x => x.GetFarmaciaByToken(tokenNoNull)).Returns(farmaciaNoNull);
 
            List<Medicamento> medicamentos = servicioProducto.GetMedicamentosDeEmpleado(tokenNoNull);

            Assert.AreEqual(medicamentos[0].Id, paracetamol.Id);
            Assert.AreEqual(medicamentos.Count, 1);
            mockProducto.VerifyAll();
        }
        
      
        [TestMethod]
        [ExpectedException(typeof(NotEnoughPrivilegesException))]
        public void GetMedicamentosDeEmpleadoSiendoAnonimo()
        {
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenNoNull)).Throws(new NotFoundException());
            
            List<Medicamento> medicamentos = servicioProducto.GetMedicamentosDeEmpleado(tokenNoNull);

            mockProducto.VerifyAll();
        }
        
        [ExpectedException(typeof(NotEnoughPrivilegesException))]
        [TestMethod]
        public void GetMedicamentosDeEmpleadoSiendoDueno()
        {
            mockSesion.Setup(x => x.GetUsuarioByToken(tokenNoNull)).Returns(new Dueño());

            List<Medicamento> medicamentos = servicioProducto.GetMedicamentosDeEmpleado(tokenNoNull);

            mockProducto.VerifyAll();
        }


    }
}
