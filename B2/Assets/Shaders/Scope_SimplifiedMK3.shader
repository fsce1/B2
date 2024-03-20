Shader "Cityrobo/ScopeSimplifiedMK3" {
    Properties{
        [NoScaleOffset]
        _MainTex("Camera Render Texture", 2D) = "white" {}
        _VignetteRadius("Vignette Radius", Range(0, 25)) = 1
        _VignetteSmoothness("Vignette Smoothness", Range(0, 1)) = 0.25
        [Toggle(SECONDARY_VIGNETTE)] 
        _SecondaryVignetteBool("Has secondary Vignette? ", Float) = 0
        _SecondaryVignetteRadius("Secondary Vignette Radius", Range(0, 10)) = 1
        _SecondaryVignetteSmoothness("Secondary Vignette Smoothness", Range(0, 1)) = 0.25
        [Toggle(CAMERA_DISTANCE)] 
        _CameraDistanceBool("Does eye distance affect vignette size? ", Float) = 0
		_ExpansionSpeed("Expansion Speed", Range(0,100)) = 20
		_EyeboxStartPoint("Point where eyebox starts expanding (cm)", Range(0,10)) = 1.5
        _EyeRelief("Eye Relief Distance", Range(0,10)) = 1
        [NoScaleOffset]
        _ReticleTex("Reticle Texture", 2D) = "white" {}
        [HDR]_ReticleColor("Reticle Color", Color) = (1,1,1,1)
        _ReticleScale("Reticle Scale", Range(0,10)) = 1
        _ReticleOffsetX("Reticle Offset X", Range(-10,10)) = 0
        _ReticleOffsetY("Reticle Offset Y", Range(-10,10)) = 0
    }
        SubShader{
              Tags { "RenderType" = "Opaque" "Queue" = "Transparent" "ForceNoShadowCasting" = "True" }
              LOD 200
              Blend SrcAlpha OneMinusSrcAlpha

        //Zoom

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature CAMERA_DISTANCE
            #pragma shader_feature SECONDARY_VIGNETTE
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 pos : TEXCOORD1;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
                float4 worldPos : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                _MainTex_ST = float4 (0,0,0,0);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.pos = UnityObjectToViewPos(v.vertex);         //transform vertex into eye space
                o.normal = mul(UNITY_MATRIX_IT_MV, v.normal);   //transform normal into eye space
                o.tangent = mul(UNITY_MATRIX_IT_MV, v.tangent); //transform normal into eye space
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float _CameraDistance;
            fixed _VignetteRadius, _VignetteSmoothness, _SecondaryVignetteRadius, _SecondaryVignetteSmoothness, _ExpansionSpeed, _EyeboxStartPoint, _CameraDistanceBool;
            fixed _EyeRelief;
            fixed4 frag(v2f i) : SV_Target
            {
                float3 normal = normalize(i.normal);    //get normal of this fragment
                float3 tangent = normalize(i.tangent);  //get tangent
                float3 cameraDir = normalize(i.pos);    //direction to eye space origin, normalize(i.pos - float3(0, 0, 0))

                float3 offset = cameraDir + normal;     //normal is facing towards camera, cameraDir - -normal

                float3x3 mat = float3x3(
                    tangent,
                    cross(normal, tangent),
                    normal
                    );

				if (_CameraDistanceBool = false)
					{
						_ExpansionSpeed == 1;
					}

                offset = mul(mat, offset);  //transform offset into tangent space
                float _camDist = distance(i.worldPos, _WorldSpaceCameraPos) * 10;
				_camDist = clamp(_camDist, 0, _EyeboxStartPoint);
                float2 uv;
                #ifdef CAMERA_DISTANCE
                _VignetteRadius -= _camDist/_EyeRelief;
                #else
                _VignetteRadius -= 1;
                #endif
                uv = (i.uv + (offset.xy) + (offset.xy)*length(offset.xy)*length(offset.xy)*length(offset.xy));   //sample and scale
                fixed4 col = tex2D(_MainTex, uv + float2(0.5, 0.5));

                //Vignette
                fixed aspectRatio = _ScreenParams.x / _ScreenParams.y;
                fixed2 position = (uv);
                position.x *= aspectRatio;
                position.y *= aspectRatio;
                fixed len = length(position) * 2;

				if (_camDist >= _EyeboxStartPoint)
					{
					_CameraDistanceBool == false;
					}

                _VignetteRadius /= _camDist * _ExpansionSpeed;
                _VignetteSmoothness /= _camDist;

                col.rgb *= lerp(1, 0, smoothstep(_VignetteRadius, _VignetteRadius + _VignetteSmoothness, len));
                #ifdef SECONDARY_VIGNETTE
                //_SecondaryVignetteRadius -= 1;
                //_SecondaryVignetteRadius /= _camDist;
                //_SecondaryVignetteSmoothness /= _camDist;
                col.rgb *= lerp(1, 0, smoothstep(_SecondaryVignetteRadius, _SecondaryVignetteRadius + _SecondaryVignetteSmoothness, len));
                #endif
                return col;
            }

              ENDCG
        }
        

        //Reticle

        Pass{
              CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #pragma shader_feature CAMERA_DISTANCE
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;
                    float3 tangent : TANGENT;
                };

                sampler2D _ReticleTex;
                float4 _ReticleTex_ST;
                fixed4 _ReticleColor;
                float _ReticleScale;
                float _CameraDistance;
                float _ReticleOffsetX;
                float _ReticleOffsetY;
                fixed _EyeRelief;
                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 pos : TEXCOORD1;
                    float3 normal : NORMAL;
                    float3 tangent : TANGENT;
                    float4 worldPos : TEXCOORD2;
                };

                v2f vert(appdata v) {

                    _ReticleTex_ST[2] = _ReticleOffsetX * 1000;
                    _ReticleTex_ST[3] = _ReticleOffsetY * 1000;
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _ReticleTex);
                    o.pos = UnityObjectToViewPos(v.vertex);         //transform vertex into eye space
                    o.normal = mul(UNITY_MATRIX_IT_MV, v.normal);   //transform normal into eye space
                    o.tangent = mul(UNITY_MATRIX_IT_MV, v.tangent); //transform normal into eye space
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {

                    float3 normal = normalize(i.normal);    //get normal of this fragment
                    float3 tangent = normalize(i.tangent);  //get tangent
                    float3 cameraDir = normalize(i.pos);    //direction to eye space origin, normalize(i.pos - float3(0, 0, 0))

                    float3 offset = cameraDir + normal;     //normal is facing towards camera, cameraDir - -normal

                    float3x3 mat = float3x3(
                        tangent,
                        cross(normal, tangent),
                        normal
                    );

                    offset = mul(mat, offset);  //transform offset into tangent space
                    float _camDist = distance(i.worldPos, _WorldSpaceCameraPos) * 10;


                    float2 uv = (i.uv + (offset.xy * 10000) - float2(0.5, 0.5)) / (_ReticleScale * 2000);   //sample and scale
                    return tex2D(_ReticleTex, uv + float2(0.5, 0.5)) * _ReticleColor;                              //shift sample to center of texture
                }

                 ENDCG
          }
   
    }
}