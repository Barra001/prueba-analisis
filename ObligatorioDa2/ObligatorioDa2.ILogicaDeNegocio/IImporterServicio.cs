using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ObligatorioDa2.ILogicaDeNegocio
{
    public interface IImporterServicio
    {
        public IEnumerable<ExportarDTO> GetExportadores();
        public ExportarDTO Exportar(ExportarDTO aExportar, Farmacia laFarmacia);


    }
}
