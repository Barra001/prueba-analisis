using System.Collections.Generic;
using ObligatorioDa2.Domain.Entidades;

namespace ObligatorioDa2.ILogicaDeNegocio
{
    public interface IFarmaciaServicio
    {
        IEnumerable<Farmacia> GetFarmacias();
        Farmacia InsertFarmacia(Farmacia farmacia);
        Farmacia GetFarmaciaById(int id);
        Farmacia UpdateFarmacia(Farmacia farmacia);
        public List<Farmacia> GetFarmaciasConProductosPorNombreyStock(string nombreMedicamento, bool conStock);
    }
}
