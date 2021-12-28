using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseBtnItem : MyUIItem
{
    protected override void OnInit()
    {
        this.closeBtn.onClick.AddListener(() => { Debug.Log("CloseBtnItem------------onClick"); });
    }

    protected override void OnClose()
    {
        Debug.Log("CloseBtnItem------------OnClose");
    }

    #region UI_AUTOCODE_RC 3659eb143d9d4f76b9f4dc7028f4b358

    public Button closeBtn;

    protected override void CacheReference()
    {
        var rc = this.gameObject.GetComponent<ReferenceCollector>();
        this.closeBtn = rc.GetReference<Button>(0); // name: CloseButton
    }

    #endregion

}
