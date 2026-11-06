namespace SkladAPI.Models
{
    public class RegisterModel
    {
        public string Familiya { get; set; } = string.Empty;
        public string Imya { get; set; } = string.Empty;
        public string? Otchestvo { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Parol { get; set; } = string.Empty;
    }
}