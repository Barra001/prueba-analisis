using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Domain.Util;
using ObligatorioDa2.ILogicaDeNegocio;



namespace ExportadorXML
{
    public class ExportadorXML : IExportador
    {
        public string GetNombre()
        {
            return "XML";
        }

        public void Exportar(List<Medicamento> medicamentos, List<ParametroDTO> parametros)
        {
            string nombre = parametros.FirstOrDefault(i => i.Nombre == "Nombre de archivo")?.Valor + ".xml";
            XmlSerializer x = new XmlSerializer(medicamentos.GetType());
            
            if (File.Exists(nombre))
            {
                File.Delete(nombre);
            }
            if (Directory.Exists("." + Path.DirectorySeparatorChar + "MedicamentosExportados") == false)
            {
                Directory.CreateDirectory("." + Path.DirectorySeparatorChar + "MedicamentosExportados");
            }
            
            using (FileStream fs = File.Create("."+ Path.DirectorySeparatorChar + "MedicamentosExportados" 
                + Path.DirectorySeparatorChar + nombre))
            {
                x.Serialize(fs, medicamentos);
            }
        }

        public List<ParametroDTO> GetParametros()
        {
            return new List<ParametroDTO>()
            {
                new ParametroDTO()
                {
                    Nombre = "Nombre de archivo",
                    Tipo = Enumeradores.Tipo.String,
                    Valor = ""
                }
            };
        }
    }
}