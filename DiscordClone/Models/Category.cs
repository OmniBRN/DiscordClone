using System.ComponentModel.DataAnnotations;

namespace DiscordClone.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Numele categoriei poate avea maxim 20 de caractere")]
        public string Name { get; set; }
        public virtual ICollection<Group>? Groups { get; set; }
    }
}
