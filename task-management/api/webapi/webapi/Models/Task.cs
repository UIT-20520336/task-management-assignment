using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace webapi.Models
{
    public class Task
    {
        [Required]
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public bool? IsCompleted { get; set; }
        [Required] public int UserId { get; set; }

        [JsonIgnore] public User? User { get; set; }
    }
}
