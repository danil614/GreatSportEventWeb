using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GreatSportEventWeb.Data;

namespace GreatSportEventWeb.Models;

[Table("Users")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class User
{
    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Логин")]
    [MinLength(1, ErrorMessage = "Логин должен иметь длину больше 3 символов")]
    [MaxLength(60, ErrorMessage = "Логин должен иметь длину меньше 60 символов")]
    [Key]
    [Column("login")]
    public string Login { get; set; } = null!;

    [Required(ErrorMessage = "Поле является обязательным.")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    [Column("password")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Тип пользователя")]
    [NotMapped]
    public UserType AccessMode => (UserType)AccessModeId;

    [Required(ErrorMessage = "Поле является обязательным.")]
    [Display(Name = "Тип пользователя")]
    [Column("access_mode")]
    public int AccessModeId { get; set; }

    [Display(Name = "Спортсмен")]
    [Column("athlete_id")]
    public int? AthleteId { get; set; }
    
    [Display(Name = "Спортсмен")]
    public virtual Athlete? Athlete { get; set; }

    [Display(Name = "Сотрудник")]
    [Column("employee_id")]
    public int? EmployeeId { get; set; }
    
    [Display(Name = "Сотрудник")]
    public virtual Employee? Employee { get; set; }

    [Display(Name = "Зритель")]
    [Column("viewer_id")]
    public int? ViewerId { get; set; }
    
    [Display(Name = "Зритель")]
    public virtual Viewer? Viewer { get; set; }

    [NotMapped]
    public Person? Person
    {
        get
        {
            if (Athlete is not null)
            {
                return Athlete;
            }
            
            if (Employee is not null)
            {
                return Employee;
            }
            
            if (Viewer is not null)
            {
                return Viewer;
            }

            return null;
        }
    }
    
    [NotMapped]
    public int IsEdit { get; set; }
}