using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Positions")]
public class Position : IComparable<Position>
{
    [Key] [Column("position_id")] public int Id { get; set; }

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Название")]
    [Column("position_name")]
    [StringLength(60, ErrorMessage = "Текст должен быть меньше 60 символов.")]
    public string Name { get; set; } = null!;
    
    public override string ToString()
    {
        return Name;
    }

    public int CompareTo(Position? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(Name, other.Name, StringComparison.Ordinal);
    }
}