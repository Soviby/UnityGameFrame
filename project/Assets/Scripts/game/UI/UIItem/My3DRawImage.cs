using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 3D贴图
/// </summary>
public class My3DRawImage : MyUIItem
{
    private Default3DRoom room;
    private RenderTexture rt;

    protected override void OnClose()
    {
        if (this.rt)
        {
            this.rt.Release();
        }
        if (this.room != null)
        {
            if (this.room.gameObject)
                GameObject.Destroy(this.room.gameObject);
            this.room = null;
        }
    }

    private void BuidRoom(string resName)
    {
        if (room != null)
            return;
        room = new Default3DRoom();
        var go = GameObject.Instantiate(Resources.Load<GameObject>(@"UI\Prefabs\3DRoom\" + resName));
        if (go)
        {
            var __3DRoom__ = GameObject.Find("__3DRoom__");
            if (!__3DRoom__)
            {
                __3DRoom__ = new GameObject("__3DRoom__");
                __3DRoom__.transform.localPosition = Vector3.zero;
                __3DRoom__.transform.localRotation = Quaternion.identity;
                __3DRoom__.transform.localScale = Vector3.one;

                GameObject.DontDestroyOnLoad(__3DRoom__);
            }
            go.transform.SetParent(__3DRoom__.transform, false);
        }
        room.SetData(go);
    }


    public void SetData(string resName, GameObject target)
    {
        BuidRoom(resName);
        this.rt = new RenderTexture(800, 800, 16, RenderTextureFormat.ARGB32);
        this.myRawImage.texture = this.rt;
        this.room.camera.targetTexture = this.rt;

        target.transform.SetParent(room.root, false);
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;
        target.transform.localScale = Vector3.one;
    }

    #region UI_AUTOCODE_RC a46d4cc3e1354ac7902b56f1ac16236a

    public RawImage myRawImage;

    public void CacheReference()
    {
        var rc = this.gameObject.GetComponent<ReferenceCollector>();
        this.myRawImage = rc.GetReference<RawImage>(0); // name: MyRawImage
    }

    #endregion

}