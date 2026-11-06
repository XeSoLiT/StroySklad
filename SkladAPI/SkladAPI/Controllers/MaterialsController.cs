using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SkladAPI.Models;
namespace SkladAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MaterialsController : ControllerBase
    {
        private readonly string _connectionString;
        public MaterialsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        [HttpGet]
        public IActionResult GetMaterials()
        {
            try
            {
                var materials = new List<MaterialModel>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID, ID_ed_izm, ID_proizvoditelya, Naimenovanie, Cena, Kolichestvo, PhotoPath FROM [26.10_Material]";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            materials.Add(new MaterialModel
                            {
                                ID = reader.GetInt32(0),
                                ID_ed_izm = reader.GetInt32(1),
                                ID_proizvoditelya = reader.GetInt32(2),
                                Naimenovanie = reader.GetString(3),
                                Cena = reader.GetDecimal(4),
                                Kolichestvo = reader.GetDecimal(5),
                                PhotoPath = reader.IsDBNull(6) ? null : reader.GetString(6)
                            });
                        }
                    }
                }
                return Ok(materials);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Ошибка при получении списка материалов." });
            }
        }
        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public IActionResult AddMaterial([FromBody] MaterialModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Naimenovanie) || model.Cena < 0 || model.Kolichestvo < 0)
            {
                return BadRequest(new { message = "Некорректные данные материала." });
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO [26.10_Material] (ID_ed_izm, ID_proizvoditelya, Naimenovanie, Cena, Kolichestvo, PhotoPath) VALUES (@ID_ed_izm, @ID_proizvoditelya, @Naimenovanie, @Cena, @Kolichestvo, @PhotoPath)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID_ed_izm", model.ID_ed_izm);
                        command.Parameters.AddWithValue("@ID_proizvoditelya", model.ID_proizvoditelya);
                        command.Parameters.AddWithValue("@Naimenovanie", model.Naimenovanie);
                        command.Parameters.AddWithValue("@Cena", model.Cena);
                        command.Parameters.AddWithValue("@Kolichestvo", model.Kolichestvo);
                        command.Parameters.AddWithValue("@PhotoPath", model.PhotoPath ?? (object)DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                }
                return StatusCode(201, new { message = "Материал добавлен." });
            }
            catch (SqlException)
            {
                return BadRequest(new { message = "Ошибка БД. Проверьте, существуют ли указанные ID единицы измерения и производителя." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера." });
            }
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Администратор")]
        public IActionResult UpdateMaterial(int id, [FromBody] MaterialModel model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE [26.10_Material] SET ID_ed_izm = @ID_ed_izm, ID_proizvoditelya = @ID_proizvoditelya, Naimenovanie = @Naimenovanie, Cena = @Cena, Kolichestvo = @Kolichestvo, PhotoPath = @PhotoPath WHERE ID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        command.Parameters.AddWithValue("@ID_ed_izm", model.ID_ed_izm);
                        command.Parameters.AddWithValue("@ID_proizvoditelya", model.ID_proizvoditelya);
                        command.Parameters.AddWithValue("@Naimenovanie", model.Naimenovanie);
                        command.Parameters.AddWithValue("@Cena", model.Cena);
                        command.Parameters.AddWithValue("@Kolichestvo", model.Kolichestvo);
                        command.Parameters.AddWithValue("@PhotoPath", model.PhotoPath ?? (object)DBNull.Value);
                        int rows = command.ExecuteNonQuery();
                        if (rows == 0) return NotFound(new { message = "Материал не найден." });
                    }
                }
                return Ok(new { message = "Материал обновлен." });
            }
            catch (SqlException)
            {
                return BadRequest(new { message = "Ошибка БД при обновлении. Проверьте связи." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера." });
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Администратор")]
        public IActionResult DeleteMaterial(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM [26.10_Material] WHERE ID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        int rows = command.ExecuteNonQuery();
                        if (rows == 0) return NotFound(new { message = "Материал не найден." });
                    }
                }
                return NoContent();
            }
            catch (SqlException)
            {
                return BadRequest(new { message = "Невозможно удалить материал, так как он используется в накладных." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Внутренняя ошибка сервера." });
            }
        }
    }
}