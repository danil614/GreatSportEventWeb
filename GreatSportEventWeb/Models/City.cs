using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Cities")]
public class City
{
    [Key]
    [Column("city_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Название")]
    [Column("city_name")]
    [StringLength(60, ErrorMessage = "Текст должен быть меньше 60 символов.")]
    public string Name { get; set; } = null!;
    
    public override string ToString()
    {
        return Name;
    }
}