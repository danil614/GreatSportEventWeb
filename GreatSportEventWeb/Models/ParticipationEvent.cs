using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GreatSportEventWeb.Models;

[PrimaryKey(nameof(SportEventId), nameof(TeamId))]
[Table("Participation_events")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class ParticipationEvent
{
    [Key, Column("sport_event_id", Order = 0)]
    public int SportEventId { get; set; }

    [Key, Column("team_id", Order = 1)]
    public int TeamId { get; set; }
    
    [Display(Name = "Спортивное мероприятие")]
    public virtual SportEvent? SportEvent { get; set; }
    
    [Display(Name = "Команда")]
    public virtual Team? Team { get; set; }
    
    [Display(Name = "Счет")]
    [Range(0, int.MaxValue, ErrorMessage = "Число должно быть больше 0 и меньше 2147483647.")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "Число должно быть целым и положительным.")]
    [Column("score")] public int? Score { get; set; }
}