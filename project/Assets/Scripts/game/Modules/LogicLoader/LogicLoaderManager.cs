
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// 登录相关
/// </summary>
public class LogicLoaderManager : Singleton<LogicLoaderManager>
{

    public void StartGame()
    {
        MyTask.Run(_StartGame());
    }


    IEnumerator _StartGame()
    {
        //让主场景中所有的物体不可删除
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var gos = scene.GetRootGameObjects();
        gos.ForEach(go => GameObject.DontDestroyOnLoad(go));
        //初始化音乐
        SoundManager.Init();
        while (!SoundManager.InitOK) { yield return null; }
        //加载资源

        yield return MyTask.Call(DBManager.LoadConfig());

        //模块初始化
        GameLogicInit.Start();

        //进主场景
        GameSceneManager.Instance.EnterScene<MainScene>();
    }
}


