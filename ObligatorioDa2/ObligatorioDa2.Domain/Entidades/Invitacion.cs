using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.Domain.Entidades
{
    public class Invitacion
    {
        public int Id { get; set; }
        public string NombreDeUsuario { get; set; }
        public int Codigo { get; set; }
        public Enumeradores.Rol RolInvitado { get; set; }
        public Farmacia Farmacia { get; set; }
        public bool Usada { get; set; }

        public Invitacion(string nombreDeUsuario, Enumeradores.Rol rolInvitado, Farmacia farmacia):this()
        {
            NombreDeUsuario = nombreDeUsuario;
            RolInvitado = rolInvitado;
            Farmacia = farmacia;
            Usada = false;
        }

        public Invitacion()
        {
            Codigo = GeneradorCodigoRandom.RandomCode();
        }

        
    }
}
