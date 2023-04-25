using System;
using ObligatorioDa2.Domain.Util;

namespace ObligatorioDa2.Domain.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Mail { get; set; }
        public string Contrasena { get; set; }
        public string Direccion { get; set; }
        public string NombreDeUsuario { get; set; }
        public DateTime FechaDeRegistro { get; set; }

        public Usuario(string unMail, string unaContraseña, string unaDireccion, string nombreDeUsuario):this()
        {
            this.Mail = unMail;
            this.Contrasena = unaContraseña;
            this.Direccion = unaDireccion;
            
            this.NombreDeUsuario = nombreDeUsuario;
        }
        public Usuario()
        {
            this.FechaDeRegistro = DateTime.Now;

        }

        public virtual Enumeradores.Rol? ConseguirTipo()
        {
            return null;
        }

    }
}
