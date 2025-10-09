using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SchoolModel;

[Table("District")]
public partial class District
{
    [Key]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("county")]
    [StringLength(50)]
    [Unicode(false)]
    public string County { get; set; } = null!;

    [InverseProperty("District")]
    public virtual ICollection<School> Schools { get; set; } = new List<School>();
}
