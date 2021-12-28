Shader "自定义/灰度"
{
	Properties
	{
		_MainTex("Main Text", 2D) = "white" {}
		_Color("Color", color) = (1,1,1,1)
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "Lighting.cginc"

				sampler2D _MainTex;
				fixed4 _Color;

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv: TEXCOORD1;
				};

				v2f vert(appdata_base v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float4 col = tex2D(_MainTex, i.uv);
					float luminance = 0.2125*col.r + 0.7154*col.g + 0.0721*col.b;
					col = float4(luminance,luminance,luminance,col.a);
					col = col * _Color;
					return col;
				}
				ENDCG
			}
		}
}