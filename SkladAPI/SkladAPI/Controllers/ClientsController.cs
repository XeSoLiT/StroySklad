using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SkladAPI.Models;
namespace SkladAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly string _connectionString;
        public ClientsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        [HttpGet]
        public IActionResult GetClients()
        {
            try
            {
                var list = new List<ClientModel>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID, Nazvanie, Gorod, Ulica, Dom, Telefon, Familiya, Imya, Otchestvo, Skidka FROM [26.10_Klient]";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ClientModel
                            {
                                ID = reader.GetInt32(0),
                                Nazvanie = reader.GetString(1),
                                Gorod = reader.GetString(2),
                                Ulica = reader.GetString(3),
                                Dom = reader.GetString(4),
                                Telefon = reader.GetString(5),
                                Familiya = reader.GetString(6),
                                Imya = reader.GetString(7),
                                Otchestvo = reader.IsDBNull(8) ? null : reader.GetString(8),
                                Skidka = reader.GetDecimal(9)
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
        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public IActionResult AddClient([FromBody] ClientModel model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO [26.10_Klient] (Nazvanie, Gorod, Ulica, Dom, Telefon, Familiya, Imya, Otchestvo, Skidka) VALUES (@Nazvanie, @Gorod, @Ulica, @Dom, @Telefon, @Familiya, @Imya, @Otchestvo, @Skidka)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nazvanie", model.Nazvanie);
                        command.Parameters.AddWithValue("@Gorod", model.Gorod);
                        command.Parameters.AddWithValue("@Ulica", model.Ulica);
                        command.Parameters.AddWithValue("@Dom", model.Dom);
                        command.Parameters.AddWithValue("@Telefon", model.Telefon);
                        command.Parameters.AddWithValue("@Familiya", model.Familiya);
                        command.Parameters.AddWithValue("@Imya", model.Imya);
                        command.Parameters.AddWithValue("@Otchestvo", string.IsNullOrWhiteSpace(model.Otchestvo) ? DBNull.Value : model.Otchestvo);
                        command.Parameters.AddWithValue("@Skidka", model.Skidka);
                        command.ExecuteNonQuery();
                    }
                }
                return StatusCode(201, new { message = "Клиент добавлен." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Ошибка при добавлении клиента." });
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Администратор")]
        public IActionResult DeleteClient(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM [26.10_Klient] WHERE ID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        command.ExecuteNonQuery();
                    }
                }
                return NoContent();
            }
            catch (SqlException)
            {
                return BadRequest(new { message = "Невозможно удалить клиента, так как он используется в накладных." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Ошибка сервера." });
            }
        }
    }
}