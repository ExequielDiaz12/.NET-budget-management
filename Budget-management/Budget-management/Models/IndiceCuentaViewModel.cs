﻿namespace Budget_management.Models
{
    public class IndiceCuentaViewModel
    {
        public string TipoCuenta { get; set; }
        public IEnumerable<Cuenta> Cuentas { get; set; }
        public decimal Balance => Cuentas.Sum(x => x.Balance);
    }
}
