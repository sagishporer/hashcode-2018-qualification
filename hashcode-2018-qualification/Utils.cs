using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hashcode_2018_qualification
{
    class Utils
    {
        public static int CalculateDistance(int r1, int c1, int r2, int c2)
        {
            return Math.Abs(r1 - r2) + Math.Abs(c1 - c2);
        }
    }
}
