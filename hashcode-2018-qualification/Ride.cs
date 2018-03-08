using System;
using System.Collections.Generic;

namespace hashcode_2018_qualification
{
    class Ride
    {
        public int ID { get; private set; }
        public int StartR { get; private set; }
        public int StartC { get; private set; }
        public int EndR { get; private set; }
        public int EndC { get; private set; }

        public int TimeStart { get; private set; }
        public int TimeEnd { get; private set; }

        public int Distance { get; private set; }
        public int TimeLatestStart { get; private set; }

        public int ClosestRideDistance { get; private set; }

        public Ride(int id, int startR, int startC, int endR, int endC, int timeStart, int timeEnd)
        {
            this.ID = id;

            this.StartR = startR;
            this.StartC = startC;
            this.EndR = endR;
            this.EndC = endC;

            this.TimeStart = timeStart;
            this.TimeEnd = timeEnd;

            this.Distance = Utils.CalculateDistance(StartR, StartC, EndR, EndC);
            this.TimeLatestStart = this.TimeEnd - this.Distance;

            if (this.TimeLatestStart < this.TimeStart)
                throw new Exception("Error in input");
        }

        public void CalculateClosestRide(List<Ride> rides)
        {
            int closest = int.MaxValue;

            for (int i = 0; i < rides.Count; i++)
            {
                Ride other = rides[i];
                if (this.ID == other.ID)
                    continue;

                int distance = Utils.CalculateDistance(this.EndR, this.EndC, other.StartR, other.StartC);
                if (this.TimeStart + this.Distance + distance > other.TimeLatestStart)
                    continue;

                closest = Math.Min(closest, distance);
            }

            ClosestRideDistance = closest;
        }

        public class CompareByStartTime : Comparer<Ride>
        {
            public override int Compare(Ride x, Ride y)
            {
                int compStart = x.TimeStart.CompareTo(y.TimeStart);
                if (compStart != 0)
                    return compStart;

                // Used to get a stable sorting - should not be relevant to the algorithm
                int compEnd = x.TimeEnd.CompareTo(y.TimeEnd);
                if (compEnd != 0)
                    return compEnd;

                return x.Distance.CompareTo(y.Distance);
            }
        }

        public class CompareByStartTimeLatest : Comparer<Ride>
        {
            public override int Compare(Ride x, Ride y)
            {
                int compLStart = x.TimeLatestStart.CompareTo(y.TimeLatestStart);
                if (compLStart != 0)
                    return compLStart;

                // Used to get a stable sorting - should not be relevant to the algorithm
                int compStart = x.TimeStart.CompareTo(y.TimeStart);
                if (compStart != 0)
                    return compStart;

                int compEnd = x.TimeEnd.CompareTo(y.TimeEnd);
                if (compEnd != 0)
                    return compEnd;

                return x.Distance.CompareTo(y.Distance);
            }
        }
    }
}
