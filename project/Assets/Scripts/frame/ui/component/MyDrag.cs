using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
namespace UnityEngine.UI
{
    //要开放  拖动开始时，拖动过程中，拖动结束时的接口
    //拖动组件，返回拖动的信息
    //public class MyDrag : MonoBehaviour,IPointerDownHandler, IPointerUpHandler, IDrag
    public class MyDrag : MonoBehaviour,IDrag, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        //bool isDrag = false;
        MyDragData myDragData = new MyDragData();
        Vector2 pos = Vector3.zero;
        Vector3 before_pos ;
        RectTransform parent_rect;
        RectTransform rect;

        private void OnEnable()
        {
            before_pos = transform.position;
            parent_rect = gameObject.transform.parent as RectTransform;
            rect = gameObject.transform as RectTransform;
        }

        public void OnDragChange()
        {
            //if (isDrag)
            {
                myDragData.go = gameObject;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parent_rect,
                    new Vector2(Input.mousePosition.x,Input.mousePosition.y),
                    Camera.main,
                    out pos);
                myDragData.ui_cur_Pos = transform.position;
                myDragData.ui_before_Pos = before_pos;
                rect.anchoredPosition = pos;
                SendMessageUpwards("OnDragEvent", myDragData);
            }
        }

        //public void OnPointerDown(PointerEventData eventData)
        //{

        //    if (eventData.dragging)
        //    {
        //        before_pos = transform.position;
        //        isDrag = true;
        //        OnDragStart();
        //    }
        //}

        //public void OnPointerUp(PointerEventData eventData)
        //{
        //    if (isDrag)
        //    {
        //        OnDragEnd();
        //        isDrag = false;
        //    } 
            
        //}
        //private void Update()
        //{
        //    if (isDrag == true)
        //        OnDragChange();
        //}

        public void OnDragStart()
        {
            myDragData.go = gameObject;
            myDragData.ui_cur_Pos = Vector3.zero;
            myDragData.ui_before_Pos = before_pos;
            SendMessageUpwards("OnDragStartEvent", myDragData);
        }

        public void OnDragEnd()
        {
            SendMessageUpwards("OnDragEndEvent", myDragData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnDragStart();
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragChange();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnDragEnd();
        }
    }

    public class MyDragData
    {
        public GameObject go;//被拖动的物体
        public Vector2 ui_cur_Pos;//当前坐标
        public Vector2 ui_before_Pos;//原坐标

    }
    //拖动开始时，拖动过程中，拖动结束时的接口
    public interface IDrag
    {
        //拖动过程中
        void OnDragChange();
        //拖动开始时
        void OnDragStart();
        //拖动结束时
        void OnDragEnd();
    }
}
