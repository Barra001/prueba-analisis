using static ObligatorioDa2.Domain.Util.Enumeradores;

namespace ObligatorioDa2.Domain.Entidades
{
    public class CantidadProductosCompra
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public Producto Producto { get; set; }

        public EstadoDeCompraProducto EstadoDeCompraProducto { get; set; }

        public CantidadProductosCompra()
        {
            EstadoDeCompraProducto = EstadoDeCompraProducto.Pendiente;

        }
        public CantidadProductosCompra(int unaCantidad, Producto unProducto): this()
        {
            this.Producto = unProducto;
            this.Cantidad = unaCantidad;
        }
    }

}
