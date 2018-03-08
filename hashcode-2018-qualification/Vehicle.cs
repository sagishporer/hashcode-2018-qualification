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
        public List<Ride> RidesAssigned;
        public int DriveDistance;
        public int BonusCollected;

        public int ClosestRideDistance;

        public Vehicle(int id)
        {
            this.ID = id;
            PosC = 0;
            PosR = 0;
            RidesAssigned = new List<Ride>();
            TimeDriveEnd = 0;
            DriveDistance = 0;
            BonusCollected = 0;

            ClosestRideDistance = 0;
        }

        public int GetScore(int bonusValue)
        {
            return DriveDistance + BonusCollected * bonusValue;
        }

        public void CalculateClosestRide(List<Ride> rides)
        {
            int closest = int.MaxValue;

            for (int i = 0; i < rides.Count; i++)
            {
                Ride other = rides[i];
                if (this.ID == other.ID)
                    continue;

                int distance = this.TimeToPosition(other.StartR, other.StartC);
                if (this.TimeDriveEnd + distance > other.TimeLatestStart)
                    continue;

                closest = Math.Min(closest, distance);
            }

            ClosestRideDistance = closest;
        }


        public int TimeToPosition(int r, int c)
        {
            return Utils.CalculateDistance(PosR, PosC, r, c);
        }

        internal void AddRide(Ride ride, int timeDriveEnd)
        {
            RidesAssigned.Add(ride);
            this.DriveDistance += ride.Distance;
            if (timeDriveEnd - ride.Distance == ride.TimeStart)
                this.BonusCollected++;

            this.PosR = ride.EndR;
            this.PosC = ride.EndC;
            this.TimeDriveEnd = timeDriveEnd;
        }

        public class CompareByClosestRideDistance : Comparer<Vehicle>
        {
            public override int Compare(Vehicle x, Vehicle y)
            {
                return x.ClosestRideDistance.CompareTo(y.ClosestRideDistance);
            }
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
