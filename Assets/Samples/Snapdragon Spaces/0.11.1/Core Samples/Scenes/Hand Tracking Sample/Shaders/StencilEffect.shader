/******************************************************************************
 * File: StencilEffect.shader
 * Copyright (c) 2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 * 
 ******************************************************************************/

Shader "Snapdragon Spaces/Stencil Effect"
{
    Properties {
        [MainTexture] _BaseMap("Base Map (RGB) Smoothness / Alpha (A)", 2D) = "white" {}
        [MainColor]   _BaseColor("Base Color", Color) = (1, 1, 1, 1)
    }
    
    SubShader {
        
        Tags { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "SimpleLit"
            }
        
        Stencil {
            Ref 1
            Comp equal
        }
        
        Pass
        {
            PackageRequirements {
                "com.unity.render-pipelines.universal":"1.0"
            }
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex LitPassVertexSimple
            #pragma fragment LitPassFragmentSimple

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitForwardPass.hlsl"
            ENDHLSL
        }
    }
    
    SubShader{
        Tags { 
            "RenderType" = "Opaque"
            "RenderPipeline" = ""
            }
        
        Stencil {
            Ref 1
            Comp equal
        }
 
        CGPROGRAM
        #pragma surface surf Lambert
 
        sampler2D _BaseMap;
 
        struct Input {
            float2 uv_MainTexture;
        };

        fixed4 _BaseColor;
 
        void surf(Input input, inout SurfaceOutput output) {
            fixed4 c = tex2D (_BaseMap, input.uv_MainTexture) * _BaseColor;
            output.Albedo = c.rgb;
            output.Alpha = c.a;
        }
        ENDCG
    }
    
    
}
