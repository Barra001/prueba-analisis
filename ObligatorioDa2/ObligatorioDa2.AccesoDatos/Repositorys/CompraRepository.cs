using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;

namespace ObligatorioDa2.AccesoDatos.Repositorys
{
    public class CompraRepository : ICompraRepository
    {

        private readonly DbSet<Compra> compraEntities;
        private readonly DbContext context;

        public CompraRepository(DbContext context)
        {
            compraEntities = context.Set<Compra>();

            this.context = context;
        }
        public bool Exists(int key)
        {
            return compraEntities.Any(unaCompra => unaCompra.Id == key);
        }

        public IEnumerable<Compra> GetAll()
        {

            List<Compra> compras = compraEntities
                .Include(compra => compra.Productos)
                .ThenInclude(producto => producto.Producto)
                .ToList();
            return compras;

        }

        public IEnumerable<Compra> GetComprasPorCodigoDeSeguimiento(int codigo)
        {

            List<Compra> compras = compraEntities
                .Where(x => x.CodigoSeguimiento.Equals(codigo))
               .Include(compra => compra.Productos)
                .ThenInclude(producto => producto.Producto)
                .ToList();

            if (compras.Count() == 0)
            {
                throw new NotFoundException("No existe compra con el código de seguimiento ingresado");
            }
            return compras;

        }

        public Compra GetById(int id)
        {

            if (!Exists(id))
            {
                throw new NotFoundException($"Compra {id} no encontrada");
            }
            return compraEntities.AsNoTracking().Include(compra => compra.Productos)
                .ThenInclude(producto => producto.Producto)
                .FirstOrDefault(unaCompra => unaCompra.Id == id);

        }


        public void InsertarCompra(Compra unaCompra, Farmacia unaFarmacia)
        {

            if (Exists(unaCompra.Id))
            {
                throw new AccesoDatosException($"Compra ya existe");
            }

           unaCompra.Productos = ActualizarProductos(unaCompra.Productos, unaFarmacia.Productos);

            Insertar(unaCompra);
            unaFarmacia.Compras.Add(unaCompra);
            context.Entry(unaFarmacia).State = EntityState.Modified;
            context.SaveChanges();
           
        }

        private List<CantidadProductosCompra> ActualizarProductos(List<CantidadProductosCompra> productosConStock, List<Producto> productosOriginales)
        {
            List<CantidadProductosCompra> devolver = new List<CantidadProductosCompra>();
            foreach (CantidadProductosCompra cantProdConStock in productosConStock)
            {
                Producto elProductoConStock = cantProdConStock.Producto;
                Producto original = productosOriginales.Find(x => x.Id == elProductoConStock.Id);                           
                context.Entry(original).State = EntityState.Modified;
                context.SaveChanges();
                CantidadProductosCompra devuelvo = new CantidadProductosCompra(cantProdConStock.Cantidad, original);
                devolver.Add(devuelvo);
            }
            return devolver;
        }

        public void Insertar(Compra value)
        {
            compraEntities.Attach(value);
            context.SaveChanges();
        }

      

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(Compra unaCompra)
        {

            if (!Exists(unaCompra.Id))
            {
                throw new NotFoundException($"Compra {unaCompra.Id} no existe");
            }

            context.Entry(unaCompra).State = EntityState.Modified;
            context.SaveChanges();

        }
        public void UpdateEstadoProducuto(CantidadProductosCompra unaCantidad)
        {


            context.Entry(unaCantidad).State = EntityState.Modified;
            context.SaveChanges();

        }
    }
}
