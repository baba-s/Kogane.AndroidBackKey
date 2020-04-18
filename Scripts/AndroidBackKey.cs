using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniAndroidBackKey
{
	/// <summary>
	/// Android の戻るキーを管理するクラス
	/// </summary>
	public static class AndroidBackKey
	{
		//==============================================================================
		// 構造体
		//==============================================================================
		private struct Data
		{
			public readonly int        m_key;
			public readonly Func<bool> m_callback;

			public Data( int key, Func<bool> callback )
			{
				m_key      = key;
				m_callback = callback;
			}
		}

		//==============================================================================
		// 変数（static readonly）
		//==============================================================================
		private static readonly List<Data> m_list = new List<Data>();

		//==============================================================================
		// プロパティ（static）
		//==============================================================================
		public static bool IsPressedVirtual { get; set; }

		//==============================================================================
		// デリゲート（static）
		//==============================================================================
		public static Func<bool> CanClick { private get; set; }

		//==============================================================================
		// イベント（static）
		//==============================================================================
		public static event Action OnClick;

		//==============================================================================
		// 関数（static）
		//==============================================================================
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
		public static void Add( int key, Func<bool> callback )
		{
			m_list.Add( new Data( key, callback ) );
		}

		/// <summary>
		/// イベントを追加します
		/// </summary>
		public static void Add( UnityEngine.Object context, Func<bool> callback )
		{
			Add( context.GetInstanceID(), callback );
		}

		/// <summary>
		/// イベントを削除します
		/// </summary>
		public static void Remove( int key )
		{
			var item = m_list.Find( c => c.m_key == key );
			m_list.Remove( item );
		}

		/// <summary>
		/// イベントを削除します
		/// </summary>
		public static void Remove( UnityEngine.Object context )
		{
			Remove( context.GetInstanceID() );
		}

		/// <summary>
		/// 更新する時に呼び出されます
		/// </summary>
		public static void Update()
		{
			if ( !Input.GetKeyDown( KeyCode.Escape ) && !IsPressedVirtual ) return;

			IsPressedVirtual = false;

			if ( m_list.Count <= 0 ) return;
			if ( CanClick != null && !CanClick() ) return;

			var count     = m_list.Count;
			var lastIndex = count - 1;
			var data      = m_list[ lastIndex ];
			var key       = data.m_key;
			var callback  = data.m_callback;

			if ( callback == null ) return;

			var result = callback();

			if ( !result ) return;

			OnClick?.Invoke();

			for ( int i = 0; i < m_list.Count; i++ )
			{
				var n = m_list[ i ];
				if ( n.m_key != key ) continue;
				m_list.Remove( n );
				break;
			}
		}
	}
}