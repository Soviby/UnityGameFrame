
using System.IO;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MyPanel : PanelBase
{
    MyPanelEventListener _listener = null;
    private UIConfig uIConfig;

    public void Show(UIConfig uIConfig)
    {
        this.uIConfig = uIConfig;
        bool isInit = !_gameObject;
        _build(Resources.Load<GameObject>(@"UI\Prefabs\Panel\" + this.uIConfig.resName), isInit);
    }

    public async UniTask AsyncShow(UIConfig uIConfig, Action callback)
    {
        this.uIConfig = uIConfig;
        bool isInit = !_gameObject;
        var asset = await Resources.LoadAsync<GameObject>(@"UI\Prefabs\Panel\" + this.uIConfig.resName);
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
        itemData.ClearUIItem();
        GameObject.Destroy(_gameObject);
    }

    private void _hide()
    {
        if (!IsVisible) return;
        if (_gameObject)
            _gameObject.SetActive(false);

        OnHide();
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

        initRC();
    }
    private void _buildPanel()
    {
        if (!_gameObject) Debug.Log($"{ this.uIConfig.resName},Resources中找不到");
        _gameObject.name = this.uIConfig.resName;
        _gameObject.transform.position = Vector3.zero;
        //加入管理
        MyGUIManager.Instance.AddPanelObject(this);

        if (_listener == null)
            _listener = new MyPanelEventListener();
        var eventBase = _gameObject.AddComponent<IUIEvent>();
        _listener.OnInit(this, eventBase);

    }
}
