namespace _584_server.DTOs
{
    public class DistrictScore
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? County { get; set; }
        public double MathScore { get; set; }
        public double ReadingScore { get; set; }
        public double WritingScore { get; set; }
    }
    
}

