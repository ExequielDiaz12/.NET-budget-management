using Budget_management.Interfaces;
using Budget_management.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Budget_management.Servicios
{
    public class RepositorioTipoCuenta:IRepositorioTipoCuenta
    {
        private readonly string? connectionString;
        public RepositorioTipoCuenta(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("TipoCuenta_Insertar",
                                                            new
                                                            {
                                                                usuarioId = tipoCuenta.UsuarioId,
                                                                nombre = tipoCuenta.Nombre
                                                            },
                                                            commandType: System.Data.CommandType.StoredProcedure);
            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string nombre, int UsuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                                            @"select 1
                                            from TipoCuenta
                                            where Nombre = @Nombre and UsuarioId = @UsuarioId;",
                                            new { nombre, UsuarioId });
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"select Id,Nombre,Orden
                                                            from TipoCuenta
                                                            where UsuarioId = @UsuarioId
                                                            order by Orden", new { usuarioId });
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"update TipoCuenta
                                            set Nombre = @Nombre
                                            where Id = @Id", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"select Id,Nombre,Orden
                                                                           from TipoCuenta
                                                                           where Id=@Id and UsuarioId=@UsuarioId", new {id,usuarioId});
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"delete from TipoCuenta where Id=@Id", new {id});
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipos)
        {
            var query = "update TipoCuenta set Orden = @Orden where Id = @Id;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query,tipos);
        }
    }
}
