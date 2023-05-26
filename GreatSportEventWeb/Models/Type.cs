using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GreatSportEventWeb.Data;

namespace GreatSportEventWeb.Models;

[Table("Types")]
public class Type
{
    [Key] [Column("type_id")] public int Id { get; set; }

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Название")]
    [Column("type_name")]
    [StringLength(60, ErrorMessage = "Текст должен быть меньше 60 символов.")]
    public string Name { get; set; } = null!;
    
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Вид")]
    [Column("type_type")]
    public TypeType TypeType
    {
        get => (TypeType)TypeTypeId;
        set => TypeTypeId = (int)value;
    }
    
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Вид")]
    [NotMapped]
    public int TypeTypeId { get; set; }

    public override string ToString()
    {
        return Name;
    }
}