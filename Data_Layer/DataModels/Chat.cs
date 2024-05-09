using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.DataModels;

[Table("Chat")]
public partial class Chat
{
    [Key]
    public int ChatId { get; set; }

    [StringLength(500)]
    public string? Message { get; set; }

    public int? AdminId { get; set; }

    public int? PhyscainId { get; set; }

    public int? RequestId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? SentDate { get; set; }

    public int? SentBy { get; set; }
}
