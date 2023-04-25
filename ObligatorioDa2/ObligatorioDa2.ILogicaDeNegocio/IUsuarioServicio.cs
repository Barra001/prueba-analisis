using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using System.Collections.Generic;

namespace ObligatorioDa2.ILogicaDeNegocio
{
    public interface IUsuarioServicio
    {
        List<UsuarioDto> GetUsuarios();
        public Usuario GetUsuarioByNombre(string nombre);
        public Usuario InsertUsuario(Usuario usuario, int codigo);


    }
}
