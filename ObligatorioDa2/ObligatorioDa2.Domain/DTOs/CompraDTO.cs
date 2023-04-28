using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Domain.Util;
using System;
using System.Collections.Generic;

namespace ObligatorioDa2.Domain.DTOs
{
    public class CompraDTO
    {

        public CompraDTO()
        {
            FechaCompra = DateTime.Now;
            CodigoSeguimiento = GeneradorCodigoRandom.RandomCode();
            MailComprador = "mail@prueba2.com";
        }

        public CompraDTO(List<CantidadProductosCompra> productos, string mailComprador) : this()
        {
            MailComprador = mailComprador;
            Productos = productos;
        }


        public List<CantidadProductosCompra> Productos { get; set; }
        public string MailComprador { get; set; }
        public DateTime FechaCompra { get; set; }

        public int CodigoSeguimiento { get; set; }

    }
}
