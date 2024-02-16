using FacturasAxoft.Clases;
using System.Collections.Generic;

Console.WriteLine("Inicio: Facturas Axoft");


// Ruta del archivo XML
string xmlFilePath = "C:\\Users\\JuanSGA\\Desktop\\Axoft\\FacturasAxoft\\FacturasAxoft\\Resources\\Ejemplo.xml";

// Connection String de la base de datos
string connectionString = "Data Source=DESKTOP-FVKVQV3\\JUANSGA;Initial Catalog=AxoftExam;Integrated Security=True;";


// Crear instancias de las listas de clientes, artículos y facturas
List<Cliente> clientes = new List<Cliente>();
List<Articulo> articulos = new List<Articulo>();
List<Factura> facturas = new List<Factura>();

// Crear una instancia de FacturasAxoft con todas las listas
FacturasAxoft.FacturasAxoft facturasAxoft = new FacturasAxoft.FacturasAxoft(connectionString, clientes, articulos, facturas);

// Llamar al método CargarFacturas para grabar los datos en la base de datos
facturasAxoft.CargarFacturas(xmlFilePath);


Console.WriteLine(facturasAxoft.Get3ArticulosMasVendidos());
Console.WriteLine(facturasAxoft.Get3Compradores());
Console.WriteLine(facturasAxoft.GetPromedioYArticuloMasCompradoDeCliente("23333457789"));
Console.WriteLine(facturasAxoft.GetTotalYPromedioFacturadoPorFecha("30-12-2020"));
Console.WriteLine(facturasAxoft.GetTop3ClientesDeArticulo("AR001"));
Console.WriteLine(facturasAxoft.GetTotalIva("30-12-2020", "30-12-2023"));



Console.WriteLine("Proceso completado con éxito.");

Console.WriteLine("Fin: Facturas Axoft");
