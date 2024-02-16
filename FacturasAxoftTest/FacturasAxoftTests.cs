using FacturasAxoft.Clases;
using FacturasAxoft.Excepciones;
using FacturasAxoft.Validaciones;
using Xunit;
using Xunit.Sdk;

namespace FacturasAxoftTest
{
    [TestClass]
    public class FacturasAxoftTests
    {
        private readonly List<Cliente> clientes;
        private readonly List<Articulo> articulos;
        private readonly List<Factura> facturas;
        private readonly ValidadorFacturasAxoft validador;

        public FacturasAxoftTests()
        {
            clientes = new List<Cliente>();
            articulos = new List<Articulo>();
            facturas = new List<Factura>();

            validador = new ValidadorFacturasAxoft(clientes, articulos, facturas);
        }

        /********Basicamente test 1 y 2 se validan en la clase FactuasAxoft.cs ya que se debe contemplar
        Todas las partes del bucle, en el validador estaba ingresando por unica vez, pero cuando entraba 
        una segunda vez, este tiraba la excepcion*********/

        [TestMethod]
        public void PimerFacturaEsValida()
        {
            // No tengo facturas preexistentes

            // La primer factura que voy a agregar tiene el n�mero 1
            Factura factura = new()
            {
                Numero = 1,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "art�culo cero uno",
                            Precio = 10,
                        },
                        Cantidad = 2,
                        
                    }
                }
            };

            // La factura es v�lida, no tiene que tirar la excepci�n.
            Exception exception = Record.Exception(() => validador.ValidarNuevaFactura(factura));
            Assert.IsNull(exception);
        }

        [TestMethod]
        public void SegundaFacturaEsValida()
        {
            // Tengo preexistente una factura n�mero 1 con fecha uno de enero
            facturas.Add(new()
            {
                Numero = 1,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                    {
                        new RenglonFactura
                        {
                            Articulo = new Articulo()
                            {
                                Codigo = "ART01",
                                Descripcion = "art�culo cero uno",
                                Precio = 10
                            },
                           Cantidad = 2,
                        }
                    }
            }
            );

            // Tengo una nueva factura nro dos con fecha 1 de enero
            Factura factura = new()
            {
                Numero = 2,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "art�culo cero uno",
                            Precio = 10
                        },
                        Cantidad = 2,
                    }
                }
            };

            // La factura es v�lida, no tiene que tirar la excepci�n.
            Exception exception = Record.Exception(() => validador.ValidarNuevaFactura(factura));
            Assert.IsNull(exception);
        }


        [TestMethod]
        public void FacturaConFechaInvalida()
        {
            // Tengo una factura n�mero 1 con fecha dos de enero
            facturas.Add(new()
            {
                Numero = 1,
                Fecha = new DateTime(2020, 1, 2),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                    {
                        new RenglonFactura
                        {
                            Articulo = new Articulo()
                            {
                                Codigo = "ART01",
                                Descripcion = "art�culo cero uno",
                                Precio = 10
                            },
                            Cantidad = 2,
                        }
                    }
            }
            );

            // Voy a querer ageegar la factura n�mero 2 con fecha 1 de enero
            Factura factura = new()
            {
                Numero = 2,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Juan"
                },
                Renglones = new List<RenglonFactura>()
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo()
                        {
                            Codigo = "ART01",
                            Descripcion = "art�culo cero uno",
                            Precio = 10
                        },
                        Cantidad = 2,
                    }
                }
            };

            /********FIN VALIDACION 1,2 y 3*********/
            //SE REALIZA VALIDACION EN FACTURAS AXOFT.CS LA EXEPCION YA ESTA CONTROLADA.

            // Al validar la nueva factura salta una excepci�n tipada, y con el mensaje de error correspondiente.
            Assert.ThrowsException<FacturaConFechaInvalidaException>(() => validador.ValidarNuevaFactura(factura),
                "La fecha de la factura es inv�lida. Existen facturas grabadas con n�mero inferior y fecha posterior a la ingresada.");
        }
    }
}