using ObligatorioDa2.Domain.DTOs;
using ObligatorioDa2.Domain.Entidades;
using System.Collections.Generic;

namespace ObligatorioDa2.ILogicaDeNegocio
{
    public interface IProductoServicio
    {
        public IEnumerable<Medicamento> GetMedicamentos();
        Producto InsertarProducto(Producto producto, string token);
        public Producto DarDeBajaProducto(int id);
        Producto CambiarStock(int id, int cantidad);
        public List<MedicamentoConFarmaciaDTO> FiltrarPorMedicamentoYFarmacia(string medicamentoNombre, string nombre);
        public void UpdateProducto(Producto unProducto);
        public List<Medicamento> GetMedicamentosDeEmpleado(string token);
    }
}
