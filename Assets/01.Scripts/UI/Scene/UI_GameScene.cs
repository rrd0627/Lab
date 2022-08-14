using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameScene : UI_Scene
{
    enum Images
    {
        test,
        JoyStick,
    }

    private void Awake()
    {
        Init();

        Bind<UnityEngine.UI.Image>(typeof(Images));

        SetJoyStick();

        Managers.GameInit();

        GetImage((int)Images.test).gameObject.BindEvent(() => Managers.Scene.LoadScene(Define.Scene.Game));
    }

    private void SetJoyStick()
    {
        GetImage((int)Images.JoyStick).GetComponent<JoyStickController>().Init(GetImage((int)Images.JoyStick));
    }


}
