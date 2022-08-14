using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Managers : MonoBehaviour
{
    ///<summary>내부적으로 사용되는 Managers 변수</summary>
    static Managers _instance;
    ///<summary>내부적으로 사용되는 Managers Property</summary>
    static Managers Instance { get { Init(); return _instance; } }

    DataManager _data = new DataManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();

    GameManager _game;

    public static DataManager Data { get { return Instance._data; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }


    public static GameManager Game { get { return Instance._game; } }



    ///<summary>가장 처음 매니저 만들때 한번 Init</summary>
    static void Init()
    {
        if (_instance == null)
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            UnityEngine.Input.multiTouchEnabled = false;

            GameObject go = new GameObject { name = "@Managers" };
            go.AddComponent<Managers>();
            DontDestroyOnLoad(go);
            _instance = go.GetComponent<Managers>();

            Managers.Resource.Instantiate("SceneTrasition").OnComplete((SceneTrasition) =>
            {
                SceneTrasition.transform.SetParent(_instance.transform);
                Scene._sceneTrasitionAni = SceneTrasition.GetComponent<Animator>();
            });

            _instance._data.Init();
            _instance._pool.Init();
            _instance._sound.Init();
            _instance._resource.Init();
            _instance._scene.Init();

            _instance._game = go.AddComponent<GameManager>();
        }
    }

    public static void GameInit()
    {
        _instance._game.Init();
    }

    ///<summary>새로운 씬으로 갈때마다 클리어</summary>
    public static void Clear()
    {
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
        Resource.Clear();
        Game.Clear();
    }
}
