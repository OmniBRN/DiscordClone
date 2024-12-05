using System.ComponentModel.DataAnnotations;

namespace DiscordClone.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(1000, ErrorMessage = "Un mesaj poate avea maxim 1000 de caractere")]
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime? EditTimeStamp { get; set; }
        public string? UserId {  get; set; }
        public bool? WasEdited { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public string? FileRPath { get; set; }
        public string? GroupId { get; set; }
        public string? ChannelId { get; set; }
    }
}
