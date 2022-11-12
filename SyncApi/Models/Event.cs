using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SyncApi.Models
{
    [Table("Events")]
    public class Event : BasicTable
    {
        [MaxLength(255)]
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
    }
}
