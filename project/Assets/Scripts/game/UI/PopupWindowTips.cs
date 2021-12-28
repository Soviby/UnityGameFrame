using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupWindowTips : MyPanel
{
    Action okEvent;
    Action closeEvent;
    public PopupWindowTips()
    {
        _panelResName = "PopupWindowTips";
    }

    protected override void OnInit()
    {
        oKButton.onClick.AddListener(() => { okEvent?.Invoke(); });
        closeButton.onClick.AddListener(() => { closeEvent?.Invoke(); this.Close(); });
    }

    protected override void OnShow()
    {

    }

    public void SetData(string content, string ok_text = "确定", string header_text = "提示", Action okEvent = null, Action closeEvent = null)
    {
        this.content.text = content;
        this.ok.text = ok_text;
        this.header.text = header_text;
        this.okEvent = okEvent;
        this.closeEvent = closeEvent;
    }

    #region UI_AUTOCODE_RC f8b34bd8203a4b778018fcbbb5abb9bb

    public TextMeshProUGUI header;
    public Button oKButton;
    public Button closeButton;
    public TextMeshProUGUI ok;
    public TextMeshProUGUI content;

    protected override void CacheReference()
    {
        var rc = this.gameObject.GetComponent<ReferenceCollector>();
        this.header = rc.GetReference<TextMeshProUGUI>(0); // name: Header
        this.oKButton = rc.GetReference<Button>(1); // name: OKButton
        this.closeButton = rc.GetReference<Button>(2); // name: CloseButton
        this.ok = rc.GetReference<TextMeshProUGUI>(3); // name: Text (TMP)
        this.content = rc.GetReference<TextMeshProUGUI>(4); // name: Text (TMP)
    }

    #endregion

}
