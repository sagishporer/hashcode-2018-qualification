using System;

namespace hashcode_2018_qualification
{
    class Program
    {
        static void Main(string[] args)
        {
            int totalScore = 0;

            Solver solver = new SolverByCarOneRideAtATime();
            Solver solver2 = new SolverByCar(false);

            totalScore += ProcessInputFile(solver, @"c:\temp\a.in");
            totalScore += ProcessInputFile(solver, @"c:\temp\b.in");
            totalScore += ProcessInputFile(solver2, @"c:\temp\c.in");
            totalScore += ProcessInputFile(solver, @"c:\temp\d.in");
            totalScore += ProcessInputFile(solver, @"c:\temp\e.in");

            System.Console.WriteLine("Total Score: {0}", totalScore);
        }

        private static int ProcessInputFile(Solver solver, string fileName)
        {
            System.Console.Write("{0} Processing: {1}", DateTime.Now, fileName);
            solver.Load(fileName);

            System.Console.Write(", Max Possible: {0}", solver.CalcMaxPossibleScore());

            long startTicks = DateTime.Now.Ticks;
            solver.Solve();
            System.Console.Write(", Run time: {0}", new TimeSpan(DateTime.Now.Ticks - startTicks));

            int score = solver.CalculateScore();
            solver.WriteOutput(fileName + ".out");
            System.Console.WriteLine(", Score: {0}", score);

            return score;
        }
    }
}
