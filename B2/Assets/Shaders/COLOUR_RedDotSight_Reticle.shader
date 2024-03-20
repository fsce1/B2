Shader "Custom/COLOUR_RedDotSight_Reticle"
{
	Properties
	{
		_RedDotTex ("Texture", 2D) = "white" {}
		[HDR]_DotColor("Color", Color) = (1,1,1,1)
		_RedDotSize("Dot Size", Range(0, 100)) = 1
		_RedDotDist("Dot Distance", Range(0, 200)) = 50
        _OffsetX ("Horizontal Offset", Float) = 0.0
		_OffsetY ("Vertical Offset", Float) = 0.0
		[HideInInspector]
		_MuzzleOffsetX ("Muzzle Offset X (automatically set)",Float) = 0.0
		[HideInInspector]
		_MuzzleOffsetY ("Muzzle Offset Y (automatically set)",Float) = 0.0
		
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent+1"}
		LOD 100
		Stencil{
			Ref 1
			Comp equal
		}

		Pass
		{
			ZTest off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _RedDotTex;
			float4 _RedDotTex_ST;
			float4 _DotColor;
			float _RedDotSize;
			float _RedDotDist;
			float _OffsetX;
			float _OffsetY;
			float _MuzzleOffsetX;
			float _MuzzleOffsetY;
			
			v2f vert (appdata v)
			{
				v2f o;
				float2 offset_dot = float2(_OffsetX,_OffsetY);

				fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 viewDir = _WorldSpaceCameraPos - worldPos;


				float2 worldpos = mul(unity_ObjectToWorld,v.vertex).xy;
                float2 objectpos = mul(unity_ObjectToWorld,float4(0,0,0,1)).xy;
				float3 objectrot = mul(unity_ObjectToWorld,float4(v.vertex.xyz,0));
				o.vertex = UnityObjectToClipPos(v.vertex * _RedDotSize * _RedDotDist + float3(_OffsetX + _MuzzleOffsetX,_OffsetY + _MuzzleOffsetY,_RedDotDist));
				

				o.uv = TRANSFORM_TEX(v.uv,_RedDotTex);
				//o.uv /= _RedDotSize;
				//o.uv += float2(0.5,0.5);
				

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 tex = tex2D(_RedDotTex, i.uv);
				fixed4 col = tex * _DotColor;
				//fixed4 col = _DotColor;
				//col.a = tex.a;

				if(i.uv.x < 0 || i.uv.x > 1 || i.uv.y < 0 || i.uv.y > 1)
					col.a = 0;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
