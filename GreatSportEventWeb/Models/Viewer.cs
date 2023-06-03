using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Viewers")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Viewer
{
    [Key]
    [Column("viewer_id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Фамилия")]
    [Column("surname")]
    [StringLength(30, ErrorMessage = "Текст должен быть меньше 30 символов.")]
    public string Surname { get; set; } = null!;

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Имя")]
    [Column("name")]
    [StringLength(30, ErrorMessage = "Текст должен быть меньше 30 символов.")]
    public string Name { get; set; } = null!;
    
    [Display(Name = "Отчество")]
    [Column("patronymic")]
    [StringLength(30, ErrorMessage = "Текст должен быть меньше 30 символов.")]
    public string? Patronymic { get; set; }

    [Display(Name = "Пол")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("gender_id")]
    public int GenderId { get; set; }
    
    [Display(Name = "Пол")] public virtual Gender? Gender { get; set; }

    [Column("phone_number")]
    [Display(Name = "Номер телефона")]
    [StringLength(20, ErrorMessage = "Текст должен быть меньше 20 символов.")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Дата рождения")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("birth_date")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }
    
    [Display(Name = "Количество покупок")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("number_purchases")]
    public int NumberPurchases { get; set; }

    public override string ToString()
    {
        return $"{Surname} {Name} {Patronymic}";
    }
}