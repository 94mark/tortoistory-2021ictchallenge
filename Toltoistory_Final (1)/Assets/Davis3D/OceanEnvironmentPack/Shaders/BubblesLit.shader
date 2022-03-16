Shader "FI/BubblesLit"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalTex ("Normal (RGB)", 2D) = "bump" {}

        _NoiseTex ("Noise Tex", 2D) = "white" {}
        _SpeedA ("Speed A", float) = 1.0
        _SpeedB ("Speed B", float) = 1.0
        _SizeA("Size A", float) = 1.0
        _SizeB("Size B", float) = 1.0
        _DisAmount("Distortion Amount", float) = 0.15

        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }

        GrabPass
        {
            "_GrabTexture"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        float _SpeedA, _SpeedB, _SizeA, _SizeB, _DisAmount;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        sampler2D _MainTex, _NormalTex, _NoiseTex, _GrabTexture;

        struct Input
        {
            float2 uv_MainTex;
            float4 grabPos;
            float4 color : COLOR;
        };

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            float4 pos = UnityObjectToClipPos(v.vertex);
            o.grabPos = ComputeGrabScreenPos(pos);
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

            float dis1 = tex2D(_NoiseTex, uv * _SizeA + float2(_Time.y * _SpeedA, 0));
            float dis2 = tex2D(_NoiseTex, uv * _SizeB + float2(0, _Time.y * _SpeedB));
            float dis = lerp(dis1, dis2, 0.5);

            fixed3 normal = UnpackNormal(tex2D(_NormalTex, uv + dis * _DisAmount));

            fixed4 c = tex2D(_MainTex, uv + dis * _DisAmount) * _Color;            
            c += tex2Dproj(_GrabTexture, IN.grabPos + float4(normal.xy, 0, 0) * 0.3);

            o.Albedo = c.rgb;
            o.Normal = normal;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.r * IN.color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
