using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.Domain.DTOs
{
    public class ParametroDTO
    {
        public string Nombre { get; set; }
        public Enumeradores.Tipo Tipo { get; set; }
        public string Valor { get; set; }
    }
  
}
