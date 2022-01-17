using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class TaskTips : MyPanel
{

    public TaskTips()
    {

    }

    protected override void OnInit()
    {
        text.text = "";
        quan.fillAmount = 0;
    }

    protected override void OnShow()
    {
        WaitQuan().Forget();
    }

    async UniTask WaitQuan()
    {
        var waitTime = text.AutoText("hhhhhhhhhhhhhhhhhhhhhhhhhhh", 0.1f);
        await UniTask.Delay((int)(waitTime * 1000));
        quan.fillAmount = 0;
        while (quan.fillAmount < 1)
        {
            await UniTask.DelayFrame(1);
            quan.fillAmount += Time.deltaTime * 0.2f;
        }

    }

    #region UI_AUTOCODE_RC 14ce2960d72542188f26677970fb02a9

    public MyText text;
    public MyImage quan;

    public void CacheReference()
    {
        var rc = this.gameObject.GetComponent<ReferenceCollector>();
        this.text = rc.GetReference<MyText>(0); // name: text
        this.quan = rc.GetReference<MyImage>(1); // name: quan
    }

    #endregion

}
