using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene CurrentScene;
    public Animator _sceneTrasitionAni { private get; set; }
    private AsyncOperation _AsyncLevelLoadingOperation;

    int fadeInHash;
    int fadeOutHash;


    public void Init()
    {
        fadeInHash = Animator.StringToHash("FadeIn");
        fadeOutHash = Animator.StringToHash("FadeOut");
    }


    ///<summary>씬 로드용 함수 // SceneTrasition 애니메이션 실행 됨</summary>
    public async void LoadScene(Define.Scene type)
    {
        _sceneTrasitionAni.Play(fadeOutHash);
        await Task.Delay(500);

        Managers.Clear();

        RuntimeAnimatorController animatorController = await Managers.Resource.Load<RuntimeAnimatorController>("SceneTrasitionAnimator");
        _sceneTrasitionAni.runtimeAnimatorController = animatorController;

        _AsyncLevelLoadingOperation = SceneManager.LoadSceneAsync(GetSceneName(type), LoadSceneMode.Single);
        _AsyncLevelLoadingOperation.allowSceneActivation = false;

        while (!_AsyncLevelLoadingOperation.isDone)
        {
            if (_AsyncLevelLoadingOperation.progress >= 0.9f)
            {
                break;
            }
            await Task.Delay(1);
        }
        _sceneTrasitionAni.Play(fadeInHash);
        _AsyncLevelLoadingOperation.allowSceneActivation = true;
    }
    ///<summary>현재 씬 이름</summary>
    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }
    ///<summary>새로운 씬으로 갈때마다 클리어</summary>
    public void Clear()
    {
        CurrentScene.Clear();
    }
}
