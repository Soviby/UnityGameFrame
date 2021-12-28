
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class MyGUIManager : Singleton<MyGUIManager>, IUpdateable
{
    private GameObject parent;
    public GameObject Parent { get => parent; }

    Dictionary<Type, PanelBase> m_panelMap = null;
    List<PanelBase> panelBases;

    protected override void OnInitialized()
    {
        parent = new GameObject("UI Root", typeof(RectTransform));
        GameObject.DontDestroyOnLoad(parent);
    }

    public T Show<T>() where T : MyPanel, new()
    {
        var panel = GetOrCreatePanel<T>();
        panel.Show();
        return panel;
    }

    public void AsyncShow<T>(Action<T> callback) where T : MyPanel, new()
    {
        var panel = GetOrCreatePanel<T>();
        panel.AsyncShow(() =>
        {
            callback(panel);

        }).Forget();
    }

    public T Hide<T>() where T : MyPanel, new()
    {
        var panel = GetExistPanel<T>();
        panel?.Hide();
        return panel;
    }

    public T GetOrCreatePanel<T>() where T : PanelBase, new()
    {
        if (m_panelMap == null) m_panelMap = new Dictionary<Type, PanelBase>();
        if (m_panelMap.ContainsKey(typeof(T)))
            return m_panelMap[typeof(T)] as T;
        T panel = new T();
        m_panelMap[typeof(T)] = panel;
        return panel;
    }
    public T GetExistPanel<T>() where T : PanelBase, new()
    {
        if (m_panelMap == null) return null;
        if (m_panelMap.ContainsKey(typeof(T)))
            return m_panelMap[typeof(T)] as T;
        return null;
    }
    public void AddPanelObject(MyPanel panel)
    {
        if (!panel.gameObject) return;
        panel.gameObject.transform.SetParent(Parent.transform);
    }

    public void Update()
    {
        if (m_panelMap != null)
        {
            panelBases = m_panelMap.Values.ToList();
            for (int i = 0; i < panelBases.Count; i++)
            {
                try
                {
                    panelBases[i].myTaskRunner.Update();
                    panelBases[i].Update();
                }
                catch (System.Exception err)
                {
                    Debug.LogError($"Mytask.UpdateAll,task:{panelBases[i].GetType().FullName} ,error:{err.Message},{err.StackTrace}");
                }

            }
        }
    }
}

