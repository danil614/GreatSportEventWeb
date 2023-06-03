using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Athletes")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Athlete : Person
{
    [Key]
    [Column("athlete_id")]
    public int Id { get; set; }

    [Display(Name = "Команда")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("team_id")]
    public int TeamId { get; set; }
    
    [Display(Name = "Команда")] public virtual Team? Team { get; set; }

    [Display(Name = "Должность")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("position_id")]
    public int PositionId { get; set; }
    
    [Display(Name = "Должность")] public virtual Position? Position { get; set; }

    [Display(Name = "Рейтинг")]
    [Range(0, int.MaxValue, ErrorMessage = "Число должно быть больше 0 и меньше 2147483647.")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "Число должно быть целым и положительным.")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    public int? Rating { get; set; }

    [Display(Name = "Описание")]
    [Column("description")]
    public string? Description { get; set; }
}