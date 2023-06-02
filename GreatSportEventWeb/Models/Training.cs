using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Trainings")]
public class Training
{
    [Key]
    [Column("training_id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Место проведения")]
    [Column("location_id")]
    public int LocationId { get; set; }
    
    [Display(Name = "Место проведения")] public virtual Location? Location { get; set; }

    [Display(Name = "Команда")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("team_id")]
    public int TeamId { get; set; }
    
    [Display(Name = "Команда")] public virtual Team? Team { get; set; }

    [Display(Name = "Дата и время")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("training_date_time")]
    [DataType(DataType.DateTime)]
    public DateTime DateTime { get; set; }

    [Display(Name = "Длительность")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("duration")]
    [Range(typeof(TimeSpan), "00:00:00", "23:59:59",
        ErrorMessage = "Время должно быть больше 00:00:00 и меньше 23:59:59.")]
    public TimeSpan Duration { get; set; }

    [Display(Name = "Описание")]
    [Column("description")]
    public string? Description { get; set; }
}