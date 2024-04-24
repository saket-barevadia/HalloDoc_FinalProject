using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.DataModels;

[Table("books")]
public partial class Book
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("book_name")]
    [StringLength(100)]
    public string BookName { get; set; } = null!;

    [Column("author")]
    [StringLength(100)]
    public string Author { get; set; } = null!;

    [Column("borrower_id")]
    public int? BorrowerId { get; set; }

    [Column("borrower_name")]
    [StringLength(100)]
    public string BorrowerName { get; set; } = null!;

    [Column("date_of_issue", TypeName = "timestamp without time zone")]
    public DateTime DateOfIssue { get; set; }

    [Column("city")]
    [StringLength(100)]
    public string City { get; set; } = null!;

    [Column("genre")]
    [StringLength(50)]
    public string Genre { get; set; } = null!;

    [ForeignKey("BorrowerId")]
    [InverseProperty("Books")]
    public virtual Borrower? Borrower { get; set; }
}
