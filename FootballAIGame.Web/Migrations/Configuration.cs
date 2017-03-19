using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using FootballAIGame.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FootballAIGame.Web.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<FootballAIGame.Web.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            // add roles
            if (!context.Roles.Any(r => r.Name == RolesNames.TournamentAdmin))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = RolesNames.TournamentAdmin };
                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == RolesNames.MainAdmin))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = RolesNames.MainAdmin };
                manager.Create(role);
            }

            // default admin
            var adminName = "admin";
            var adminPassword = "admin28";
            var adminRoles = new List<string>() { RolesNames.MainAdmin, RolesNames.TournamentAdmin };

            // create default admin
            if (!context.Users.Any(u => u.UserName == adminName))
            {
                var store = new UserStore<User>(context);
                var manager = new UserManager<User>(store);
                var user = new User { UserName = adminName }; // email is not set yet!
                manager.Create(user, adminPassword);
                context.Players.Add(new Player(user));
            }

            // give all admin roles to the default admin
            var admin = context.Users.SingleOrDefault(u => u.UserName == adminName);
            if (admin != null)
            {
                var store = new UserStore<User>(context);
                var manager = new UserManager<User>(store);
                foreach (var adminRole in adminRoles)
                {
                    if (!manager.IsInRole(admin.Id, adminRole))
                        manager.AddToRole(admin.Id, adminRole);
                }
            }
        }
    }
}
