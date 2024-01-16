using Microsoft.AspNetCore.Mvc.Rendering;

namespace Budget_management.Models
{
    public class CuentaCreacionViewModel : Cuenta
    {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
    }
}
