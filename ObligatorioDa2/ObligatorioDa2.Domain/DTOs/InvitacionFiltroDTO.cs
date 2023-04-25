using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.Domain.DTOs
{
    public class InvitacionFiltroDTO
    {
        public int? FarmaciaId { get; set; }
        public string NombreDeUsuario { get; set; }
        public Enumeradores.Rol? Rol { get; set; }
        public string token;

        public InvitacionFiltroDTO(int? unaFarmaciaId, string nombreDeUsuario, Enumeradores.Rol? rol, string token)
        {
            FarmaciaId = unaFarmaciaId;
            NombreDeUsuario = nombreDeUsuario;
            Rol = rol;
            this.token = token;
        }
    }
}
