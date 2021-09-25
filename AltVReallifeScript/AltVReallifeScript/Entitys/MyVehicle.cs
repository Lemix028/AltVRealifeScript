using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVReallifeScript.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static AltVReallifeScript.Utils.Enums;

namespace AltVReallifeScript.Entitys
{
    public class MyVehicle : Vehicle
    {
        public static float MAX_FUEL = 100.0f;
        public FuelTypes FuelType { get; set; }
        public float Fuel { get; set; }
        public MyVehicle(IntPtr nativePointer, ushort id) : base(nativePointer, id)
        {
            
        }

        public MyVehicle(uint model, Position position, Rotation rotation, FuelTypes fueltype = FuelTypes.None) : base(model, position, rotation)
        {
            FuelType = fueltype;
            //FuelType = FuelTypes.Diesel;
            Fuel = 0;
            //ManualEngineControl is buggy workaround clientside pedconfigflag 429 used
            ManualEngineControl = true;
            this.NumberplateText = "12345678"; // max 8 chars
        }
        public void Repair()
        {
            if (NetworkOwner != null)
            {
                Fuel = MAX_FUEL;
                NetworkOwner.Emit("ars:fixveh");
                PlayerEvents.SendNotification(NetworkOwner, "Fahrzeug repariert");
            }
        }

        public void ToggleEngine()
        {
            if(!EngineOn && FuelType != FuelTypes.None && Fuel == 0)
            {
                PlayerEvents.SendNotification(NetworkOwner, "Tank leer!");
            }
            ManualEngineControl = true;
            EngineOn = !EngineOn;
        }
        public void ToggleLock()
        {
            if(LockState == AltV.Net.Enums.VehicleLockState.None || LockState == AltV.Net.Enums.VehicleLockState.Unlocked)
            {
                LockState = AltV.Net.Enums.VehicleLockState.LockedCanBeDamaged;
            }
            else
            {
                LockState = AltV.Net.Enums.VehicleLockState.Unlocked;
            }
        }

       
    }
}
