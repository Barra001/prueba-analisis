using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.Domain.DTOs
{
    public class TokenDTO
    {
        public string Token { get; set; }
        public Enumeradores.Rol Rolusuario { get; set; }
    }
}
