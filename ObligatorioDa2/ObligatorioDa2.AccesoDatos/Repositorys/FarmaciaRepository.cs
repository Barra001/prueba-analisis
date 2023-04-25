using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.Exceptions;
using ObligatorioDa2.IAccesoDatos;

namespace ObligatorioDa2.AccesoDatos.Repositorys
{
    public class FarmaciaRepository : IFarmaciaRepository
    {
        private readonly DbSet<Farmacia> farmacias;
        private readonly DbContext context;

        public FarmaciaRepository(DbContext context)
        {
            farmacias = context.Set<Farmacia>();
            this.context = context;
        }

        public Farmacia GetById(int id)
        {


            if (!Exists(id))
            {
                throw new NotFoundException($"Farmacia no encontrada");
            }

            return farmacias
                .AsNoTracking()
                .Include(unaFarmacia => unaFarmacia.SolicitudesDeReposicion).ThenInclude(soli => soli.Productos).ThenInclude(cantProd => cantProd.Producto)
                .Include(unaFarmacia => unaFarmacia.Productos)
                .Include(unaFarmacia => unaFarmacia.Compras).ThenInclude(comp => comp.Productos).ThenInclude(cantProd => cantProd.Producto)
                .FirstOrDefault(unaFarmacia => unaFarmacia.Id == id);

        }

        public void Insertar(Farmacia unaFarmacia)
        {

            if (Exists(unaFarmacia.Id))
            {
                throw new AccesoDatosException($"Farmacia ya existe");
            }
            farmacias.Add(unaFarmacia);
            context.SaveChanges();


        }

        public void Update(Farmacia unaFarmacia)
        {

            if (!Exists(unaFarmacia.Id))
            {
                throw new NotFoundException($"Farmacia no existe");
            }


            context.Entry(unaFarmacia).State = EntityState.Modified;
            context.SaveChanges();

        }

        public bool Exists(int key)
        {

            return farmacias.Any(farmaciaEntity => farmaciaEntity.Id == key);

        }

        public IEnumerable<Farmacia> GetAll()
        {

            List<Farmacia> farmaciasLista = farmacias
                .Include(unaFarmacia => unaFarmacia.SolicitudesDeReposicion).ThenInclude(soli => soli.Productos).ThenInclude(cantProd => cantProd.Producto)
                .Include(unaFarmacia => unaFarmacia.Productos)
                .Include(unaFarmacia => unaFarmacia.Compras).ThenInclude(comp => comp.Productos).ThenInclude(cantProd => cantProd.Producto)
                .ToList();
            return farmaciasLista;

        }



        public bool ExisteFarmaciaConNombre(string nombre)
        {

            return farmacias.Any(farmaciaEntity => farmaciaEntity.Nombre == nombre);

        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
