using System;

namespace ExampleThree
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new ExampleThree())
            {
                game.Run();
            }
        }
    }
}
