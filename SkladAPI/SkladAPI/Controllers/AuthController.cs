using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using SkladAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace SkladAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Login) || string.IsNullOrWhiteSpace(model.Parol))
            {
                return BadRequest(new { message = "Пожалуйста, заполните все поля." });
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"SELECT s.Familiya, s.Imya, r.Nazvanie 
                                     FROM [26.10_Polzovatel] p
                                     JOIN [26.10_Sotrudnik] s ON p.ID_sotrudnika = s.ID
                                     JOIN [26.10_Rol] r ON s.ID_roli = r.ID
                                     WHERE p.Login = @Login AND p.Parol = @Parol";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Login", model.Login);
                        command.Parameters.AddWithValue("@Parol", model.Parol);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string familiya = reader.GetString(0);
                                string imya = reader.GetString(1);
                                string rol = reader.GetString(2);
                                string token = GenerateJwtToken(model.Login, rol);
                                return Ok(new { token = token, role = rol, fio = $"{familiya} {imya}" });
                            }
                        }
                    }
                }
                return Unauthorized(new { message = "Неверный логин или пароль." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Ошибка сервера при авторизации." });
            }
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Login) || string.IsNullOrWhiteSpace(model.Parol) || string.IsNullOrWhiteSpace(model.Familiya))
            {
                return BadRequest(new { message = "Пожалуйста, заполните обязательные поля." });
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string checkQuery = "SELECT COUNT(1) FROM [26.10_Polzovatel] WHERE Login = @Login";
                            using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection, transaction))
                            {
                                checkCmd.Parameters.AddWithValue("@Login", model.Login);
                                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                                if (count > 0)
                                {
                                    return BadRequest(new { message = "Пользователь с таким логином уже существует." });
                                }
                            }
                            string insertSotrudnik = @"INSERT INTO [26.10_Sotrudnik] (Familiya, Imya, Otchestvo, ID_roli) 
                                                       VALUES (@Familiya, @Imya, @Otchestvo, 2);
                                                       SELECT SCOPE_IDENTITY();";
                            int newSotrudnikId = 0;
                            using (SqlCommand cmdSotrudnik = new SqlCommand(insertSotrudnik, connection, transaction))
                            {
                                cmdSotrudnik.Parameters.AddWithValue("@Familiya", model.Familiya);
                                cmdSotrudnik.Parameters.AddWithValue("@Imya", model.Imya);
                                cmdSotrudnik.Parameters.AddWithValue("@Otchestvo", string.IsNullOrWhiteSpace(model.Otchestvo) ? DBNull.Value : model.Otchestvo);
                                newSotrudnikId = Convert.ToInt32(cmdSotrudnik.ExecuteScalar());
                            }
                            string insertPolzovatel = "INSERT INTO [26.10_Polzovatel] (ID_sotrudnika, Login, Parol) VALUES (@ID_sotrudnika, @Login, @Parol)";
                            using (SqlCommand cmdPolzovatel = new SqlCommand(insertPolzovatel, connection, transaction))
                            {
                                cmdPolzovatel.Parameters.AddWithValue("@ID_sotrudnika", newSotrudnikId);
                                cmdPolzovatel.Parameters.AddWithValue("@Login", model.Login);
                                cmdPolzovatel.Parameters.AddWithValue("@Parol", model.Parol);
                                cmdPolzovatel.ExecuteNonQuery();
                            }
                            transaction.Commit();
                            return Ok(new { message = "Регистрация успешна." });
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            return BadRequest(new { message = "Ошибка базы данных при регистрации." });
                        }
                    }
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Ошибка подключения к серверу БД." });
            }
        }
        private string GenerateJwtToken(string login, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, login),
                new Claim(ClaimTypes.Role, role)
            };
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}