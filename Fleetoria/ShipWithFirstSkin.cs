using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fleetoria
{
    public class ShipWithFirstSkin : Ship
    {
        public ShipWithFirstSkin(int deckCount) : base(deckCount) { }

        protected override string SkinFolder => "Ship_skin_1";
    }
}
