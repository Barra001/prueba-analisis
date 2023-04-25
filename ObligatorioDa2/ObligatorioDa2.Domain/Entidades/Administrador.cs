using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.Domain.Entidades
{
    public class Administrador : Usuario
    {
        public Administrador() { }
        public Administrador(string unMail, string unaContraseña, string unaDireccion, string nombreDeUsuario) : 
            base(unMail,  unaContraseña,  unaDireccion,  nombreDeUsuario)
        {
        }
        public override Enumeradores.Rol? ConseguirTipo()
        {
            return Enumeradores.Rol.Administrador;
        }
    }
}
