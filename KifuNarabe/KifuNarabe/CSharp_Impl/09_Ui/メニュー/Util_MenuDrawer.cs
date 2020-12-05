﻿
using Grayscale.KifuwaraneLib;
using Grayscale.KifuwaraneLib.L04_Common;
using Xenon.KifuNarabe.L07_Shape;

namespace Xenon.KifuNarabe.L09_Ui
{
    public abstract class Util_MenuDrawer
    {


        public static void DrawKomaokuri1(ref RequestForMain requestForMain, Shape_PnlTaikyoku shape_PnlTaikyoku, Kifu_Document kifuD, ILarabeLoggerTag logTag)
        {
            //------------------------------------------------------------
            // 駒・再描画
            //------------------------------------------------------------
            foreach (K40 koma in requestForMain.RequestRefresh_Komas)
            {
                Shape_BtnKoma btn_koma = Converter09.KomaToBtn(koma, shape_PnlTaikyoku);

                //if (K40Util.OnKoma((int)koma))
                //{
                Ui_02Action.Refresh_KomaLocation(koma, shape_PnlTaikyoku, kifuD, logTag);
                //}
            }
            requestForMain.RequestRefresh_Komas.Clear();
        }


        public static void DrawKomaokuri2(ref RequestForMain requestForMain, Shape_PnlTaikyoku shape_PnlTaikyoku, Kifu_Document kifuD, ILarabeLoggerTag logTag)
        {
            //------------------------------
            // メナス
            //------------------------------
            FlowB_1TumamitaiKoma.Menace(ref requestForMain, shape_PnlTaikyoku, kifuD, logTag);
        }


    }
}
