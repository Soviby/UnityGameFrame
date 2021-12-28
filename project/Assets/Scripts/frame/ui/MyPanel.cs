using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MyPanel : PanelBase
{
    protected string _panelResName;
    MyPanelEventListener _listener = null;
    public override bool IsVisible => gameObject && gameObject.activeSelf;

    public void Show()
    {
        bool isInit = !gameObject;
        _build(Resources.Load<GameObject>(@"UI\Prefabs\Panel\" + _panelResName), isInit);
    }

    public async UniTask AsyncShow(Action callback)
    {
        bool isInit = !gameObject;
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
        GameObject.Destroy(gameObject);
    }

    private void _hide()
    {
        if (!IsVisible) return;
        if (gameObject)
            gameObject.SetActive(false);

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
        gameObject.SetActive(true);
        OnShow();
    }
    private void _init()
    {
        var canvas = _gameObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCamera;

        myTaskRunner.Stop();
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
