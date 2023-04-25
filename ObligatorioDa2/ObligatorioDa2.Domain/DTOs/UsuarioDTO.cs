using ObligatorioDa2.Domain.Entidades;

namespace ObligatorioDa2.Domain.DTOs
{
    public class UsuarioDto
    {
        public UsuarioDto(Usuario usuario)
        {
            NombreDeUsuario = usuario.NombreDeUsuario;
        }

        public string NombreDeUsuario { get; set; }
    }
}
