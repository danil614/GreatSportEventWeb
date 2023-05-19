using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Locations")]
public class Location
{
    [Key] [Column("location_id")] public int Id { get; set; }

    [Required(ErrorMessage = "Необходимо ввести название.")]
    [Display(Name = "Название")]
    [Column("location_name")]
    [StringLength(60, ErrorMessage = "Текст должен быть меньше 60 символов.")]
    public string Name { get; set; }

    [Required] [Column("city_id")] public long CityId { get; set; }

    public virtual City City { get; set; }

    [Required(ErrorMessage = "Необходимо ввести адрес.")]
    [Column("address")]
    [Display(Name = "Адрес")]
    [StringLength(60)]
    public string Address { get; set; }

    [Required]
    [Column("location_type_id")]
    public int TypeId { get; set; }

    public virtual Type Type { get; set; }

    [Display(Name = "Вместимость")]
    [Range(1, int.MaxValue, ErrorMessage = "Значение вместимости должно быть больше 0.")]
    [Required] [Column("capacity")] public int Capacity { get; set; }

    [Display(Name = "Описание")]
    [Column("description")] public string? Description { get; set; }

// [Column("picture")]
// public byte[] Picture { get; set; }

    public override string ToString()
    {
        return $"{Name}, {Address}";
    }
}