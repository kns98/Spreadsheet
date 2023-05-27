using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroSpread
{
    public class CellChangeEvent
    {
        public CellCoordinates Coordinates { get; }
        public object Value { get; }

        public int Row => Coordinates.RowNumber;
        public int Column => Coordinates.ColNumber;

        public CellChangeEvent(CellCoordinates coordinates, object value)
        {
            Coordinates = coordinates;
            Value = value;
        }
    }


}
