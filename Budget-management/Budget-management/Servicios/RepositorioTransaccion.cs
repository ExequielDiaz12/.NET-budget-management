using Budget_management.Interfaces;
using Budget_management.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Budget_management.Servicios
{
    public class RepositorioTransaccion : IRepositorioTransaccion
    {
        private readonly string connectionString;
        public RepositorioTransaccion(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var con = new SqlConnection(connectionString);
            var id = await con.QuerySingleAsync<int>("Transacciones_Insertar",
                new
                {
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota
                },
                commandType: System.Data.CommandType.StoredProcedure);
            transaccion.Id = id;
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionPorCuenta modelo)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QueryAsync<Transaccion>(@"select t.Id, t.Monto, t.FechaTransaccion, c.Nombre as Categoria
                                                        from Transacciones t
                                                        inner join Categoria c
                                                        on c.Id = t.CategoriaId
                                                        inner join Cuenta cu
                                                        on cu.Id = t.CuentaId
                                                        where t.CuentaId = @CuentaId and t.UsuarioId = @UsuarioId
                                                        and FechaTransaccion between @FechaInicio and @FechaFin;", modelo);
        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior,
            int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar",
                new
                {
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota,
                    montoAnterior,
                    cuentaAnteriorId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"select Transacciones.*, cat.TipoOperacionId
                                                                            from Transacciones 
                                                                            inner join Categoria cat
                                                                            on cat.Id = Transacciones.CategoriaId
                                                                            where Transacciones.Id = @Id
                                                                            and Transacciones.UsuarioId = @UsuarioId;", new { id, usuarioId });
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Borrar", new {id }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
