using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine.UI
{
    public class MyPanelEventListener
    {
        PanelBase _event;
        IUIEvent _eventBase;
        public void OnInit(PanelBase panel, IUIEvent eventBase)
        {
            _event = panel;
            _eventBase = eventBase;

            _eventBase.EventOnClick += OnClickEvent;
            _eventBase.EventOnDrag += OnDragEvent;
            _eventBase.EventOnDragStart += OnDragStartEvent;
            _eventBase.EventOnDragEnd += OnDragEndEvent;
            _eventBase.EventOnUIRoomLoadComlete += OnUIRoomLoadComlete;
        }

        void OnClickEvent(MonoBehaviour behaviour)
        {
            _event.OnClick(behaviour);
        }

        void OnDragEvent(MyDragData myDragData)
        {
            _event.OnDrag(myDragData);
        }
        void OnDragStartEvent(MyDragData myDragData)
        {
            _event.OnDragStart(myDragData);
        }
        void OnDragEndEvent(MyDragData myDragData)
        {
            _event.OnDragEnd(myDragData);
        }
        void OnUIRoomLoadComlete(My3DRoomImage my3DRoomImage)
        {
            _event.OnUIRoomLoadComlete(my3DRoomImage);
        }

    }




}
