namespace _584_server.Data;

public class Comp584Csv
{
    public required int index { get; set; }
    public required string rtype { get; set; }
    public required string sname { get; set; }
    public required string dname { get; set; }
    public required string cname { get; set; }
    public required int enroll12 { get; set; }
    public required int NumTstTakr { get; set; }
    public int? AvgScrRead { get; set; }
    public int? AvgScrMath { get; set; }
    public int? AvgScrWrit { get; set; }
    public int? NumGE1500 { get; set; }
    public double? PctGE1500 { get; set; }
    public required int year { get; set; }

}