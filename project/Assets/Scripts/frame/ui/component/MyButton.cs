using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
namespace UnityEngine.UI
{
    //如果按键没响应，看下是否有加EventSystem组件，
    //该组件身上包括EventSystem和Standalone Input Module
    //这三种都可以
    //public class MyButton:Button,IPointerDownHandler,IPointerUpHandler
    //public class MyButton:Button, IPointerClickHandler
    public class MyButton:Button
    {
        public override void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("OnPointerDown--");
            SendMessageUpwards("__OnPointerDown",this);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("OnPointerUp--");
            SendMessageUpwards("__OnPointerUp", this);
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("OnPointerClick--");
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            SendMessageUpwards("OnClickEvent", this);
        }
        
    }
}
