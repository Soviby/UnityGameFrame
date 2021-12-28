using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 namespace UnityEngine.UI
{
    public abstract class MyHorizontalOrVerticalLayoutGroup : HorizontalOrVerticalLayoutGroup
    {
        protected RectTransform _seed = null;
        public List<RectTransform> children { get { return rectChildren; } }

        public virtual void InitChildren(int count ,System.Action<int,RectTransform>  initItemCallback=null) {
            if (rectChildren.Count == 0)
                CalculateLayoutInputHorizontal();
            _initChildCount(count);
            for (int i = 0; i < rectChildren.Count; ++i)
            {
                initItemCallback?.Invoke(i,rectChildren[i]);
            }

        }

        void _initChildCount(int num) {
            //清除节点
            if (num < rectChildren.Count)
            {
                if (num < rectChildren.Count)
                {
                    if (num == 0)
                    {
                        if (rectChildren.Count > 0)
                        {
                            _seed = rectChildren[0];
                            _seed.gameObject.SetActive(false);
                            for (int i = 1; i < rectTransform.childCount; ++i)
                            {
                                GameObject.Destroy(rectChildren[i].gameObject);
                            }

                        }
                        rectChildren.Clear();
                    }
                }
                else {
                    for (int i = num; i < rectChildren.Count; ++i)
                    {
                        GameObject.Destroy(rectChildren[i].gameObject);
                    }
                    rectChildren.RemoveRange(num,rectChildren.Count -num);
                }
            }
            //添加节点
            var n = rectChildren.Count;
            for (int i=n;i<num;++i) {
                RectTransform newItem = null;
                if (rectChildren.Count == 0)
                {
                    if (_seed)
                    {
                        _seed.gameObject.SetActive(true);
                        newItem = _seed;
                        _seed = null;
                    }
                    else
                    {
                        Debug.LogError("not seek , cannot add new item");
                        var go = new GameObject();
                        newItem = go.AddComponent<RectTransform>();
                    }
                }
                else
                {
                    var go = GameObject.Instantiate(rectChildren[rectChildren.Count -1].gameObject,gameObject.GetRectTransform());
                    //var itws = go.GetComponentsInChildren<iTween>();
                    //for (int j = 0; j < itws.Length; ++j)
                    //    GameObject.Destroy(itws[j]);
                    newItem = go.GetRectTransform();
                }
                rectChildren.Add(newItem);
            }
        }

        


    }
}
