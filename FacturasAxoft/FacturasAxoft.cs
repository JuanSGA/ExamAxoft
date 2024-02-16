namespace FacturasAxoft
{
    public class FacturasAxoft
    {
        private readonly string connectionString;

        /// <summary>
        /// Instancia un FacturasAxoft que usaremos como fachada de la aplicación.
        /// </summary>
        /// <param name="conectionString">Datos necesarios para conectarse a la base de datos</param>
        /// <exception>Debe tirar una excepción con mensaje de error correspondiente en caso de no poder conectar a la base de datos</exception>
        public FacturasAxoft(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Lee las facturas desde el archivo XML y las graba en la base de datos.
        /// Da de alta los clientes o los artículos que lleguen en el xml y no estén en la base de datos.
        /// </summary>
        /// <param name="path">Ubicación del archivo xml que contiene las facturas</param>
        /// <exception>Si no se puede acceder al archivo, no es un xml válido, o no cumple con las reglas de negocio, 
        /// devuelve una excepción con el mensaje de error correspondiente</exception>/// 
        public void CargarFacturas(string path)
        {
            // Completar acá con el código necesario para cargar las facturas desde el xml.
            // Al momento de hacer las validaciones, utilizar la clase ValidadorFacturasAxoft.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtiene los 3 artículos mas vendidos
        /// </summary>
        /// <returns>JSON con los 3 artículos mas vendidos</returns>
        /// <exception>Nunca devuelve excepción, en caso de no existir 3 artículos vendidos devolver los que existan, en caso de
        /// tener artículos con la misma cantidad de ventas devolver cualquiera</exception>
        public string Get3ArticulosMasVendidos()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtiene los 3 clientes que mas compraron
        /// </summary>
        /// <returns>JSON con los 3 clientes que mas compraron</returns>
        /// <exception>Mismo criterio que para artículos</exception>
        public string Get3Compradores()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Devuelve el promedio de facturas y el artículo que mas compro.
        /// </summary>
        /// <param name="cuil"></param>
        /// <returns>JSON con los datos requeridos</returns>
        /// <exception>Si no existe el cliente, o si no tiene compras devolver un mensaje de error con el mensaje correspondiente</exception>
        public string GetPromedioYArticuloMasCompradoDeCliente(string cuil)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Devuelve el total y promedio facturado para una fecha.
        /// </summary>
        /// <param name="fecha"></param>
        /// <returns>JSON con los datos requeridos</returns>
        /// <exception>Si el dato de fecha ingresado no es válido, o si no existen facturas para la fecha dada,
        /// mostrar el mensaje de error correspondiente</exception>
        public string GetTotalYPromedioFacturadoPorFecha(string fecha)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Devuelve los 3 clientes que mas compraron el artículo
        /// </summary>
        /// <param name="codigoArticulo"></param>
        /// <returns>JSON con los datos pedidos</returns>
        /// <exception>Si el artículo no existe, o no fue comprado por al menos 3 clientes devolver un mensaje de error correspondiente</exception>
        public string GetTop3ClientesDeArticulo(string codigoArticulo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Devuelve el total de IVA de las facturas generadas desde la fechaDesde hasta la fechaHasta, ambas inclusive.
        /// </summary>
        /// <returns>JSON con el dato requerido</returns>
        /// <exception>Si no existen facturas para las fechas ingresadas mostrar un mensaje de error</exception>
        public string GetTotalIva(string fechaDesde, string fechaHasta)
        {
            throw new NotImplementedException();
        }
    }
}
