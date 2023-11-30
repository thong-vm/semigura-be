using Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("User", Schema = "semigura")]
public class User : IEntity
{
    [Key]
    [Column(TypeName = "varchar(32)")]
    public string? Id { get; set; } = Guid.NewGuid().ToString("N");
    [MaxLength(50)]
    public string Account { get; set; } = null!;
    [MaxLength(50)]
    public string Role { get; set; } = "";

    [Column(TypeName = "varchar(128)")]
    [JsonIgnore]
    public string? HashedPassword { get; set; }
    public String Password
    {
        set
        {
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(value);
        }
    }
}
