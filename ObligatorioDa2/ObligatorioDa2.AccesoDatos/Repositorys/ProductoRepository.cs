using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;

namespace ObligatorioDa2.AccesoDatos.Repositorys
{
    public class ProductoRepository : IProductoRepository
    {

        private readonly DbSet<Producto> productoEntities;
        private readonly DbContext context;

        public ProductoRepository(DbContext context)
        {
            productoEntities = context.Set<Producto>();
            this.context = context;
        }
        public bool Exists(int key)
        {

            return productoEntities.Any(unProducto => unProducto.Id == key);

        }

        public IEnumerable<Producto> GetAll()
        {

            return productoEntities.OfType<Producto>().ToList();

        }

        public IEnumerable<Medicamento> GetAllMedicamentos()
        {


            return productoEntities.ToList().OfType<Medicamento>();

        }

        public Producto GetById(int id)
        {

            if (!Exists(id))
            {
                throw new NotFoundException($"Producto no existe");
            }
            return productoEntities.FirstOrDefault(unaProducto => unaProducto.Id == id);

        }

        public void Insertar(Producto unProducto)
        {

            if (Exists(unProducto.Id))
            {
                throw new AccesoDatosException($"Producto ya existe");
            }
            productoEntities.Add(unProducto);
            context.SaveChanges();

        }

        public void InsertarEnFarmacia(Producto unProducto, Farmacia unaFarmacia)
        {

            if (Exists(unProducto.Id))
            {
                throw new AccesoDatosException($"Producto ya existe");
            }
            productoEntities.Add(unProducto);
            context.SaveChanges();
            unaFarmacia.Productos.Add(unProducto);
            context.Entry(unaFarmacia).State = EntityState.Modified;
            context.SaveChanges();


        }

        public void Update(Producto unProducto)
        {

            if (!Exists(unProducto.Id))
            {
                throw new NotFoundException($"Producto no existe");
            }

            context.Entry(unProducto).State = EntityState.Modified;
            context.SaveChanges();

        }

        public void Save()
        {
            context.SaveChanges();
        }

        public List<Medicamento> GetAllByNombre(string nombre)
        {
            return productoEntities.OfType<Medicamento>().Where(x=>x.Nombre==nombre).ToList();
        }
    }
}
