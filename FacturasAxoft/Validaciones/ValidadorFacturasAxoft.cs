using FacturasAxoft.Clases;
using FacturasAxoft.Excepciones;

namespace FacturasAxoft.Validaciones
{
    /// <summary>
    /// En esta clase implementarán todas las validaciones.
    /// Se da una validación ya implementada a modo de ejemplo.
    /// </summary>
    public class ValidadorFacturasAxoft
    {
        private readonly List<Cliente> clientes;
        private readonly List<Articulo> articulos;
        private readonly List<Factura> facturas;

        /// <summary>
        /// Instancia un Validador facturas
        /// </summary>
        /// <param name="clientes">Clientes preexistentes, ya grabados en la base de datos</param>
        /// <param name="articulos">Artículos preexistentes, ya grabados en la base de datos</param>
        /// <param name="facturas">Facturas preexistentes, ya grabadas en la base de datos</param></param>
        public ValidadorFacturasAxoft(List<Cliente> clientes, List<Articulo> articulos, List<Factura> facturas)
        {
            this.clientes = clientes;
            this.articulos = articulos;
            this.facturas = facturas;
        }

        /// <summary>
        /// Valida la factura pasada por parámetro según lo lógica de negocios requerida.
        /// </summary>
        /// <param name="factura">Factura a validar</param>
        /// <exception>En caso de que la factura no cumpla con las reglas de negocio requeridas
        /// debe lanzar una excepción con el mensaje de error correspondiente</exception>/// 
        public void ValidarNuevaFactura(Factura factura)
        {
            if (facturas.Any(f => f.Fecha > factura.Fecha))
            {
                throw new FacturaConFechaInvalidaException();
            }
        }
    }
}
