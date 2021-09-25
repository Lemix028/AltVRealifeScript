using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVReallifeScript.Entitys;
using System;

namespace AltVReallifeScript.Factories
{
    class PlayerFactory : IEntityFactory<IPlayer>
    {
        public IPlayer Create(IntPtr entityPointer, ushort id)
        {
            return new MyPlayer(entityPointer, id);
        }
    }
}
