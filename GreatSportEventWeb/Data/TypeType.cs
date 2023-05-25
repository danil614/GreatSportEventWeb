using System.ComponentModel.DataAnnotations;

namespace GreatSportEventWeb.Data;

public enum TypeType
{
    [Display(Name = "Тип места")]
    Location = 1,
    
    [Display(Name = "Тип мероприятия")]
    SportEvent = 2
}