namespace FacturasAxoft.Excepciones
{
    public class FacturaAxoftException : Exception
    {
        public FacturaAxoftException(string message) : base(message)
        {
        }
    }

    public class FacturaConFechaInvalidaException : FacturaAxoftException
    {
        public FacturaConFechaInvalidaException() :
            base("La fecha de la factura es inválida. Existen facturas grabadas con fecha posterior a la ingresada.")
        {
        }
    }
}
