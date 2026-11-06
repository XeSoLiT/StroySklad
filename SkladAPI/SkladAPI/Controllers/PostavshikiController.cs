using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SkladAPI.Models;
namespace SkladAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostavshikiController : ControllerBase
    {
        private readonly string _connectionString;
        public PostavshikiController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        [HttpGet]
        public IActionResult GetPostavshiki()
        {
            try
            {
                var list = new List<PostavshikModel>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID, Nazvanie, Gorod, Ulica, Dom, Telefon, Familiya, Imya, Otchestvo FROM [26.10_Postavshik]";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new PostavshikModel
                            {
                                ID = reader.GetInt32(0),
                                Nazvanie = reader.GetString(1),
                                Gorod = reader.GetString(2),
                                Ulica = reader.GetString(3),
                                Dom = reader.GetString(4),
                                Telefon = reader.GetString(5),
                                Familiya = reader.GetString(6),
                                Imya = reader.GetString(7),
                                Otchestvo = reader.IsDBNull(8) ? null : reader.GetString(8)
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
        public IActionResult AddPostavshik([FromBody] PostavshikModel model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO [26.10_Postavshik] (Nazvanie, Gorod, Ulica, Dom, Telefon, Familiya, Imya, Otchestvo) VALUES (@Nazvanie, @Gorod, @Ulica, @Dom, @Telefon, @Familiya, @Imya, @Otchestvo)";
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
                        command.ExecuteNonQuery();
                    }
                }
                return StatusCode(201, new { message = "Поставщик добавлен." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Ошибка при добавлении поставщика." });
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Администратор")]
        public IActionResult DeletePostavshik(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM [26.10_Postavshik] WHERE ID = @ID";
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
                return BadRequest(new { message = "Невозможно удалить поставщика, так как он используется в накладных." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Ошибка сервера." });
            }
        }
    }
}