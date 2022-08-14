using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    ///<summary>Manager생산할때 만들어짐</summary>
    public void Init()
    {
        GetData();
    }

    int _example = 0;
    public void SaveData()
    {
        _example = ES3.Load<int>("Key");
    }
    public void GetData()
    {
        ES3.Save("Key", _example);
    }
}
