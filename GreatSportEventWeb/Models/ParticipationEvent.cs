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
    
    public virtual SportEvent? SportEvent { get; set; }
    
    public virtual Team? Team { get; set; }

    [Column("score")] public int Score { get; set; }
}