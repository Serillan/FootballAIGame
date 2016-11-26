using System.Collections.Generic;
using FootballAIGameWeb.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FootballAIGameWeb.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FootballAIGameWeb.Models.ApplicationDbContext>
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
            if (!context.Roles.Any(r => r.Name == "TournamentAdmin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "TournamentAdmin" };
                manager.Create(role);
            }

            // default admin
            var adminName = "admin";
            var adminPassword = "admin28";
            var adminRoles = new List<string>() { "TournamentAdmin" };

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
