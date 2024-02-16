namespace FacturasAxoft.Clases
{
    /// <summary>
    /// Clase que representa a una factura.
    /// Puede que sea necesario modificarla para hacer las implementaciones requeridas.
    /// </summary>
    public class Factura
    {
        public int Numero;
        public DateTime Fecha;
        public Cliente Cliente;
        public List<RenglonFactura> Renglones;
    }

    /// <summary>
    /// Clase que representa el renglon de una factura.
    /// Puede que sea necesario modificarla para hacer las implementaciones requeridas.
    /// </summary>
    public class RenglonFactura
    {
        public Articulo Articulo;
        public int cantidad;
    }
}
