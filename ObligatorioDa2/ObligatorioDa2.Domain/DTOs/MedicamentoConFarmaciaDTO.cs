using ObligatorioDa2.Domain.Entidades;

namespace ObligatorioDa2.Domain.DTOs
{
    public class MedicamentoConFarmaciaDTO
    {
        public MedicamentoConFarmaciaDTO(string nombreFarmacia, int idFarmacia, Medicamento medicamento)
        {
            NombreFarmacia = nombreFarmacia;
            IdFarmacia = idFarmacia;
            Medicamento = medicamento;
        }

        public string NombreFarmacia { get; set; }
        public int IdFarmacia { get; set; }
        public Medicamento Medicamento { get; set; }
    }
}
