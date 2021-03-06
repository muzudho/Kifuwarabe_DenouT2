﻿using System.Runtime.CompilerServices;
using Grayscale.Kifuwarane.Entities.ApplicatedGame.Architecture;
using Grayscale.Kifuwarane.Entities.Logging;

namespace Grayscale.Kifuwarane.Entities.ApplicatedGame
{
    /// <summary>
    /// 指し手。
    /// </summary>
    public interface IMove : IKomaPos
    {

        /// <summary>
        /// ------------------------------------------------------------------------------------------------------------------------
        /// 先後、升、配役
        /// ------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        RO_Star SrcStar { get; }

        /// <summary>
        /// ------------------------------------------------------------------------------------------------------------------------
        /// あれば、取った駒の種類。
        /// ------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        PieceType TottaSyurui { get; }

        /// <summary>
        /// ************************************************************************************************************************
        /// SFEN符号表記。（取った駒付き）
        /// ************************************************************************************************************************
        /// </summary>
        /// <returns></returns>
        string ToSfenText_TottaKoma();

        bool isEnableSfen();

        /// <summary>
        /// ************************************************************************************************************************
        /// SFEN符号表記。
        /// ************************************************************************************************************************
        /// </summary>
        /// <returns></returns>
        string ToSfenText(
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
        );

        /// <summary>
        /// ************************************************************************************************************************
        /// 元位置。
        /// ************************************************************************************************************************
        /// </summary>
        /// <returns></returns>
        IMove Src();

        /// <summary>
        /// “打” ＜アクション時＞
        /// </summary>
        /// <returns></returns>
        bool IsDaAction{ get; }

        
        /// <summary>
        /// 成った
        /// </summary>
        /// <returns></returns>
        bool IsNatta_Process{ get; }

    }
}
