Shader "Unlit/ScreenShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Constant("Constant", Float) = .5
        _Divisor("Divisor", Float) = 1
        _MaxChromatic("MaxChromatic", float) = 5.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent +1" }
        Stencil{
            Ref 1
            Comp equal
            Pass keep
        }
            ZTest Greater
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
                 UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                UNITY_VERTEX_OUTPUT_STEREO //Insert
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Constant;
            float _Divisor;
            float _MaxChromatic;




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
                 
                
                 float distance = sqrt(pow((0.5 - i.uv.x), 2) + pow((0.5 - i.uv.y), 2));


            
                 float distanceD = distance * (1 - _Constant * distance * distance);

                 float x = (2 * i.uv.x - 1);
                 float y = -(2 * i.uv.y - 1);

                 x *= distanceD / distance / _Divisor;
                 y *= distanceD / distance /_Divisor;

                 float chrom = (distanceD - distance) * _MaxChromatic;
                 float uxg = (x + 1) / 2;
                 float uyg = (-y + 1) / 2;
                 float uxr = uxg;
                 float uyr = uyg;
                 float uxb = uxg;
                 float uyb = uyg;
                 if (x > 0)
                 {
                      uxr = (x + chrom + 1) / 2;
                      if (y >= 0) {
                          uyr = (-y + chrom + 1) / 2;
                      }
                      else {
                          uyr = (-y - chrom + 1) / 2;
                      }
                 }
                 else
                 {
                      uxr = (x - chrom + 1) / 2;
                      if (y >= 0) {
                          uyr = (-y + chrom + 1) / 2;
                      }
                      else {
                          uyr = (-y - chrom + 1) / 2;
                      }
                 }
                 if (y > 0)
                 {
                      uyb = (-y - chrom + 1) / 2;
                      if (x >= 0)
                      {
                          uxb = (x - chrom + 1) / 2;
                      }
                      else
                      {
                          uxb = (x + chrom + 1) / 2;
                      }
                 }
                 else
                 {
                      uyb = (-y + chrom + 1) / 2;
                      if (x >= 0)
                      {
                          uxb = (x - chrom + 1) / 2;
                      }
                      else
                      {
                          uxb = (x + chrom + 1) / 2;
                      }
                 }

                 float r = tex2D(_MainTex, float2(uxr, uyr)).r;
                 float g = tex2D(_MainTex, float2(uxg, uyg)).g;
                 float b = tex2D(_MainTex, float2(uxb, uyb)).b;
                 
                 return float4(r, g, b, 1);         
            }
            ENDCG
        }
    }
}
