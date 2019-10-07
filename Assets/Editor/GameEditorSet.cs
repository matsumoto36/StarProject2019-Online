using UnityEngine;
using System.Collections;
using UnityEditor;

public class GameEditorSet : EditorWindow
{
    [MenuItem("Custom/SampleWindow")]
    static void ShowWindow()
    {
        GetWindow<GameEditorSet>();
    }

    void OnGUI()
    {
        EditorWindow gameView = GetGameView();

        float positionY = gameView.position.y;

        if (GUILayout.Button("上"))
        {
            positionY -= 5;
            gameView.position = new Rect(gameView.position.x, positionY, gameView.position.width, gameView.position.height);
            Debug.Log("↑");
        }

        if (GUILayout.Button("下"))
        {
            positionY += 5;
            gameView.position = new Rect(gameView.position.x, positionY, gameView.position.width, gameView.position.height);
            Debug.Log("↓");
        }
    }


    private static EditorWindow GetGameView()
    {
        // ウィンドウが存在しない場合は生成される
        return EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor"));
    }
}
