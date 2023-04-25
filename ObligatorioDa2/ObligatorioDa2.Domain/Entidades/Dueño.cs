using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.Domain.Entidades
{
    public class Dueño : Trabajador
    {
        public Dueño(string unMail, string unaContraseña, string unaDireccion, string nombreDeUsuario, Farmacia unaFarmacia) : 
            base(unMail, unaContraseña, unaDireccion, nombreDeUsuario,unaFarmacia)
        {
        }

        public Dueño()
        {

        }
        public override Enumeradores.Rol? ConseguirTipo()
        {
            return Enumeradores.Rol.Dueño;
        }
    }
}
