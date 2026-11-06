namespace SkladAPI.Models
{
    public class MaterialModel
    {
        public int ID { get; set; }
        public int ID_ed_izm { get; set; }
        public int ID_proizvoditelya { get; set; }
        public string Naimenovanie { get; set; } = string.Empty;
        public decimal Cena { get; set; }
        public decimal Kolichestvo { get; set; }
        public string? PhotoPath { get; set; }
    }
}