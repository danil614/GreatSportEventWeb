using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Sport_events")]
public class SportEvent
{
    [Key]
    [Column("sport_event_id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Место проведения")]
    [Column("location_id")]
    public int LocationId { get; set; }
    
    [Display(Name = "Место проведения")]
    public virtual Location? Location { get; set; }

    [Display(Name = "Тип")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("type_id")]
    public int TypeId { get; set; }
    
    [Display(Name = "Тип мероприятия")]
    public virtual Type? Type { get; set; }

    [Display(Name = "Начало")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("sport_event_date_time")]
    [DataType(DataType.DateTime)]
    public DateTime DateTime { get; set; }

    [Display(Name = "Длительность")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("duration")]
    public TimeSpan Duration { get; set; }

    [Display(Name = "Описание")]
    [Column("description")]
    public string? Description { get; set; }
}