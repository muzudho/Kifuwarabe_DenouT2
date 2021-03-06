﻿using System.Runtime.CompilerServices;
using System.Text;
using Grayscale.Kifuwarane.Entities.ApplicatedGame;
using Grayscale.Kifuwarane.Entities.ApplicatedGame.Architecture;
using Grayscale.Kifuwarane.Entities.Logging;
using Grayscale.Kifuwarane.Entities.Performer;
using Grayscale.Kifuwarane.Gui.L02_DammyConsole;
using Grayscale.Kifuwarane.Gui.L07_Shape;
using Grayscale.Kifuwarane.Gui.L08_Server;

namespace Grayscale.Kifuwarane.Gui.L09_Ui
{

    /// <summary>
    /// UI(*1)に関する操作です。
    /// 
    ///         *1…目に触れ、入力のために触る部分です。ユーザーインターフェース。ボタンやラベルなど。
    /// 
    /// </summary>
    public abstract class Ui_01Menu
    {

        /// <summary>
        /// [戻る]ボタン
        /// </summary>
        public static bool Modoru(
            Shape_PnlTaikyoku shape_PnlTaikyoku,
            TreeDocument kifuD, out string backedInputText)
        {
            bool successful = false;
            backedInputText = DammyConsole.DefaultDammyConsole.ReadLine1().Trim();

            //------------------------------
            // 棋譜から１手削ります
            //------------------------------
            //Logger.Trace($"ポップカレントする前　：　kifuD.Old_KomaDoors.CountPathNodes()=[{ kifuD.CountTeme(kifuD.Current8) }]");
            IKifuElement removeeLeaf = kifuD.Current8;

            if (removeeLeaf is TreeRoot6)
            {
                goto gt_EndMethod;
            }


            //------------------------------
            // 符号
            //------------------------------
            //
            // 移動前と、移動後では、駒が変わっていることがあります。
            // 例えば、「▲２二角成」なら　移動前は角、移動後は馬です。
            //
            // そこで戻るボタンでは、移動前の駒に従って、「進んできた動きとは逆の動き」を行います。

            string fugoJStr = JFugoCreator15Array.ItemMethods[(int)Haiyaku184Array.Syurui(removeeLeaf.TeProcess.SrcStar.Haiyaku)](removeeLeaf.TeProcess, kifuD).ToText_UseDou(removeeLeaf);
            //MessageBox.Show($"戻る符号＝{ fugoJStr}", "デバッグ");

            // 入力欄
            backedInputText = $"{fugoJStr} {backedInputText}";
            // 符号表示
            shape_PnlTaikyoku.SetFugo(fugoJStr);



            //------------------------------
            // 前の手に戻します
            //------------------------------
            Piece40 movedKoma;
            //K40 tottaKoma = K40.Error;
            Piece40 underKoma = Piece40.Error;
            bool isBack = true;

            //MessageBox.Show($"戻る符号＝{ removeeLeaf.TeProcess.ToSfenText()}", "デバッグ");

            KifuIO.Ittesasi3(
                removeeLeaf.TeProcess,
                kifuD,
                isBack,
                out movedKoma,
                //out tottaKoma,
                out underKoma
                );

            //IKifuElement removedLeaf = kifuD.PopCurrent1();
            ////Logger.Trace($"ポップカレントした後　：　kifuD.Old_KomaDoors.CountPathNodes()=[{ kifuD.CountTeme(kifuD.Current8) }]");

            Shape_BtnKoma btn_movedKoma = Converter09.KomaToBtn(movedKoma, shape_PnlTaikyoku);
            //Shape_BtnKoma btn_tottaKoma = Converter09.KomaToBtn(tottaKoma, shape_PnlTaikyoku);
            Shape_BtnKoma btn_underKoma = Converter09.KomaToBtn(underKoma, shape_PnlTaikyoku);
            //------------------------------------------------------------
            // 駒・再描画
            //------------------------------------------------------------
            if (null != btn_movedKoma)
            {
                Ui_02Action.Refresh_KomaLocation(btn_movedKoma.Koma, shape_PnlTaikyoku, kifuD);
            }

            //if (null != btn_tottaKoma)
            //{
            //    Ui_02Action.Refresh_KomaLocation(btn_tottaKoma.Koma, shape_PnlTaikyoku, kifuD);
            //}

            if (null != btn_underKoma)
            {
                Ui_02Action.Refresh_KomaLocation(btn_underKoma.Koma, shape_PnlTaikyoku, kifuD);
            }



            //------------------------------
            // チェンジターン
            //------------------------------
            ShogiEngineService.Message_ChangeTurn(kifuD);//[戻る]ボタンを押したあと

            successful = true;

            //------------------------------
            // メナス
            //------------------------------
            RequestForMain requestForMain = new RequestForMain();
            FlowB_1TumamitaiKoma.Menace(ref requestForMain, shape_PnlTaikyoku, kifuD);


        gt_EndMethod:
            return successful;
        }


        public delegate void DELEGATE_DrawKomaokuri(ref RequestForMain requestForMain, Shape_PnlTaikyoku shape_PnlTaikyoku, TreeDocument kifuD);


        /// <summary>
        /// [コマ送り]ボタン
        /// </summary>
        public static bool Komaokuri_GUI(
            ref RequestForMain requestForMain,
            Shape_PnlTaikyoku shape_PnlTaikyoku,
            TreeDocument kifuD,
            DELEGATE_DrawKomaokuri delegate_DrawKomaokuri1,
            DELEGATE_DrawKomaokuri delegate_DrawKomaokuri2,
            string hint
            ,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {

            //
            // 次のような状況を想定しています。
            //
            //      「次の一手は、もう将棋エンジンに溜められていて、ReadLine() で取り出せる」
            //

            Ui_01MenuB ui_01MenuB = new Ui_01MenuB(requestForMain, shape_PnlTaikyoku);
            bool toBreak = false;

            Logger.Trace( $"[コマ送り]ボタンが押されて　一手進む　実行☆　：　呼出箇所＝{ memberName }.{ sourceFilePath }.{ sourceLineNumber }");
            bool successful = ui_01MenuB.ReadLine_TuginoItteSusumu(kifuD, ref toBreak, $"{hint}:コマ送りGUI");

            // 再描画1
            delegate_DrawKomaokuri1(ref requestForMain, shape_PnlTaikyoku, kifuD);


            //------------------------------
            // チェンジ・ターン
            //------------------------------
            if (requestForMain.ChangedTurn)
            {
                ShogiEngineService.Message_ChangeTurn(kifuD);
            }


            // 再描画2
            delegate_DrawKomaokuri2(ref requestForMain, shape_PnlTaikyoku, kifuD);

            return successful;
        }







        /// <summary>
        /// 将棋盤の上の駒を、全て駒台に移動します。（クリアー）
        /// </summary>
        public static void ClearKifu(TreeDocument kifuD)
        {
            kifuD.ClearA();// 棋譜を空っぽにします。

            int lastTeme = kifuD.CountTeme(kifuD.Current8);

            IKifuElement dammyNode5 = kifuD.ElementAt8(lastTeme);
            PositionKomaHouse house5 = dammyNode5.KomaHouse;

            Piece40 k40;

            // 先手
            k40 = Piece40.K1;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen01, PieceType.K, "クリアー棋譜")); //先手王
            k40 = Piece40.K2;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go01, PieceType.K, "クリアー棋譜")); //後手王

            k40 = Piece40.R_1;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen02, PieceType.R, "クリアー棋譜")); //飛
            k40 = Piece40.R_2;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go02, PieceType.R, "クリアー棋譜"));

            k40 = Piece40.B_1;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen03, PieceType.B, "クリアー棋譜")); //角
            k40 = Piece40.B_2;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go03, PieceType.B, "クリアー棋譜"));

            k40 = Piece40.G_1;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen04, PieceType.G, "クリアー棋譜")); //金
            k40 = Piece40.G_2;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen05, PieceType.G, "クリアー棋譜"));
            k40 = Piece40.G_3;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go04, PieceType.G, "クリアー棋譜"));
            k40 = Piece40.G_4;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go05, PieceType.G, "クリアー棋譜"));

            k40 = Piece40.S_1;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen06, PieceType.S, "クリアー棋譜")); //銀
            k40 = Piece40.S_2;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen07, PieceType.S, "クリアー棋譜"));
            k40 = Piece40.S_3;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go06, PieceType.S, "クリアー棋譜"));
            k40 = Piece40.S_4;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go07, PieceType.S, "クリアー棋譜"));

            k40 = Piece40.N_1;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen08, PieceType.N, "クリアー棋譜")); //桂
            k40 = Piece40.N_2;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen09, PieceType.N, "クリアー棋譜"));
            k40 = Piece40.N_3;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go08, PieceType.N, "クリアー棋譜"));
            k40 = Piece40.N_4;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go09, PieceType.N, "クリアー棋譜"));

            k40 = Piece40.L_1;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen10, PieceType.L, "クリアー棋譜")); //香
            k40 = Piece40.L_2;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen11, PieceType.L, "クリアー棋譜"));
            k40 = Piece40.L_3;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go10, PieceType.L, "クリアー棋譜"));
            k40 = Piece40.L_4;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go11, PieceType.L, "クリアー棋譜"));

            k40 = Piece40.P_1;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen12, PieceType.P, "クリアー棋譜")); //歩
            k40 = Piece40.P_2;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen13, PieceType.P, "クリアー棋譜"));
            k40 = Piece40.P_3;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen14, PieceType.P, "クリアー棋譜"));
            k40 = Piece40.P_4;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen15, PieceType.P, "クリアー棋譜"));
            k40 = Piece40.P_5;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen16, PieceType.P, "クリアー棋譜"));
            k40 = Piece40.P_6;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen17, PieceType.P, "クリアー棋譜"));
            k40 = Piece40.P_7;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen18, PieceType.P, "クリアー棋譜"));
            k40 = Piece40.P_8;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen19, PieceType.P, "クリアー棋譜"));
            k40 = Piece40.P_9;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Sente, M201.sen20, PieceType.P, "クリアー棋譜"));

            k40 = Piece40.P_10;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go12, PieceType.P, "クリアー棋譜")); //歩
            k40 = Piece40.P_11;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go13, PieceType.P, "クリアー棋譜"));
            k40 = Piece40.P_12;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go14, PieceType.P, "クリアー棋譜"));
            k40 = Piece40.P_13;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go15, PieceType.P, "クリアー棋譜"));
            k40 = Piece40.P_14;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go16, PieceType.P, "クリアー棋譜"));
            k40 = Piece40.P_15;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go17, PieceType.P, "クリアー棋譜"));
            k40 = Piece40.P_16;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go18, PieceType.P, "クリアー棋譜"));
            k40 = Piece40.P_17;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go19, PieceType.P, "クリアー棋譜"));
            k40 = Piece40.P_18;
            house5.SetKomaPos(kifuD, k40, house5.KomaPosAt(k40).Next(Sengo.Gote, M201.go20, PieceType.P, "クリアー棋譜"));


            IKifuElement dammyNode6 = kifuD.ElementAt8(kifuD.Root7_Teme);
            PositionKomaHouse house7 = dammyNode6.KomaHouse;

            house7.SetStartpos("9/9/9/9/9/9/9/9/9 b K1R1B1G2S2N2L2P9 k1r1b1g2s2n2l2p9 1");
        }

        /// <summary>
        /// HTML出力。（これは作者のホームページ用に書かれています）
        /// </summary>
        public static string CreateHtml(TreeDocument kifuD1)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<div style=\"position:relative; left:0px; top:0px; border:solid 1px black; width:250px; height:180px;\">");

            // 後手の持ち駒
            sb.AppendLine("    <div style=\"position:absolute; left:0px; top:2px; width:30px;\">");
            sb.AppendLine("        △後手");
            sb.AppendLine("        <div style=\"margin-top:10px; width:30px;\">");
            sb.Append("            ");

            IKifuElement dammyNode6 = kifuD1.ElementAt8(kifuD1.CountTeme(kifuD1.Current8));
            PositionKomaHouse house7 = dammyNode6.KomaHouse;

            house7.Foreach_Items(kifuD1, (TreeDocument kifuD2, RO_KomaPos koma, ref bool toBreak) =>
            {
                if (M201Util.GetOkiba(koma.Star.Masu) == Okiba.Gote_Komadai)
                {
                    sb.Append(KomaSyurui14Array.Fugo[(int)Haiyaku184Array.Syurui(koma.Star.Haiyaku)]);
                }
            });

            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");

            // 将棋盤
            sb.AppendLine("    <div style=\"position:absolute; left:30px; top:2px; width:182px;\">");
            sb.AppendLine("        <table>");
            for (int dan = 1; dan <= 9; dan++)
            {
                sb.Append("        <tr>");
                for (int suji = 9; 1 <= suji; suji--)
                {
                    bool isSpace = true;

                    IKifuElement dammyNode8 = kifuD1.ElementAt8(kifuD1.CountTeme(kifuD1.Current8));
                    PositionKomaHouse house8 = dammyNode8.KomaHouse;

                    house8.Foreach_Items(kifuD1, (TreeDocument kifuD2, RO_KomaPos koma, ref bool toBreak) =>
                    {
                        if (
                            M201Util.GetOkiba(koma.Star.Masu) == Okiba.ShogiBan //盤上
                            && Mh201Util.MasuToSuji(koma.Star.Masu) == suji
                            && Mh201Util.MasuToDan(koma.Star.Masu) == dan
                        )
                        {
                            if (Sengo.Gote == koma.Star.Sengo)
                            {
                                sb.Append("<td><span class=\"koma2x\">");
                                sb.Append(KomaSyurui14Array.Fugo[(int)Haiyaku184Array.Syurui(koma.Star.Haiyaku)]);
                                sb.Append("</span></td>");
                                isSpace = false;
                            }
                            else
                            {
                                sb.Append("<td><span class=\"koma1x\">");
                                sb.Append(KomaSyurui14Array.Fugo[(int)Haiyaku184Array.Syurui(koma.Star.Haiyaku)]);
                                sb.Append("</span></td>");
                                isSpace = false;
                            }
                        }
                    });

                    if (isSpace)
                    {
                        sb.Append("<td>　</td>");
                    }
                }

                sb.AppendLine("</tr>");
            }
            sb.AppendLine("        </table>");
            sb.AppendLine("    </div>");

            // 先手の持ち駒
            sb.AppendLine("    <div style=\"position:absolute; left:215px; top:2px; width:30px;\">");
            sb.AppendLine("        ▲先手");
            sb.AppendLine("        <div style=\"margin-top:10px; width:30px;\">");
            sb.Append("            ");

            IKifuElement dammyNode7 = kifuD1.ElementAt8(kifuD1.CountTeme(kifuD1.Current8));
            PositionKomaHouse house9 = dammyNode7.KomaHouse;

            house9.Foreach_Items(kifuD1, (TreeDocument kifuD2, RO_KomaPos koma, ref bool toBreak) =>
            {
                if (M201Util.GetOkiba(koma.Star.Masu) == Okiba.Sente_Komadai)
                {
                    sb.Append(KomaSyurui14Array.Fugo[(int)Haiyaku184Array.Syurui(koma.Star.Haiyaku)]);
                }
            });

            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");

            //
            sb.AppendLine("</div>");

            return sb.ToString();
        }




        /*
        /// <summary>
        /// 棋譜最後の駒の成／不成を切り替えます。
        /// </summary>
        public static void Nari_KifuLast(Shape_BtnKoma btnKoma, Shape_PnlTaikyoku shape_PnlTaikyoku, Kifu kifu)
        {
            TeProcess last;
            kifu.PopLast(out last);


            if (null != last)
            {
                // 最後の符号の成／不成も変えます。

                if (kifu.Kyokumen.KomaDoors[btnKoma.Handle].Nari)
                {
                    //------------------------------
                    // 成→不成
                    //------------------------------

                    //----------
                    // 棋譜（成ったとき）
                    //----------
                    TeProcess process = new TeProcess(
                        last.Sengo,
                        last.Okiba,
                        last.SrcOkiba,
                        last.Suji,
                        last.Dan,
                        last.SrcSuji,
                        last.SrcDan,
                        kifu.Kyokumen.KomaDoors[btnKoma.Handle].ToFunariCase(),//不成
                        last.SrcSyurui,
                        last.TottaKoma
                        );
                    kifu.Add(process);

                    //----------
                    // 駒
                    //----------
                    kifu.Kyokumen.KomaDoors[btnKoma.Handle] = KomaPos.Create(
                        kifu.Kyokumen.KomaDoors[btnKoma.Handle].Sengo,
                        kifu.Kyokumen.KomaDoors[btnKoma.Handle].Okiba,
                        kifu.Kyokumen.KomaDoors[btnKoma.Handle].Suji,
                        kifu.Kyokumen.KomaDoors[btnKoma.Handle].Dan,
                        kifu.Kyokumen.KomaDoors[btnKoma.Handle].ToFunariCase()//不成
                        );

                }
                else
                {
                    //------------------------------
                    // 不成→成
                    //------------------------------

                    //----------
                    // 棋譜（成らなかったとき）
                    //----------
                    TeProcess process = new TeProcess(
                        last.Sengo,
                        last.Okiba,
                        last.SrcOkiba,
                        last.Suji,
                        last.Dan,
                        last.SrcSuji,
                        last.SrcDan,
                        kifu.Kyokumen.KomaDoors[btnKoma.Handle].ToNariCase(),//成り
                        last.SrcSyurui,
                        last.TottaKoma
                        );
                    kifu.Add(process);

                    //----------
                    // 駒
                    //----------
                    kifu.Kyokumen.KomaDoors[btnKoma.Handle] = KomaPos.Create(
                        kifu.Kyokumen.KomaDoors[btnKoma.Handle].Sengo,
                        kifu.Kyokumen.KomaDoors[btnKoma.Handle].Okiba,
                        kifu.Kyokumen.KomaDoors[btnKoma.Handle].Suji,
                        kifu.Kyokumen.KomaDoors[btnKoma.Handle].Dan,
                        kifu.Kyokumen.KomaDoors[btnKoma.Handle].ToNariCase()//成り
                        );

                }
            }


            //----------
            // 駒の選択解除
            //----------
            btnKoma.Deselect();
        }
         */

    }


}
