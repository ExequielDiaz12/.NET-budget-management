using Budget_management.Models;

namespace Budget_management.Interfaces
{
    public interface IServicioReporte
    {
        Task<ReporteTransaccionDetallada> ObtenerTransaccionesDetalladasPorCuenta(int usuarioId, int cuentaId, int mes, int anio, dynamic ViewBag);
    }
}
