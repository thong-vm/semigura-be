using Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Models
{
    [Table("Sensor")]
    [Index(nameof(Code), IsUnique = true)]
    public class Sensor:IEntity
    {

        public enum SensorType
        {
            SEIGIKU = 1,
            LOCATION = 2,
            TANK = 3
        }

        [Key]
        [Column(TypeName = "varchar(32)")]
        public string? Id { get; set; } = ModelsHelper.NewId();

        [MaxLength(10)]
        public string? Code { get; set; }

        [MaxLength(20)]
        public string? Name { get; set; }
        public SensorType? Type { get; set; }
        [MaxLength(20)]
        public string? Factory { get; set; }
        [MaxLength(20)]
        public string? Tank { get; set; }
    }
}
