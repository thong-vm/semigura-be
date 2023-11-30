using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("File", Schema = "semigura")]
public class MyFile
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string FileName { get; set; }
    [JsonIgnore]
    public string LocalPath { get; set; }
    public string Data { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}