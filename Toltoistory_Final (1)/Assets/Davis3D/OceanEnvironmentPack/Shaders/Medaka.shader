Shader "FI/Medaka"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        [HDR]_EmissiveColor("Emissive Color", Color) = (0,0,0,0)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalTex("Normal", 2D) = "bump" {}
        _NormalIntensity("Normal Intensity", Float) = 1.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.2

        _VAT("VAT (RGB)", 2D) = "black" {}
        _VATSpeed("VAT Speed", float) = 1.0
        _OffsetScale("Offset Scale", float) = 1.0
        _VAT_Amount("VAT Amount", float) = 1.0

        [Header(Caustics)] _CausticsIntensity("Caustics Intensity", float) = 1.0
        _CausticsScale("Caustics Scale", float) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" }
        LOD 200

        Cull Off

        CGPROGRAM

        #include "FIUtils.cginc"

        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert alphatest:_Cutoff

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        half _NormalIntensity;
        sampler2D _MainTex;
        sampler2D _NormalTex;
        sampler2D _VAT;

        float _VATSpeed;
        float _OffsetScale;
        float _VAT_Amount;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 wNormal;
        };

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.wNormal = UnityObjectToWorldNormal(v.normal);

            float3 worldPos = mul(unity_ObjectToWorld, v.vertex);

            float3 vertexOffset;
            MS_MorphTargets(v.texcoord1, frac(_Time.y * _VATSpeed + floor(worldPos.x + worldPos.z) * _OffsetScale), _VAT, _VAT, 51, vertexOffset);

            v.vertex.xyz += float3(-vertexOffset.x, vertexOffset.z, vertexOffset.y) * 0.05 * _VAT_Amount;
        }

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        half _EmissiveColor;

        // Caustics
        float _CausticsIntensity, _CausticsScale;
        
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_MainTex;
            float3 worldPos = IN.worldPos;
            float3 worldNormal = IN.wNormal;

            fixed4 c = tex2D(_MainTex, uv) * _Color;
            o.Albedo = c.rgb;

            fixed3 normal = UnpackScaleNormal(tex2D(_NormalTex, uv), _NormalIntensity);
            o.Normal = normal;

            o.Emission = GetCaustics(worldPos.xz, worldNormal, _CausticsScale, _CausticsIntensity) + _EmissiveColor;

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
