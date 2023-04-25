using System;
using EmailValidation;
using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;
using ObligatorioDa2.ILogicaDeNegocio;
using System.Collections.Generic;
using System.Linq;
using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.LogicaDeNegocio
{
    public class CompraServicio: ICompraServicio
    {

        private readonly ICompraRepository compraRepository;
        private readonly ISesionServicio sesionServicio;
        private readonly IFarmaciaServicio farmaciaServicio;
        private readonly IProductoServicio productoServicio;
        public CompraServicio(ICompraRepository compraRepository, ISesionServicio sesionServicio, IFarmaciaServicio farmaciaServicio, IProductoServicio productoServicio)
        {
            this.compraRepository = compraRepository;
            this.sesionServicio = sesionServicio;
            this.farmaciaServicio = farmaciaServicio;
            this.productoServicio = productoServicio;
        }

        public List<Compra> GetComprasPorFarmaciaDeEmpleado(string token)
        {
            Usuario usuarioByToken = sesionServicio.GetUsuarioByToken(token);
            if (usuarioByToken.ConseguirTipo() != Enumeradores.Rol.Empleado)
            {
                throw new NotEnoughPrivilegesException("Solo los empleados pueden acceder a esta función");
            }
           

            if (usuarioByToken.ConseguirTipo() != Enumeradores.Rol.Empleado)
            {
                throw new NotEnoughPrivilegesException("Solo los empleados pueden acceder a esta función");
            }
            Farmacia unaFarmacia = sesionServicio.GetFarmaciaByToken(token);
            if (unaFarmacia.Compras.Count == 0)
            {

                throw new NotFoundException("No se ha hecho ninguna compra en la farmacia");
            }
            else 
            {
                return unaFarmacia.Compras;
            }
           
        }


        public InfoComprasDto GetComprasPorRangoEnFarmacia(string token, DateTime? desde, DateTime? hasta)
        {
           
            Usuario usuarioByToken = sesionServicio.GetUsuarioByToken(token);
            if (usuarioByToken.ConseguirTipo() != Enumeradores.Rol.Dueño)
            {
                throw new NotEnoughPrivilegesException("Solo los Dueños pueden acceder a esta función");
            }
            if (hasta < desde)
            {
                throw new BusinessLogicException("La fecha hasta no puede ser mayor a la fecha desde");
            }

            InfoComprasDto infoComprasDto = new();
            Farmacia farmacia = sesionServicio.GetFarmaciaByToken(token);
            List<Compra> compras = farmacia.Compras;
            foreach (Compra compra in compras)
            {
                if(compra.FechaCompra >= desde && compra.FechaCompra <= hasta)
                {
                    List<CantidadProductosCompra> stay = new List<CantidadProductosCompra>();
                    foreach (CantidadProductosCompra cant in compra.Productos)
                    {
                        if (cant.EstadoDeCompraProducto == Enumeradores.EstadoDeCompraProducto.Aceptada)
                        {
                            stay.Add(cant);
                        }
                    }
                    compra.Productos = stay;
                    infoComprasDto.ListaCompras.Add(compra);
                    infoComprasDto.Total += compra.Monto;
                }
            }
            return infoComprasDto;
        }

        public CodigoSeguimientoDTO InsertarCompra(CompraDTO compraDto)
        {

            if (EsCompraValida(compraDto))
            {
                List<CompraConFarmaciaDTO> comprasPorFarmacia = AgruparComprasPorFarmacia(compraDto);
                AgregarMontoACadaCompra(comprasPorFarmacia);
                foreach (CompraConFarmaciaDTO compraAInsertarDto in comprasPorFarmacia)
                {
                    Compra compraAInsertar = compraAInsertarDto.Compra;
                    Farmacia farmaciaCompra = compraAInsertarDto.Farmacia;
                    compraRepository.InsertarCompra(compraAInsertar, farmaciaCompra);
                }
            }

            return new CodigoSeguimientoDTO() {Codigo = compraDto.CodigoSeguimiento};

        }
        public Compra GetCompraPorCodigoDeSeguimiento(int codigoSeguimiento)
        {
            if (codigoSeguimiento.ToString().Length != 6)
            {
                throw new BusinessLogicException("Codigo de seguimiento ingresado incorrecto : Debe tener 6 dígitos ");
            }
            List<Compra> comprasSeparadas = GetComprasPorCodigoDeSeguimiento(codigoSeguimiento);
            Compra compraADevolver = new Compra();
            AsignarInformacionCompraADevolver(compraADevolver, comprasSeparadas[0]);
            foreach (Compra compra in comprasSeparadas)
            {
                compraADevolver.Monto += compra.Monto;
                foreach (CantidadProductosCompra cantProductoCompra in compra.Productos)
                {
                    compraADevolver.Productos.Add(cantProductoCompra);

                }
            }
            return compraADevolver;


        }


        public Medicamento AceptarORechazarMedicamento(int idCompra, string codigoMedicamento, Enumeradores.EstadoDeCompraProducto estado, string token)
        {
            Compra laCompra = compraRepository.GetById(idCompra);
            bool laCompraEsDeLaMismaFarmaciaQueElEmpleado = sesionServicio.GetFarmaciaByToken(token).Compras.Any(x => x.Id == laCompra.Id);
            if (!laCompraEsDeLaMismaFarmaciaQueElEmpleado)
            {
                throw new BusinessLogicException("Solo los empleados de la farmacia de donde se hizo la compra pueden modificar su estado");
            }

            CantidadProductosCompra cantidadProductosCompra = laCompra.Productos.FirstOrDefault(x => x.Producto.Codigo == codigoMedicamento);
            if (cantidadProductosCompra == null)
            {
                throw new NotFoundException("No existe ese medicamento en la compra");
            }
            if (cantidadProductosCompra.EstadoDeCompraProducto != Enumeradores.EstadoDeCompraProducto.Pendiente)
            {
                throw new BusinessLogicException("El producto no está en estado pendiente");
            }
            if (estado == Enumeradores.EstadoDeCompraProducto.Pendiente)
            {
                throw new BusinessLogicException("El producto se debe aceptar o rechazar");
            }

            cantidadProductosCompra.EstadoDeCompraProducto = estado;
            if (estado == Enumeradores.EstadoDeCompraProducto.Aceptada)
            {
                cantidadProductosCompra.Producto.Stock -= cantidadProductosCompra.Cantidad;
                productoServicio.UpdateProducto(cantidadProductosCompra.Producto);
            }
            compraRepository.UpdateEstadoProducuto(cantidadProductosCompra);

            return (Medicamento)cantidadProductosCompra.Producto;
        }

        private List<CompraConFarmaciaDTO> AgruparComprasPorFarmacia( CompraDTO compra)
        {
            List<CompraConFarmaciaDTO> comprasOrdenadas = new List<CompraConFarmaciaDTO>();
            List<Farmacia> farmacias = (List<Farmacia>)farmaciaServicio.GetFarmacias();
            foreach (CantidadProductosCompra cantProducto in compra.Productos)
            {
                Farmacia farmaciaProducto = farmacias.Find(x => x.Productos.Any(y => y.Id == cantProducto.Producto.Id));
                ChequeoProductoNoDadoDeBaja(cantProducto.Producto, farmaciaProducto);
                VerificarStock(cantProducto, farmaciaProducto);

                if (!(comprasOrdenadas.Any(x => x.Farmacia.Id == farmaciaProducto.Id)))
                {
                    CompraConFarmaciaDTO unaCompraDTO = new CompraConFarmaciaDTO();
                    unaCompraDTO.Farmacia = farmaciaProducto;
                    unaCompraDTO.Compra.Productos = new List<CantidadProductosCompra> { cantProducto };
                    unaCompraDTO.Compra.MailComprador = compra.MailComprador;
                    unaCompraDTO.Compra.FechaCompra = compra.FechaCompra;
                    unaCompraDTO.Compra.CodigoSeguimiento= compra.CodigoSeguimiento;
                    comprasOrdenadas.Add(unaCompraDTO);
         
                }
                else
                {
                    comprasOrdenadas.Find(x => x.Farmacia.Id == farmaciaProducto.Id).Compra.Productos.Add(cantProducto);
                }
                
            }

            return comprasOrdenadas;
        }

        private bool EsCompraValida(CompraDTO unaCompraDto)
        {
            
            if(unaCompraDto == null)
            {
                throw new NullReferenceException("La compra no puede ser null");
            }
       
            if (unaCompraDto.Productos==null)
            {
                throw new BusinessLogicException("La compra debe tener productos");
            }

            if (unaCompraDto.MailComprador==null || !MailValido(unaCompraDto.MailComprador))
            {
                throw new BusinessLogicException("El mail del usuario debe ser válido");
            }

            return true;

        }

        private void ChequeoProductoNoDadoDeBaja(Producto producto, Farmacia unaFarmacia)
        {
                Producto productoVerificar = unaFarmacia.Productos.Find(x => x.Id == producto.Id);
                if (productoVerificar.DadoDeBaja)
                {
                    throw new BusinessLogicException($"El producto ${productoVerificar.Nombre} ha sido dado de baja y no está disponible para su compra");
                }
            
        }


        private void VerificarStock(CantidadProductosCompra cantProducto, Farmacia unaFarmacia)
        {
          
                Producto productoCalcular = unaFarmacia.Productos.Find(x => x.Id == cantProducto.Producto.Id);
                if (productoCalcular.Stock < cantProducto.Cantidad)
                {
                    throw new BusinessLogicException($"No hay suficiente stock de {productoCalcular.Nombre}");
                }
            
        }

        private bool MailValido(string mail)
        {
            return EmailValidator.Validate(mail.Trim());
        }

        private void AgregarMontoACadaCompra(List<CompraConFarmaciaDTO> comprasDto)
        {
         
            foreach(CompraConFarmaciaDTO compraDto in comprasDto)
            {
                foreach (CantidadProductosCompra cantProducto in compraDto.Compra.Productos)
                {
                    Producto productoCalcular = compraDto.Farmacia.Productos.Find(x => x.Id == cantProducto.Producto.Id);
                    Producto productoActual = cantProducto.Producto;
                    compraDto.Compra.Monto += (cantProducto.Cantidad * productoCalcular.Precio);
                }
            }
            
        }


      

        private void AsignarInformacionCompraADevolver(Compra compraADevolver, Compra compra)
        {
            compraADevolver.FechaCompra = compra.FechaCompra;
            compraADevolver.MailComprador = compra.MailComprador;
        }

        private List<Compra> GetComprasPorCodigoDeSeguimiento(int codigoSeguimiento)
        {
            return (List<Compra>)compraRepository.GetComprasPorCodigoDeSeguimiento(codigoSeguimiento);
        }
    }
}
