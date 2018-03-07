﻿using System;
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
            return Utils.CalculateDistance(PosR, PosC, r, c);
        }

        internal void AddRide(Ride ride, int timeDriveEnd)
        {
            RidesAssigned.Add(ride.ID);
            this.DriveDistance += ride.Distance;
            if (timeDriveEnd - ride.Distance == ride.TimeStart)
                this.BonusCollected++;

            this.PosR = ride.EndR;
            this.PosC = ride.EndC;
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
