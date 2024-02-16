using FacturasAxoft.Clases;
using FacturasAxoft.Excepciones;
using FacturasAxoft.Validaciones;
using System.Data.SqlClient;
using System.Globalization;
using System.Xml.Linq;

namespace FacturasAxoft
{
    public class FacturasAxoft
    {

        private readonly string connectionString;
        private readonly ValidadorFacturasAxoft validador;

        public FacturasAxoft(string connectionString, List<Cliente> clientes, List<Articulo> articulos, List<Factura> facturas)
        {
            this.connectionString = connectionString;
            this.validador = new ValidadorFacturasAxoft(clientes, articulos, facturas);
        }

        public void CargarFacturas(string path)
        {
            try
            {
                XDocument xmlDoc = XDocument.Load(path);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();



                    /*****************************VALIDACIONES***********************/

                    // 1) La numeración de facturas comienza en 1.
                    // 2) Al grabar una nueva factura debe ya estar grabada la anterior.Por ejemplo, para grabar la factura 3 es requisito que ya esté grabada la 2.
                    // 3) Las facturas están emitidas en órden cronológico, por lo que si la factura 1 tiene fecha 5 de enero, la factura 2 no puede tener fecha 4 de enero.

                    //creo primer foreach que recorra para validar las facturas secuenciales
                    //Luego pasarlo al modulo de validaciones correspondientes.
                    int numeroEsperado = 1; // Inicializar el primer número esperado
                    DateTime fechaAnterior = DateTime.MinValue; // Inicializar la fecha anterior con un valor mínimo

                    foreach (XElement facturaElementSecuencial in xmlDoc.Descendants("factura"))
                    {
                        string numeroFactura = facturaElementSecuencial.Element("numero").Value;
                        string fechaFactura = facturaElementSecuencial.Element("fecha").Value;

                        // Convertir el número de factura a entero para la comparación
                        if (int.TryParse(numeroFactura, out int numeroActual))
                        {
                            // Validar si el número actual es igual al número esperado
                            if (numeroActual != numeroEsperado)
                            {
                                throw new FacturaNumeracionInvalidaConsecutivaException();
                            }
                            // Incrementar el número esperado para la próxima iteración
                            numeroEsperado++;
                        }
                        else
                        {
                            // Lanzar error si el número de factura no es un número válido secuencial
                            Console.WriteLine("Error: El número de factura no es un número válido.");
                            break;
                        }

                        // Convertir la fecha de factura a formato DateTime
                        var fechaActual = DateTime.ParseExact(fechaFactura, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        // Validar que la fecha actual no sea menor que la fecha anterior
                        if (fechaActual < fechaAnterior)
                        {
                            throw new FacturaConFechaInvalidaException();
                        }

                        // Actualizar la fecha anterior para la próxima iteración
                        fechaAnterior = fechaActual;
                    }

                    /*****************************VALIDACIONES***********************/




                    foreach (XElement facturaElement in xmlDoc.Descendants("factura"))
                    {
                        // Obtener datos de la factura del XML
                        string numeroFactura = facturaElement.Element("numero").Value;
                        string fechaFactura = facturaElement.Element("fecha").Value;

                        // Obtener datos del cliente del XML
                        XElement clienteElement = facturaElement.Element("cliente");
                        string cuilCliente = clienteElement.Element("CUIL").Value;
                        string nombreCliente = clienteElement.Element("nombre").Value;
                        string direccionCliente = clienteElement.Element("direccion").Value;

                        // Obtener y procesar los renglones de la factura
                        XElement renglonesElement = facturaElement.Element("renglones");
                        decimal totalSinImpuestos = decimal.Parse(facturaElement.Element("totalSinImpuestos").Value, CultureInfo.InvariantCulture);
                        decimal iva = decimal.Parse(facturaElement.Element("iva").Value, CultureInfo.InvariantCulture);
                        decimal importeIva = decimal.Parse(facturaElement.Element("importeIva").Value, CultureInfo.InvariantCulture);
                        decimal totalConImpuestos = decimal.Parse(facturaElement.Element("totalConImpuestos").Value, CultureInfo.InvariantCulture);

                        int numeroFacturaInt = int.Parse(numeroFactura);
                        // Crear instancia de Factura con los datos obtenidos del XML
                        Factura factura = new Factura
                        {
                            Numero = numeroFacturaInt,
                            Fecha = DateTime.ParseExact(fechaFactura, "dd/MM/yyyy", CultureInfo.InvariantCulture),

                            Cliente = new Cliente
                            {
                                Cuil = cuilCliente,
                                Nombre = nombreCliente,
                                Direccion = direccionCliente
                            },

                            Renglones = new List<RenglonFactura>(),

                            TotalSinImpuestos = totalSinImpuestos,
                            Iva = iva,
                            ImporteIva = importeIva,
                            TotalConImpuestos = totalConImpuestos

                        };

                        // Llamar al validador antes de realizar la inserción en la base de datos
                        validador.ValidarNuevaFactura(factura);

                        // Insertar datos del cliente en la base de datos (si no existe)
                        int clienteId = InsertarClienteEnBaseDeDatos(connection, cuilCliente, nombreCliente, direccionCliente);

                        // Obtener y procesar los renglones de la factura
                        ProcesarRenglones(connection, numeroFactura, fechaFactura, clienteId, renglonesElement, totalSinImpuestos, iva, importeIva, totalConImpuestos);

                    }
                }

                Console.WriteLine("Facturas cargadas correctamente en la base de datos.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar las facturas: {ex.Message}");

            }
        }

        private int InsertarClienteEnBaseDeDatos(SqlConnection connection, string cuil, string nombre, string direccion)
        {


            /****************** VALIDACION 5)Un mismo cliente siempre tiene el mismo CUIL, nombre, dirección, y porcentaje de IVA.***********************/


            // Verificar si el cliente ya existe en la base de datos
            string query = "SELECT Id FROM Cliente WHERE Cuil = @Cuil";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Cuil", cuil);
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    // El cliente ya existe, devolver el Id existente
                    return (int)result;
                }
            }

            // Si no existe, insertar el nuevo cliente y devolver su Id
            query = "INSERT INTO Cliente (Cuil, Nombre, Direccion) VALUES (@Cuil, @Nombre, @Direccion); SELECT SCOPE_IDENTITY();";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Cuil", cuil);
                command.Parameters.AddWithValue("@Nombre", nombre);
                command.Parameters.AddWithValue("@Direccion", direccion);

                return Convert.ToInt32(command.ExecuteScalar());
            }



            /***********************FIN VALIDACION 5*******************************/
        }

        private void ProcesarRenglones(SqlConnection connection, string numeroFactura, string fechaFactura, int clienteId, XElement renglonesElement, decimal totalSinImpuestos, decimal iva, decimal importeIva, decimal totalConImpuestos)
        {
            foreach (XElement renglonElement in renglonesElement.Elements("renglon"))
            {

                /**********VALIDACION 6)Un mismo artículo siempre tiene el mismo código, descripción, y precio unitario.*******************/


                // Obtener datos del renglón del XML
                string codigoArticulo = renglonElement.Element("codigoArticulo").Value;
                string descripcion = renglonElement.Element("descripcion").Value;
                decimal precioUnitario = decimal.Parse(renglonElement.Element("precioUnitario").Value, CultureInfo.InvariantCulture);
                int cantidad = int.Parse(renglonElement.Element("cantidad").Value);
                decimal total = decimal.Parse(renglonElement.Element("total").Value, CultureInfo.InvariantCulture);

                // Obtener el Id del artículo (o insertarlo si no existe)
                int articuloId = ObtenerOInsertarArticuloId(connection, codigoArticulo, descripcion, precioUnitario);


                /**********************FIN VALIDACION 6************/


                // Insertar datos del renglón en la base de datos
                InsertarRenglonEnBaseDeDatos(connection, numeroFactura, fechaFactura, clienteId, articuloId, cantidad, total, totalSinImpuestos, iva, importeIva, totalConImpuestos);

                // Obtener otros datos necesarios y realizar las inserciones correspondientes...
            }
        }

        private int ObtenerOInsertarArticuloId(SqlConnection connection, string codigoArticulo, string descripcion, decimal precioUnitario)
        {
            // Verificar si el artículo ya existe en la base de datos
            string query = "SELECT Id FROM Articulo WHERE Codigo = @Codigo";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Codigo", codigoArticulo);
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    // El artículo ya existe, devolver el Id existente
                    return (int)result;
                }
            }

            // Si no existe, insertar el nuevo artículo y devolver su Id
            query = "INSERT INTO Articulo (Codigo, Descripcion, Precio) VALUES (@Codigo, @Descripcion, @Precio); SELECT SCOPE_IDENTITY();";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Codigo", codigoArticulo);
                command.Parameters.AddWithValue("@Descripcion", descripcion);
                command.Parameters.AddWithValue("@Precio", precioUnitario);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private void InsertarRenglonEnBaseDeDatos(SqlConnection connection, string numeroFactura, string fechaFactura, int clienteId, int articuloId, int cantidad, decimal total, decimal totalSinImpuestos, decimal iva, decimal importeIva, decimal totalConImpuestos)
        {
            // Insertar datos del renglón en la base de datos
            string query = "INSERT INTO Factura (Numero, Fecha, ClienteId, TotalSinImpuestos, Iva, ImporteIva, TotalConImpuestos) VALUES (@Numero, @Fecha, @ClienteId, @TotalSinImpuestos, @Iva, @ImporteIva, @TotalConImpuestos); SELECT SCOPE_IDENTITY();";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Numero", numeroFactura);
                command.Parameters.AddWithValue("@Fecha", DateTime.ParseExact(fechaFactura, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                command.Parameters.AddWithValue("@ClienteId", clienteId);
                command.Parameters.AddWithValue("@TotalSinImpuestos", totalSinImpuestos);
                command.Parameters.AddWithValue("@Iva", iva);
                command.Parameters.AddWithValue("@ImporteIva", importeIva);
                command.Parameters.AddWithValue("@TotalConImpuestos", totalConImpuestos);

                int facturaId = Convert.ToInt32(command.ExecuteScalar());

                // Insertar el renglón asociado a la factura
                query = "INSERT INTO RenglonFactura (FacturaId, ArticuloId, Cantidad, PrecioUnitario, Total) VALUES (@FacturaId, @ArticuloId, @Cantidad, @PrecioUnitario, @Total)";
                using (SqlCommand renglonCommand = new SqlCommand(query, connection))
                {
                    renglonCommand.Parameters.AddWithValue("@FacturaId", facturaId);
                    renglonCommand.Parameters.AddWithValue("@ArticuloId", articuloId);
                    renglonCommand.Parameters.AddWithValue("@Cantidad", cantidad);
                    renglonCommand.Parameters.AddWithValue("@PrecioUnitario", ObtenerPrecioUnitarioArticulo(connection, articuloId));
                    renglonCommand.Parameters.AddWithValue("@Total", total);

                    renglonCommand.ExecuteNonQuery();
                }
            }
        }

        private decimal ObtenerPrecioUnitarioArticulo(SqlConnection connection, int articuloId)
        {
            string query = "SELECT Precio FROM Articulo WHERE Id = @ArticuloId";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ArticuloId", articuloId);
                return Convert.ToDecimal(command.ExecuteScalar());
            }
        }


        public string Get3ArticulosMasVendidos()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 3 A.Descripcion, SUM(R.Cantidad) AS TotalVendido " +
                               "FROM RenglonFactura R " +
                               "JOIN Articulo A ON R.ArticuloId = A.Id " +
                               "GROUP BY A.Descripcion " +
                               "ORDER BY TotalVendido DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    string result = "Top 3 de artículos más vendidos:\n";

                    while (reader.Read())
                    {
                        result += $"{reader["Descripcion"]} - Total Vendido: {reader["TotalVendido"]}\n";
                    }

                    Console.WriteLine(result);


                    return result;
                }
            }
        }

        public string Get3Compradores()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 3 C.Nombre, COUNT(F.Id) AS TotalCompras " +
                               "FROM Cliente C " +
                               "JOIN Factura F ON C.Id = F.ClienteId " +
                               "GROUP BY C.Nombre " +
                               "ORDER BY TotalCompras DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    string result = "Top 3 de clientes que más compraron:\n";

                    while (reader.Read())
                    {
                        result += $"{reader["Nombre"]} - Total Compras: {reader["TotalCompras"]}\n";
                    }
                    Console.WriteLine(result);
                    return result;
                }
            }
        }

        public string GetPromedioYArticuloMasCompradoDeCliente(string cuil)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT AVG(F.TotalConImpuestos) AS PromedioCompras, A.Descripcion AS ArticuloMasComprado " +
                               "FROM Factura F " +
                               "JOIN Cliente C ON F.ClienteId = C.Id " +
                               "JOIN RenglonFactura RF ON F.Id = RF.FacturaId " +
                               "JOIN Articulo A ON RF.ArticuloId = A.Id " +
                               "WHERE C.Cuil = @Cuil " +
                               "GROUP BY A.Descripcion " +
                               "ORDER BY PromedioCompras DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Cuil", cuil);
                    SqlDataReader reader = command.ExecuteReader();
                    string result = "Promedio de compras y artículo más comprado del cliente:\n";

                    if (reader.Read())
                    {
                        result += $"Promedio de Compras: {reader["PromedioCompras"]}\n";
                        result += $"Artículo más comprado: {reader["ArticuloMasComprado"]}\n";
                    }
                    Console.WriteLine(result);
                    return result;
                }
            }
        }


        public string GetTotalYPromedioFacturadoPorFecha(string fecha)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT SUM(F.TotalConImpuestos) AS TotalFacturado, AVG(F.TotalConImpuestos) AS PromedioFactura " +
                               "FROM Factura F " +
                               "WHERE F.Fecha = @Fecha";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Fecha", DateTime.ParseExact(fecha, "dd-MM-yyyy", CultureInfo.InvariantCulture));
                    SqlDataReader reader = command.ExecuteReader();
                    string result = "Total facturado y promedio de importes de factura para una fecha:\n";

                    if (reader.Read())
                    {
                        result += $"Total Facturado: {reader["TotalFacturado"]}\n";
                        result += $"Promedio de Factura: {reader["PromedioFactura"]}\n";
                    }
                    Console.WriteLine(result);
                    return result;
                }
            }
        }

        public string GetTop3ClientesDeArticulo(string codigoArticulo)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 3 C.Nombre, COUNT(F.Id) AS TotalCompras " +
                               "FROM Cliente C " +
                               "JOIN Factura F ON C.Id = F.ClienteId " +
                               "JOIN RenglonFactura RF ON F.Id = RF.FacturaId " +
                               "WHERE RF.ArticuloId = (SELECT Id FROM Articulo WHERE Codigo = @Codigo) " +
                               "GROUP BY C.Nombre " +
                               "ORDER BY TotalCompras DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Codigo", codigoArticulo);
                    SqlDataReader reader = command.ExecuteReader();
                    string result = $"Top 3 de clientes que más compraron el artículo {codigoArticulo}:\n";

                    while (reader.Read())
                    {
                        result += $"{reader["Nombre"]} - Total Compras: {reader["TotalCompras"]}\n";
                    }
                    Console.WriteLine(result);
                    return result;
                }
            }
        }

    }
}
