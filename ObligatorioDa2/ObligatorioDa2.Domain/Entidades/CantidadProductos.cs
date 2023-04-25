namespace ObligatorioDa2.Domain.Entidades
{
    public class CantidadProductos
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public Producto Producto { get; set; }

        public CantidadProductos()
        {

        }
        public CantidadProductos(int unaCantidad, Producto unProducto)
        {
            this.Producto = unProducto;
            this.Cantidad = unaCantidad;
        }
    }
   
}
