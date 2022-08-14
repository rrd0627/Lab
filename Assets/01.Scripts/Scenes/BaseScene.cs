using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

    void Awake()
    {
        Init();
        Managers.Scene.CurrentScene = this;

        //EventSystem
        if (GameObject.FindObjectOfType<EventSystem>() == null)
            Managers.Resource.Instantiate("EventSystem").Forget();
    }

    protected abstract void Init();

    public abstract void Clear();
}
