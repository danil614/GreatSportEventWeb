using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Types")]
public class Type
{
    [Key, Column("type_id")]
    public int Id { get; set; }

    [Required]
    [Column("type_name")]
    [StringLength(maximumLength: 60)]
    public string Name { get; set; }

    public override string ToString()
    {
        return Name;
    }
}