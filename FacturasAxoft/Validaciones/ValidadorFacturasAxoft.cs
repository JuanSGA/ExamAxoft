using FacturasAxoft.Clases;
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
           // ValidarArticulosRenglones(factura);
           // ValidarTotalesRenglones(factura);
            ValidarPorcentajeIVA(factura);
            ValidarImporteIVA(factura);
            ValidarTotalConImpuestos(factura);
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


        //private void ValidarArticulosRenglones(Factura factura)
        //{
        //    foreach (var renglon in factura.Renglones)
        //    {
        //        if (!articulos.Any(a => a.Id == renglon.Articulo.Id))
        //        {
        //            throw new FacturaArticuloInexistenteException();
        //        }
        //    }
        //}

        //private void ValidarTotalesRenglones(Factura factura)
        //{
        //    foreach (var renglon in factura.Renglones)
        //    {
        //        if (renglon.Total != renglon.Cantidad * renglon.Articulo.Precio)
        //        {
        //            throw new FacturaTotalRenglonInvalidoException();
        //        }
        //    }
        //}

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
            return porcentajeIVA == 0 || porcentajeIVA == 10.5m || porcentajeIVA == 21m || porcentajeIVA == 27m;
        }

        private void ValidarImporteIVA(Factura factura)
        {
            foreach (var renglon in factura.Renglones)
            {
                decimal ivaCalculado = renglon.Total * (factura.Iva / 100);
                if (factura.ImporteIva != ivaCalculado)
                {
                    throw new FacturaImporteIVAInvalidoException();
                }
            }
        }

        private void ValidarTotalConImpuestos(Factura factura)
        {
            decimal totalSinImpuestos = factura.Renglones.Sum(r => r.Total);
            decimal totalConImpuestos = totalSinImpuestos + factura.Renglones.Sum(r => factura.ImporteIva);
            foreach (var renglon in factura.Renglones)
            {
                if (renglon.Total != totalConImpuestos)
                {
                    throw new FacturaTotalConImpuestosInvalidoException();
                }
            }
        }
    }
}