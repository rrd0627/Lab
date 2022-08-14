using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    public static void BindEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }
    public static void BindEvent(this GameObject go, Action action)
    {
        UI_Base.BindEvent(go, action);
    }
    public static void AddButtonEvent(this Button go, UnityEngine.Events.UnityAction action)
    {
        go.onClick.RemoveAllListeners();
        go.onClick.AddListener(action);
    }

    public async static void OnComplete<T>(this Task<T> task, Action action)
    {
        await task;
        action.Invoke();
    }
    public async static void OnComplete<T>(this Task<T> task, Action<T> action)
    {
        T ret = await task;
        action.Invoke(ret);
    }
    public static void Forget<T>(this Task<T> task)
    {
    }
}
