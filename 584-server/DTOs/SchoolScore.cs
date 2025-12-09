namespace _584_server.DTOs;

public class SchoolScore
{
    public required int DistrictId { get; set; } 
    public required string Name { get; set; }
    public int MathScore { get; set; }
    public int ReadingScore { get; set; }
    public int WritingScore { get; set; }
    public int NumTestTakers { get; set; }
}