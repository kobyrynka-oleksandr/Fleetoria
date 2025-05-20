using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fleetoria
{
    public static class ShipFactory
    {
        public static Ship CreateShip(string skin, int count)
        {
            switch (skin)
            {
                case "Ship_skin_1":
                    return new ShipWithFirstSkin(count);
                case "Ship_skin_2":
                    return new ShipWithSecondSkin(count);
                default:
                    throw new ArgumentException("Unknown ship skin");
            }
        }
    }
}
