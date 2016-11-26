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

        protected override void Seed(FootballAIGameWeb.Models.ApplicationDbContext context)
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

            // add all admin roles to default admin (if the admin exists)
            if (context.Users.Any(u => u.UserName == "admin"))
            {
                var store = new UserStore<User>(context);
                var manager = new UserManager<User>(store);
                var admin = context.Users.Single(u => u.UserName == "admin");

                // add admin to all admin roles
                var adminRoles = new List<string>() {"TournamentAdmin"};

                foreach (var adminRole in adminRoles)
                {
                    if (!manager.IsInRole(admin.Id, adminRole))
                        manager.AddToRole(admin.Id, adminRole);
                }
                
            }
        }
    }
}
