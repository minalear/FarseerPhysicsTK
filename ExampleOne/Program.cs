using System;

namespace ExampleOne
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new ExampleOne())
            {
                game.Run();
            }
        }
    }
}
