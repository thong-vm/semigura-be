using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Message", Schema = "semigura")]
public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");


    [JsonProperty(PropertyName = "deviceName")]
    public string Sender { get; set; }
    public string Data { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}