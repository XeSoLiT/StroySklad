using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
namespace SkladAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NakladnyeController : ControllerBase
    {
        private readonly string _connectionString;
        public NakladnyeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        [HttpGet]
        public IActionResult GetNakladnye()
        {
            try
            {
                var list = new List<object>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"SELECT n.ID, n.Nomer, n.Data_nakladnoy, 
                                     COALESCE(p.Nazvanie, k.Nazvanie) AS Kontragent,
                                     CASE WHEN n.ID_postavshika IS NOT NULL THEN 'Приход' ELSE 'Расход' END AS Tip
                                     FROM [26.10_Nakladnaya] n
                                     LEFT JOIN [26.10_Postavshik] p ON n.ID_postavshika = p.ID
                                     LEFT JOIN [26.10_Klient] k ON n.ID_klienta = k.ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new
                            {
                                id = reader.GetInt32(0),
                                nomer = reader.GetString(1),
                                data_nakladnoy = reader.GetDateTime(2).ToString("dd.MM.yyyy"),
                                kontragent = reader.GetString(3),
                                tip = reader.GetString(4)
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