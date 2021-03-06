﻿using System.Windows.Forms;
using Grayscale.Kifuwarane.Entities.ApplicatedGame.Architecture;
using Grayscale.Kifuwarane.Entities.Logging;
using Grayscale.Kifuwarane.Gui.L07_Shape;
using Grayscale.Kifuwarane.Gui.L08_Server;

namespace Grayscale.Kifuwarane.Gui.L09_Ui
{

    /// <summary>
    /// メニューのうち、ループ処理をするものを、ここに書きます。
    /// </summary>
    public abstract class Ui_01MenuLoop
    {
        /// <summary>
        /// [再生]ボタン
        /// </summary>
        public static void Saisei(
            Ui_PnlMain ui_PnlMain,
            Shape_PnlTaikyoku shape_PnlTaikyoku,
            TreeDocument kifuD
            )
        {
            RequestForMain requestForMain = new RequestForMain();

            //
            // 次のような状況を想定しています。
            //
            //      「次の一手は、もう将棋エンジンに溜められていて、ReadLine() で取り出せる」
            //

            Ui_01MenuB ui_01MenuB = new Ui_01MenuB(requestForMain, shape_PnlTaikyoku);


            // コマ送りに成功している間、コマ送りし続けます。
            bool toBreak = false;

            while (ui_01MenuB.ReadLine_TuginoItteSusumu(kifuD, ref toBreak, "再生ボタン") && !toBreak)
            {
                // 他のアプリが固まらないようにします。
                Application.DoEvents();

                // 早すぎると描画されないので、ウェイトを入れます。
                System.Threading.Thread.Sleep(45);


                //------------------------------------------------------------
                // 駒・再描画
                //------------------------------------------------------------
                foreach (Piece40 koma in requestForMain.RequestRefresh_Komas)
                {
                    Shape_BtnKoma btn_koma = Converter09.KomaToBtn(koma, shape_PnlTaikyoku);

                    if (K40Util.OnKoma( (int)koma))
                    {
                        Ui_02Action.Refresh_KomaLocation(koma, shape_PnlTaikyoku, kifuD);
                    }
                }
                requestForMain.RequestRefresh_Komas.Clear();

                //------------------------------
                // チェンジ・ターン
                //------------------------------
                if (requestForMain.ChangedTurn)
                {
                    ShogiEngineService.Message_ChangeTurn(kifuD);
                }

                //------------------------------
                // メナス
                //------------------------------
                FlowB_1TumamitaiKoma.Menace(ref requestForMain, shape_PnlTaikyoku, kifuD);


                //------------------------------------------------------------
                // パネル
                //------------------------------------------------------------
                ui_PnlMain.Response(requestForMain);


                //
                //
                //  ここで、次の一手が、もう将棋エンジンに溜められているものとして、処理を進めます。
                //
                //

            }
        }

    }
}
