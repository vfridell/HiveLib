using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models;
using HiveLib.ViewModels;
using PieceColor = HiveLib.Models.Pieces.PieceColor;
using AutoMapper;

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

        public static void Initialize()
        {
            Mapper.CreateMap<Board, BoardAnalysisDataVM>()
                  .ForMember(dest => dest.blackArticulationPoints, opt => opt.MapFrom(src => src.articulationPoints.Count(p => p.color == PieceColor.Black)))
                  .ForMember(dest => dest.whiteArticulationPoints, opt => opt.MapFrom(src => src.articulationPoints.Count(p => p.color == PieceColor.White)));
                  //.ForMember()
        }
    }
}
