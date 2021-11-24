using System;

namespace RestExercise5
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing the Rest service from RestExercise4");
            Worker myWorker = new Worker();
            myWorker.Start();
        }
    }
}
