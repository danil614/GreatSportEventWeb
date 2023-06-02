using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Gender")]
public class Gender : IComparable<Gender>
{
    [Key] [Column("gender_id")] public int Id { get; set; }

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Название")]
    [Column("gender_name")]
    [StringLength(20, ErrorMessage = "Текст должен быть меньше 20 символов.")]
    public string Name { get; set; } = null!;
    
    public override string ToString()
    {
        return Name;
    }

    public int CompareTo(Gender? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(Name, other.Name, StringComparison.Ordinal);
    }
}