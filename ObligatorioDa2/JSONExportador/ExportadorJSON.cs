using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Domain.Util;
using ObligatorioDa2.ILogicaDeNegocio;



namespace ExportadorJSON
{
    public class ExportadorJSON : IExportador
    {
        public string GetNombre()
        {
            return "JSON";
        }

        public void Exportar(List<Medicamento> medicamentos, List<ParametroDTO> parametros)
        {

            string nombre = parametros.FirstOrDefault(i => i.Nombre == "Nombre de archivo")?.Valor+".json";
            string json = JsonSerializer.Serialize(medicamentos);
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
                Byte[] jsonBytes = new UTF8Encoding(true).GetBytes(json);
                fs.Write(jsonBytes, 0, jsonBytes.Length);
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
