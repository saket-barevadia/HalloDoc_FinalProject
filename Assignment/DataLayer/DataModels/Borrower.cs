using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.DataModels;

[Table("borrower")]
public partial class Borrower
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("city")]
    [StringLength(100)]
    public string City { get; set; } = null!;

    [InverseProperty("Borrower")]
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
