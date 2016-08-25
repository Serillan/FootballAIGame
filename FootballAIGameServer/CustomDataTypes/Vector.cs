using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGameServer.CustomDataTypes
{
    public class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector() { }

        public Vector(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double Length =>
            Math.Sqrt(X * X + Y * Y);

        public static double DistanceBetween(Vector a, Vector b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public static double DotProduct(Vector a, Vector b)
        {
            return a.X*b.X + a.Y*b.Y;
        }
    }
}
