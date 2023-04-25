using ObligatorioDa2.Domain.Entidades;

namespace ObligatorioDa2.Domain.DTOs
{
    public class CompraConFarmaciaDTO
    {

        public CompraConFarmaciaDTO()
        {
          Compra = new Compra();
        }

        public CompraConFarmaciaDTO(Compra unaCompra)
        {
            Compra = unaCompra;
        }

        public Farmacia Farmacia { get; set; }
        public Compra Compra { get; set; }


    }
}
