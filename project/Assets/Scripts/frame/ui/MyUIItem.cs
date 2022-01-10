using UnityEngine;
using UnityEngine.UI;
public class MyUIItem : PanelBase
{

    public void Init(GameObject go)
    {
        _gameObject = go;


        initRC();
        this.CacheReference();
        this.OnInit();
    }

    public void Close()
    {
        this.OnClose();
    }

}