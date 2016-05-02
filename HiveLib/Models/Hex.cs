using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveLib.Models
{
    class Hex
    {
        internal Hex(int column, int row)
        {
            this.column = column;
            this.row = row;
        }

        internal int column { get; set; }
        internal int row { get; set; }
    }
}
