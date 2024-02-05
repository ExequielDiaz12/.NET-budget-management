using Budget_management.Interfaces;
using Budget_management.Models;

namespace Budget_management.Servicios
{
    public class ServicioReporte : IServicioReporte
    {
        private readonly IRepositorioTransaccion repositorioTransaccion;
        private readonly HttpContext httpContext;

        public ServicioReporte(IRepositorioTransaccion repositorioTransaccion, IHttpContextAccessor httpContextAccessor)
        {
            this.repositorioTransaccion = repositorioTransaccion;
            this.httpContext = httpContextAccessor.HttpContext;
        }
        public async Task<ReporteTransaccionDetallada>ObtenerTransaccionesDetalladasPorCuenta(int usuarioId, int cuentaId, int mes, int anio, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechaInicioYFin(mes, anio);

            var obtenerTransaccionesPorCuenta = new ObtenerTransaccionPorCuenta()
            {
                CuentaId = cuentaId,
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var transacciones = await repositorioTransaccion.ObtenerPorCuentaId(obtenerTransaccionesPorCuenta);

            var modelo = new ReporteTransaccionDetallada();


            var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion).GroupBy(x => x.FechaTransaccion)
                .Select(grupo => new ReporteTransaccionDetallada.TransaccionesPorFecha()
                {
                    FechaTransaccion = grupo.Key,
                    Transacciones = grupo.AsEnumerable()
                });

            modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = fechaInicio;
            modelo.FechaFin = fechaFin;

            ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            ViewBag.anioAnterior = fechaInicio.AddMonths(-1).Year;
            ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;
            ViewBag.anioPosterior = fechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = httpContext.Request.Path + httpContext.Request.QueryString;
            return modelo;
        }

        private (DateTime fechaInicio, DateTime fechaFin) GenerarFechaInicioYFin(int mes, int anio)
        {
            DateTime fechaInicio;
            DateTime fechaFin;

            if (mes <= 0 || mes >= 12 || anio <= 1900)
            {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            }
            else
            {
                fechaInicio = new DateTime(anio, mes, 1);
            }

            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            return (fechaInicio, fechaFin);
        }
    }
}
