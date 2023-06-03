using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

public class Person : IComparable<Person>
{
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
    
    public override string ToString()
    {
        if (string.IsNullOrEmpty(Surname) && string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Patronymic))
        {
            return "";
        }
        
        return $"{Surname} {Name} {Patronymic}";
    }

    public string GetGivenName()
    {
        if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Patronymic))
        {
            return "";
        }
        
        return $"{Name} {Patronymic}";
    }

    public int CompareTo(Person? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var surnameComparison = string.Compare(Surname, other.Surname, StringComparison.Ordinal);
        if (surnameComparison != 0) return surnameComparison;
        var nameComparison = string.Compare(Name, other.Name, StringComparison.Ordinal);
        if (nameComparison != 0) return nameComparison;
        return string.Compare(Patronymic, other.Patronymic, StringComparison.Ordinal);
    }
}