using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainScene : SceneBehavior
{
    GameObject voicer;
    public MainScene(Scene _scene) : base(_scene)
    {

    }

    public override async UniTask EnterScene(LoadItem item, Action UpdateLoading)
    {
        Debug.Log($"EnterScene--MainScene,Scene.path:{this.Scene.path}");
        voicer = this.FindGameObject("voicer");
        item.laod_value = 0.5f;
        UpdateLoading();
        await UniTask.Delay(3000);
        item.laod_value = 0.8f;
        UpdateLoading();
        MyGUIManager.Instance.Show<PopupWindowTips>().SetData(content: "您好！", okEvent:
        () =>
        {
            Debug.Log("Test!");
        });
        // MyGUIManager.Instance.AsyncShow<PopupWindowTips>((ui) =>
        // {
        //     ui.SetData(content: "您好！!!!!!!!!!!!!", okEvent:
        //     () =>
        //     {
        //         Debug.Log("Test!----------------");
        //     });
        // });
        await UniTask.Delay(3000);
        item.laod_value = 1f;
        UpdateLoading();
        SoundManager.PlayMusic("event:/bgm_01");
        //SoundManager.PlaySound("event:/bianshen_effect");
        //SoundManager.PlaySound("event:/3d_effect", voicer);
    }

    public override async UniTask LeaveScene(LoadItem item, Action UpdateLoading)
    {
        Debug.Log("LeaveScene--MainScene");
    }

}

