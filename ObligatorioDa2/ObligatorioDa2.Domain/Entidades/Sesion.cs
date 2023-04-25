using System;


namespace ObligatorioDa2.Domain.Entidades
{
    public class Sesion
    {
        public Sesion()
        {
            Token = Guid.NewGuid().ToString();
        }

        public Sesion(Usuario usuario) :this()
        {
            UsuarioSesion = usuario;
        
        }

        public Usuario UsuarioSesion { get; set; }

        public int Id { get; set; }

        public string Token { get; set; }
    }
}
