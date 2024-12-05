using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordClone.Models
{
    public class UserGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
       
        public string? GroupId { get; set; }
        public string? UserId { get; set; }
        public string? Culoare { get; set; }
        public string? Role {  get; set; }

        public virtual ApplicationUser? User { get; set; }
    }
}
