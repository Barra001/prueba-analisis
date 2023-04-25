using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ObligatorioDa2.Api.DTOs;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Domain.Util;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.ILogicaDeNegocio;

namespace ObligatorioDa2.Api.Filters
{
    public class AuthorizationWithParameterFilter : Attribute, IAuthorizationFilter
    {
        private readonly string rolesHabilitados;
        private AuthorizationFilterContext theContext;
        public AuthorizationWithParameterFilter(string rolesHabilitados)
        {
            this.rolesHabilitados = rolesHabilitados;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            this.theContext = context;
            string token = context.HttpContext.Request.Headers["token"];
            if (String.IsNullOrEmpty(token))
            {
                string errorMessage = "Debe ingresar un userToken en el header";
                int internalCode = 2004;
                int statusCode = 401;

                SendResponse(errorMessage, internalCode, statusCode);
            }
            else
            {
                ISesionServicio sessions = this.GetSessionLogic();
                Usuario usuarioByToken;
                try
                {
                    usuarioByToken = sessions.GetUsuarioByToken(token);
                }
                catch (AccesoDatosException)
                {
                    int internalCode = 2003;
                    string errorMessage = "El userToken ingresado no es correcto";
                    int statusCode = 401;
                    SendResponse(errorMessage, internalCode, statusCode);
                    return;
                }

                string rolRecibido = PasarRolAString(usuarioByToken.ConseguirTipo());
                if (!rolesHabilitados.Contains(rolRecibido))
                {
                    int internalCode = 2006;
                    string errorMessage = "El userToken ingresado no tiene habilitada esta operación";
                    int statusCode = 403;
                    SendResponse(errorMessage, internalCode, statusCode);
                }


            }
        }

        public ISesionServicio GetSessionLogic()
        {
            Type sessionType = typeof(ISesionServicio);

            return theContext.HttpContext.RequestServices.GetService(sessionType) as ISesionServicio;
        }

        private string PasarRolAString(Enumeradores.Rol? unRol)
        {

            switch (unRol)
            {
                case Enumeradores.Rol.Administrador:
                    return "Administrador";

                case Enumeradores.Rol.Dueño:
                    return "Dueño";
                case Enumeradores.Rol.Empleado:
                    return "Empleado";
                default:
                    string mensajeDeError = "Error interno";
                    int internalCode = 2007;
                    int externalCode = 500;
                    SendResponse(mensajeDeError, internalCode, externalCode);
                    return null;
            }
        }

        private void SendResponse(string mensajeDeError, int internalCode, int externalCode)
        {
            ResponseDto response = new ResponseDto
            {
                Code = internalCode,
                ErrorMessage = mensajeDeError
            };
            theContext.Result = new ObjectResult(response)
            {
                StatusCode = externalCode,
            };
        }
    }


}