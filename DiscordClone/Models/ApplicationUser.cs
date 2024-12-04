using Microsoft.AspNetCore.Identity;

namespace DiscordClone.Models
{
    // PASUL 1: Facem tabelul "AplicationUser" care va mosteni clasa default de la individual accounts
    // + atributele necesare aplicatiei noastre
    // P2 -> program.cs
    public class ApplicationUser: IdentityUser
    {

        //PASUL 10: Adaugam atributele care ne intereseaza
        public virtual ICollection<Message>? Messages { get; set; }

        public virtual ICollection<Notification>? Notifications { get; set; }

        public virtual ICollection<UserGroup>? UserGroups { get; set; }

        public string? ProfilePicture { get; set; }

    }
}
