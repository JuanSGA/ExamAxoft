﻿using FacturasAxoft.Clases;
using FacturasAxoft.Excepciones;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FacturasAxoft.Validaciones
{
    public class ValidadorFacturasAxoft
    {
        private readonly List<Cliente> clientes;
        private readonly List<Articulo> articulos;
        private readonly List<Factura> facturas;

        public ValidadorFacturasAxoft(List<Cliente> clientes, List<Articulo> articulos, List<Factura> facturas)
        {
            this.clientes = clientes;
            this.articulos = articulos;
            this.facturas = facturas;
        }

        public void ValidarNuevaFactura(Factura factura)
        {
            ValidarCUIL(factura);
            ValidarTotalesRenglones(factura);
            ValidarPorcentajeIVA(factura);
            ValidarImporteIVA(factura);
            ValidarTotalConImpuestos(factura);
            ValidarTotalSinImpuestos(factura);
        }


        /************VALIDACION 4) El CUIL debe ser válido. (Validacion basica, por una mas compleja me estaba rompiendo el regex)*******************/

        public void ValidarCUIL(Factura factura)
        {
            if (!EsCUILValido(factura.Cliente.Cuil))
            {
                throw new FacturaCUILInvalidoException();
            }
        }
        public bool EsCUILValido(string cuil)
        {
            // Eliminar espacios en blanco
            cuil = cuil.Replace(" ", "");

            // Verificar la longitud correcta (debería ser 11 caracteres)
            if (cuil.Length != 11)
            {
                return false;
            }

            // Verificar si todos los caracteres son dígitos
            if (!cuil.All(char.IsDigit))
            {
                return false;
            }

            return true;
        }

        /***************************FIN VALIDACION 4**************************************/

        private void ValidarTotalesRenglones(Factura factura)
        {
            foreach (var renglon in factura.Renglones)
            {
                if (renglon.Total != renglon.Cantidad * renglon.PrecioUnitario)
                {
                    throw new FacturaTotalRenglonInvalidoException();
                }
            }
        }

        private void ValidarPorcentajeIVA(Factura factura)
        {
            if (!ValidarPorcentajeIVA(factura.Iva))
            {
                throw new FacturaPorcentajeIVAInvalidoException();
            }
        }

        private bool ValidarPorcentajeIVA(decimal porcentajeIVA)
        {
            // Implementar lógica de validación del porcentaje de IVA según requisitos específicos
            return porcentajeIVA == 0m || porcentajeIVA == 10.5m || porcentajeIVA == 21m || porcentajeIVA == 27m;
        }

        private void ValidarImporteIVA(Factura factura)
        {
            foreach (var renglon in factura.Renglones)
            {
                decimal ivaCalculado = factura.TotalSinImpuestos * (factura.Iva / 100); // Corrección aquí
                if (factura.ImporteIva != ivaCalculado)
                {
                    throw new FacturaImporteIVAInvalidoException();
                }
            }
        }


        private void ValidarTotalConImpuestos(Factura factura)
        {
            foreach (var renglon in factura.Renglones)
            {
                decimal ivaCalculado = factura.TotalSinImpuestos * (factura.Iva / 100);
                decimal totalConImpuestosCalculado = factura.TotalSinImpuestos + ivaCalculado;

                if (factura.TotalConImpuestos != totalConImpuestosCalculado)
                {
                    throw new FacturaTotalConImpuestosInvalidoException();
                }
            }
        }

        private void ValidarTotalSinImpuestos(Factura factura)
        {
            foreach (var renglon in factura.Renglones)
            {
                decimal ivaCalculado = factura.TotalSinImpuestos * (factura.Iva / 100);
                decimal totalSinImpuestosCalculado = factura.TotalConImpuestos - ivaCalculado;

                if (factura.TotalSinImpuestos != totalSinImpuestosCalculado)
                {
                    throw new FacturaTotalSinImpuestosInvalidoException();
                }
            }
        }

    }
}