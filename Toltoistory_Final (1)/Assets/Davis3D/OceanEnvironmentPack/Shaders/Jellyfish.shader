Shader "FI/Jellyfish"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        [HDR]_EmissiveColor("Emissive Color", Color) = (0,0,0,0)
        [HDR]_EmissiveAditive("Emissive Aditive", Color) = (0,0,0,0)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalTex("Normal", 2D) = "bump" {}
        _NormalIntensity("Normal Intensity", Float) = 1.0
        _EmissiveTex("Emissive Map", 2D) = "black" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _VAT("VAT (RGB)", 2D) = "black" {}
        _VATSpeed("VAT Speed", float) = 1.0
        _OffsetScale("Offset Scale", float) = 1.0

        [Header(Caustics)] _CausticsIntensity("Caustics Intensity", float) = 1.0
        _CausticsScale("Caustics Scale", float) = 1.0

        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Cull [_Cull]
        //ZTest Off

        CGPROGRAM

        #include "FIUtils.cginc"

        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        float _VATSpeed;
        half _NormalIntensity;
        float _CausticsIntensity, _CausticsScale;

        sampler2D _MainTex;
        sampler2D _NormalTex;
        sampler2D _EmissiveTex;
        sampler2D _VAT;

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float3 _EmissiveColor;
        float3 _EmissiveAditive;
        float _OffsetScale;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex);

            float3 vertexOffset;
            MS_MorphTargets(v.texcoord1, frac(_Time.y * _VATSpeed + floor(worldPos.x + worldPos.z) * _OffsetScale), _VAT, _VAT, 51, vertexOffset);
            
            v.vertex.xyz += float3(-vertexOffset.x, vertexOffset.z, vertexOffset.y) * 0.01;
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_MainTex;

            fixed4 c0 = tex2D(_MainTex, uv);
            fixed4 c = c0 * _Color;
            o.Albedo = c.rgb;

            fixed3 normal = UnpackScaleNormal(tex2D(_NormalTex, uv), _NormalIntensity);
            o.Normal = normal;

            o.Emission = tex2D(_EmissiveTex, uv) * _EmissiveColor + _EmissiveAditive;

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c0.r;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
