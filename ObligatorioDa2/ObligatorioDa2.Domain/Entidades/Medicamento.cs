using ObligatorioDa2.Domain.Util;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioDa2.Domain.Entidades
{
    public class Medicamento : Producto
    {
        public Medicamento()
        {
        }

        public Medicamento(string nombre,string codigo, int precio, string sintomas, Enumeradores.Presentacion presentacion, Enumeradores.Unidad unidad, int cantidad, bool receta)
        : base(nombre, codigo, precio)
        {
            Sintomas = sintomas;
            Presentacion = presentacion;
            Unidad = unidad;
            CantidadPorPresentacion = cantidad;
            Receta = receta;
        }
        [Required]
        public string Sintomas { get; set; }
        [Required]
        public Enumeradores.Presentacion Presentacion { get; set; }
        [Required]
        public Enumeradores.Unidad Unidad { get; set; }
        [Required]
        public int CantidadPorPresentacion { get; set; }
        [Required]
        public bool Receta { get; set; }

    }
}
