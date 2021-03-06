﻿using Grayscale.Kifuwarane.Entities.ApplicatedGame.Architecture;
using Grayscale.Kifuwarane.Entities.Logging;
using Grayscale.Kifuwarane.Gui.L07_Shape;

namespace Grayscale.Kifuwarane.Gui.L09_Ui
{
    public abstract class Util_MenuDrawer
    {


        public static void DrawKomaokuri1(ref RequestForMain requestForMain, Shape_PnlTaikyoku shape_PnlTaikyoku, TreeDocument kifuD)
        {
            //------------------------------------------------------------
            // 駒・再描画
            //------------------------------------------------------------
            foreach (Piece40 koma in requestForMain.RequestRefresh_Komas)
            {
                Shape_BtnKoma btn_koma = Converter09.KomaToBtn(koma, shape_PnlTaikyoku);

                //if (K40Util.OnKoma((int)koma))
                //{
                Ui_02Action.Refresh_KomaLocation(koma, shape_PnlTaikyoku, kifuD);
                //}
            }
            requestForMain.RequestRefresh_Komas.Clear();
        }


        public static void DrawKomaokuri2(ref RequestForMain requestForMain, Shape_PnlTaikyoku shape_PnlTaikyoku, TreeDocument kifuD)
        {
            //------------------------------
            // メナス
            //------------------------------
            FlowB_1TumamitaiKoma.Menace(ref requestForMain, shape_PnlTaikyoku, kifuD);
        }


    }
}
