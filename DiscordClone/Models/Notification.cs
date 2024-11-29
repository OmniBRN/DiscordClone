using System.ComponentModel.DataAnnotations;

namespace DiscordClone.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string type { get; set; }
        [Required]
        public string content {  get; set; } 

    }
}
