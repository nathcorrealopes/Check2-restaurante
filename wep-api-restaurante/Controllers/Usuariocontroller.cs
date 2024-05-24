using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;
using wep_api_restaurante.Entidades;

namespace wep_api_restaurante.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Usuariocontroller : ControllerBase
    {
        private readonly string? _connectionString;

        public Usuariocontroller(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection OpenConnection()
        {
            IDbConnection dbConnection = new SqliteConnection(_connectionString);
            dbConnection.Open();
            return dbConnection;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using IDbConnection dbConnection = OpenConnection();
            String sql = "select id, nome, senha from Usuario; ";
            var result = await dbConnection.QueryAsync<Usuario>(sql);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using IDbConnection dbConnection = OpenConnection();
            String sql = "select id, nome, senha from Usuario where id = @id; ";
            var usuario = await dbConnection.QueryFirstOrDefaultAsync<Usuario>(sql, new { id });
            dbConnection.Close();

            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            using IDbConnection dbConnection = OpenConnection();
            string query = @"INSERT INTO Usuario(nome, senha)
          VALUES(@Nome, @Senha);";
            await dbConnection.ExecuteAsync(query, usuario);
            dbConnection.Close();
            return Ok();
        }
        [HttpPut]
        public IActionResult Put([FromBody] Usuario usuario)
        {

            using IDbConnection dbConnection = OpenConnection();

            
            var query = @"UPDATE Usuario SET 
                          Nome = @Nome,
                          Senha = @Senha
                          WHERE Id = @Id";

            dbConnection.Execute(query, usuario);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using IDbConnection dbConnection = OpenConnection();

            var usuario = await dbConnection.QueryAsync<Usuario>("delete from Usuario where id = @id;", new { id });
            return Ok();
        }

    }
}

