using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVReallifeScript.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltVReallifeScript.Classes
{
    public class Colshapes : IScript
    {
        static bool IsLoaded = false;
        public static void LoadColshapes()
        {
            if (IsLoaded)
                return;

            //Create colshapes
            CreateColshape(new Position(798, -274, 65.5f), 3, 1, "test", 255, 0, 255, 100);
            CreateColshape(new Position(-813.38904f, -183.4945f, 37.0f), 1, 1, "barber", 255, 255, 255, 100);

            
            IsLoaded = true;
            if (Alt.GetColShapesArray().Length == 1)
                Alt.Log($"{Alt.GetColShapesArray().Length} colshape are loaded!");
            else
                Alt.Log($"{Alt.GetColShapesArray().Length} colshapes are loaded!"); 
        }
        public static void DrawMarkes(MyPlayer player)
        {
            foreach(var entry in Alt.GetColShapesArray())
            {
                entry.Value.GetData("radius", out string radius);
                entry.Value.GetData("height", out string height);
                entry.Value.GetData("r", out string r);
                entry.Value.GetData("g", out string g);
                entry.Value.GetData("b", out string b);
                entry.Value.GetData("alpha", out string alpha);
                player.Emit("ars:DrawColshapeMarker", entry.Value.Position.X, entry.Value.Position.Y, entry.Value.Position.Z, radius, height, r, g, b, alpha);
            }
        }

        private static void CreateColshape(Position pos, float radius, float height, string name, int r, int g, int b, int alpha)
        {
            var colshape = Alt.CreateColShapeSphere(new Position(pos.X, pos.Y, pos.Z), radius);
            colshape.SetData("name", name);
            colshape.SetData("radius", radius.ToString());
            colshape.SetData("height", height.ToString());
            colshape.SetData("r", r.ToString());
            colshape.SetData("g", g.ToString());
            colshape.SetData("b", b.ToString());
            colshape.SetData("alpha", alpha.ToString());
        }


        //Colsphape handler
        [ScriptEvent(ScriptEventType.ColShape)]
        public void OnColshape(IColShape shape, IEntity entity, bool state)
        {
            shape.GetData<string>("name", out string name);
            switch (name)
            {
                case "test":
                    if (entity is MyPlayer player)
                    {
                        if (state)
                        {
                            player.Emit("ars:notify", "Welcome");
                        }
                        else
                        {
                            player.Emit("ars:notify", "Byeee");
                        }
                    }
                    else if (entity is MyVehicle vehicle)
                    {
                        if (state) 
                        {

                            vehicle.PrimaryColorRgb = new Rgba(255, 0, 0, 255);
                            if(vehicle.Driver != null)
                                ((MyPlayer)vehicle.Driver).Emit("ars:notify", "Hi car");
                        }
                        else
                        {
                            vehicle.PrimaryColorRgb = new Rgba(0, 0, 255, 255);
                            if (vehicle.Driver != null)
                                ((MyPlayer)vehicle.Driver).Emit("ars:notify", "bye car");
                        }
                    }
                    break;
                case "barber":
                    if (entity is MyPlayer player_barber)
                    {
                            player_barber.Emit("ars:ToggleBarberInteractionText");
                    }
                    break;
            }

          
        }

    }
}
