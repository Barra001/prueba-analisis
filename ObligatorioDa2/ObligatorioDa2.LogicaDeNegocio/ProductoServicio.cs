using System;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;
using ObligatorioDa2.ILogicaDeNegocio;
using System.Collections.Generic;
using ObligatorioDa2.Domain.DTOs;
using System.Linq;
using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.LogicaDeNegocio
{
    public class ProductoServicio : IProductoServicio
    {
        private readonly IProductoRepository productoRepository;
        private readonly ISesionServicio sesionServicio;
        private readonly IFarmaciaServicio farmaciaServicio;
        public ProductoServicio(IProductoRepository productoRepository, ISesionServicio sesionServicio, IFarmaciaServicio farmaciaServicio)
        {
            this.productoRepository = productoRepository;
            this.sesionServicio = sesionServicio;
            this.farmaciaServicio = farmaciaServicio;
        }

        public Producto DarDeBajaProducto(int id)
        {
            Producto elProducto = productoRepository.GetById(id);
            if (elProducto.DadoDeBaja)
            {
                throw new BusinessLogicException("Producto ya fue dado de baja");
            }
            elProducto.DadoDeBaja = true;
            productoRepository.Update(elProducto);
            return elProducto;
        }

        public IEnumerable<Medicamento> GetMedicamentos()
        {
            List<Medicamento> returno = new List<Medicamento>();
            foreach (Medicamento unaMed in productoRepository.GetAllMedicamentos())
            {
                if (!unaMed.DadoDeBaja)
                {
                    returno.Add(unaMed);
                }

            }
            return returno;
        }

        public Producto InsertarProducto(Producto producto, string token)
        {
            Farmacia laFarmacia = sesionServicio.GetFarmaciaByToken(token);
            if (EsValidoProducto(producto, laFarmacia))
            {                
                productoRepository.InsertarEnFarmacia(producto, laFarmacia);
            }
            return producto;
        }
        public Producto CambiarStock(int id, int cantidad)
        {
            Producto producto = productoRepository.GetById(id);
            if (producto.DadoDeBaja)
            {
                throw new BusinessLogicException("El producto esta dado de baja");
            }
            producto.Stock += cantidad;
            productoRepository.Update(producto);


            return producto;
        }

        public List<MedicamentoConFarmaciaDTO> FiltrarPorMedicamentoYFarmacia(string medicamentoNombre, string nombreFarmacia)
        {
            List<MedicamentoConFarmaciaDTO> devuelvo = new();

            bool estaNombreFarmacia = !(nombreFarmacia == null || nombreFarmacia == "");

            List<Farmacia> farmaciasConElMedicamento = farmaciaServicio
                .GetFarmaciasConProductosPorNombreyStock(medicamentoNombre, estaNombreFarmacia);


            foreach (Farmacia unaFarmacia in farmaciasConElMedicamento)
            {
                if (estaNombreFarmacia && unaFarmacia.Nombre != nombreFarmacia)
                {
                    continue;
                }

                foreach (Medicamento medicamento in unaFarmacia.Productos)
                {
                    if (medicamento.Nombre == medicamentoNombre && !medicamento.DadoDeBaja)
                    {
                        MedicamentoConFarmaciaDTO combo = new(unaFarmacia.Nombre, unaFarmacia.Id, medicamento);
                        devuelvo.Add(combo);
                    }
                }

            }


            return devuelvo;
        }
        public void UpdateProducto(Producto unProducto)
        {
            productoRepository.Update(unProducto);
        }

        public List<Medicamento> GetMedicamentosDeEmpleado(string token)
        {
            bool huboError = false;
            Usuario usuarioByToken = null;
            try
            {
                usuarioByToken = sesionServicio.GetUsuarioByToken(token);
            }
            catch (NotFoundException)
            {
                huboError = true;
            }
            huboError = huboError || usuarioByToken.ConseguirTipo() != Enumeradores.Rol.Empleado;
            if (huboError)
            {
                throw new NotEnoughPrivilegesException("Solo los empleados pueden acceder a esta función");
            }
            Farmacia laFarm = sesionServicio.GetFarmaciaByToken(token);
            List<Medicamento> devuelvo = new List<Medicamento>();
            foreach (Producto elemnt in laFarm.Productos)
            {
                Medicamento elMed = (Medicamento)elemnt;
                if (!elMed.DadoDeBaja)
                {
                    devuelvo.Add(elMed);
                }
            }
            return devuelvo;

        }
        private bool EsValidoProducto(Producto producto, Farmacia farmacia)
        {
            if (producto == null)
            {
                throw new NullReferenceException("El medicamento no puede ser null");
            }
            if (producto.Codigo == null)
            {
                throw new NullReferenceException("El codigo no puede ser null");
            }
            if (producto.Nombre == null)
            {
                throw new NullReferenceException("El nombre no puede ser null");
            }
            if (TieneCodigoRepetido(producto.Codigo, farmacia))
            {
                throw new BusinessLogicException("El codigo ya existe en la farmacia");
            }
            return true;
        }
        
        private bool TieneCodigoRepetido(string codigo, Farmacia farmacia)
        {
            return farmacia.Productos.Any(x => x.Codigo == codigo);
        }

        
    }
}
