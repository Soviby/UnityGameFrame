
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class MyGUIManager : Singleton<MyGUIManager>, IUpdateable
{

    #region Ui配置
    Dictionary<string, UIConfig> panelCofigMap = new Dictionary<string, UIConfig>()
    {
        ["ProgressPanel"] =
        new UIConfig()
        {
            resName = "ProgressPanel",
            className = "ProgressPanel",
        },
        ["PopupWindowTips"] =
        new UIConfig()
        {
            className = "PopupWindowTips",
        },
    };

    public UIConfig GetPanelConfigByClassType<T>() where T : MyPanel
    {
        UIConfig config = null;
        panelCofigMap.TryGetValue(typeof(T).Name, out config);

        if (config != null)
        {
            config.resName = string.IsNullOrWhiteSpace(config.resName) ? typeof(T).Name : config.resName;
        }
        else
        {
            Debug.LogError($"ui panel config no find,  {typeof(T).Name} ");
        }
        return config;
    }

    Dictionary<string, UIConfig> itemCofigMap = new Dictionary<string, UIConfig>()
    {
        ["CloseButton"] =
        new UIConfig()
        {
            className = "CloseButton",
            resName = "CloseButton",
        },

    };

    public UIConfig GetItemConfigByClassType<T>() where T : MyUIItem
    {
        UIConfig config = null;
        itemCofigMap.TryGetValue(typeof(T).Name, out config);
        if (config != null)
        {
            config.resName = string.IsNullOrWhiteSpace(config.resName) ? typeof(T).Name : config.resName;
        }
        else
        {
            Debug.LogError($"ui panel config no find,  {typeof(T).Name} ");
        }
        return config;
    }
    #endregion



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
        var config = GetPanelConfigByClassType<T>();
        if (config == null)
        {
            return null;
        }
        panel.Show(config);
        return panel;
    }

    public void AsyncShow<T>(Action<T> callback) where T : MyPanel, new()
    {
        var panel = GetOrCreatePanel<T>();
        var config = GetPanelConfigByClassType<T>();
        if (config == null)
        {
            return;
        }
        panel.AsyncShow(config, () =>
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

    private T GetOrCreatePanel<T>() where T : PanelBase, new()
    {
        if (m_panelMap == null) m_panelMap = new Dictionary<Type, PanelBase>();
        if (m_panelMap.ContainsKey(typeof(T)))
            return m_panelMap[typeof(T)] as T;
        T panel = new T();
        m_panelMap[typeof(T)] = panel;
        return panel;
    }
    private T GetExistPanel<T>() where T : PanelBase, new()
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

public class UIConfig
{
    public string resName = "";  // @"UI\Prefabs\Panel\" + _panelResName
    public string className = "";
}

