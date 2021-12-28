using UnityEngine;
using UnityEditor;

public static class GameObjectUtils 
{
    /// <summary>
    /// 得到组件，如果没有则会创建
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static T GetMissComponent<T>(this GameObject go)where T: Component
    {
        var t=go.GetComponent<T>();
        if (t)
        {
            return t;
        }
        else {
            return go.AddComponent<T>();
        }
    }
    public static void SetLayerRecursively(this GameObject go,int layer ,uint mask) {
        GameObject[] buf = new GameObject[1024];
        int n = 0;
        buf[n++] = go;
        while (n>0) {
            go = buf[--n];
            if (((1 << go.layer) & mask) != 0)
            {
                go.layer = layer;
            }
            foreach (Transform t in go.transform)//??
                buf[n++] = t.gameObject;
        }

    }
    public static void SetLayerRecursively(this GameObject go, int layer)
    {
        SetLayerRecursively(go,layer,0xffffffff);
    }
}