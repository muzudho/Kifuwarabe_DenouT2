﻿using System.Collections.Generic;
using Grayscale.Kifuwarane.Entities.ApplicatedGame.Architecture;

namespace Grayscale.Kifuwarane.Entities.ApplicatedGame
{

    /// <summary>
    /// ------------------------------------------------------------------------------------------------------------------------
    /// 枡ハンドルの一覧。
    /// ------------------------------------------------------------------------------------------------------------------------
    /// 
    /// ┌─┬─┬─┬─┬─┬─┬─┬─┬─┐
    /// │72│63│54│45│36│27│18│ 9│ 0│
    /// ├─┼─┼─┼─┼─┼─┼─┼─┼─┤
    /// │73│64│55│46│37│28│19│10│ 1│
    /// ├─┼─┼─┼─┼─┼─┼─┼─┼─┤
    /// │74│65│56│47│38│29│20│11│ 2│
    /// ├─┼─┼─┼─┼─┼─┼─┼─┼─┤
    /// │75│66│57│48│39│30│21│12│ 3│
    /// ├─┼─┼─┼─┼─┼─┼─┼─┼─┤
    /// │76│67│58│49│40│31│22│13│ 4│
    /// ├─┼─┼─┼─┼─┼─┼─┼─┼─┤
    /// │77│68│59│50│41│32│23│14│ 5│
    /// ├─┼─┼─┼─┼─┼─┼─┼─┼─┤
    /// │78│69│60│51│42│33│24│15│ 6│
    /// ├─┼─┼─┼─┼─┼─┼─┼─┼─┤
    /// │79│70│61│52│43│34│25│16│ 7│
    /// ├─┼─┼─┼─┼─┼─┼─┼─┼─┤
    /// │80│71│62│53│44│35│26│17│ 8│
    /// └─┴─┴─┴─┴─┴─┴─┴─┴─┘
    /// 先手駒台：81～120
    /// 後手駒台：121～160
    /// 駒袋：161～200
    /// エラー：201
    /// の、計202。
    /// 
    /// 将棋盤上の枡のリスト。
    /// 
    /// ・Add、Removeといった、データ構造に縛られるメソッドは持たせません。
    ///   変わりに、Minus といった汎用的に操作できるメソッドを持たせます。
    /// 
    /// ・Clearメソッドは持たせません。インスタンスを作り直して親要素にセットし直してください。
    ///   空にすることができないオブジェクト（線分など）があることが理由です。
    /// </summary>
    public interface IMasus
    {

        IMasus Clone();

        /// <summary>
        /// 将棋盤上の枡番号。
        /// 
        /// これは、筋も木っ端微塵に切ってしまっているので、順序はばらばら。
        /// 
        /// 要素ｘ、集合Ｘが「ｘ∈Ｘ」の関係とき、全てのｘを取る操作です。
        /// 
        /// 例えばＡが、「１→２」と、「３」を持っているとき、
        /// ｘとは「１」「２」「３」といった最も細かい各要素のどれか１つのことです。
        /// ｘを全部拾うということです。
        /// </summary>
        IEnumerable<M201> Elements
        {
            get;
        }

        /// <summary>
        /// 将棋盤上の枡番号。
        /// 
        /// これは、筋も残しているので、先頭から順番に読むことを想定しています。
        /// 
        /// 集合Ａ、Ｂは「Ａ⊆Ｂ」の関係とし、ＡがＢを取る操作です。
        /// 
        /// 例えばＡが、「１→２」と、「３」を持っているとき、
        /// Ｂとは「１→２」「３」といった子要素のことです。
        /// </summary>
        IEnumerable<IMasus> Supersets
        {
            get;
        }

        int Count
        {
            get;
        }



        #region 一致系
        
        /// <summary>
        /// ************************************************************************************************************************
        /// この図形の指定の場所に、指定のドットが全て打たれているか。
        /// ************************************************************************************************************************
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        bool ContainsAll(IMasus masus);

        /// <summary>
        /// ************************************************************************************************************************
        /// この図形に、指定のドットが含まれているか。
        /// ************************************************************************************************************************
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        bool Contains(M201 masuHandle);

        /// <summary>
        /// ************************************************************************************************************************
        /// 空集合なら真です。
        /// ************************************************************************************************************************
        /// </summary>
        /// <returns></returns>
        bool IsEmptySet();

        #endregion




        #region 操作系

        /// <summary>
        /// この集合Ａと、その要素ａが「ａ∈Ａ」の関係のとき、ａを１つ仲間に加える操作です。
        /// 
        /// 例えばＡが、「１→２」と、「３」を持っているときに、
        /// ここに「４」を追加する、といった操作がこれです。
        /// </summary>
        /// <param name="masuHandle"></param>
        void AddElement(M201 masuHandle);

        /// <summary>
        /// この集合Ａと、引数に指定する集合Ｂの関係が「Ａ⊆Ｂ」になるようにする操作です。
        /// </summary>
        /// <param name="supersetB"></param>
        void AddSupersets(IMasus supersetB);

        /// <summary>
        /// この集合Ａと、その要素ａが「ａ∈Ａ」の関係のとき、ａを１つ仲間から外す操作です。
        /// 
        /// 例えばＡが、「１→２」と、「３」を持っているときに、
        /// ここから「２」を外して　「１」と「３」だけにする、といった操作がこれです。
        /// 
        /// もしこのときＡが、「１→２→３」を持っていた場合は、
        /// 「２」が外れたことによって「２→３」は丸ごと消え、「１」だけが残ります。
        /// </summary>
        /// <param name="masuHandle"></param>
        void RemoveElement(M201 bMasu);

        /// <summary>
        /// もし順序があるならば、「ａ　＝　１→２→３→４」のときに
        /// 「ａ　ＲｅｍｏｖｅＥｌｅｍｅｎｔ＿ＯｖｅｒＴｈｅｒｅ（　２　）」とすれば、
        /// 答えは「３→４」
        /// となる操作がこれです。
        /// 
        /// ｂを含めず、それより後ろを切る、という操作です。
        /// 順序がなければ、ＲｅｍｏｖｅＥｌｅｍｅｎｔと同等です。
        /// </summary>
        /// <param name="masuHandle"></param>
        void RemoveElement_OverThere(M201 bMasu);

        /// <summary>
        /// this - b = c
        /// 
        /// この集合のメンバーから、指定の集合のメンバーを削除します。
        /// </summary>
        /// <param name="masus"></param>
        /// <returns></returns>
        IMasus Minus(IMasus b);

        /// <summary>
        /// this - b以降 = c
        /// 
        /// この集合のメンバーから、指定の集合のメンバー「より向こう」を削除します。
        /// </summary>
        /// <param name="masus"></param>
        /// <returns></returns>
        IMasus Minus_OverThere(IMasus b);

        #endregion


        /// <summary>
        /// 筋も残し、全件網羅
        /// </summary>
        /// <returns></returns>
        string LogString_Concrete();

        /// <summary>
        /// 重複をなくした表現
        /// </summary>
        /// <returns></returns>
        string LogString_Set();

    }
}
