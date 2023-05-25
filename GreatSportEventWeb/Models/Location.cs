using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Locations")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Location : AsSerializable
{
    [Key] [Column("location_id")] public int Id { get; set; }

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Название")]
    [Column("location_name")]
    [StringLength(60, ErrorMessage = "Текст должен быть меньше 60 символов.")]
    public string Name { get; set; } = null!;

    [Display(Name = "Город")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("city_id")]
    public long CityId { get; set; }
    
    [Display(Name = "Город")]
    public virtual City? City { get; set; }

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("address")]
    [Display(Name = "Адрес")]
    [StringLength(60, ErrorMessage = "Текст должен быть меньше 60 символов.")]
    public string Address { get; set; } = null!;
    
    [Display(Name = "Тип места")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("location_type_id")]
    public int TypeId { get; set; }
    
    [Display(Name = "Тип места")]
    public virtual Type? Type { get; set; }

    [Display(Name = "Вместимость")]
    [Range(1, int.MaxValue, ErrorMessage = "Число должно быть больше 0 и меньше 2147483647.")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "Число должно быть целым и положительным.")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("capacity")]
    public int Capacity { get; set; }

    [Display(Name = "Описание")]
    [Column("description")]
    public string? Description { get; set; }

// [Column("picture")]
// public byte[] Picture { get; set; }

    public override string ToString()
    {
        return $"{City?.Name}, {Name}";
    }
}