using ObligatorioDa2.Domain.Entidades;
using System.Collections.Generic;

namespace ObligatorioDa2.IAccesoDatos
{

    public interface IRepository<T>
    {
        void Insertar(T value);
        T GetById(int id);
        void Update(T value);
        IEnumerable<T> GetAll();
        bool Exists(int key);

        void Save();

    }

    public interface IFarmaciaRepository : IRepository<Farmacia>
    {
        public bool ExisteFarmaciaConNombre(string nombre);
      


    }
    public interface ISolicitudDeReposicionRepository : IRepository<SolicitudDeReposicion>
    {
        public void InsertarSolicitud(SolicitudDeReposicion unaSolicitudDeReposicion, Farmacia unaFarmacia);
        
        public IEnumerable<SolicitudDeReposicion> GetSolicitudesDe(Empleado unEmpleado);
    }
    public interface IProductoRepository : IRepository<Producto>
    {
        void InsertarEnFarmacia(Producto value, Farmacia laFarmacia);
        public IEnumerable<Medicamento> GetAllMedicamentos();
        public List<Medicamento> GetAllByNombre(string nombre);
    }
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        bool ExisteUsuarioConMail(string mail);
        Trabajador GetByIdTrabajador(int id);
    
        public Usuario GetByNombre(string nombre);
    }

    public interface ICompraRepository : IRepository<Compra>
    {
        public void UpdateEstadoProducuto(CantidadProductosCompra unaCantidad);
        public void InsertarCompra(Compra unaCompra, Farmacia unaFarmacia);

        public IEnumerable<Compra> GetComprasPorCodigoDeSeguimiento(int codigo);
    }

    public interface IInvitacionRepository : IRepository<Invitacion>
    {
        bool ExisteInvitacionConNombre(string invitacionNombreDeUsuario);
        public Invitacion GetByNombre(string invitacionNombreDeUsuario);
        bool ExisteOtraInvitacionConEsteNombre(Invitacion invitacion);

    }
    

    public interface ISesionRepository : IRepository<Sesion>
    {
        public Usuario GetUsuarioByToken(string token);
        public Sesion GetSesionByToken(string value);
        public void Delete(int id);
        public bool ExistsUsuario(string nombreUsuario);
        public string GetTokenFromUserName(string nombreUsuario);


        public Farmacia GetFarmaciaByToken(string token);

    }


}