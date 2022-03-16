Shader "FI/Bubbles"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NormalTex ("Normal", 2D) = "bump" {}
        _Color("Color", Color) = (1, 1, 1, 1)

        _Refraction("Refraction", float) = 1.0
        _Power("Power", float) = 1.0
        _AlphaPower("Alpha Power", float) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent+1" }

        GrabPass
        {
            "_GrabTexture"
        }

        Pass
        {
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "FIUtils.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : TEXCOORD2;
                float4 grabPos : TEXCOORD3;
                float3 vNormalWs : TEXCOORD4;
                float3 vTangentUWs : TEXCOORD5;
                float3 vTangentVWs : TEXCOORD6;
            };

            float _Refraction, _Power, _AlphaPower;
            sampler2D _MainTex, _NormalTex, _GrabTexture;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);

                o.vNormalWs = UnityObjectToWorldNormal(v.normal);
                o.vTangentUWs.xyz = UnityObjectToWorldDir(v.tangent.xyz); 
                o.vTangentVWs.xyz = cross(o.vNormalWs.xyz, o.vTangentUWs.xyz) * v.tangent.w;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 oCol = tex2D(_MainTex, i.uv);
                //fixed4 col = oCol * _Color * i.color;


                //fixed3 normal = UnpackNormal(tex2D(_NormalTex, i.uv)) * 0.03;
                //fixed4 sceneCol = tex2Dproj(_GrabTexture, i.grabPos + float4(normal, 0.0));

                //col += sceneCol;
                //UNITY_APPLY_FOG(i.fogCoord, col);

                //col.a = i.color.a * oCol.r;
                //col.a = 1;

                float3 vNormalTs = UnpackNormal(tex2D(_NormalTex, i.uv));
                float3 vNormalWs = Vec3TsToWsNormalized(vNormalTs.xyz, i.vNormalWs.xyz, i.vTangentUWs.xyz, i.vTangentVWs.xyz);
                float3 vNormalVs = normalize(mul((float3x3)UNITY_MATRIX_V, vNormalWs));
                
                float2 offset = vNormalVs.xy * _Refraction;
                offset *= pow(length(vNormalVs.xy), _Power);
                offset /= float2(_ScreenParams.x, _ScreenParams.y);
                offset /= i.vertex.z;
                offset *= pow(i.color.a, _AlphaPower);

                float4 vDistortColor = tex2Dproj(_GrabTexture, i.grabPos + float4(offset, 0.0, 0.0));
                //vDistortColor *= (1 - oCol.r);

                return vDistortColor;
            }
            ENDCG
        }
    }
}
