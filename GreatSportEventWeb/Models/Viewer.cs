using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Viewers")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Viewer : Person
{
    [Key]
    [Column("viewer_id")]
    public int Id { get; set; }

    [Display(Name = "Количество покупок")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("number_purchases")]
    public int NumberPurchases { get; set; }
}