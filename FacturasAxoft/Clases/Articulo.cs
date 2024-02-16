namespace FacturasAxoft.Clases
{
    /// <summary>
    /// Clase que representa a un artículo.
    /// Puede que sea necesario modificarla para hacer las implementaciones requeridas.
    /// </summary>
    public class Articulo
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
    }
}
