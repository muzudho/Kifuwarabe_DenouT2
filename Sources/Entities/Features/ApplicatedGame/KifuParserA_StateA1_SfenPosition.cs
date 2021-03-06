﻿using System;
using Grayscale.Kifuwarane.Entities.ApplicatedGame.Architecture;
using Grayscale.Kifuwarane.Entities.Logging;

namespace Grayscale.Kifuwarane.Entities.ApplicatedGame
{
    /// <summary>
    /// 「position」を読込みました。
    /// </summary>
    public class KifuParserA_StateA1_SfenPosition : IKifuParserAState
    {


        public static KifuParserA_StateA1_SfenPosition GetInstance()
        {
            if (null == instance)
            {
                instance = new KifuParserA_StateA1_SfenPosition();
            }

            return instance;
        }
        private static KifuParserA_StateA1_SfenPosition instance;


        private KifuParserA_StateA1_SfenPosition()
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

            if (inputLine.StartsWith("startpos"))
            {
                // 平手の初期配置です。
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                Logging.Logger.Trace("（＾△＾）「{ inputLine }」vs【{ this.GetType().Name }】　：　平手のようなんだぜ☆");

                inputLine = inputLine.Substring("startpos".Length);
                inputLine = inputLine.Trim();


                SyokiHaichi.ToHirate(kifuD);

                nextState = KifuParserA_StateA1a_SfenStartpos.GetInstance();
            }
            else
            {
                Logging.Logger.Trace("（＾△＾）「{ inputLine }」vs【{ this.GetType().Name }】　：　局面の指定のようなんだぜ☆");
                nextState = KifuParserA_StateA1b_SfenLnsgkgsnl.GetInstance();
            }

            return inputLine;
        }

    }
}
