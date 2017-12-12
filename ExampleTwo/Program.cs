using System;


namespace ExampleTwo
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new ExampleTwo())
            {
                game.Run();
            }
        }
    }
}
