using System;
using System.Collections.Generic;

namespace hashcode_2018_qualification
{
    class Vehicle
    {
        public int ID { get; private set; }
        public int PosR;
        public int PosC;

        public int TimeDriveEnd;
        public List<int> RidesAssigned;
        public int DriveDistance;
        public int BonusCollected;

        public Vehicle(int id)
        {
            this.ID = id;
            PosC = 0;
            PosR = 0;
            RidesAssigned = new List<int>();
            TimeDriveEnd = 0;
            DriveDistance = 0;
            BonusCollected = 0;
        }

        public int TimeToPosition(int r, int c)
        {
            return CalculateDistance(PosR, PosC, r, c);
        }

        public static int CalculateDistance(int r1, int c1, int r2, int c2)
        {
            return Math.Abs(r1 - r2) + Math.Abs(c1 - c2);
        }

        internal void AddRide(Ride ride, int endR, int endC, int timeDriveEnd)
        {
            RidesAssigned.Add(ride.ID);
            this.DriveDistance += ride.Distance;
            if (timeDriveEnd - ride.Distance == ride.TimeStart)
                this.BonusCollected++;

            this.PosR = endR;
            this.PosC = endC;
            this.TimeDriveEnd = timeDriveEnd;
        }

        public class CompareByTimeDriveEnd : Comparer<Vehicle>
        {
            public override int Compare(Vehicle x, Vehicle y)
            {
                return x.TimeDriveEnd.CompareTo(y.TimeDriveEnd);
            }
        }

    }

}
