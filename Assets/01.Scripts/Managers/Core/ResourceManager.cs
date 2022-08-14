using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using UnityEngine.U2D;

public class ResourceManager
{
    ///<summary>Sprite는 Atlas로 만들면 아틀라스를 호출해야 하기 때문에 있는 Dic</summary>
    public readonly Dictionary<string, Sprite> SpriteInAtlas = new Dictionary<string, Sprite>();
    ///<summary>Managers 생산시 호출</summary>
    public async void Init()
    {
        var validateAddress = Addressables.LoadResourceLocationsAsync("SpriteAtlas", typeof(SpriteAtlas));
        await validateAddress.Task;
        if (validateAddress.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            if (validateAddress.Result.Count > 0)
            {
                //Sprite Atlas는 태그를 반드시 SpriteAtlas로 만들것
                await Addressables.LoadAssetsAsync<SpriteAtlas>("SpriteAtlas", (spriteAtlas) =>
                 {
                     Sprite[] sprites = new Sprite[spriteAtlas.spriteCount];
                     spriteAtlas.GetSprites(sprites);
                     for (int j = 0; j < sprites.Length; j++)
                     {
                         SpriteInAtlas[sprites[j].name.Replace("(Clone)", "")] = sprites[j];
                     }
                 }).Task;
            }
        }
    }
    ///<summary>이미 로드 되었던 Asset을 담고있는 Dic</summary>
    private readonly Dictionary<string, object> loadedAsset = new Dictionary<string, object>();

    ///<summary>Resources.Load의 역할을 대신함</summary>
    public async Task<T> Load<T>(string name) where T : Object
    {
        //풀링되어있는경우 풀 오브젝트를 줄것
        if (typeof(T) == typeof(GameObject))
        {
            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }
        //스프라이트인 경우 아틀라스에서 줄것
        if (typeof(T) == typeof(Sprite))
        {
            if (SpriteInAtlas.TryGetValue(name, out Sprite sprite))
                return sprite as T;
        }
        if (loadedAsset.TryGetValue(name, out object value))
        {
            return value as T;
        }
        //그 외에 경우
        T ret = null;
        ret = await Addressables.LoadAssetAsync<T>(name).Task;// LoadAssetAsync<T>(name);
        loadedAsset[name] = ret;
        return ret;
    }
    ///<summary>Object.Instance의 역할을 대신함</summary>
    public async Task<GameObject> Instantiate(string name, Transform parent = null)
    {
        GameObject original = await Load<GameObject>(name);
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {name}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;

        return go;
    }
    public async Task<GameObject> Instantiate(string name, float timer, Transform parent = null)
    {
        GameObject original = await Load<GameObject>(name);
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {name}");
            return null;
        }
        if (original.GetComponent<Poolable>() != null)
        {
            Poolable poolable = Managers.Pool.Pop(original, parent);
            poolable.Timer(timer);
            return poolable.gameObject;
        }

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        Destroy(go, timer);
        return go;
    }
    ///<summary>Object.Destroy 역할을 대신함 // timer에 변수 할당 시 timer(초) 후 반환 또는 파괴</summary>
    public async void Destroy(GameObject go, float timer = 0)
    {
        if (go == null)
            return;

        if (timer != 0)
        {
            await Task.Delay((int)(timer * 1000));
        }

        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }
        Object.Destroy(go);
    }

    ///<summary>씬 전환시 호출 // 그동안 사용한 에셋 모두 Release</summary>
    public void Clear()
    {
        foreach (var value in loadedAsset)
        {
            Addressables.Release(value.Value);
        }
        loadedAsset.Clear();
    }
}
