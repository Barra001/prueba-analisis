using ObligatorioDa2.Domain.Entidades;
using System.Collections.Generic;

namespace ObligatorioDa2.Domain.DTOs
{
    public class InfoComprasDto
    {
        public InfoComprasDto()
        {
            Total = 0;
            ListaCompras = new List<Compra>();
        }

        public int Total { get; set; }

        public List<Compra> ListaCompras { get; set; }
    }
}
