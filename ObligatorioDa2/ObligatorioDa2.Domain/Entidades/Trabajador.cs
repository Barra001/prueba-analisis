using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.Domain.Entidades
{
    public class Trabajador : Usuario
    {
        public Farmacia Farmacia { get; set; }

        public Trabajador(string unMail, string unaContraseña, string unaDireccion, string nombreDeUsuario, Farmacia unaFarmacia) 
            : base( unMail, unaContraseña, unaDireccion, nombreDeUsuario) 
        {
            Farmacia = unaFarmacia;
        }

        public Trabajador() : base()
        {
           
        }
        public override Enumeradores.Rol? ConseguirTipo()
        {
            return null;
        }


    }
}
