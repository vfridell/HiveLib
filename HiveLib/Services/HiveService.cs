using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models;
using HiveLib.ViewModels;

namespace HiveLib.Services
{
    public class HiveService
    {
        public static bool IsValidNotationString(string notation)
        {
            try
            {
                Move move;
                return NotationParser.TryParseNotation(notation, out move);
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
