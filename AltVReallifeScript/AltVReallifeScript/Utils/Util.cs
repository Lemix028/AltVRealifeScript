using AltV.Net;
using AltV.Net.Data;
using AltVReallifeScript.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltVReallifeScript.Utils
{
    public static class Util 
    {

        public static double GetDistance(Position pos1, Position pos2)
        {
            Position connectionvector = new Position();
            double distance;
            connectionvector.X = pos1.X - pos2.X;
            connectionvector.Y = pos1.Y - pos2.Y;
            connectionvector.Z = pos1.Z - pos2.Z;

            distance = Math.Sqrt((connectionvector.X * connectionvector.X) + (connectionvector.Y * connectionvector.Y) + (connectionvector.Z * connectionvector.Z));
            return distance;
        }
        public static MyVehicle getNearestVehicle(MyPlayer player, double radius = -1)
        {
            if (Alt.GetAllVehicles().Count == 0)
                return null;
            if(radius <= 0)
            {
                Tuple<MyVehicle, double> current = new Tuple<MyVehicle, double>(null, -1);
                foreach (var entry in Alt.GetAllVehicles())
                {
                    if (current.Item2 == -1)
                    {
                        current = new Tuple<MyVehicle, double>((MyVehicle)entry, Util.GetDistance(player.Position, entry.Position));
                    }
                    else
                    {
                        if (current.Item2 > Util.GetDistance(player.Position, entry.Position))
                        {
                            current = new Tuple<MyVehicle, double>((MyVehicle)entry, Util.GetDistance(player.Position, entry.Position));
                        }
                    }
                }
                return current.Item1;
            }
            else
            {
                Tuple<MyVehicle, double> current = new Tuple<MyVehicle, double>(null, -1);
                foreach (var entry in Alt.GetAllVehicles())
                {
                    if (current.Item2 == -1 && Util.GetDistance(player.Position, entry.Position) <= radius)
                    {
                        current = new Tuple<MyVehicle, double>((MyVehicle)entry, Util.GetDistance(player.Position, entry.Position));
                    }
                    else
                    {
                        if (current.Item2 > Util.GetDistance(player.Position, entry.Position) && Util.GetDistance(player.Position, entry.Position) <= radius)
                        {
                            current = new Tuple<MyVehicle, double>((MyVehicle)entry, Util.GetDistance(player.Position, entry.Position));
                        }
                    }
                }
                return current.Item1;
            }
           
        }

    }
   
}
