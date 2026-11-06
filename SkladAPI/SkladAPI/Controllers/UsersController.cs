using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
namespace SkladAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Администратор")]
    public class UsersController : ControllerBase
    {
        private readonly string _connectionString;
        public UsersController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                var list = new List<object>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"SELECT s.ID, s.Familiya, s.Imya, s.Otchestvo, r.Nazvanie AS Rol, p.Login 
                                     FROM [26.10_Sotrudnik] s 
                                     JOIN [26.10_Rol] r ON s.ID_roli = r.ID 
                                     LEFT JOIN [26.10_Polzovatel] p ON s.ID = p.ID_sotrudnika";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new
                            {
                                id = reader.GetInt32(0),
                                familiya = reader.GetString(1),
                                imya = reader.GetString(2),
                                otchestvo = reader.IsDBNull(3) ? null : reader.GetString(3),
                                rol = reader.GetString(4),
                                login = reader.IsDBNull(5) ? "Нет аккаунта" : reader.GetString(5)
                            });
                        }
                    }
                }
                return Ok(list);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Ошибка сервера." });
            }
        }
    }
}