using System;
using System.Collections.Generic;
using System.Linq;

namespace Ships
{
    public enum Orientation { None, Horizontal, Vertical }

    public enum Unit { None, Submarine, Destroyer, Cruiser, Battleship, Carrier }

    public class UnitList : List<Unit>
    {
        public static IEnumerable<Unit> GenerateShips()
        {
            for (int i = 1; i <= 5; i++)
                for (int j = 0; j < (i > 2 ? 1 : 2); j++)
                    yield return (Unit)i;
        }

        public UnitList()
            : base(GenerateShips())
        { }

        public UnitList(IEnumerable<Unit> units)
            : base(units)
        { }
    }
}
