
using UnityEngine;
public class MyUIItem : PanelBase
{

    public void Init(GameObject go)
    {
        _gameObject = go;


        initRC();
        ReferenceCollectorHelper.CacheReferenceHandle(this);
        this.OnInit();
    }

    public void Close()
    {
        this.OnClose();
    }

}