namespace SkladAPI.Models
{
    public class ClientModel
    {
        public int ID { get; set; }
        public string Nazvanie { get; set; } = string.Empty;
        public string Gorod { get; set; } = string.Empty;
        public string Ulica { get; set; } = string.Empty;
        public string Dom { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string Familiya { get; set; } = string.Empty;
        public string Imya { get; set; } = string.Empty;
        public string? Otchestvo { get; set; }
        public decimal Skidka { get; set; }
    }
}