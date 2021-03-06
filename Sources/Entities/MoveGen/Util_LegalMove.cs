﻿using System.Collections.Generic;
using System.Text;
using Grayscale.Kifuwarane.Entities.ApplicatedGame;
using Grayscale.Kifuwarane.Entities.ApplicatedGame.Architecture;
using Grayscale.Kifuwarane.Entities.Configuration;
using Grayscale.Kifuwarane.Entities.Logging;

namespace Grayscale.Kifuwarane.Entities.MoveGen
{
    public abstract class Util_LegalMove
    {
        /// <summary>
        /// 王手されていないか、調べます。
        /// </summary>
        public static bool Is_Mate(
            TreeNode6 siteiNode,
            Sengo selfSengo,
            IKifuElement node1,//調べたい局面
            StringBuilder sbGohosyu
            )
        {

            sbGohosyu.AppendLine("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓自玉王手確認");
            KomaAndMasusDictionary komaAndMove_Enemy;// 相手の駒の移動可能範囲
            Util_LegalMove.GetAvailableMove(
                siteiNode,
                GameTranslator.AlternateSengo(selfSengo), out komaAndMove_Enemy, sbGohosyu);

            // 自分の王の座標
            M201 kingMasu;

            if (Sengo.Gote == selfSengo)
            {
                // 後手
                kingMasu = node1.KomaHouse.KomaPosAt(Piece40.K2).Star.Masu;
                sbGohosyu.AppendLine($"後手王＝[{ kingMasu }]");
            }
            else
            {
                // 先手
                kingMasu = node1.KomaHouse.KomaPosAt(Piece40.K1).Star.Masu;
                sbGohosyu.AppendLine($"先手王＝[{ kingMasu }]");
            }


            // 相手の利きに、自分の王がいるかどうか確認します。
            bool mate = false;
            komaAndMove_Enemy.Foreach_Entry((KeyValuePair<Piece40, IMasus> entry, ref bool toBreak) =>
            {
                foreach (int masuHandle in entry.Value.Elements)
                {
                    sbGohosyu.Append("[");
                    sbGohosyu.Append(M201Array.Items_All[masuHandle]);
                    sbGohosyu.Append("]");

                    if ((int)kingMasu == masuHandle)
                    {
                        sbGohosyu.Append("←★Hit!!");

                        mate = true;
                        toBreak = true;
                    }
                }
            });
            sbGohosyu.AppendLine();
            sbGohosyu.AppendLine("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛自玉王手確認");
            return mate;
        }


        /// <summary>
        /// 次の局面を返します。（全ての合法手）
        /// </summary>
        /// <param name="kifuD">ツリー構造の棋譜です。</param>
        /// <param name="kmDic_Self">自軍の各駒の移動できる升セット</param>
        /// <param name="sbGohosyu"></param>
        public static void GetLegalMove(
            TreeDocument kifuD,
            out KomaAndMasusDictionary kmDic_Self)
        {
            StringBuilder sbGohosyu = new StringBuilder();

            // 現在の局面
            int teme_genzai = kifuD.CountTeme(kifuD.Current8);
            TreeNode6 siteiNode_genzai = (TreeNode6)kifuD.ElementAt8(teme_genzai);
            Sengo sengo_comp = kifuD.CountSengo(teme_genzai);


            // 自分の駒の移動可能範囲
            Util_LegalMove.GetAvailableMove(
                siteiNode_genzai,
                sengo_comp, out kmDic_Self, sbGohosyu);

            //------------------------------------------------------------
            // 自玉が王手されているかどうかの確認
            //------------------------------------------------------------

            bool mate = Util_LegalMove.Is_Mate(siteiNode_genzai, sengo_comp, siteiNode_genzai, sbGohosyu);



            if (mate)
            {
                //>>>>> 王手がかかっているのなら
                StringBuilder sbOhteDebug = new StringBuilder();

                // TODO: 王手を解除できない手は、リーガルムーブから省きます。


                //------------------------------------------------------------
                // どの駒が、どんな手を指すことができるか
                //------------------------------------------------------------

                // 変換「自駒が動ける升」→「自駒が動ける手」
                Dictionary<Piece40, List<IMove>> teMap_All = GameTranslator.KmDic_ToKtDic(
                    kmDic_Self,
                    siteiNode_genzai
                    );
                Dictionary<IMove, PositionKomaHouse> kyokumenList = new Dictionary<IMove, PositionKomaHouse>();



                int teme = kifuD.CountTeme(kifuD.Current8);// 手目
                Sengo sengo = kifuD.CountSengo(teme);// 先後
                //------------------------------------------------------------
                // (a)ひとまず全ての手を、局面に変換します。
                //------------------------------------------------------------

                sbOhteDebug.AppendLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
                sbOhteDebug.AppendLine("■(a)ひとまず全ての手を、局面に変換します。");
                sbOhteDebug.AppendLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
                foreach (KeyValuePair<Piece40, List<IMove>> entry1 in teMap_All)
                {

                    Piece40 koma = entry1.Key;// 駒
                    PieceType syurui = Ks14Converter.FromKoma(koma);// Haiyaku184Array.Syurui(kyokumen.Stars[(int)koma].Star.Haiyaku);//駒の種類
                    // 駒の動ける升全て
                    foreach (IMove teProcess in entry1.Value)
                    {
                        M201 masu = teProcess.Star.Masu;

                        PositionKomaHouse kyokumen = new PositionKomaHouse();// 局面（デフォルトで、平手初期局面）
                        kyokumen.Reset_ToHirateSyokihaichi();
                        kyokumen.SetKomaPos(kifuD, koma, RO_KomaPos.Reset(new RO_Star(sengo, masu, Data_HaiyakuTransition.ToHaiyaku(syurui, (int)masu))));
                        kyokumenList.Add(teProcess, kyokumen);
                    }
                }

                // ログ出力
                foreach (PositionKomaHouse house in kyokumenList.Values)
                {
                    sbOhteDebug.AppendLine(house.Log_Kyokumen(kifuD, teme, "(a)ひとまず全ての手"));// 局面をテキストで作成
                }




                // compの移動先リスト
                Dictionary<Piece40, IMove> enable_teMap = new Dictionary<Piece40, IMove>();
                List<TreeNode6> enable_nextNodes = new List<TreeNode6>();
                foreach (KeyValuePair<Piece40, List<IMove>> teEntry in teMap_All)
                {
                    Piece40 koma = teEntry.Key;
                    List<IMove> teList = teEntry.Value;

                    foreach (IMove te in teList)
                    {
                        TreeNode6 nextNode = kifuD.CreateNodeA(
                            te.SrcStar,
                            te.Star,
                            te.TottaSyurui
                            );

                        bool mate2 = Util_LegalMove.Is_Mate(
                            nextNode,
                            GameTranslator.AlternateSengo(sengo_comp),
                            nextNode,
                            sbGohosyu
                            );

                        if (!mate2)
                        {
                            // 王手がかかっていない局面だけが有効です。

                            if (enable_teMap.ContainsKey(koma))
                            {
                                enable_teMap[koma] = te;
                            }
                            else
                            {
                                enable_teMap.Add(koma, te);
                            }

                            enable_nextNodes.Add(nextNode);
                        }
                        // 王手がかかっている局面は追加しません。
                    }

                }


                sbOhteDebug.AppendLine(@"■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
■デバッグ出力(b)enable_teMap
■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
                // デバッグ出力(b)
                foreach (KeyValuePair<Piece40, IMove> entry1 in enable_teMap)
                {
                    sbOhteDebug.AppendLine($"(b){ entry1.Key }={ entry1.Value}");
                }

                foreach (TreeNode6 nextNode in enable_nextNodes)
                {
                    kifuD.AppendChildA_New(
                        nextNode,
                        "王手回避したい"
                        );

                    // カレントが進んでいるので、戻す。
                    kifuD.Move_Previous();
                }


                // 作り直し
                kmDic_Self = new KomaAndMasusDictionary();// 「どの駒を、どこに進める」の一覧
                foreach (KeyValuePair<Piece40, IMove> entry in enable_teMap)
                {
                    Piece40 koma = entry.Key;
                    IMove te = entry.Value;

                    // ポテンシャル・ムーブを調べます。
                    IMasus masus_PotentialMove = new Masus_Set();
                    masus_PotentialMove.AddElement(te.Star.Masu);

                    if (!masus_PotentialMove.IsEmptySet())
                    {
                        // 空でないなら
                        kmDic_Self.AddOverwrite(koma, masus_PotentialMove);
                    }
                }

                sbOhteDebug.AppendLine(@"■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
■デバッグ出力(c)作り直しkomaAndMove_Self
■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
                // デバッグ出力(c)
                kmDic_Self.Foreach_Entry((KeyValuePair<Piece40, IMasus> entry2, ref bool toBreak) =>
                {
                    sbOhteDebug.AppendLine($"(c){ entry2.Key }={ entry2.Value}");
                }
                );



                sbOhteDebug.AppendLine(@"■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
■デバッグ出力(d)enable_nextNodes
■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
                foreach (TreeNode6 nextNode in enable_nextNodes)
                {
                    sbOhteDebug.AppendLine($"(d){ nextNode.TeProcess.ToSfenText()}");
                }

                Logger.WriteFile(SpecifyFiles.LegalMoveEvasion, sbOhteDebug.ToString());


                //------------------------------------------------------------
                // 候補手の局面を全て見ます。
                //------------------------------------------------------------
                //
                // どの駒が、どの升へ動いたとき、どのような棋譜になるか☆
                //
                Dictionary<Piece40, Dictionary<M201, TreeDocument>> nextKyokumens = new Dictionary<Piece40, Dictionary<M201, TreeDocument>>();

                //foreach (string inputLine in sfenList)
                //{
                //    Kifu kifu2 = new Kifu();

                //    KifuParserA_Impl parser = new KifuParserA_Impl();
                //    parser.Execute_All(inputLine, kifu2, logger);


                //}

            }
        }


        /// <summary>
        /// 自軍の駒の移動可能升。
        /// </summary>
        /// <param name="kouho"></param>
        /// <param name="sbGohosyu"></param>
        /// <param name="logger"></param>
        public static void GetAvailableMove(
            TreeNode6 siteiNode,
            Sengo selfSengo,
            out KomaAndMasusDictionary kouho,
            StringBuilder sbGohosyu
            )
        {
            // 自駒（将棋盤上）
            List<Piece40> jiKomas_OnBan = null;
            // 自駒（駒台上）
            List<Piece40> jiKomas_OnDai = null;

            // 自駒（将棋盤上）
            IMasus jiMasus_OnBan = Thought.Masus_BySengoOkiba(siteiNode, selfSengo, Okiba.ShogiBan, sbGohosyu);

            // 敵駒（将棋盤上）
            IMasus tekiMasus_OnBan = Thought.Masus_BySengoOkiba(siteiNode, GameTranslator.AlternateSengo(selfSengo), Okiba.ShogiBan, sbGohosyu);


            // 自駒の移動候補（将棋盤上）
            KomaAndMasusDictionary ido_onBan = null;
            // 自駒の移動候補（駒台上）
            KomaAndMasusDictionary ido_OnDai = null;

            // 自分の駒だけを抽出。
            jiKomas_OnBan = Util_KyokumenReader.Komas_ByOkibaSengo(siteiNode, Okiba.ShogiBan, selfSengo);
            jiKomas_OnDai = Util_KyokumenReader.Komas_ByOkibaSengo(siteiNode, Okiba.Sente_Komadai | Okiba.Gote_Komadai, selfSengo);


            ido_onBan = Thought.GetPotentialMovesByKoma(siteiNode, jiKomas_OnBan);
            ido_OnDai = Thought.GetPotentialMovesByKoma(siteiNode, jiKomas_OnDai);

            sbGohosyu.AppendLine($@"┏━━━━━━━━━━┓自分の駒の動き(将棋盤Set)
{ido_onBan.LogString_Set()}
┗━━━━━━━━━━┛自分の駒の動き(将棋盤Set)");

            sbGohosyu.AppendLine($@"┏━━━━━━━━━━┓自分の駒の動き(駒台Set)
{ido_onBan.LogString_Set()}
┗━━━━━━━━━━┛自分の駒の動き(駒台Set)");

            //------------------------------------------------------------
            // 調べたい側の駒がある枡。
            //------------------------------------------------------------

            // 盤上の自駒の移動候補から、 自駒がある枡を除外します。
            ido_onBan = Thought_KomaAndMove.MinusMasus(ido_onBan, jiMasus_OnBan);
            //LarabeLogger.GetInstance().WriteLineError(LibLoggerAddresses.ERROR, $"(①自駒升除去)　盤＝{ ido_onBan.DebugString_Set()}");

            // そこから、敵駒がある枡「以降」を更に除外します。
            // FIXME:
            ido_onBan = Thought_KomaAndMove.Minus_OverThereMasus(ido_onBan, tekiMasus_OnBan);
            //LarabeLogger.GetInstance().WriteLineError(LibLoggerAddresses.ERROR, $"(②邪魔敵後)　盤＝{ ido_onBan.DebugString_Set()}");


            // 自駒台の移動候補から、敵駒がある升を除外します。
            ido_OnDai = Thought_KomaAndMove.MinusMasus(ido_OnDai, tekiMasus_OnBan);
            //LarabeLogger.GetInstance().WriteLineError(LibLoggerAddresses.ERROR, $"(③打)　台＝{ ido_OnDai.DebugString_Set()}");

            // 移動候補　＝　盤上の移動駒　＋　駒台の打駒
            kouho = ido_onBan;
            kouho.Merge(ido_OnDai);
            //LarabeLogger.GetInstance().WriteLineError(LibLoggerAddresses.ERROR, $"(④盤・台マージ後)　候補＝{ kouho.DebugString_Set()}");
        }



    }
}
