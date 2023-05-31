using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Athletes")]
public class Athlete
{
    [Key]
    [Column("athlete_id")]
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

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Отчество")]
    [Column("patronymic")]
    [StringLength(30, ErrorMessage = "Текст должен быть меньше 30 символов.")]
    public string Patronymic { get; set; } = null!;

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

    [Display(Name = "Команда")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("team_id")]
    public int TeamId { get; set; }
    
    [Display(Name = "Команда")] public virtual Team? Team { get; set; }

    [Display(Name = "Должность")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Column("position_id")]
    public int PositionId { get; set; }
    
    [Display(Name = "Должность")] public virtual Position? Position { get; set; }

    [Display(Name = "Рейтинг")]
    [Range(1, int.MaxValue, ErrorMessage = "Число должно быть больше 0 и меньше 2147483647.")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "Число должно быть целым и положительным.")]
    [Required(ErrorMessage = "Поле является обязательным.")]
    public int? Rating { get; set; }

    [Display(Name = "Описание")]
    [Column("description")]
    public string? Description { get; set; }

    public override string ToString()
    {
        return $"{Surname} {Name} {Patronymic}";
    }
}