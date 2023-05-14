using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreatSportEventWeb.Models;

[Table("Cities")]
public class City
{
    [Key, Column("city_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    [Required]
    [Column("city_name")]
    [StringLength(maximumLength: 60)]
    public string Name { get; set; }

    public override string ToString()
    {
        return Name;
    }
}