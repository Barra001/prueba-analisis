using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObligatorioDa2.Domain.Entidades
{
    public class Farmacia
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }

        public List<SolicitudDeReposicion> SolicitudesDeReposicion { get; set; }

        public List<Producto> Productos { get; set; }

        public List<Compra> Compras { get; set; }
        public Farmacia()
        {

        }

        public Farmacia(string nombre, string direccion)
        {
            Nombre = nombre;
            Direccion = direccion;
            SolicitudesDeReposicion = new List<SolicitudDeReposicion> { };
            Productos= new List<Producto> { };
            Compras= new List<Compra> { };

        }

      


    }
}
