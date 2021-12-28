using System.Collections.Generic;
using System.IO;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MyPanel : PanelBase
{
    protected string _panelResName;
    MyPanelEventListener _listener = null;

    private Dictionary<string, List<MyUIItem>> _uiItemMap = new Dictionary<string, List<MyUIItem>>();
    public void Show()
    {
        bool isInit = !_gameObject;
        _build(Resources.Load<GameObject>(@"UI\Prefabs\Panel\" + _panelResName), isInit);
    }

    public async UniTask AsyncShow(Action callback)
    {
        bool isInit = !_gameObject;
        var asset = await Resources.LoadAsync<GameObject>(@"UI\Prefabs\Panel\" + _panelResName);
        _build(asset, isInit, callback);
    }

    public void Hide()
    {
        _hide();
    }

    public void Close()
    {
        OnClose();

        myTaskRunner.Stop();
        _closeItems();
        GameObject.Destroy(_gameObject);
    }

    private void _hide()
    {
        if (!IsVisible) return;
        if (_gameObject)
            _gameObject.SetActive(false);

        OnHide();
    }

    private void _initUIItems()
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
                    uiItem.Init(config.gameObject);
                    if (!_uiItemMap.ContainsKey(config.uiItemClassName))
                        _uiItemMap.Add(config.uiItemClassName, new List<MyUIItem>());
                    _uiItemMap[config.uiItemClassName].Add(uiItem);
                }
            }
        }
    }

    private void _closeItems()
    {
        foreach (var kv in _uiItemMap)
        {
            kv.Value.ForEach((item) => { item.Close(); });
        }
    }

    private void _build(UnityEngine.Object asset, bool isInit, Action callback = null)
    {
        if (isInit)
        {
            _gameObject = GameObject.Instantiate((GameObject)asset);
            _buildPanel();
            CacheReference();
            _init();
            OnInit();
        }
        _show();
        callback?.Invoke();
    }

    private void _show()
    {
        _gameObject.SetActive(true);
        OnShow();
    }
    private void _init()
    {
        var canvas = _gameObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCamera;
        myTaskRunner.Stop();

        _initUIItems();
    }
    private void _buildPanel()
    {
        if (!_gameObject) Debug.Log($"{_panelResName},Resources中找不到");
        _gameObject.name = this._panelResName;
        _gameObject.transform.position = Vector3.zero;
        //加入管理
        MyGUIManager.Instance.AddPanelObject(this);

        if (_listener == null)
            _listener = new MyPanelEventListener();
        var eventBase = _gameObject.AddComponent<IUIEvent>();
        _listener.OnInit(this, eventBase);

    }
}
