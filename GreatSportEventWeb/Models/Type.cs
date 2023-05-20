using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Types")]
public class Type
{
    [Key] [Column("type_id")] public int Id { get; set; }

    [Required(ErrorMessage = "Необходимо ввести название.")]
    [Column("type_name")]
    [StringLength(60, ErrorMessage = "Текст должен быть меньше 60 символов.")]
    public string Name { get; set; } = null!;

    public override string ToString()
    {
        return Name;
    }
}