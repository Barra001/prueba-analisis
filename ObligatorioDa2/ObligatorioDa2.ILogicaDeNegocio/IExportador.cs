using System.Collections.Generic;
using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;

namespace ObligatorioDa2.ILogicaDeNegocio
{
    public interface IExportador
    {
        string GetNombre();
        void Exportar(List<Medicamento> medicamentos, List<ParametroDTO> parametros);
        List<ParametroDTO> GetParametros();

    }
}
