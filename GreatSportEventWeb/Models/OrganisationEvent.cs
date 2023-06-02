using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GreatSportEventWeb.Models;

[PrimaryKey(nameof(SportEventId), nameof(EmployeeId))]
[Table("Organisation_events")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganisationEvent
{
    [Key, Column("sport_event_id", Order = 0)]
    public int SportEventId { get; set; }

    [Key, Column("employee_id", Order = 1)]
    public int EmployeeId { get; set; }
    
    [Display(Name = "Спортивное мероприятие")]
    public virtual SportEvent? SportEvent { get; set; }
    
    [Display(Name = "Сотрудник")]
    public virtual Employee? Employee { get; set; }
}