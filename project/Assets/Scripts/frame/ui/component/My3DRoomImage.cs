using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public enum DropEventType
{
    None = 0,
    HorizontalRotation = 1,
    VerticalRotation = 0,
    FreeRotation = 0,
}

[AddComponentMenu("UI/My3DRoomImage", 5)]
public class My3DRoomImage : MonoBehaviour, IDragHandler, IPointerClickHandler, IPointerDownHandler
{
    [SerializeField]
    public string resName;
    [SerializeField]
    public bool usePointLight = false;//使用点光源
    [SerializeField]
    public DropEventType dropType = DropEventType.None;
    [HideInInspector]
    public Camera roomCamera;
    [HideInInspector]
    public GameObject rootObj;

    private MyImage _texObj = null;
    private GameObject childGameObject;
    private bool _isInit = false;
    private bool _setCamera = false;
    private bool CameraInitEnd = false;
    private void Update()
    {
        if (!_isInit)
        {
            __initRoom();
        }

        if (_setCamera)
        {
            DoSetCamera();
        }
    }

    private void OnDestroy()
    {
        CameraInitEnd = false;
    }

    private void DoSetCamera()
    {

    }

    private void __initRoom()
    {
        InitContent();
        LoadRoomRes();
        _isInit = true;
    }

    private void LoadRoomRes()
    {
        if (!childGameObject)
        {
            childGameObject = new GameObject();
            if (!MyUITools.roomRoot)
            {
                MyUITools.roomRoot = new GameObject();
                MyUITools.roomRoot.name = "RoomRoot";
            }
            childGameObject.transform.parent = MyUITools.roomRoot.transform;
            childGameObject.name = gameObject.name + "'s childGameObject";

        }
        childGameObject.transform.position = new Vector3(UnityEngine.Random.Range(-100, -1000),
            UnityEngine.Random.Range(-100, -1000) - 1000, UnityEngine.Random.Range(-100, -1000));
        childGameObject.SetActive(true);
        if (!rootObj)
        {
            rootObj = new GameObject();
            rootObj.transform.parent = childGameObject.transform;
            rootObj.name = "rootObj";
        }
        rootObj.transform.localPosition = Vector3.zero;
        rootObj.transform.localRotation = Quaternion.identity;
        rootObj.SetActive(true);

        if (!string.IsNullOrEmpty(resName) && resName != "0")
        {
            var prefab = Resources.Load<GameObject>(resName);
            var go = Instantiate(prefab);
            if (!this || !gameObject || !gameObject.activeInHierarchy) return;
            go.transform.parent = rootObj.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            CreatePointLight();
            StartCoroutine(SetCameraTexture());
        }

    }

    RenderTexture _renderTexture;
    public string ResName
    {
        get => resName;
        set
        {
            bool isInit = value != resName;
            resName = value;
            if (isInit)
            {
                __initRoom();
            }
        }
    }

    /// <summary>
    /// 将摄像机照到的内容映射到image中
    /// </summary>
    /// <returns></returns>
    IEnumerator SetCameraTexture()
    {
        if (_texObj)
        {
            _texObj.texture = null;
            _texObj.enabled = false;
        }
        if (!this || !gameObject || !gameObject.activeInHierarchy) yield break;
        if (!roomCamera)
        {
            var camObj = childGameObject.FindInChild<Camera>();
            if (!camObj)
            {
                roomCamera = childGameObject.AddChild<Camera>();
                roomCamera.orthographic = true;
                roomCamera.farClipPlane = 10;
                roomCamera.nearClipPlane = -10;
                roomCamera.renderingPath = RenderingPath.Forward;
                roomCamera.useOcclusionCulling = false;
            }

        }
        roomCamera.clearFlags = CameraClearFlags.SolidColor;
        roomCamera.backgroundColor = new Color(0, 0, 0, 0);
        roomCamera.allowMSAA = false;
        roomCamera.targetTexture = null;
        roomCamera.useOcclusionCulling = true;
        roomCamera.depth = -10;
        roomCamera.enabled = false;
        yield return null;
        roomCamera.enabled = true;

        while (!roomCamera.enabled)
            yield return 50;

        if (!_renderTexture)
        {
            var _camera_size = _texObj.rectTransform.rect.size;
            float accuaryValue = 1f;
            _renderTexture = RenderTexture.GetTemporary((int)(_camera_size.x * accuaryValue),
                (int)(_camera_size.y * accuaryValue), 24, RenderTextureFormat.Default);
            _renderTexture.useMipMap = false;
        }
        if (_renderTexture)
            roomCamera.targetTexture = _renderTexture;
        roomCamera.depth = -99;
        roomCamera.gameObject.tag = "Untagged";
        yield return null;
        if (_texObj)
            _texObj.enabled = true;
        yield return null;
        if (roomCamera && _texObj && this && gameObject && gameObject.activeInHierarchy)
        {
            if (_renderTexture)
                _texObj.texture = roomCamera.targetTexture;
            SendMessageUpwards("OnUIRoomLoadComlete", this);
        }
        _texObj.color = new Color(_texObj.color.r, _texObj.color.g, _texObj.color.b, 1);
        CameraInitEnd = true;
    }
    /// <summary>
    /// 创建灯光
    /// </summary>
    private void CreatePointLight()
    {

    }

    private void InitContent()
    {
        _texObj = GetComponent<MyImage>();
        if (!_texObj)
            _texObj = gameObject.AddComponent<MyImage>();
        _texObj.color = new Color(_texObj.color.r, _texObj.color.g, _texObj.color.b, 0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dropType == DropEventType.None)
            return;
        var target = rootObj;
        if (target)
        {
            var rat = target.transform.localRotation.eulerAngles;
            var rats_y = rat.y;
            var rats_z = rat.z;

            if (dropType == DropEventType.HorizontalRotation || dropType == DropEventType.FreeRotation)
                rats_y -= ((eventData.delta).x / Screen.width) * 360f;
            if (dropType == DropEventType.VerticalRotation || dropType == DropEventType.FreeRotation)
                rats_z += ((eventData.delta).y / Screen.height) * 360f;

            target.transform.localRotation = Quaternion.Euler(new Vector3(rat.x, rats_y, rats_z));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }
}