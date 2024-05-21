Shader "Unlit/FocusShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _XPos("Xpos", Float) = 0.5
        _YPos("Ypos", Float) = 0.5
        _Mult("Mult", Float) = 2
        _Min("Min", Float) = .1
        _Max("Max", Float) = 1
    }
    SubShader
    {

        Tags { 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType"="Transparent"
            "ForceNoShadowCasting" = "True"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
               
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _XPos;
            float _YPos;
            float _Mult;
            float _Min;
            float _Max;

           
            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

             
                float distance = sqrt( pow((_XPos - i.uv.x), 2) + pow((_YPos - i.uv.y), 2));

                float ammount = lerp(_Max, _Min, _Mult);
                    
                return fixed4(0, 0, 0, distance / ammount);
            }
            ENDCG
        }
    }
}
