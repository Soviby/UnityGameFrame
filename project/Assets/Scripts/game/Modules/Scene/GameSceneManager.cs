
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;


public delegate void UpdateProgress(float progress);
public class LoadItem
{
    public float laod_value = 0;
    public readonly float weight = 1;
    public LoadItem(float weight = 1)
    {
        this.weight = weight;
    }
}

public class GameSceneManager : Singleton<GameSceneManager>
{

    private SceneBehavior curSceneBehavior;

    public SceneBehavior CurSceneBehavior { get => curSceneBehavior; }

    public void EnterScene(string sceneID)
    {
        AsyncEnterScene(sceneID).Forget();
    }

    public void EnterScene<T>() where T : SceneBehavior
    {
        AsyncEnterScene(typeof(T).Name).Forget();
    }


    private async UniTask AsyncEnterScene(string unitySceneId, LoadSceneMode loadMode = LoadSceneMode.Single)
    {
        float totalWeight = 1f;
        Dictionary<string, LoadItem> LoadItems = new Dictionary<string, LoadItem>();
        LoadItems.Add("GameSceneLoad", new LoadItem(0.4f));
        totalWeight -= 0.4f;

        if (curSceneBehavior != null)
        {
            LoadItems.Add("LeaveSceneLoad", new LoadItem(0.3f));
            totalWeight -= 0.3f;
        }
        LoadItems.Add("EnterSceneLoad", new LoadItem(totalWeight));

        ProgressPanel progressPanel = null;
        UpdateProgress update = (progress) =>
        {
            if (progress >= 1) progressPanel?.Hide();
            else progressPanel = MyGUIManager.Instance.Show<ProgressPanel>();
            progressPanel.SetData(progress);
        };
        void UpdateLoading()
        {
            float totle = 0;
            float cur_value = 0;
            foreach (var item in LoadItems.Values)
            {
                totle += item.weight;
                cur_value += item.weight * item.laod_value;
            }
            update(cur_value / totle);
        }
        void Update(string loadKey, float loadValue)
        {
            LoadItems[loadKey].laod_value = loadValue;
            UpdateLoading();
        }


        if (curSceneBehavior != null)
        {
            await curSceneBehavior.LeaveScene(LoadItems["LeaveSceneLoad"], UpdateLoading);
            Update("LeaveSceneLoad", 1f);
        }

        var operation = SceneManager.LoadSceneAsync(unitySceneId, loadMode);
        await UniTask.WaitUntil(() =>
        {
            Update("GameSceneLoad", operation.progress);
            return operation.isDone;
        });

        curSceneBehavior = Activator.
            CreateInstance(Type.GetType(unitySceneId),
            SceneManager.GetSceneByName(unitySceneId))
            as SceneBehavior;

        await curSceneBehavior.EnterScene(LoadItems["EnterSceneLoad"], UpdateLoading);
        Update("EnterSceneLoad", 1f);
    }

    public void GMJumpScene(string[] scene_name)
    {
        GameSceneManager.Instance.EnterScene(scene_name[0]);
    }

    public void GMJumpMainScene(string[] scene_name)
    {
        GameSceneManager.Instance.EnterScene("MainScene");
    }

}

interface ISceneBehavior
{
    UniTask LeaveScene(LoadItem item, Action UpdateLoading);
    UniTask EnterScene(LoadItem item, Action UpdateLoading);
}

public class SceneBehavior : ISceneBehavior
{
    public readonly Scene Scene;

    public SceneBehavior(Scene scene)
    {
        this.Scene = scene;
    }


    public virtual async UniTask EnterScene(LoadItem item, Action UpdateLoading)
    {
        await UniTask.CompletedTask;
    }

    public virtual async UniTask LeaveScene(LoadItem item, Action UpdateLoading)
    {
        await UniTask.CompletedTask;
    }

    public GameObject FindGameObject(string name)
    {
        var gos = Scene.GetRootGameObjects();
        for (int i = 0; i < gos.Length; i++)
        {
            if (gos[i].name == name) return gos[i];
            var go = gos[i].FindChild(name, false);
            if (go) return go;
        }
        return null;
    }
}





