 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public interface IPanel {
    bool IsVisible { get; }
}
public class PanelBase : IPanel
{
    protected GameObject _gameObject;

    public virtual bool IsVisible {
        get {
            return false;
        }
    }

    public GameObject gameObject { get => _gameObject;  }
    public Camera uiCamera { get => CameraManager.Instance.UiCamera;  }
    public MyTaskRunner myTaskRunner = new MyTaskRunner(5);

    public virtual void OnClick(MonoBehaviour behaviour) {

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