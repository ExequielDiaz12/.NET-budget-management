using Budget_management.Interfaces;
using Budget_management.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Budget_management.Servicios
{
    public class RepositorioCuenta : IRepositorioCuenta
    {
        private readonly string? connectionString;
        public RepositorioCuenta(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"insert into Cuenta (Nombre, TipoCuentaId, Descripcion, Balance)
                                                        values (@Nombre, @TipoCuentaId, @Descripcion, @Balance);

                                                        select SCOPE_IDENTITY();",cuenta);
            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>>Buscar(int usuarioId)
        {
            using var conn = new SqlConnection(connectionString);
            return await conn.QueryAsync<Cuenta>(@"select Cuenta.Id, Cuenta.Nombre, Balance, tc.Nombre as TipoCuenta 
                                                    from Cuenta
                                                    inner join TipoCuenta tc
                                                    on tc.Id = Cuenta.TipoCuentaId
                                                    where tc.UsuarioId = @UsuarioId
                                                    order by tc.Orden;", new { usuarioId });
        }
    }
}
