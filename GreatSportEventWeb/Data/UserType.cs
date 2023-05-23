using System.ComponentModel.DataAnnotations;

namespace GreatSportEventWeb.Data;

public enum UserType
{
    [Display(Name = "Не определен")]
    Null = 0,
    
    [Display(Name = "Администратор")]
    Admin = 1,
    
    [Display(Name = "Продавец")]
    Seller = 2,
    
    [Display(Name = "Организатор")]
    Organizer = 3,
    
    [Display(Name = "HR менеджер")]
    Hr = 4,
    
    [Display(Name = "Тренер")]
    Trainer = 5,
    
    [Display(Name = "Спортсмен")]
    Athlete = 6
}