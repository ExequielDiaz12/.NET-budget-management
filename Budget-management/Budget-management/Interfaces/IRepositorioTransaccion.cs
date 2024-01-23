using Budget_management.Models;

namespace Budget_management.Interfaces
{
    public interface IRepositorioTransaccion
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
    }
}
