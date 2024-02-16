namespace FacturasAxoft.Excepciones
{
    public class FacturaAxoftException : Exception
    {
        public FacturaAxoftException(string message) : base(message)
        {
        }
    }

    public class FacturaConFechaInvalidaException : FacturaAxoftException
    {
        public FacturaConFechaInvalidaException() :
            base("La fecha de la factura es inválida. Existen facturas grabadas con fecha posterior a la ingresada.")
        {
        }
    }


    public class FacturaNumeracionInvalidaException : FacturaAxoftException
    {
        public FacturaNumeracionInvalidaException() :
            base("La numeración de la factura es inválida.")
        {
        }
    }

    public class FacturaNumeracionInvalidaConsecutivaException : FacturaAxoftException
    {
        public FacturaNumeracionInvalidaConsecutivaException() :
            base("El número de factura:no es consecutivo/no comienza en 1.")
        {
        }
    }

    public class FacturaFechaInvalidaException : FacturaAxoftException
    {
        public FacturaFechaInvalidaException() :
            base("La fecha de la factura es inválida. Existen facturas grabadas con fecha posterior a la ingresada.")
        {
        }
    }

    public class FacturaCUILInvalidoException : FacturaAxoftException
    {
        public FacturaCUILInvalidoException() :
            base("El CUIL del cliente es inválido.")
        {
        }
    }

    public class FacturaClienteInexistenteException : FacturaAxoftException
    {
        public FacturaClienteInexistenteException() :
            base("El cliente de la factura no existe.")
        {
        }
    }

    public class FacturaArticuloInexistenteException : FacturaAxoftException
    {
        public FacturaArticuloInexistenteException() :
            base("Uno o más artículos de la factura no existen.")
        {
        }
    }

    public class FacturaTotalRenglonInvalidoException : FacturaAxoftException
    {
        public FacturaTotalRenglonInvalidoException() :
            base("El total del renglón de la factura es inválido.")
        {
        }
    }

    public class FacturaPorcentajeIVAInvalidoException : FacturaAxoftException
    {
        public FacturaPorcentajeIVAInvalidoException() :
            base("El porcentaje de IVA en uno o más renglones de la factura es inválido.")
        {
        }
    }

    public class FacturaImporteIVAInvalidoException : FacturaAxoftException
    {
        public FacturaImporteIVAInvalidoException() :
            base("El importe de IVA en uno o más renglones de la factura es inválido.")
        {
        }
    }

    public class FacturaTotalConImpuestosInvalidoException : FacturaAxoftException
    {
        public FacturaTotalConImpuestosInvalidoException() :
            base("El total con impuestos de la factura es inválido.")
        {
        }
    }
    public class FacturaTotalSinImpuestosInvalidoException : FacturaAxoftException
    {
        public FacturaTotalSinImpuestosInvalidoException() :
            base("El total sin impuestos de la factura es inválido.")
        {
        }
    }

}
