using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.Domain.Entidades
{
    public class Empleado : Trabajador
    {
        public Empleado(string unMail, string unaContraseña, string unaDireccion, string nombreDeUsuario, Farmacia unaFarmacia) :
            base(unMail, unaContraseña, unaDireccion, nombreDeUsuario, unaFarmacia)
        {
        }
        public Empleado() 
        {
        }
        public override Enumeradores.Rol? ConseguirTipo()
        {
            return Enumeradores.Rol.Empleado;
        }
    }
}
