using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace GreatSportEventWeb.Models;

[Table("Seats")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Seat
{
    [Key]
    [Column("seat_id")]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Номер места")]
    [Column("seat_name")]
    [StringLength(20, ErrorMessage = "Текст должен быть меньше 20 символов.")]
    public string Name { get; set; } = null!;

    [Display(Name = "Спортивное мероприятие")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("sport_event_id")]
    public int SportEventId { get; set; }
    
    [Display(Name = "Спортивное мероприятие")] public virtual SportEvent? SportEvent { get; set; }

    [Display(Name = "Цена")]
    [Range(typeof(decimal),"0,00", "1000000,00", ErrorMessage = "Число должно быть больше 0 и меньше 1000000.")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("seat_price")]
    public decimal Price { get; set; }

    [NotMapped]
    [Display(Name = "Цена")]
    [Range(typeof(decimal),"0,00", "1000000,00", ErrorMessage = "Число должно быть больше 0 и меньше 1000000.")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    public string PriceString
    {
        get => Price.ToString(CultureInfo.CurrentCulture);
        set
        {
            if (decimal.TryParse(value, out var result))
            {
                Price = result;
            }
            else
            {
                Price = -1;
            }
        }
    }

    [Display(Name = "Занято")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("is_occupied")]
    public bool IsOccupied { get; set; }
}