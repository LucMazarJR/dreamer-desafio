using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dedg_back.Models.Enums;

namespace dedg_back.Models.Entities;

[Table("tb_monthly_period")]
public class MonthlyPeriod
{
    [Key]
    [Column("id_monthly_period")]
    public int Id { get; set; }

    [Required]
    [Column("year")]
    public int Year { get; set; }

    [Required]
    [Column("month")]
    public int Month { get; set; }

    [Required]
    [Column("status")]
    public PeriodStatus Status { get; set; } = PeriodStatus.Open;

    [Column("closed_by_id")]
    public int? ClosedById { get; set; }

    [Column("closed_at")]
    public DateTime? ClosedAt { get; set; }

    // Navigation properties
    public User? ClosedBy { get; set; }
}
