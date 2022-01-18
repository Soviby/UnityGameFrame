using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestPanel : MyPanel
{
    private GameObject target;

    protected override void OnInit()
    {

        this.closeButton.onClick.AddListener(() => { this.Close(); });
        this.target = GameObject.Instantiate(Resources.Load<GameObject>(@"Model\Cube"));
        this.target.transform.DOLocalRotate(new Vector3(0, 180, 0), 5).SetEase(Ease.Linear).SetLoops(-1);
        this.myRawImage.SetData("Default3DRoom", this.target);


        // this.myVideoRawImage.SetData("syf", MyVideoRawImage.VideoImageType.path);
        // this.myVideoRawImage.Play();
        this.myVideoRawImage.SetData(path: @"http://vfx.mtime.cn/Video/2019/03/18/mp4/190318231014076505.mp4", type: MyVideoRawImage.VideoImageType.url);
        this.myVideoRawImage.Play();
    }

    protected override void OnClose()
    {
        if (this.target)
        {
            GameObject.Destroy(this.target);
            this.target = null;
        }
    }

    #region UI_AUTOCODE_RC a89c89c4fd9f4e2887d42c335ffd9441

    public Button closeButton;
    public My3DRawImage myRawImage;
    public MyVideoRawImage myVideoRawImage;

    public void CacheReference()
    {
        var rc = this.gameObject.GetComponent<ReferenceCollector>();
        this.closeButton = rc.GetReference<Button>(0); // name: CloseButton
    }

    #endregion

}
