
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public interface IPanel
{
    bool IsVisible { get; }
}

public class PanelBase : IPanel
{
    protected GameObject _gameObject;

    protected MyUIItemData itemData = new MyUIItemData();

    public bool IsVisible
    {
        get
        {
            return _gameObject && _gameObject.activeSelf;
        }
    }

    public GameObject gameObject { get => _gameObject; }
    public Camera uiCamera { get => CameraManager.Instance.UiCamera; }
    public MyTaskRunner myTaskRunner = new MyTaskRunner(5);


    protected void initRC()
    {
        var rc = this._gameObject.GetComponent<ReferenceCollector>();
        if (rc)
        {
            foreach (var data in rc.data)
            {
                if (data.gameObject.GetType() == typeof(UIItemConfig))
                {
                    var config = (UIItemConfig)data.gameObject;
                    var uiItem = Activator.
                                CreateInstance(Type.GetType(config.uiItemClassName))
                                as MyUIItem;
                    itemData.AddUIItem(uiItem);

                    uiItem.Init(config.gameObject);
                }
            }
        }
    }


    public T AddUIItem<T>(Transform root) where T : MyUIItem, new()
    {
        var config = MyGUIManager.Instance.GetItemConfigByClassType<T>();
        if (config == null)
        {
            return null;
        }
        var go = GameObject.Instantiate(Resources.Load<GameObject>(@"UI\Prefabs\UIItem\" + config.resName));
        if (root)
        {
            go.transform.SetParent(root, false);
        }
        var uiItem = new T();
        itemData.AddUIItem(uiItem);
        uiItem.Init(go);

        return uiItem;
    }

    public virtual void OnClick(MonoBehaviour behaviour)
    {

    }

    protected virtual void OnClose()
    {

    }

    protected virtual void OnInit()
    {

    }

    protected virtual void CacheReference()
    {

    }

    protected virtual void OnShow()
    {

    }
    protected virtual void OnHide()
    {

    }

    public virtual void OnDrag(MyDragData myDragData)
    {

    }

    public virtual void OnDragStart(MyDragData myDragData)
    {

    }

    public virtual void OnDragEnd(MyDragData myDragData)
    {

    }

    public virtual void OnUIRoomLoadComlete(My3DRoomImage my3DRoomImage)
    {

    }

    public virtual void Update()
    {

    }

    protected void RunUITask(IEnumerator e)
    {
        myTaskRunner.Run(e);
    }


}