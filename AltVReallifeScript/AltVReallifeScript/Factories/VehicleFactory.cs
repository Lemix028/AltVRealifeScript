using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVReallifeScript.Entitys;
using System;

namespace AltVReallifeScript.Factories
{
    class VehicleFactory : IEntityFactory<IVehicle>
    {
        public IVehicle Create(IntPtr entityPointer, ushort id)
        {
            return new MyVehicle(entityPointer, id);
        }
    }
}
