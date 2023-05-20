using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Locations")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Location
{
    public const string TypeName = "Location";

    [Key] [Column("location_id")] public int Id { get; set; }

    [Required(ErrorMessage = "Необходимо ввести название.")]
    [Display(Name = "Название")]
    [Column("location_name")]
    [StringLength(60, ErrorMessage = "Текст должен быть меньше 60 символов.")]
    public string Name { get; set; } = null!;

    [Required] [Column("city_id")] public long CityId { get; set; }

    [Required(ErrorMessage = "Необходимо выбрать город.")]
    [Display(Name = "Город")]
    public virtual City City { get; set; } = null!;

    [Required(ErrorMessage = "Необходимо ввести адрес.")]
    [Column("address")]
    [Display(Name = "Адрес")]
    [StringLength(60, ErrorMessage = "Текст должен быть меньше 60 символов.")]
    public string Address { get; set; } = null!;

    [Required]
    [Column("location_type_id")]
    public int TypeId { get; set; }

    [Display(Name = "Тип")]
    [Required(ErrorMessage = "Необходимо выбрать тип.")]
    public virtual Type Type { get; set; } = null!;

    [Display(Name = "Вместимость")]
    [Range(1, int.MaxValue, ErrorMessage = "Значение вместимости должно быть больше 0.")]
    [Required(ErrorMessage = "Необходимо ввести вместимость.")]
    [Column("capacity")]
    public int Capacity { get; set; }

    [Display(Name = "Описание")]
    [Column("description")]
    public string? Description { get; set; }

// [Column("picture")]
// public byte[] Picture { get; set; }

    public override string ToString()
    {
        return $"{Name}, {Address}";
    }
}