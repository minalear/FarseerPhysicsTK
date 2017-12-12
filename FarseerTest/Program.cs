using System;

namespace FarseerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new FarseerTest())
            {
                game.Run();
            }
        }
    }
}
