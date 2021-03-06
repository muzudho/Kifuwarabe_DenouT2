﻿using Grayscale.Kifuwarane.Entities.ApplicatedGame.Architecture;
using Grayscale.Kifuwarane.Entities.Sfen;

namespace Grayscale.Kifuwarane.Entities.ApplicatedGame
{
    /// <summary>
    /// 指定局面から始める配置です。
    /// 
    /// 「lnsgkgsnl/1r5b1/ppppppppp/9/9/6P2/PPPPPP1PP/1B5R1/LNSGKGSNL w - 1」といった文字の読込み
    /// </summary>
    public class KifuParserA_StateA1b_SfenLnsgkgsnl : IKifuParserAState
    {


        public static KifuParserA_StateA1b_SfenLnsgkgsnl GetInstance()
        {
            if (null == instance)
            {
                instance = new KifuParserA_StateA1b_SfenLnsgkgsnl();
            }

            return instance;
        }
        private static KifuParserA_StateA1b_SfenLnsgkgsnl instance;



        private KifuParserA_StateA1b_SfenLnsgkgsnl()
        {
        }


        public string Execute(
            string inputLine,
            TreeDocument kifuD,
            out IKifuParserAState nextState,
            IKifuParserA owner,
            ref bool toBreak,
            string hint
            )
        {
            nextState = this;

            string restText;
            SfenStartpos sfenStartpos;

            if (!TuginoItte_Sfen.GetDataStartpos_FromText(inputLine, out restText, out sfenStartpos))
            {
                // 解析に失敗しました。
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                toBreak = true;
            }
            else
            {
                inputLine = restText;

                owner.OnRefreshShiteiKyokumen(
                    kifuD,
                    ref inputLine,
                    sfenStartpos
                    );

                nextState = KifuParserA_StateA2_SfenMoves.GetInstance();
            }

            return inputLine;
        }

    }
}
