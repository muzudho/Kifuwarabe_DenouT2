﻿using System.Collections.Generic;
using Grayscale.Kifuwarane.Entities.ApplicatedGame;
using Grayscale.Kifuwarane.Entities.ApplicatedGame.Architecture;
using Grayscale.Kifuwarane.Entities.Logging;

namespace Grayscale.Kifuwarane.Entities.ApplicatedGame.Architecture
{
    /// <summary>
    /// 局面から、いろいろな形でデータを取るクラスです。
    /// 
    /// メソッド数が煩雑になりそうなことから、こちらに分けました。
    /// </summary>
    public abstract class Util_KyokumenReader
    {
        /// <summary>
        /// 駒の利き
        /// </summary>
        /// <param name="koma"></param>
        /// <param name="kifu"></param>
        /// <returns></returns>
        public static IMasus KomanoKiki(IKomaPos koma)
        {
            IMasus dotZukei;

            {
                dotZukei = Rule01_PotentialMove_15Array.ItemMethods[(int)Haiyaku184Array.Syurui(koma.Star.Haiyaku)](koma.Star.Sengo, koma.Star.Masu);


                //Logger.Trace($"masuList.Count={ masuList.Count}");
                //foreach (Masu masu in masuList)
                //{
                //    Logger.Trace($"masu.Sengo={ masu.Sengo }　masu.Suji={ masu.Suji }　masu.Dan={ masu.Dan }");
                //}

                //dotZukei.AddMasuHandleRange(Converter04.MasuToHandle(dotZukei.ToMasuList()));
                //foreach (int kikiMasu in kikiMasuList)
                //{
                //    Logger.Trace($"kikiMasu={ kikiMasu }");
                //}
            }

            return dotZukei;
        }


        /// <summary>
        /// 軌道上の駒たち
        /// </summary>
        /// <param name="km"></param>
        /// <returns></returns>
        public static List<Piece40> KomaHandles_EachSrc(TreeDocument kifuD, Sengo sengo, IKomaPos itaru, IMasus srcList)
        {
            List<Piece40> komaHandleList = new List<Piece40>();

            foreach (M201 srcHandle in srcList.Elements)
            {
                Piece40 koma = Util_KyokumenReader.Koma_AtMasu_Shogiban(kifuD, sengo, srcHandle);
                if (K40Util.OnKoma((int)koma))
                {
                    // 指定の升に駒がありました。
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    komaHandleList.Add(koma);
                }
            }

            return komaHandleList;
        }

        public static IMasus KomaHaichi(TreeDocument kifuD1, Sengo sengo)
        {
            Masus_Set ban = new Masus_Set();

            IKifuElement node2 = kifuD1.ElementAt8(kifuD1.CountTeme(kifuD1.Current8));
            PositionKomaHouse house1 = node2.KomaHouse;

            house1.Foreach_Items(kifuD1, (TreeDocument kifuD2, RO_KomaPos koma, ref bool toBreak) =>
            {
                if (koma.Star.Sengo == sengo && M201Util.GetOkiba(koma.Star.Masu) == Okiba.ShogiBan)
                {
                    ban.AddElement(koma.Star.Masu);
                }
            });

            return ban;
        }

        /// <summary>
        /// 駒の種類（不成として扱います）を指定して、駒を検索します。
        /// </summary>
        /// <param name="okiba"></param>
        /// <param name="syurui"></param>
        /// <param name="uc_Main"></param>
        /// <returns>無ければ -1</returns>
        public static Piece40 Koma_BySyuruiIgnoreCase(TreeDocument kifuD, Okiba okiba, PieceType syurui)
        {
            Piece40 found = Piece40.Error;
            int lastTeme = kifuD.CountTeme(kifuD.Current8);

            PieceType syuruiFunariCase = KomaSyurui14Array.FunariCaseHandle(syurui);

            foreach(Piece40 koma in K40Array.Items_KomaOnly)
            {
                IKifuElement node2 = kifuD.ElementAt8(lastTeme);
                PositionKomaHouse house1 = node2.KomaHouse;

                IKomaPos komaP = house1.KomaPosAt(koma);

                if (M201Util.GetOkiba(komaP.Star.Masu) == okiba
                    && KomaSyurui14Array.Matches(komaP.ToFunariCase(), syuruiFunariCase))
                {
                    found = koma;// K40Array.Items_All[i];
                    break;
                }
            }

            //for (int i = 0; i < kifuD.Kifu_Old.KomaDoorsLength; i++)
            //{
            //    RO_KomaPos koma = kifuD.Kifu_Old.KomaDoorsAt(i);

            //    if (M201Util.GetOkiba(koma.Masu) == okiba
            //        && KomaSyurui14Array.Matches(koma.ToFunariCase(), syuruiFunariCase))
            //    {
            //        found = K40Array.Items_All[i];
            //        break;
            //    }
            //}

            return found;
        }

        /// <summary>
        /// 指定の場所にある駒を返します。
        /// 
        ///         置き場は指定できますが、先後は見ません。
        /// 
        /// </summary>
        /// <param name="okiba">置き場</param>
        /// <param name="masu">筋、段</param>
        /// <param name="uc_Main">メインパネル</param>
        /// <returns>駒。無ければヌル。</returns>
        public static Piece40 Koma_AtMasu(TreeDocument kifuD, M201 masu)
        {
            Piece40 komaFound = Piece40.Error;
            int lastTeme = kifuD.CountTeme(kifuD.Current8);

            foreach (Piece40 koma in K40Array.Items_KomaOnly)
            {
                IKifuElement node2 = kifuD.ElementAt8(lastTeme);
                PositionKomaHouse house1 = node2.KomaHouse;

                IKomaPos komaP = house1.KomaPosAt(koma);

                if (komaP.Star.Masu == masu)
                {
                    komaFound = koma;
                    break;
                }
            }

            //for (int i = 0; i < kifuD.Kifu_Old.KomaDoorsLength; i++)
            //{
            //    RO_KomaPos komaP = kifuD.Kifu_Old.KomaDoorsAt(i);

            //    if ((int)komaP.Masu == masuHandle)
            //    {
            //        hFound = K40Array.Items_All[i];
            //        break;
            //    }
            //}

            return komaFound;
        }

        /// <summary>
        /// 指定の場所にある駒を返します。
        /// 
        ///         先後は見ますが、将棋盤限定です。
        /// 
        /// </summary>
        /// <param name="okiba">置き場</param>
        /// <param name="masu1">筋、段</param>
        /// <param name="uc_Main">メインパネル</param>
        /// <returns>駒。無ければヌル。</returns>
        public static Piece40 Koma_AtMasu_Shogiban(TreeDocument kifuD, Sengo sengo, M201 masu1)
        {
            Piece40 foundKoma = Piece40.Error;
            int lastTeme = kifuD.CountTeme(kifuD.Current8);

            foreach(Piece40 koma in K40Array.Items_KomaOnly)
            {
                IKifuElement dammyNode2 = kifuD.ElementAt8(lastTeme);
                PositionKomaHouse house1 = dammyNode2.KomaHouse;

                IKomaPos komaP2 = house1.KomaPosAt(koma);

                // 先後は見ますが、将棋盤限定です。
                if (
                    komaP2.Star.Sengo == sengo
                    && M201Util.GetOkiba(komaP2.Star.Masu) == Okiba.ShogiBan
                    && Mh201Util.MasuToSuji(komaP2.Star.Masu) == Mh201Util.MasuToSuji(masu1)
                    && Mh201Util.MasuToDan(komaP2.Star.Masu) == Mh201Util.MasuToDan(masu1)
                    )
                {
                    foundKoma = koma;// K40Array.Items_All[i];
                    break;
                }
            }

            //for (int i = 0; i < kifuD.Kifu_Old.KomaDoorsLength; i++)
            //{
            //    RO_KomaPos komaP2 = kifuD.Kifu_Old.KomaDoorsAt(i);

            //    // 先後は見ますが、将棋盤限定です。
            //    if (
            //        komaP2.Sengo == sengo
            //        && M201Util.GetOkiba(komaP2.Masu) == Okiba.ShogiBan
            //        && Mh201Util.MasuToSuji(komaP2.Masu) == Mh201Util.MasuToSuji(masu1)
            //        && Mh201Util.MasuToDan(komaP2.Masu) == Mh201Util.MasuToDan(masu1)
            //        )
            //    {
            //        foundKoma = K40Array.Items_All[i];
            //        break;
            //    }
            //}

            return foundKoma;
        }

        /// <summary>
        /// 駒のハンドル(*1)を返します。
        /// 
        ///         *1…将棋の駒１つ１つに付けられた番号です。
        /// 
        /// </summary>
        /// <param name="syurui"></param>
        /// <param name="hKomas"></param>
        /// <returns></returns>
        public List<Piece40> Komas_BySyurui(TreeDocument kifuD, PieceType syurui)
        {
            List<Piece40> komas = new List<Piece40>();
            int lastTeme = kifuD.CountTeme(kifuD.Current8);

            foreach(Piece40 koma in K40Array.Items_KomaOnly)
            {
                IKifuElement dammyNode2 = kifuD.ElementAt8(lastTeme);
                PositionKomaHouse house1 = dammyNode2.KomaHouse;

                IKomaPos komaP = house1.KomaPosAt(koma);

                if (
                    KomaSyurui14Array.Matches(syurui, Haiyaku184Array.Syurui(komaP.Star.Haiyaku))
                    )
                {
                    komas.Add(koma);
                }
            }

            //for (int hKoma = 0; hKoma < kifuD.Kifu_Old.KomaDoorsLength; hKoma++)
            //{
            //    RO_KomaPos komaP = kifuD.Kifu_Old.KomaDoorsAt(hKoma);

            //    if (
            //        KomaSyurui14Array.Matches(syurui, Haiyaku184Array.Syurui(komaP.Haiyaku))
            //        )
            //    {
            //        komas.Add(K40Array.Items_All[hKoma]);
            //    }
            //}

            return komas;
        }

        /// <summary>
        /// 駒のハンドルを返します。
        /// </summary>
        /// <param name="sengo"></param>
        /// <param name="hKomas"></param>
        /// <returns></returns>
        public static List<Piece40> Komas_BySengo(
            //Kifu_Document kifuD,
            IKifuElement siteiNode,// = kifuD.ElementAt8(kifuD.CountTeme(kifuD.Current8));
            Sengo sengo)
        {
            List<Piece40> komas = new List<Piece40>();

            foreach(Piece40 koma in K40Array.Items_KomaOnly)
            {
                IKomaPos komaP = siteiNode.KomaHouse.KomaPosAt(koma);

                if (sengo == komaP.Star.Sengo)
                {
                    komas.Add(koma);
                }
            }

            return komas;
        }

        /// <summary>
        /// 駒のハンドルを返します。　：　置き場、種類
        /// </summary>
        /// <param name="okiba"></param>
        /// <param name="syurui"></param>
        /// <param name="kifuD"></param>
        /// <returns></returns>
        public static List<Piece40> Komas_ByOkibaSyurui(TreeDocument kifuD, Okiba okiba, PieceType syurui)
        {
            List<Piece40> komas = new List<Piece40>();
            int lastTeme = kifuD.CountTeme(kifuD.Current8);

            foreach(Piece40 koma in K40Array.Items_KomaOnly)
            {
                IKifuElement dammyNode3 = kifuD.ElementAt8(lastTeme);
                PositionKomaHouse house1 = dammyNode3.KomaHouse;

                IKomaPos komaP = house1.KomaPosAt(koma);

                if (
                    okiba == M201Util.GetOkiba(komaP.Star.Masu)
                    && KomaSyurui14Array.Matches(syurui, Haiyaku184Array.Syurui(komaP.Star.Haiyaku))
                    )
                {
                    komas.Add(koma);
                }
            }

            //for (int i = 0; i < kifuD.Kifu_Old.KomaDoorsLength; i++)
            //{
            //    RO_KomaPos koma = kifuD.Kifu_Old.KomaDoorsAt(i);

            //    if (
            //        okiba == M201Util.GetOkiba(koma.Masu)
            //        && KomaSyurui14Array.Matches(syurui, Haiyaku184Array.Syurui(koma.Haiyaku))
            //        )
            //    {
            //        hKomas.Add(K40Array.Items_All[i]);
            //    }
            //}

            return komas;
        }

        /// <summary>
        /// 駒のハンドルを返します。
        /// </summary>
        /// <param name="okiba"></param>
        /// <param name="sengo"></param>
        /// <param name="syurui"></param>
        /// <param name="kifuD"></param>
        /// <returns></returns>
        public static List<Piece40> Komas_ByOkibaSengoSyurui(TreeDocument kifuD, Okiba okiba, Sengo sengo, PieceType syurui)
        {
            List<Piece40> komas = new List<Piece40>();
            int lastTeme = kifuD.CountTeme(kifuD.Current8);

            foreach(Piece40 koma in K40Array.Items_KomaOnly)
            {
                IKifuElement dammyNode3 = kifuD.ElementAt8(lastTeme);
                PositionKomaHouse house2 = dammyNode3.KomaHouse;

                IKomaPos komaP = house2.KomaPosAt(koma);

                if (
                    okiba == M201Util.GetOkiba(komaP.Star.Masu)
                    && sengo == komaP.Star.Sengo
                    && syurui == Haiyaku184Array.Syurui(komaP.Star.Haiyaku)
                    )
                {
                    komas.Add(koma);
                }
            }

            //for (int hKoma = 0; hKoma < kifuD.Kifu_Old.KomaDoorsLength; hKoma++)
            //{
            //    RO_KomaPos komaP = kifuD.Kifu_Old.KomaDoorsAt(hKoma);

            //    if (
            //        okiba == M201Util.GetOkiba(komaP.Masu)
            //        && sengo == komaP.Sengo
            //        && syurui == Haiyaku184Array.Syurui(komaP.Haiyaku)
            //        )
            //    {
            //        komas.Add(K40Array.Items_All[hKoma]);
            //    }
            //}

            return komas;
        }

        /// <summary>
        /// 指定した置き場にある駒のハンドルを返します。
        /// </summary>
        /// <param name="kifuD"></param>
        /// <param name="okiba"></param>
        /// <returns></returns>
        public static List<Piece40> Komas_ByOkiba(TreeDocument kifuD, Okiba okiba)
        {
            List<Piece40> komas = new List<Piece40>();
            int lastTeme = kifuD.CountTeme(kifuD.Current8);

            foreach(Piece40 koma in K40Array.Items_KomaOnly)
            {
                IKifuElement dammyNode4 = kifuD.ElementAt8(lastTeme);
                PositionKomaHouse house3 = dammyNode4.KomaHouse;
                IKomaPos komaP = house3.KomaPosAt(koma);

                if (okiba == M201Util.GetOkiba(komaP.Star.Masu))
                {
                    komas.Add(koma);
                }
            }

            //for (int hKoma = 0; hKoma < kifuD.Kifu_Old.KomaDoorsLength; hKoma++)
            //{
            //    RO_KomaPos komaP = kifuD.Kifu_Old.KomaDoorsAt(hKoma);

            //    if (okiba == M201Util.GetOkiba(komaP.Masu))
            //    {
            //        komas.Add(K40Array.Items_All[hKoma]);
            //    }
            //}

            return komas;
        }

        /// <summary>
        /// 駒のハンドルを返します。
        /// </summary>
        /// <param name="okiba"></param>
        /// <param name="kifuD"></param>
        /// <returns></returns>
        public static List<Piece40> Komas_ByOkibaSengo(
            //Kifu_Document kifuD,
            TreeNode6 siteiNode,//IKifuElement siteiNode = kifuD.ElementAt8(kifuD.CountTeme(kifuD.Current8));
            Okiba okiba, Sengo sengo)
        {
            List<Piece40> komas = new List<Piece40>();

            foreach(Piece40 koma in K40Array.Items_KomaOnly)
            {
                PositionKomaHouse house3 = siteiNode.KomaHouse;

                IKomaPos komaP = house3.KomaPosAt(koma);

                if (
                    M201Util.GetOkiba(komaP.Star.Masu).HasFlag(okiba)
                    && sengo == komaP.Star.Sengo
                    )
                {
                    komas.Add(koma);
                }
            }

            return komas;
        }
    }
}
