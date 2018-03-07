using System;

namespace hashcode_2018_qualification
{
    class Program
    {
        private static readonly string[] INPUT_FILES =
        {
            //@"c:\temp\a.in",
            //@"c:\temp\b.in",
            @"c:\temp\c.in",
            @"c:\temp\d.in"
            //@"c:\temp\e.in"
        };

        static void Main(string[] args)
        {
            int totalScore = 0;

            Solver solver = new SolverByCarTime();
            Solver solver2 = new SolverByCar(false);
            Solver solver3 = new SolverByCar(true);

            totalScore += ProcessInputFile(solver, @"c:\temp\a.in");
            totalScore += ProcessInputFile(solver, @"c:\temp\b.in");
            totalScore += ProcessInputFile(solver2, @"c:\temp\c.in");
            totalScore += ProcessInputFile(solver3, @"c:\temp\d.in");
            totalScore += ProcessInputFile(solver, @"c:\temp\e.in");

            System.Console.WriteLine("Total Score: {0}", totalScore);
        }

        private static int ProcessInputFile(Solver solver, string fileName)
        {
            System.Console.Write("Processing: {0}", fileName);
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
