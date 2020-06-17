# Uni Anroid Back Key

Android でバックキーが押された時の処理を管理するクラス

## 使用例

```cs
using Kogane;
using UnityEngine;

public class Example : MonoBehaviour
{
    private void Awake()
    {
        // 戻るキーが押された時に呼び出されるコールバックを追加
        AndroidBackKey.Add( this, () => OnBack() );
    }

    // 戻るキーが押された時に呼び出されるコールバック
    private bool OnBack()
    {
        Debug.Log( "ピカチュウ" );

        // コールバック実行後に自動で解除したい場合は true を渡す
        return true;
    }

    private void OnDestroy()
    {
        // 戻るキーが押された時に呼び出されるコールバックを解除
        // すでに解除されている場合は何も起きない
        AndroidBackKey.Remove( this );
    }

    private void Update()
    {
        // 戻るキーが押されたかどうかを確認するために毎フレーム Update 関数を実行
        AndroidBackKey.Update();

        if ( Input.GetKeyDown( KeyCode.Space ) )
        {
            // 戻るキーが押されたことにする
            // デバッグ用の UI から使用させることを想定
            AndroidBackKey.IsPressedVirtual = true;
        }
    }
}
```
