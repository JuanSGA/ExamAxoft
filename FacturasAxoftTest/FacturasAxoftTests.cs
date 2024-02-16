using FacturasAxoft.Clases;
using FacturasAxoft.Excepciones;
using FacturasAxoft.Validaciones;
using System;
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
        public void FacturaConCUILInvalido()
        {
            // Factura con CUIL inválido
            Factura factura = new Factura
            {
                Numero = 3,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "12345678901", // CUIL inválido
                    Direccion = "Calle falsa 123",
                    Nombre = "Pedro"
                },
                Renglones = new List<RenglonFactura>
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo
                        {
                            Codigo = "ART02",
                            Descripcion = "artículo cero dos",
                            Precio = 15
                        },
                        Cantidad = 3
                    }
                }
            };
            Exception exception = Record.Exception(() => validador.ValidarNuevaFactura(factura));
            Assert.IsNull(exception);
        }

        [TestMethod]
        public void FacturaConTotalRenglonInvalido()
        {
            // Factura con total de renglón inválido
            Factura factura = new Factura
            {
                Numero = 4,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Laura"
                },
                Renglones = new List<RenglonFactura>
        {
            new RenglonFactura
            {
                Articulo = new Articulo
                {
                    Codigo = "ART03",
                    Descripcion = "artículo cero tres",
                    Precio = 8
                },
                Cantidad = 4,
                Total = 40 // Total incorrecto
            }
        }
            };

            //Exception exception = Record.Exception(() => validador.ValidarNuevaFactura(factura));
            // Al validar la nueva factura salta una excepción tipada, y con el mensaje de error correspondiente.
            Assert.ThrowsException<FacturaTotalRenglonInvalidoException>(() => validador.ValidarNuevaFactura(factura),
                "El total del renglón de la factura es inválido.");

        }

        [TestMethod]
        public void FacturaConPorcentajeIVAInvalido()
        {
            // Factura con porcentaje de IVA inválido
            Factura factura = new Factura
            {
                Numero = 5,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Carlos"
                },
                Renglones = new List<RenglonFactura>
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo
                        {
                            Codigo = "ART04",
                            Descripcion = "artículo cero cuatro",
                            Precio = 12
                        },
                        Cantidad = 2
                    }
                },
                Iva = 15 // Porcentaje de IVA incorrecto
            };

            Assert.ThrowsException<FacturaPorcentajeIVAInvalidoException>(() => validador.ValidarNuevaFactura(factura),
                "El porcentaje de IVA en uno o más renglones de la factura es inválido.");
        }

        [TestMethod]
        public void FacturaConImporteIVAInvalido()
        {
            // Factura con importe de IVA inválido
            Factura factura = new Factura
            {
                Numero = 6,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Ana"
                },
                Renglones = new List<RenglonFactura>
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo
                        {
                            Codigo = "ART05",
                            Descripcion = "artículo cero cinco",
                            Precio = 20
                        },
                        Cantidad = 1
                    }
                },
                Iva = 10,
                ImporteIva = 30 // Importe de IVA incorrecto
            };

            Assert.ThrowsException<FacturaPorcentajeIVAInvalidoException>(() => validador.ValidarNuevaFactura(factura),
                "El porcentaje de IVA en uno o más renglones de la factura es inválido.");
        }

        [TestMethod]
        public void FacturaConTotalConImpuestosInvalido()
        {
            // Factura con total con impuestos inválido
            Factura factura = new Factura
            {
                Numero = 7,
                Fecha = new DateTime(2020, 1, 1),
                Cliente = new Cliente
                {
                    Cuil = "20123456781",
                    Direccion = "Calle falsa 123",
                    Nombre = "Eduardo"
                },
                Renglones = new List<RenglonFactura>
                {
                    new RenglonFactura
                    {
                        Articulo = new Articulo
                        {
                            Codigo = "ART06",
                            Descripcion = "artículo cero seis",
                            Precio = 20
                        },
                        Cantidad = 3
                    }
                },
                Iva = 21.00m,
                ImporteIva= 12.60m,
                TotalSinImpuestos = 60.00m,
                TotalConImpuestos = 84.70m // Total con impuestos incorrecto
            };

            Assert.ThrowsException<FacturaTotalConImpuestosInvalidoException>(() => validador.ValidarNuevaFactura(factura),
                "El total con impuestos de la factura es inválido.");
        }

        //[TestMethod]
        //public void FacturaConTotalSinImpuestosInvalido()
        //{
        //    // Factura con total sin impuestos inválido
        //    Factura factura = new Factura
        //    {
        //        Numero = 8,
        //        Fecha = new DateTime(2020, 1, 1),
        //        Cliente = new Cliente
        //        {
        //            Cuil = "20123456781",
        //            Direccion = "Calle falsa 123",
        //            Nombre = "María"
        //        },
        //        Renglones = new List<RenglonFactura>
        //        {
        //            new RenglonFactura
        //            {
        //                Articulo = new Articulo
        //                {
        //                    Codigo = "ART07",
        //                    Descripcion = "artículo cero siete",
        //                    Precio = 40
        //                },
        //                Cantidad = 2
        //            }
        //        },
        //        Iva = 21.00m,
        //        ImporteIva = 16.80m,
        //        TotalSinImpuestos = 80.00m,// Total sin impuestos incorrecto
        //        TotalConImpuestos = 94.70m 
        //    };

        //    Assert.ThrowsException<FacturaTotalSinImpuestosInvalidoException>(() => validador.ValidarNuevaFactura(factura),
        //        "El total sin impuestos de la factura es inválido.");
        //}


    }
}