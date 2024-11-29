using System.ComponentModel.DataAnnotations;

namespace DiscordClone.Models
{
    public class Channel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Numele Canalului este obligatoriu")]
        [StringLength(25, ErrorMessage = "Numele Canalului poate avea maxim 25 de caractere")]
        public string Name { get; set; }
        [StringLength(100, ErrorMessage = "Descrierea Canalului poate avea maxim 100 de caractere")]
        public string? Description { get; set; }
        public int? GroupId { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
    }
}
