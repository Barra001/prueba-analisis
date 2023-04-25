using System;
using System.Collections.Generic;
using ObligatorioDa2.Domain.Entidades;
using ObligatorioDa2.ILogicaDeNegocio;
using ObligatorioDa2.IAccesoDatos;
using ObligatorioDa2.Exceptions;
using System.Linq;

namespace ObligatorioDa2.LogicaDeNegocio
{
    public class FarmaciaServicio : IFarmaciaServicio
    {
        private readonly IFarmaciaRepository farmaciaRepository;
        private readonly int maxCaracteresNombreFarmacia= 50;

        public FarmaciaServicio(IFarmaciaRepository farmaciaRepository)
        {
            this.farmaciaRepository = farmaciaRepository;
        }


        public IEnumerable<Farmacia> GetFarmacias()
        {
            return farmaciaRepository.GetAll();
        }

     
        public Farmacia GetFarmaciaById(int id)
        {
            return farmaciaRepository.GetById(id);
        }
        public Farmacia UpdateFarmacia(Farmacia farmacia)
        {
            if (EsFarmaciaValida(farmacia))
            {
                farmaciaRepository.Update(farmacia);
            }
            return farmacia;
        }

        public Farmacia InsertFarmacia(Farmacia farmacia)
        {           
            if (EsFarmaciaValida(farmacia) && !NombreFarmaciaYaExiste(farmacia.Nombre))
            {
                farmaciaRepository.Insertar(farmacia);
            }
            return farmacia;
        }

        public List<Farmacia> GetFarmaciasConProductosPorNombreyStock(string nombreMedicamento, bool conStock)
        {
            List<Farmacia> todas = (List<Farmacia>)farmaciaRepository.GetAll();
            List<Farmacia> filtradas = new List<Farmacia>();
            foreach (Farmacia unaFarmacia in todas)
            {

                if (conStock)
                {
                    bool laFarmaciaContieneElMedicamentoConStock = unaFarmacia.Productos.Any(x => x.Nombre == nombreMedicamento && x.Stock > 0);
                    if (laFarmaciaContieneElMedicamentoConStock)
                    {
                        filtradas.Add(unaFarmacia);

                    }

                }
                else
                {
                    bool laFarmaciaContieneElMedicamento = unaFarmacia.Productos.Any(x => x.Nombre == nombreMedicamento);
                    if (laFarmaciaContieneElMedicamento)
                    {
                        filtradas.Add(unaFarmacia);

                    }
                }

            }
            return filtradas;
        }

        private bool NombreFarmaciaYaExiste(string nombre)
        {
            if (farmaciaRepository.ExisteFarmaciaConNombre(nombre)){
                throw new BusinessLogicException("El nombre ya existe");
            }
            return false;
        }

       

       
   

        private bool EsFarmaciaValida(Farmacia farmacia)
        {
            if(farmacia == null)
            {
                throw new NullReferenceException("Debe ingresar una Farmacia");
            }
            if(string.IsNullOrEmpty(farmacia.Nombre))
            {
                throw new BusinessLogicException("La farmacia debe tener nombre");
            }
            if (farmacia.Nombre.Length > maxCaracteresNombreFarmacia)
            {
                throw new BusinessLogicException("El largo máximo del nombre de la farmacia es 50 caracteres");
            }
            
            if (string.IsNullOrEmpty(farmacia.Direccion))
            {
                throw new BusinessLogicException("La farmacia debe tener direccion");
            }
            return true;
        }

      
    }
}
