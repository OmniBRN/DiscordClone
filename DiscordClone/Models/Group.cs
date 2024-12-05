using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// PASUL 6:
// P7->_LoginPartial.cshtml
namespace DiscordClone.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Numele grupului este obligatoriu")]
        [StringLength(50, ErrorMessage = "Numele are maxim 50 de caractere")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Descrierea grupului este obligatorie")]
        public string Description { get; set; }

        public DateTime Date {  get; set; }
        public int? CategoryId { get; set; }
        public string? ImageRPath { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<Channel>? Channels { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categ {  get; set; }
    }
}
