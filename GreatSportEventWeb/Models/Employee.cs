using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Employees")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Employee : Person
{
    [Key]
    [Column("employee_id")]
    public int Id { get; set; }

    [Display(Name = "Команда")]
    [Column("team_id")]
    public int? TeamId { get; set; }
    
    [Display(Name = "Команда")] public virtual Team? Team { get; set; }

    [Display(Name = "Должность")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("position_id")]
    public int PositionId { get; set; }
    
    [Display(Name = "Должность")] public virtual Position? Position { get; set; }
    
    [NotMapped]
    public List<int>? SelectedEventIds { get; set; }
}