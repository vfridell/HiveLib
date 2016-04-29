using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveLib.Models
{
    class HumanReadableMove
    {
        public static bool IsValidString(string notation)
        {
            throw new NotImplementedException();
        }

        public static bool IsLegalMove(string notation, Board board)
        {
            throw new NotImplementedException();
        }

        public string Notation { get; set; }

        public HumanReadableMove(string notation)
        {
            Notation = notation;
        }

    }
}
