using UnityEngine;
using UnityEngine.UI;
public class MyUIItem : PanelBase
{
    // 1. 打开父界面自动初始化  ，打开时调用 show， 隐藏 hide ， 父界面销毁时 销毁所有子界面

    // 2. 手动向父界面上添加子界面，需要自己加载资源，加载好之后调用初始化 ，打开时调用 show， 隐藏 hide ， 父界面销毁时 销毁所有子界面


    public void Init(GameObject go)
    {
        _gameObject = go;
        this.CacheReference();
        this.OnInit();
    }

    public void Close()
    {
        this.OnClose();
    }

}