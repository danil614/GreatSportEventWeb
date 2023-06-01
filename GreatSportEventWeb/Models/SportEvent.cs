using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace GreatSportEventWeb.Models;

[Table("Sport_events")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class SportEvent
{
    [Key] [Column("sport_event_id")] public int Id { get; set; }

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Место проведения")]
    [Column("location_id")]
    public int LocationId { get; set; }

    [Display(Name = "Место проведения")] public virtual Location? Location { get; set; }

    [Display(Name = "Тип мероприятия")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("type_id")]
    public int TypeId { get; set; }

    [Display(Name = "Тип мероприятия")] public virtual Type? Type { get; set; }

    [Display(Name = "Начало мероприятия")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("sport_event_date_time")]
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
    
    //[InverseProperty("SportEvent")]
    public virtual ICollection<ParticipationEvent>? ParticipationEvents { get; set; }
    
    [NotMapped]
    //[BindProperty]
    public List<int>? SelectedTeamIds { get; set; }
}