using Budget_management.Models;

namespace Budget_management.Interfaces
{
    public interface IRepositorioCuenta
    {
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
    }
}
