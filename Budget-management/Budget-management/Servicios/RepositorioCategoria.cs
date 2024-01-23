using Budget_management.Interfaces;
using Budget_management.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Budget_management.Servicios
{
    public class RepositorioCategoria : IRepositorioCategoria
    {
        private readonly string connectionString;
        public RepositorioCategoria(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"insert into Categoria (Nombre, TipoOperacionId, UsuarioId)
                                                        values (@Nombre, @TipoOperacionId, @UsuarioId);

                                                        select SCOPE_IDENTITY();", categoria);
            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>>Obtener(int usuarioId)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QueryAsync<Categoria>("select * from Categoria where UsuarioId = @UsuarioId", new { usuarioId });
        }
        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacionId)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QueryAsync<Categoria>(@"select * from Categoria 
                                                    where UsuarioId = @UsuarioId 
                                                    and TipoOperacionId = @TipoOperacionId", 
                                                    new { usuarioId, tipoOperacionId });
        }

        public async Task<Categoria> ObtenerPorId(int id, int usuarioId)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QueryFirstOrDefaultAsync<Categoria>(@"select * from Categoria 
                                                                    where Id = @Id 
                                                                    and UsuarioId = @UsuarioId;",
                                                                    new { id, usuarioId });
        }

        public async Task Actualizar(Categoria categoria)
        {
            using var con = new SqlConnection(connectionString);
            await con.ExecuteAsync(@"update Categoria set Nombre=@Nombre, TipoOperacionId = @TipoOperacionId where Id = @Id", categoria);
        }

        public async Task Borrar(int id)
        {
            using var con = new SqlConnection(connectionString);
            await con.ExecuteAsync("delete Categoria where Id = @Id", new { id });
        }
    }
}
