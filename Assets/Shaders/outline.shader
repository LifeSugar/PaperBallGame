Shader "Custom/OutlineHLSL"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1, 0.5, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.02
    }
    SubShader
    {

        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        Pass
        {
            Name "OutlinePass"

            Cull Back
            ZWrite On
            ZTest LEqual
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"

            float4 _OutlineColor;
            float _OutlineWidth;


            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };


            struct v2f
            {
                float4 pos : SV_POSITION;
            };
            v2f vert(appdata v)
            {
                v2f o;

                // float3 offset = normalize(v.normal) * _OutlineWidth;
                // float4 pos = v.vertex;
                // pos.xyz += offset;

                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}