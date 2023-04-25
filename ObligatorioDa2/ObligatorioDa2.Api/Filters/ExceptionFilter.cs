using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ObligatorioDa2.Api.DTOs;
using ObligatorioDa2.Exceptions;

namespace ObligatorioDa2.Api.Filters
{
    public class ExceptionFilter:Attribute,IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            int statusCode = 500;

            ResponseDto response = new ResponseDto()
            {
                ErrorMessage = "Error interno, intente de nuevo mas tarde",
                Code = 2002
            };
         
            if (context.Exception is NullReferenceException)
            {
                statusCode = 400;
                response.Code = 2005;
                response.ErrorMessage = context.Exception.Message;
            }
            if (context.Exception is NotFoundException)
            {
                statusCode = 404;
                response.Code = 2003;
                response.ErrorMessage = context.Exception.Message;
            }
            if (context.Exception is AccesoDatosException)
            {
                statusCode = 409;
                response.Code = 2008;
                response.ErrorMessage = context.Exception.Message;

            }
            if (context.Exception is NotEnoughPrivilegesException)
            {
                statusCode = 403;
                response.Code = 2006;
                response.ErrorMessage = context.Exception.Message;

            }
            if (context.Exception is BusinessLogicException)
            {
                statusCode = 400;
                response.Code = 2001;
                response.ErrorMessage = context.Exception.Message;

            }

            context.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };
        }
    }
}
