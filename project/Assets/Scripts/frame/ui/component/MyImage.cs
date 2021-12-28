using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/MyImage",0)]
public class MyImage : Image
{
    [SerializeField]
    private bool isFade = false;
    [SerializeField]
    private Texture _tex;
    [SerializeField]
    public Color _color = Color.white;
    [SerializeField]
    public bool m_x_reversal =false;//x轴翻转
    [SerializeField]
    public bool m_y_reversal = false;//y轴翻转
    [SerializeField]
    public float _scale = 1f;
    public bool IsFade { get => isFade; set { isFade = value; OnInit(); }  }

    public override Material material
    {
        get
        {
            if (isFade)
            {
                return m_fade_ui_mat;
            }
            else
            {
                return base.material;
            }
        }
    }

    static Material _fade_ui_mat = null;
    public static Material m_fade_ui_mat
    {
        get
        {
            if (!_fade_ui_mat)
            {
                _fade_ui_mat = new Material(Shader.Find("自定义/灰度"));
                _fade_ui_mat.hideFlags = HideFlags.DontSave;
                _fade_ui_mat.enableInstancing = true;
                _fade_ui_mat.name = "UI_Img Fade Alpha";
            }
            return _fade_ui_mat;
        }
    }


    static Material _default_ui_mat = null;
    public static Material m_default_ui_mat
    {
        get
        {
            if (!_default_ui_mat)
            {
                _fade_ui_mat = new Material(Shader.Find("UI/Default"));
                _fade_ui_mat.hideFlags = HideFlags.DontSave;
                _fade_ui_mat.enableInstancing = true;
                _fade_ui_mat.name = "UIDefault";
            }
            return _default_ui_mat;
        }
    }

    public override Texture mainTexture
    {
        get {

            if (_tex != null)
            {
                return _tex;
            }
            return base.mainTexture;

        }
    }

    public Texture texture { get => _tex; set { _tex = value; OnInit(); } }

    void OnInit()
    {
        if (!gameObject.activeInHierarchy) return;

        SetAllDirty();

    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        if (_tex != null)
        {
            var border = Vector4.zero;
            var uv = ReversalUV(new Vector4(0,0,1,1));
            var size = new Vector2(_tex.width, _tex.height);
            var pos = GetDrawingDimensions(Vector4.zero,size);
            if (_scale != 1f)
            {
                pos = pos * _scale;
            }

            AddSprite(vh, pos, uv,uv,border,true);
        }
    }

    private Vector4 GetDrawingDimensions(Vector4 padding,Vector2 imgSize)
    {
        Rect piexAdjustedRect = GetPixelAdjustedRect();
        int size_x = Mathf.RoundToInt(imgSize.x);
        int size_y = Mathf.RoundToInt(imgSize.y);
        Vector4 result = new Vector4(padding.x/size_x,padding.y/size_y,
            (size_x-padding.z)/ size_x, (size_y - padding.w) / size_y);
        result = new Vector4(piexAdjustedRect.x+ piexAdjustedRect.width*result.x,
            piexAdjustedRect.y + piexAdjustedRect.height * result.y,
            piexAdjustedRect.x + piexAdjustedRect.width * result.z,
            piexAdjustedRect.y + piexAdjustedRect.height * result.w);
        return result;
    }

    void AddSprite(VertexHelper vh,Vector4 pos,Vector4 outerUV,Vector4 innerUV,Vector4 border,bool fillCenter =true)
    {
        var width = pos.z - pos.x;
        var height = pos.w - pos.y;
        if (border.z + border.x >= width)
        {
            border.z = 0;
            border.x = 0;
        }
        if (border.w+border.y>=height)
        {
            border.w = 0;
            border.y = 0;
        }

        AddQuad(vh,new Vector2(pos.x, pos.y),new Vector2(pos.z,pos.w),
            new Vector2(outerUV.x, outerUV.y),new Vector2(outerUV.z, outerUV.w));

    }

    void AddQuad(VertexHelper vertexHelper ,Vector2 posMin,Vector2 posMax,Vector2 uvMin,Vector2 uvMax)
    {
        int currentVertCount = vertexHelper.currentVertCount;
        vertexHelper.AddVert(new Vector3(posMin.x, posMin.y,0f),_color,new Vector2(uvMin.x, uvMin.y));
        vertexHelper.AddVert(new Vector3(posMin.x, posMax.y,0f),_color,new Vector2(uvMin.x, uvMax.y));
        vertexHelper.AddVert(new Vector3(posMax.x, posMax.y,0f),_color,new Vector2(uvMax.x, uvMax.y));
        vertexHelper.AddVert(new Vector3(posMax.x, posMin.y,0f),_color,new Vector2(uvMax.x, uvMin.y));
        vertexHelper.AddTriangle(currentVertCount,currentVertCount+1,currentVertCount+2);
        vertexHelper.AddTriangle(currentVertCount+2,currentVertCount+3,currentVertCount);
    }

    private Vector4 ReversalUV(Vector4 uv)
    {
        Vector4 temp;
        if (m_x_reversal && m_y_reversal)
            temp = new Vector4(uv.z, uv.w, uv.x, uv.y);
        else if (m_y_reversal)
            temp = new Vector4(uv.x, uv.w, uv.z, uv.y);
        else if (m_x_reversal)
            temp = new Vector4(uv.z, uv.y, uv.x, uv.w);
        else
            return uv;
        return temp;
    }
}
