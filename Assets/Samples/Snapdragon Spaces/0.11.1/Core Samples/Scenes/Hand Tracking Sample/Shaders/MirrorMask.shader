/******************************************************************************
 * File: MirrorMask.shader
 * Copyright (c) 2021 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 * 
 ******************************************************************************/

Shader "Snapdragon Spaces/Mirror Mask" {
    SubShader {
        Tags {
            "Queue" = "Geometry-1"
            "RenderPipeline" = "UniversalPipeline"
            }

        ColorMask 0
        ZWrite Off

        Stencil {
            Ref 1
            Comp always
            Pass replace
        }
        
        Pass{
            PackageRequirements {
                "com.unity.render-pipelines.universal":"1.0"
            }
            
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex LitPassVertexSimple
            #pragma fragment LitPassFragmentSimple
            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitForwardPass.hlsl"
            ENDHLSL
        }
    }
    
    SubShader {
        Tags {
            "Queue" = "Geometry-1"
            "RenderPipeline" = ""
            }

        ColorMask 0
        ZWrite Off

        Stencil {
            Ref 1
            Comp always
            Pass replace
        }

        CGPROGRAM
        #pragma surface surf Lambert
 
        sampler2D _AlbedoTexture;
 
        struct Input {
            float2 uv_MainTexture;
        };
 
        void surf (Input input, inout SurfaceOutput output) {
            output.Albedo = tex2D(_AlbedoTexture, input.uv_MainTexture).rgb;
        }
        ENDCG
    }
}
