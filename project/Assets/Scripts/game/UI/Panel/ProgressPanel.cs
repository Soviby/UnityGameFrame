using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ProgressPanel : MyPanel
{

    public ProgressPanel()
    {

    }

    protected override void OnInit()
    {

    }

    protected override void OnShow()
    {

    }

    public void SetData(float progress)
    {
        percentage.text = (int)(progress * 100) + "%";
        fill.fillAmount = progress;

        fill.DOFillAmount(progress, 0.4f);
    }

    protected override void OnClose()
    {
        fill.DOKill(true);
    }

    #region UI_AUTOCODE_RC db20d46dfdc0460db45d36df73e0da39

    public Image fill;
    public TextMeshProUGUI percentage;

    public void CacheReference()
    {
        var rc = this.gameObject.GetComponent<ReferenceCollector>();
        this.fill = rc.GetReference<Image>(0); // name: Fill
        this.percentage = rc.GetReference<TextMeshProUGUI>(1); // name: Percentage
    }

    #endregion

}
