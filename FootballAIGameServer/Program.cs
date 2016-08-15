using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.Models;

namespace FootballAIGameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new ApplicationDbContext())
            {
                foreach (var player in context.Players)
                {
                    Console.WriteLine(player.Name);
                }
            }
        }
    }
}
