using System;
using System.Collections.Generic;

namespace ObligatorioDa2.Domain.Entidades
{
    public class Compra
    {
        public int Id { get; set; }
        public List<CantidadProductosCompra> Productos { get; set; }
        public int Monto { get; set; }
        public string MailComprador { get; set; }
        public DateTime FechaCompra { get; set; }

        public int CodigoSeguimiento { get; set; }

        public Compra()
        {
            Productos = new List<CantidadProductosCompra>();
            Monto = 0;
        }

        public Compra (List<CantidadProductosCompra> productos, string mailComprador): this()
        {
            MailComprador = mailComprador;
            Productos= productos;
        }


      
    }
}