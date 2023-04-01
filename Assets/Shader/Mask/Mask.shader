Shader "Effect/ImageMask"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _ToneColor("ToneColor",Color) = (1,1,1,1)
        _ToneIntensity("ToneIntensity", Range(-1,1)) = 0
        _Saturation("Saturation",Range(0,1)) = 1
        _Contrast("Contrast",Float) = 1


        _StencilComp ("Stencil Comparison", Range(0,255)) = 8
        _Stencil ("Stencil ID", Range(0,255)) = 0
        _StencilOp ("Stencil Operation", Range(0,255)) = 0
        _StencilWriteMask ("Stencil Write Mask", Range(0,255)) = 255
        _StencilReadMask ("Stencil Read Mask", Range(0,255)) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        _DistanceKey("Gaussian Blur Key",Range(0,0.1))=0.001
        _Distance("Gaussian Blur Level",Range(0,1))=0
        
        _low_xClip("Left",Range(0,1))=0
        _high_xClip("Right",Range(0,1))=1
        _low_yClip("Bottom",Range(0,1))=0
        _high_yClip("Top",Range(0,1))=1
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        //_StencilComp==8 _StencilOp==2 mask
        //_StencilComp==3 _StencilOp==0 sub

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_ALPHACLIP
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };
            
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = IN.texcoord;
                
                #ifdef UNITY_HALF_TEXEL_OFFSET
                OUT.vertex.xy += (_ScreenParams.zw-1.0) * float2(-1,1) * OUT.vertex.w;
                #endif
                
                OUT.color = IN.color * _Color;
                return OUT;
            }

            sampler2D _MainTex;
            half _ToneIntensity;
            half _Saturation;
            half _Contrast;
            fixed3 _ToneColor;
            half _Distance;
            half _DistanceKey;
            
       half _low_xClip;
       half _high_xClip;
       half _low_yClip;
       half _high_yClip;

            //重点是片段着色器那段
fixed4 frag(v2f IN) : SV_Target
{
clip(IN.texcoord.x-_low_xClip);
clip(_high_xClip-IN.texcoord.x);
clip(IN.texcoord.y-_low_yClip);
clip(_high_yClip-IN.texcoord.y);

    fixed4 renderTex = tex2D(_MainTex, IN.texcoord);
    half4 color = (renderTex + _TextureSampleAdd) * IN.color;
    fixed3 finalColor;
    //当_ToneIntensity越靠近-1，则颜色越接近黑色，当_ToneIntensity越靠近1，则颜色越接近_ToneColor，当_ToneIntensity为0，则为原颜色
    int test_tone=step(_ToneIntensity,0);
    finalColor=(color + color*_ToneIntensity)*test_tone+(1-test_tone)*(color + (_ToneColor-color)*_ToneIntensity);
    //灰度化，经验公式
    fixed gray = dot(fixed3(0.2154,0.7154,0.0721),finalColor);
    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
    
    finalColor = lerp(gray,finalColor,_Saturation);
    //lerp函数用于线性插值，可以利用这个函数做出很多效果（变亮，调高对比度）
    finalColor = lerp(0.5,finalColor,_Contrast);
    #ifdef UNITY_UI_ALPHACLIP
    clip (color.a - 0.001);
    #endif
    //高斯模糊
    //#ifdef UNITY_UI_GAUSSIAN_BLUR
                half distance = _Distance*_DistanceKey;
                finalColor = finalColor*0.09474;
                finalColor += tex2D(_MainTex, half2(IN.texcoord.x + distance , IN.texcoord.y)) *0.11831;
                finalColor += tex2D(_MainTex, half2(IN.texcoord.x , IN.texcoord.y + distance )) *0.11831;
                finalColor += tex2D(_MainTex, half2(IN.texcoord.x - distance , IN.texcoord.y - distance )) *0.09474;
                finalColor += tex2D(_MainTex, half2(IN.texcoord.x + distance , IN.texcoord.y - distance )) *0.09474;
                finalColor += tex2D(_MainTex, half2(IN.texcoord.x - distance , IN.texcoord.y + distance )) *0.09474;
                finalColor += tex2D(_MainTex, half2(IN.texcoord.x - distance , IN.texcoord.y)) *0.11831;
                finalColor += tex2D(_MainTex, half2(IN.texcoord.x , IN.texcoord.y - distance )) *0.11831;
   // return fixed4(1,1,1, color.a);  

   // #endif

    


    return fixed4(finalColor, color.a);  
}
        ENDCG
        }
    }
}
