using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Tickets")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Ticket
{
    [Key]
    [Column("ticket_id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Зритель")]
    [Range(0, int.MaxValue, ErrorMessage = "Поле является обязательным.")]
    [Column("viewer_id")]
    public int ViewerId { get; set; }
    
    [Display(Name = "Зритель")] public virtual Viewer? Viewer { get; set; }

    [Display(Name = "Продавец")]
    [Column("employee_id")]
    public int? EmployeeId { get; set; }
    
    [Display(Name = "Продавец")] public virtual Employee? Employee { get; set; }

    [Display(Name = "Посадочное место")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Range(0, int.MaxValue, ErrorMessage = "Поле является обязательным.")]
    [Column("seat_id")]
    public int SeatId { get; set; }
    
    [Display(Name = "Посадочное место")] public virtual Seat? Seat { get; set; }
    
    [Display(Name = "Дата продажи")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("sale_date_time")]
    [DataType(DataType.DateTime)]
    public DateTime DateTime { get; set; }
}