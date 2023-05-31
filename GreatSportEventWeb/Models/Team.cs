using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Teams")]
public class Team
{
    [Key]
    [Column("team_id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Место проживания")]
    [Column("location_id")]
    public int LocationId { get; set; }
    
    [Display(Name = "Место проживания")]
    public virtual Location? Location { get; set; }

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Название команды")]
    [Column("team_name")]
    [StringLength(60, ErrorMessage = "Текст должен быть меньше 60 символов.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Откуда команда")]
    [Column("come_from")]
    [StringLength(60, ErrorMessage = "Текст должен быть меньше 60 символов.")]
    public string ComeFrom { get; set; } = null!;

    [Display(Name = "Рейтинг команды")]
    [Range(1, int.MaxValue, ErrorMessage = "Число должно быть больше 0 и меньше 2147483647.")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "Число должно быть целым и положительным.")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("rating")]
    public int Rating { get; set; }

    [Display(Name = "Описание")]
    [Column("description")]
    public string? Description { get; set; }

    public override string ToString()
    {
        return Name;
    }
}