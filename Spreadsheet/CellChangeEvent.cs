using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroSpread
{
    public class CellChangeEvent
    {
        public int Row { get; }
        public int Column { get; }
        public object Value { get; }

        public CellChangeEvent(int row, int col, object value)
        {
            Row = row;
            Column = col;
            Value = value;
        }
    }
}
