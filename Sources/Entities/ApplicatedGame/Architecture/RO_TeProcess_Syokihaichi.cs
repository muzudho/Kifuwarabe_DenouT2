﻿using System.Runtime.CompilerServices;

namespace Grayscale.Kifuwarane.Entities.ApplicatedGame.Architecture
{
    public class RO_TeProcess_Syokihaichi : MoveImpl, IMove
    {
        public RO_TeProcess_Syokihaichi()
            : base(
                new RO_Star(Sengo.Empty, M201.Error, Kh185.n000_未設定),
                new RO_Star(Sengo.Empty, M201.Error, Kh185.n000_未設定),
                Ks14.H00_Null
            )
        {
        }

        
        /// <summary>
        /// ************************************************************************************************************************
        /// SFEN符号表記。
        /// ************************************************************************************************************************
        /// </summary>
        /// <returns></returns>
        public override string ToSfenText(
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            return "初期局面手です";
        }

    }
}