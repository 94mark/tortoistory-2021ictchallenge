Shader "FI/HumpbackWhale"
{
    Properties
    {
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        [NoScaleOffset] _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0

        [NoScaleOffset] _NormalMap("Normal Map", 2D) = "bump" {}
        [NoScaleOffset] _NormalDetail_1("Normal Detail 1", 2D) = "bump" {}
        [NoScaleOffset] _NormalDetail_2("Normal Detail 2", 2D) = "bump" {}
        [NoScaleOffset] _NormalDetail_3("Normal Detail 3", 2D) = "bump" {}
        [NoScaleOffset] _NormalMask("Normal Mask", 2D) = "black" {}
        [NoScaleOffset] _RoughnessMap("Roughness Map", 2D) = "black" {}

        [Header(Details)] _NormalDetailScale_1 ("Normal Detail Scale 1", float) = 4
        _NormalDetailScale_2 ("Normal Detail Scale 2", float) = 8
        _NormalDetailScale_3("Normal Detail Scale 3", float) = 40

        _NormalDetailAmount_1("Normal Detail Amount 1", float) = 2
        _NormalDetailAmount_2("Normal Detail Amount 2", float) = 2
        _NormalDetailAmount_3("Normal Detail Amount 3", float) = 1

        [Header(Variation)] _Desaturation("Desaturation", float) = 0.0
        _LevelsBlack("Levels Black", float) = 0.0
        _LevelsMiddle("Levels Middle", float) = 0.5
        _LevelsWhite("Levels White", float) = 1.0

        [Header(Caustics)] _CausticsIntensity("Caustics Intensity", float) = 1.0
        _CausticsScale("Caustics Scale", float) = 1.0
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM

        #include "FIUtils.cginc"

        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex, _NormalMap, _NormalDetail_1, 
            _NormalDetail_2, _NormalDetail_3, _NormalMask, _RoughnessMap;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 wNormal;
        };

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.wNormal = UnityObjectToWorldNormal(v.normal);
        }

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Normals Details
        float _NormalDetailScale_1;
        float _NormalDetailScale_2;
        float _NormalDetailScale_3;

        float _NormalDetailAmount_1;
        float _NormalDetailAmount_2;
        float _NormalDetailAmount_3;

        // Three Point Levels
        float _Desaturation, _LevelsBlack,
            _LevelsMiddle, _LevelsWhite;

        // Caustics
        float _CausticsIntensity, _CausticsScale;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float3 GetNormals(float2 uv)
        {
            float3 normals = UnpackScaleNormal(tex2D(_NormalMap, uv), 1);

            float3 detail1 = UnpackScaleNormal(tex2D(_NormalDetail_1, uv * _NormalDetailScale_1), _NormalDetailAmount_1);
            float3 detail2 = UnpackScaleNormal(tex2D(_NormalDetail_2, uv * _NormalDetailScale_2), _NormalDetailAmount_2);
            float3 detail3 = UnpackScaleNormal(tex2D(_NormalDetail_3, uv * _NormalDetailScale_3), _NormalDetailAmount_3);

            detail1 = BlendNormals(detail1, detail2);
            detail1 = BlendNormals(detail1, detail3);
            normals = BlendNormals(detail1, normals);

            float mask = tex2D(_NormalMask, uv).r;
            normals = lerp(normals, float3(0, 0, 1), mask);

            return normals;
        }

        float GetSmoothness(float2 uv)
        {
            return _Glossiness * (1 - tex2D(_RoughnessMap, uv).r);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_MainTex;
            float3 worldPos = IN.worldPos;
            float3 worldNormal = IN.wNormal;

            fixed4 c = tex2D (_MainTex, uv);
            
            o.Albedo = c * _Color;

            o.Emission = GetCaustics(worldPos.xz, worldNormal, _CausticsScale, _CausticsIntensity);

            o.Normal = GetNormals(uv);

            o.Metallic = _Metallic;
            o.Smoothness = GetSmoothness(uv);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
