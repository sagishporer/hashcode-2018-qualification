using System;

namespace hashcode_2018_qualification
{
    class Program
    {
        private static readonly string[] INPUT_FILES =
        {
            @"c:\temp\a.in",
            @"c:\temp\b.in",
            @"c:\temp\c.in",
            @"c:\temp\d.in",
            @"c:\temp\e.in"
        };

        static void Main(string[] args)
        {
            int totalScore = 0;
            foreach (string fileName in INPUT_FILES)
            {
                System.Console.Write("Processing: {0}", fileName);
                Solver solver = new SolverByCarTime();
                Solver solver2 = new SolverByCar();
                solver.Load(fileName);
                solver2.Load(fileName);

                System.Console.Write(", Max Possible: {0}", solver.CalcMaxPossibleScore());

                long startTicks = DateTime.Now.Ticks;
                solver.Solve();
                solver2.Solve();
                System.Console.Write(", Run time: {0}", new TimeSpan(DateTime.Now.Ticks - startTicks));

                Solver bestSolver;
                if (solver.CalculateScore() > solver2.CalculateScore())
                    bestSolver = solver;
                else
                    bestSolver = solver2;

                int score = bestSolver.CalculateScore();
                bestSolver.WriteOutput(fileName + ".out");
                System.Console.Write(", Score: {0}", score);
                totalScore += score;

                System.Console.WriteLine();
            }

            System.Console.WriteLine("Total Score: {0}", totalScore);
        }
    }
}
