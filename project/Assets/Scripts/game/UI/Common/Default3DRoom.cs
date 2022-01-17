
using UnityEngine;
/// <summary>
/// MyRawImage 的默认配置
/// </summary>
public class Default3DRoom
{
    public GameObject gameObject;


    public void SetData(GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.CacheReference();
    }

    #region UI_AUTOCODE_RC b536b98c1c3042a19a01f0963cc81a74

    public Camera camera;
    public Transform root;

    public void CacheReference()
    {
        var rc = this.gameObject.GetComponent<ReferenceCollector>();
        this.camera = rc.GetReference<Camera>(0); // name: Camera
        this.root = rc.GetReference<Transform>(1); // name: Root
    }

    #endregion

}