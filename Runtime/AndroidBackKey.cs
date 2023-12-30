using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kogane
{
    /// <summary>
    /// Android の戻るキーを管理するクラス
    /// </summary>
    public static class AndroidBackKey
    {
        //================================================================================
        // 構造体
        //================================================================================
        private readonly struct Data
        {
            public Object     Key      { get; }
            public Func<bool> Callback { get; }

            public Data
            (
                Object     key,
                Func<bool> callback
            )
            {
                Key      = key;
                Callback = callback;
            }
        }

        //================================================================================
        // 定数
        //================================================================================
        private const int INITIAL_CAPACITY = 8;

        //================================================================================
        // 変数（static readonly）
        //================================================================================
        private static List<Data> m_list = new( INITIAL_CAPACITY );

        //================================================================================
        // プロパティ（static）
        //================================================================================
        public static bool     IsPressedVirtual { get; set; }
        public static int      DisableCount     { get; private set; }
        public static bool     IsDisable        => 0 < DisableCount;
        public static int      Count            => m_list.Count;
        public static Object[] Keys             => m_list.Select( x => x.Key ).ToArray();

        //================================================================================
        // デリゲート（static）
        //================================================================================
        public static Func<bool> CanClick { private get; set; }

        //================================================================================
        // イベント（static）
        //================================================================================
        public static event Action OnClick;

        //================================================================================
        // 関数（static）
        //================================================================================
#if UNITY_EDITOR
        /// <summary>
        /// ゲーム起動時に呼び出されます
        /// </summary>
        [UnityEditor.InitializeOnEnterPlayMode]
        private static void RuntimeInitializeOnLoadMethod()
        {
            m_list?.Clear();
            m_list = new( INITIAL_CAPACITY );

            IsPressedVirtual = default;
            DisableCount     = default;
            CanClick         = default;
            OnClick          = default;
        }
#endif

        /// <summary>
        /// 初期化します
        /// </summary>
        public static void Clear()
        {
            m_list.Clear();
            IsPressedVirtual = false;
        }

        /// <summary>
        /// イベントを追加します
        /// </summary>
        [MustUseReturnValue]
        public static IDisposable Add( Object key, IAndroidBackKeyClickable clickable )
        {
            Add( key, () => clickable.Click() );
            return Disposable.Create( () => Remove( key ) );
        }

        /// <summary>
        /// イベントを追加します
        /// </summary>
        public static void Add( Object key, Action callback )
        {
            Add
            (
                key,
                () =>
                {
                    callback();
                    return true;
                }
            );
        }

        /// <summary>
        /// イベントを追加します
        /// </summary>
        public static void Add( Object key, Func<bool> callback )
        {
            m_list.Add( new( key, callback ) );
        }

        /// <summary>
        /// イベントを削除します
        /// </summary>
        public static void Remove( Object key )
        {
            var item = m_list.Find( x => x.Key == key );
            m_list.Remove( item );
        }

        /// <summary>
        /// 無効化するスコープを返します
        /// </summary>
        public static IDisposable DisableScope()
        {
            DisableCount++;
            return Disposable.Create( () => DisableCount-- );
        }

        /// <summary>
        /// 更新する時に呼び出されます
        /// </summary>
        public static void Update()
        {
            for ( var i = m_list.Count - 1; 0 <= i; i-- )
            {
                var x = m_list[ i ];
                if ( x.Key != null ) continue;
                m_list.Remove( x );
            }

            if ( !Input.GetKeyDown( KeyCode.Escape ) && !Input.GetMouseButtonDown( 3 ) && !IsPressedVirtual ) return;

            IsPressedVirtual = false;

            if ( IsDisable ) return;
            if ( m_list.Count <= 0 ) return;
            if ( CanClick != null && !CanClick() ) return;

            var count     = m_list.Count;
            var lastIndex = count - 1;
            var data      = m_list[ lastIndex ];
            var key       = data.Key;
            var callback  = data.Callback;

            if ( callback == null ) return;

            var result = callback();

            if ( !result ) return;

            OnClick?.Invoke();

            for ( var i = 0; i < m_list.Count; i++ )
            {
                var x = m_list[ i ];
                if ( x.Key != key ) continue;
                m_list.Remove( x );
                break;
            }
        }
    }
}