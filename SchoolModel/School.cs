using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SchoolModel;

[Table("School")]
public partial class School
{
    [Key]
    public int Id { get; set; }

    [Column("district_id")]
    public int DistrictId { get; set; }

    [Column("name")]
    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("math_score")]
    public int MathScore { get; set; }

    [Column("writing_score")]
    public int WritingScore { get; set; }

    [Column("reading_score")]
    public int ReadingScore { get; set; }

    [Column("num_test_takers")]
    public int NumTestTakers { get; set; }

    [ForeignKey("DistrictId")]
    [InverseProperty("Schools")]
    public virtual District District { get; set; } = null!;
}
