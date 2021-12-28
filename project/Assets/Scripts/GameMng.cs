
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMng : MonoSingleton<GameMng>
{
    private void Start()
    {
        //开始游戏
        LogicLoaderManager.Instance.StartGame();
    }

    private void OnApplicationQuit()
    {
        //游戏关闭
        SoundManager.StopAllSound();
    }

    private void Update()
    {
        for (int i = 0; i < Global.Updateables.Count; i++)
        {
            try
            {
                Global.Updateables[i].Update();
            }
            catch (System.Exception err)
            {
                Debug.LogError($"Mytask.UpdateAll,task:{Global.Updateables[i].GetType().FullName} ,error:{err.Message},{err.StackTrace}");
            }
        }

    }

}
