using System.Reflection;
using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.ILogicaDeNegocio;

namespace ObligatorioDa2.Reflection
{
  
    public class ImporterServicio : IImporterServicio
    {
        

        private List<Medicamento> ObtenerMeds(List<Producto> poductos)
        {
            List<Medicamento> devuelvo = new List<Medicamento>();
            for (int i = 0; i < poductos.Count; i++)
            {
               
                try
                {
                    Medicamento med = (Medicamento)poductos[i];
                    devuelvo.Add(med);
                }
                catch (InvalidCastException){continue;};

            }

            return devuelvo;
        }
        public ExportarDTO Exportar(ExportarDTO aExportar, Farmacia laFarmacia)
        {
            List<Medicamento> medicamentos = ObtenerMeds(laFarmacia.Productos);
            IEnumerable<IExportador> exportadors = GetDlls();
            IExportador exportador = exportadors.Where(i => i.GetNombre() == aExportar.Nombre).FirstOrDefault();
            if (exportador != null)
            {
                exportador.Exportar(medicamentos, aExportar.Parametros);
            }
            else
            {
                throw new NotFoundException("No se encontró el exportador");
            }
            return aExportar;
        }

        public IEnumerable<ExportarDTO> GetExportadores()
        {

            IEnumerable<IExportador> exportadors = GetDlls();
            List<ExportarDTO> aDevolver = new List<ExportarDTO>();

            foreach (IExportador exportador in exportadors)
            {
                aDevolver.Add(
                    new ExportarDTO
                    {
                        Nombre = exportador.GetNombre(),
                        Parametros = exportador.GetParametros()
                    }
                );
            }

            return aDevolver;


        }
        private IEnumerable<IExportador> GetDlls()
        {
            string[] files;
            string pathExportadores = Directory.GetCurrentDirectory() +
                                      Path.DirectorySeparatorChar + "Exportadores";
            if (Directory.Exists(pathExportadores))
            {
                files = Directory.GetFiles(pathExportadores, "*.dll");
            }
            else
            {
                Directory.CreateDirectory(pathExportadores);
                files = Directory.GetFiles(pathExportadores, "*.dll");
            }

            if (files.Length == 0)
                throw new NotFoundException("No se encontraron exportadores");
            List<IExportador> aDevolver = new List<IExportador>();

            foreach (string file in files)
            {
                Assembly dll = Assembly.UnsafeLoadFrom(file);
                Type? type = dll.GetTypes().FirstOrDefault(i => typeof(IExportador).IsAssignableFrom(i));

                if (type != null)
                {
                    aDevolver.Add(Activator.CreateInstance(type) as IExportador);
                }
            }

            return aDevolver;
        }

    }

}
