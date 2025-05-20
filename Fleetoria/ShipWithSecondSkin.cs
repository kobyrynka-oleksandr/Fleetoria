using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fleetoria
{
    public class ShipWithSecondSkin : Ship
    {
        public ShipWithSecondSkin(int deckCount) : base(deckCount) { }

        protected override string SkinFolder => "Ship_skin_2";
    }
}
