﻿namespace Kogane
{
    /// <summary>
    /// Android の戻るキーが押された時に呼び出されるイベントを持つインターフェイス
    /// </summary>
    public interface IAndroidBackKeyClickable
    {
        //================================================================================
        // 関数
        //================================================================================
        /// <summary>
        /// 戻るキーが押された時に呼び出されます
        /// </summary>
        void Click();
    }
}