namespace ObligatorioDa2.Domain.Util
{
    public class Enumeradores
    {

        public enum EstadoDeSolicitud
        {
            Pendiente,
            Aceptada,
            Rechazada
        }
        public enum Rol
        {
            Dueño,
            Empleado,
            Administrador
        }

        public enum Presentacion
        {
            Capsulas,
            Comprimidos,
            SolucionSoluble,
            StickPack,
            Liquido
        }
        public enum Tipo
        {
            String,
            Int,
            Double,
            DateTime,
            Boolean
        }
        public enum Unidad
        {
            Miligramos,
            Gramos,
            Mililitros,
            Litros,
            Comprimidos
        }

        public enum EstadoDeCompraProducto
        {
            Pendiente,
            Aceptada,
            Rechazada
        }
    }

}

