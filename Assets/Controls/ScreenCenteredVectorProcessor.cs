using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class ScreenCenteredVectorProcessor : InputProcessor<Vector2>
{
    #if UNITY_EDITOR
    static ScreenCenteredVectorProcessor()
    {
        Initialize();
    }
    #endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        InputSystem.RegisterProcessor<ScreenCenteredVectorProcessor>();
    }

    public override Vector2 Process(Vector2 value, InputControl control)
    {
        int width = Screen.width;
        int height = Screen.height;
        value.x -= width/2;
        value.y -= height/2;
        value /= Mathf.Min(width, height) / 2;
        return value;
    }

    //...
}