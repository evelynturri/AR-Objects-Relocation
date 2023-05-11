/******************************************************************************
 * File: MeshVisualization.shader
 * Copyright (c) 2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 * 
 ******************************************************************************/

Shader "Unlit/Mesh Visualization"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 worldPos : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                float3 uv_dx = ddx(i.worldPos);
                float3 uv_dy = ddy(i.worldPos);
                float3 normal = normalize(cross(uv_dx, uv_dy));

                float light = clamp(dot(normal, normalize(i.worldPos - _WorldSpaceCameraPos)), 0.05, 1);

                return float4(light, light, light, 1.0) * _Color;
            }
            ENDCG
        }
    }
}
