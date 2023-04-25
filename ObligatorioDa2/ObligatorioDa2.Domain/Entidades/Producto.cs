namespace ObligatorioDa2.Domain.Entidades
{
    public class Producto
    {
        public int Id { get; set; }
        public Producto()
        {

        }

        public Producto(string nombre, string codigo, int precio) 
        {
            Nombre = nombre;
            Codigo = codigo;
            Precio = precio;
            Stock = 0;
            DadoDeBaja = false;
        }
      
        public string Nombre { get; set; }
     
        public string Codigo { get; set; }
       
        public int Precio { get; set; }
       
        public int Stock { get; set; }
        public bool DadoDeBaja { get; set; }


    }
}