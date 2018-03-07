using System;
using System.Collections.Generic;

namespace hashcode_2018_qualification
{
    // A: 10; B: 172,877; C: 15,818,350; D: 11,280,734; E: 12,618,975
    // Best: A, C
    //SolveByCar();

    // A: 10, B: 176,527; C: 15,818,350; D: 11,317,079; E: 21,400,975
    // Best: A, C
    //SolveByCarBonus();

    // A: 4, B: 176,877; C: 14,994,000; D: 9,963,479; E: 21,461,975
    // Best: B, E
    //SolveByComplete();

    // A: 4, B: 176,877; C: 12,468,914; D: 11,092,180; E: 21,461,975
    // Best: B, E
    //SolveSimple();

    // A: 10; B: 176,877; C: 15,790,168; D: 11,741,173; E: 21,461,975
    // Best: A, B, E
    //SolveByTime();

    // A: 10; B: 176,877; C: 15,790,168; D: 11,787,251; E: 21,461,975
    // Best: A, B, D, E
    //SolveByCarTime();

    abstract class Solver
    {
        private int Rows;
        private int Columns;

        public List<Vehicle> Vehicles { get; private set; }
        public int Bonus { get; private set; }
        protected int Steps;

        protected List<Ride> Rides;

        public void Load(string fileName)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(fileName))
            {
                string line = sr.ReadLine();
                string[] parts = line.Split(' ');
                int rows = int.Parse(parts[0]);
                int columns = int.Parse(parts[1]);
                int vehiclesCount = int.Parse(parts[2]);
                int ridesCount = int.Parse(parts[3]);
                int bonus = int.Parse(parts[4]);
                int steps = int.Parse(parts[5]);

                List<Vehicle> vehicles = new List<Vehicle>();
                for (int i = 0; i < vehiclesCount; i++)
                    vehicles.Add(new Vehicle(i));

                List<Ride> rides = new List<Ride>();
                for (int i = 0; i < ridesCount; i++)
                {
                    line = sr.ReadLine();
                    parts = line.Split(' ');
                    int startR = int.Parse(parts[0]);
                    int startC = int.Parse(parts[1]);
                    int endR = int.Parse(parts[2]);
                    int endC = int.Parse(parts[3]);
                    int timeStart = int.Parse(parts[4]);
                    int timeEnd = int.Parse(parts[5]);

                    Ride ride = new Ride(i, startR, startC, endR, endC, timeStart, timeEnd);

                    rides.Add(ride);
                }

                this.Rows = rows;
                this.Columns = columns;

                this.Vehicles = vehicles;
                this.Rides = rides;

                this.Bonus = bonus;
                this.Steps = steps;
            }
        }

        public int CalculateScore()
        {
            int totalScore = 0;

            foreach (Vehicle vehicle in this.Vehicles)
                totalScore += vehicle.DriveDistance + vehicle.BonusCollected * this.Bonus;

            return totalScore;
        }

        public void WriteOutput(string fileName)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName))
            {
                foreach (Vehicle vehicle in this.Vehicles)
                {
                    List<Ride> ridesAssigned = vehicle.RidesAssigned;
                    sw.Write(ridesAssigned.Count);
                    for (int i = 0; i < ridesAssigned.Count; i++)
                    {
                        sw.Write(" ");
                        sw.Write(ridesAssigned[i].ID);
                    }
                    sw.WriteLine();
                }
            }
        }

        public int CalcMaxPossibleScore()
        {
            int totalDistance = 0;
            foreach (Ride ride in Rides)
                totalDistance += ride.Distance;

            return totalDistance + Rides.Count * Bonus;
        }

        public abstract void Solve();

        protected bool TryCarsReallocate(bool useClosestRide)
        {
            for (int i = 0; i < Vehicles.Count; i++)
            {
                Vehicle car = Vehicles[i];
                List<Ride> freeRides = new List<Ride>(Rides);
                freeRides.AddRange(car.RidesAssigned);

                Vehicle newCar = new Vehicle(car.ID);
                AllocateRidesToCar_StartEarliest(useClosestRide, newCar, freeRides, this.Bonus);
                if (newCar.DriveDistance + newCar.BonusCollected * this.Bonus > car.DriveDistance + car.BonusCollected * this.Bonus)
                {
                    Rides = freeRides;
                    Vehicles[i] = newCar;

                    return true;
                }
            }

            return false;
        }

        protected void AllocateRidesToCar_StartEarliest(bool useClosestRide, Vehicle newCar, List<Ride> freeRides, int bonusValue)
        {
            while (true)
            {
                Ride bestRide;
                int bestCompleteTime;

                AllocateRidesToCar_FindEarlyStartRide(useClosestRide, newCar, freeRides, bonusValue, out bestRide, out bestCompleteTime);

                if (bestRide != null)
                {
                    newCar.AddRide(bestRide, bestCompleteTime);
                    // Remove ride from list
                    for (int i = 0; i < freeRides.Count; i++)
                        if (freeRides[i].ID == bestRide.ID)
                        {
                            freeRides.RemoveAt(i);
                            break;
                        }
                }
                else
                    break;
            }
        }

        private void AllocateRidesToCar_FindEarlyStartRide(bool useClosestRide, Vehicle car, List<Ride> rides, int bonusValue, out Ride bestRide, out int bestCompleteTime)
        {
            bestRide = null;
            bestCompleteTime = 0;
            double bestStartTime = 0;
            int bestTimeToDrive = 0;
            double bestScoreDensity = 0;

            foreach (Ride ride in rides)
            {
                int timeToDrive = car.TimeToPosition(ride.StartR, ride.StartC);
                int carToStart = car.TimeDriveEnd + timeToDrive;
                if (carToStart >= ride.TimeEnd)
                    continue;
                double startTime = Math.Max(carToStart, ride.TimeStart);
                int completeTime = (int)startTime + ride.Distance;
                if (completeTime >= ride.TimeEnd)
                    continue;

                int bonus = (startTime == ride.TimeStart) ? bonusValue : 0;
                double scoreDensity = (double)(ride.Distance + bonus) / (double)(startTime + ride.Distance - car.TimeDriveEnd);

                if (useClosestRide == true)
                    if ((double)completeTime <= 0.98 * this.Steps)
                        startTime += (double)ride.ClosestRideDistance * 0.98;

                if (bestRide == null)
                {
                    bestCompleteTime = completeTime;
                    bestRide = ride;
                    bestStartTime = startTime;
                    bestTimeToDrive = timeToDrive;
                    bestScoreDensity = scoreDensity;
                }
                else if (startTime < bestStartTime)
                {
                    bestCompleteTime = completeTime;
                    bestRide = ride;
                    bestStartTime = startTime;
                    bestTimeToDrive = timeToDrive;
                    bestScoreDensity = scoreDensity;
                }
            }
        }

        protected bool TryCarsPushRide()
        {
            for (int i = 0; i < Vehicles.Count; i++)
            {
                Vehicle car = Vehicles[i];
                for (int j = 0; j < Rides.Count; j++)
                {
                    Ride newRide = Rides[j];
                    List<Ride> rides = new List<Ride>(car.RidesAssigned);

                    Vehicle newCar = new Vehicle(car.ID);
                    for (int r = 0; r < rides.Count; r++)
                    {
                        Ride ride = rides[r];
                        int timeToRide = newCar.TimeDriveEnd + newCar.TimeToPosition(ride.StartR, ride.StartC);
                        timeToRide = Math.Max(timeToRide, ride.TimeStart);
                        int timeToRideEnd = timeToRide + ride.Distance;
                        if (newRide != null)
                        {
                            int ridesDistance = Utils.CalculateDistance(ride.EndR, ride.EndC, newRide.StartR, newRide.StartC);
                            if (timeToRideEnd + ridesDistance > newRide.TimeLatestStart)
                            {
                                // Try to add new ride
                                ride = newRide;
                                newRide = null;
                                r--;
                                timeToRide = newCar.TimeDriveEnd + newCar.TimeToPosition(ride.StartR, ride.StartC);
                                timeToRide = Math.Max(timeToRide, ride.TimeStart);
                                timeToRideEnd = timeToRide + ride.Distance;
                            }
                        }

                        if (timeToRideEnd < ride.TimeEnd)
                            newCar.AddRide(ride, timeToRideEnd);
                    }

                    if (newCar.DriveDistance + newCar.BonusCollected * this.Bonus > car.DriveDistance + car.BonusCollected * this.Bonus)
                    {
                        Rides.AddRange(rides);
                        foreach (Ride ride in newCar.RidesAssigned)
                            if (Rides.Remove(ride) == false)
                                throw new Exception("Bug");

                        Vehicles[i] = newCar;
                        return true;
                    }
                }
            }

            return false;
        }
    }

    class SolverByCarTime : Solver
    {
        public override void Solve()
        {
            Rides.Sort(new Ride.CompareByStartTime());
            Rides.Reverse();
            Vehicle.CompareByTimeDriveEnd carSortTimeDriveEnd = new Vehicle.CompareByTimeDriveEnd();
            Vehicles.Sort(carSortTimeDriveEnd);
            int currentTime = -1;

            while (Rides.Count > 0)
            {
                Vehicles.Sort(carSortTimeDriveEnd);

                currentTime++;
                if (Vehicles[0].TimeDriveEnd > currentTime)
                    currentTime = Vehicles[0].TimeDriveEnd;

                if (currentTime >= this.Steps)
                    break;

                // Clean rides list
                for (int ridePos = Rides.Count - 1; ridePos >= 0; ridePos--)
                {
                    Ride ride = Rides[ridePos];

                    // If ride not possible
                    if (ride.TimeLatestStart < currentTime)
                    {
                        Rides.RemoveAt(ridePos);
                        continue;
                    }
                }

                for (int i = 0; i < Vehicles.Count; i++)
                {
                    Vehicle car = Vehicles[i];

                    // This car (and all after) can't do rides in time
                    if (car.TimeDriveEnd > currentTime)
                        break;

                    Ride bestRide;
                    int bestStartTime;
                    FindBestRideForCarMaxTime(car, currentTime, out bestRide, out bestStartTime);

                    if (bestRide == null)
                        continue;

                    // Check if valid
                    // Complete ride on time
                    int rideStartTime = Math.Max(bestRide.TimeStart, bestStartTime);
                    if (rideStartTime + bestRide.Distance >= bestRide.TimeEnd)
                        continue;

                    if (rideStartTime + bestRide.Distance >= Steps)
                        continue;

                    // Add ride to car
                    car.AddRide(bestRide, rideStartTime + bestRide.Distance);
                    Rides.Remove(bestRide);
                }
            }
        }

        private void FindBestRideForCarMaxTime(Vehicle car, int maxTimeToStart, out Ride bestRide, out int bestStartTime)
        {
            bestRide = null;
            bestStartTime = 0;
            int bestTimeToDrive = 0;
            double bestScoreDensity = 0;

            for (int ridePos = Rides.Count - 1; ridePos >= 0; ridePos--)
            {
                Ride ride = Rides[ridePos];
                if (ride.TimeStart > maxTimeToStart)
                    break;

                int timeToDrive = car.TimeToPosition(ride.StartR, ride.StartC);
                int carToStart = car.TimeDriveEnd + timeToDrive;
                if (carToStart > maxTimeToStart)
                    continue;
                if (carToStart >= ride.TimeEnd)
                    continue;

                int startTime = Math.Max(carToStart, ride.TimeStart);
                if (startTime + ride.Distance >= ride.TimeEnd)
                    continue;

                int bonus = (startTime == ride.TimeStart) ? Bonus : 0;
                double scoreDensity = (double)(ride.Distance + bonus) / (double)(startTime + ride.Distance - car.TimeDriveEnd);

                if (bestRide == null)
                {
                    bestRide = ride;
                    bestStartTime = startTime;
                    bestTimeToDrive = timeToDrive;
                    bestScoreDensity = scoreDensity;
                }
                else if (ride.Distance < bestRide.Distance)
                {
                    bestRide = ride;
                    bestStartTime = startTime;
                    bestTimeToDrive = timeToDrive;
                    bestScoreDensity = scoreDensity;
                }

                /*
                else if (timeToDrive < bestTimeToDrive)
                {
                    bestRide = ride;
                    bestStartTime = startTime;
                    bestTimeToDrive = timeToDrive;
                    bestScoreDensity = scoreDensity;
                }
                */
                /*else if (startTime < bestStartTime)
                {
                    bestRide = ride;
                    bestStartTime = startTime;
                    bestTimeToDrive = timeToDrive;
                    bestScoreDensity = scoreDensity;
                }*/
            }
        }
    }

    class SolverByRideTime : Solver
    {
        public override void Solve()
        {
            Rides.Sort(new Ride.CompareByStartTime());
            Rides.Reverse();
            Vehicle.CompareByTimeDriveEnd carSortTimeDriveEnd = new Vehicle.CompareByTimeDriveEnd();
            Vehicles.Sort(carSortTimeDriveEnd);
            int currentTime = -1;

            while (Rides.Count > 0)
            {
                currentTime++;
                if (Vehicles[0].TimeDriveEnd > currentTime)
                    currentTime = Vehicles[0].TimeDriveEnd;

                if (currentTime >= this.Steps)
                    break;

                for (int ridePos = Rides.Count - 1; ridePos >= 0; ridePos--)
                {
                    Ride ride = Rides[ridePos];

                    // When too far in list
                    if (ride.TimeStart > currentTime)
                        break;

                    // If ride not possible
                    if (ride.TimeLatestStart < currentTime)
                    {
                        Rides.RemoveAt(ridePos);
                        continue;
                    }

                    Vehicle bestCar;
                    int bestStartTime;
                    FindBestCarForRideMaxTime(ride, currentTime, out bestCar, out bestStartTime);

                    if (bestCar == null)
                        continue;

                    // Check if valid
                    // Complete ride on time
                    int rideStartTime = Math.Max(ride.TimeStart, bestStartTime);
                    if (rideStartTime + ride.Distance >= ride.TimeEnd)
                        continue;

                    if (rideStartTime + ride.Distance >= Steps)
                        continue;

                    // Add ride to car
                    bestCar.AddRide(ride, rideStartTime + ride.Distance);
                    Rides.RemoveAt(ridePos);

                    // Optimization - if not vehicle is free at this time - not need to continue checking
                    Vehicles.Sort(carSortTimeDriveEnd);
                    if (Vehicles[0].TimeDriveEnd > currentTime)
                        break;
                }
            }
        }

        private void FindBestCarForRideMaxTime(Ride ride, int maxTimeToStart, out Vehicle bestCar, out int bestStartTime)
        {
            bestCar = null;
            bestStartTime = 0;

            foreach (Vehicle car in Vehicles)
            {
                if (car.TimeDriveEnd > maxTimeToStart)
                    break;

                int carToStart = car.TimeDriveEnd + car.TimeToPosition(ride.StartR, ride.StartC);
                if (carToStart > maxTimeToStart)
                    continue;

                if (carToStart + ride.Distance >= ride.TimeEnd)
                    continue;

                if (bestCar == null)
                {
                    bestCar = car;
                    bestStartTime = carToStart;
                }
                else if (carToStart < bestStartTime)
                {
                    bestCar = car;
                    bestStartTime = carToStart;
                }
            }
        }
    }

    class SolverByCarBonus : Solver
    {
        public override void Solve()
        {
            for (int carPos = 0; carPos < Vehicles.Count; carPos++)
            {
                Vehicle car = Vehicles[carPos];
                while (true)
                {
                    Ride bestRide;
                    int bestStartTime;

                    FindBestRideForCarBonus(car, out bestRide, out bestStartTime);

                    if (bestRide != null)
                    {
                        car.AddRide(bestRide, bestStartTime + bestRide.Distance);
                        // Remove ride from list
                        for (int i = 0; i < Rides.Count; i++)
                            if (Rides[i].ID == bestRide.ID)
                            {
                                Rides.RemoveAt(i);
                                break;
                            }
                    }
                    else
                        break;
                }
            }

        }

        private void FindBestRideForCarBonus(Vehicle car, out Ride bestRide, out int bestStartTime)
        {
            bestRide = null;
            bestStartTime = 0;
            int bestHasBonus = 0;

            foreach (Ride ride in Rides)
            {
                int hasBonus;
                int carToStart = car.TimeDriveEnd + car.TimeToPosition(ride.StartR, ride.StartC);
                if (carToStart >= ride.TimeEnd)
                    continue;
                int startTime = Math.Max(carToStart, ride.TimeStart);
                if (startTime + ride.Distance >= ride.TimeEnd)
                    continue;

                hasBonus = (carToStart <= ride.TimeStart) ? 50 * Bonus : 0;
                if (bestRide == null)
                {
                    bestRide = ride;
                    bestStartTime = startTime;
                    bestHasBonus = hasBonus;
                }
                else if (startTime - hasBonus < bestStartTime - bestHasBonus)
                {
                    bestRide = ride;
                    bestStartTime = startTime;
                    bestHasBonus = hasBonus;
                }
            }
        }
    }

    class SolverByCar : Solver
    {
        private bool UseClosestRide { get; set; }

        public SolverByCar(bool useClosestRide)
        {
            this.UseClosestRide = useClosestRide;
        }

        public override void Solve()
        {
            foreach (Ride ride in Rides)
                ride.CalculateClosestRide(this.Rides);

            for (int carPos = 0; carPos < Vehicles.Count; carPos++)
            {
                Vehicle car = Vehicles[carPos];
                AllocateRidesToCar_StartEarliest(this.UseClosestRide, car, Rides, this.Bonus);
            }

            while (true)
            {
                if (TryCarsReallocate(this.UseClosestRide))
                    continue;

                if (TryCarsReallocate(!this.UseClosestRide))
                    continue;

                if (TryCarsPushRide())
                    continue;

                break;
            }
        }
    }

    class SolverByRideComplete : Solver
    {
        public override void Solve()
        {
            Rides.Sort(new Ride.CompareByStartTime());
            Vehicle.CompareByTimeDriveEnd carSortTimeDriveEnd = new Vehicle.CompareByTimeDriveEnd();

            while (Rides.Count > 0)
            {
                Vehicles.Sort(carSortTimeDriveEnd);

                Vehicle bestGlobalCar = null;
                int bestGlobalCompleteTime = int.MaxValue;
                Ride bestGlobalRide = null;

                foreach (Ride ride in Rides)
                {
                    // All other rides will start later
                    if (ride.TimeStart > bestGlobalCompleteTime)
                        break;

                    int rideTime = ride.Distance;

                    // Find fastest car for the ride
                    Vehicle bestCar;
                    int bestCompleteTime;
                    FindBestCarForRideComplete(ride, out bestCar, out bestCompleteTime);

                    // Check if valid
                    // Complete ride on time
                    if (bestCompleteTime >= ride.TimeEnd)
                        continue;

                    if (bestCompleteTime >= Steps)
                        continue;

                    if (bestGlobalCar == null)
                    {
                        bestGlobalCar = bestCar;
                        bestGlobalCompleteTime = bestCompleteTime;
                        bestGlobalRide = ride;
                    }
                    else if (bestCompleteTime < bestGlobalCompleteTime)
                    {
                        bestGlobalCar = bestCar;
                        bestGlobalCompleteTime = bestCompleteTime;
                        bestGlobalRide = ride;
                    }
                }

                if (bestGlobalRide == null)
                    return;

                // Add ride to car
                bestGlobalCar.AddRide(bestGlobalRide,
                    bestGlobalCompleteTime);

                // Remove ride from list
                for (int i = 0; i < Rides.Count; i++)
                    if (Rides[i].ID == bestGlobalRide.ID)
                    {
                        Rides.RemoveAt(i);
                        break;
                    }
            }
        }

        private void FindBestCarForRideComplete(Ride ride, out Vehicle bestCar, out int bestCompleteTime)
        {
            bestCar = null;
            bestCompleteTime = 0;
            int bestDriveTime = 0;

            foreach (Vehicle car in Vehicles)
            {
                int carDriveTime = car.TimeToPosition(ride.StartR, ride.StartC);
                int carToStart = car.TimeDriveEnd + carDriveTime;
                int completeTime = Math.Max(carToStart, ride.TimeStart) + ride.Distance;

                if (bestCar == null)
                {
                    bestCar = car;
                    bestCompleteTime = completeTime;
                    bestDriveTime = carDriveTime;
                }
                else if (completeTime < bestCompleteTime)
                {
                    bestCar = car;
                    bestCompleteTime = completeTime;
                    bestDriveTime = carDriveTime;
                }
                // Optional optimization - works in some cases
                else if ((completeTime == bestCompleteTime) && (carDriveTime < bestDriveTime))
                {
                    bestCar = car;
                    bestCompleteTime = completeTime;
                    bestDriveTime = carDriveTime;
                }
            }
        }
    }

    class SolverByRide : Solver
    {
        public override void Solve()
        {
            Rides.Sort(new Ride.CompareByStartTime());
            Vehicle.CompareByTimeDriveEnd carSortTimeDriveEnd = new Vehicle.CompareByTimeDriveEnd();

            while (Rides.Count > 0)
            {
                Ride ride = Rides[0];

                Rides.RemoveAt(0);

                // Find fastest car for the ride
                Vehicles.Sort(carSortTimeDriveEnd);
                Vehicle bestCar;
                int bestStartTime;
                FindBestCarForRide(ride, out bestCar, out bestStartTime);

                if (bestCar == null)
                    continue;

                // Check if valid
                // Complete ride on time
                int rideStartTime = Math.Max(ride.TimeStart, bestStartTime);
                if (rideStartTime + ride.Distance >= ride.TimeEnd)
                    continue;

                if (rideStartTime + ride.Distance >= Steps)
                    continue;

                // Add ride to car
                bestCar.AddRide(ride, rideStartTime + ride.Distance);
            }
        }

        private void FindBestCarForRide(Ride ride, out Vehicle bestCar, out int bestStartTime)
        {
            bestCar = null;
            bestStartTime = 0;

            foreach (Vehicle car in Vehicles)
            {
                int carToStart = car.TimeDriveEnd + car.TimeToPosition(ride.StartR, ride.StartC);
                if (carToStart + ride.Distance >= ride.TimeEnd)
                    continue;

                if (bestCar == null)
                {
                    bestCar = car;
                    bestStartTime = carToStart;
                }
                else if (carToStart < bestStartTime)
                {
                    bestCar = car;
                    bestStartTime = carToStart;
                }
            }
        }
    }
}
