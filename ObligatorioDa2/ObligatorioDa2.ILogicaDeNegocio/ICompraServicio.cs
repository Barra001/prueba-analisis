using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using System;
using ObligatorioDa2.Domain.Util;
using System.Collections.Generic;

namespace ObligatorioDa2.ILogicaDeNegocio
{
    public interface ICompraServicio
    {
        public InfoComprasDto GetComprasPorRangoEnFarmacia(string token, DateTime? desde, DateTime? hasta);
        public List<Compra> GetComprasPorFarmaciaDeEmpleado(string token);
        public CodigoSeguimientoDTO InsertarCompra(CompraDTO compraDto);
        public Compra GetCompraPorCodigoDeSeguimiento(int codigoSeguimiento);
        public Medicamento AceptarORechazarMedicamento(int idCompra, string codigoMedicamento, Enumeradores.EstadoDeCompraProducto estado, string token);
    }
}
