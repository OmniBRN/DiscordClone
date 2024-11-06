using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Models
public class AppDBContext : DbContext
{
    public AppDBContext() : base ()
    {

    }
    protected override void OnConfiguring
    (DbContextOptionsBuilder options)
    {

    var connectionString =

    "server=localhost;database=DiscordClone;uid=daw_example;password=parola1";

    var serverVersion = new MySqlServerVersion(new

    Version(8, 0, 31));

    options.UseMySql(connectionString, serverVersion);

    }
}
