Shader "Effect/Rainning_1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Texture("Rainning_Texture",2D)="white" {}
        _TextureW("_TextureW",2D)="white" {}
        _speed("_speed",Range(0,10))=1
        _Intensity("_Intensity",Range(0,1))=1
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _Texture;
            float4 _Texture_ST;
            
            sampler2D _TextureW;
            float4 _TextureW_ST;

            float _speed;
            float _Intensity;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 objectPos : TEXCOORD3;
            };
VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.normalDir = normalize(UnityObjectToWorldNormal(v.normal));
                o.objectPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
                return o;
            }

           v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv0 = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

 float4 frag(VertexOutput i) : COLOR {
               //float2 verticalUv = float2(i.posWorld.x, i.posWorld.y) ;//* _Tiling;
               //float2 verticalUv2 = float2(i.posWorld.z, i.posWorld.y) ;//* _Tiling;
               float2 verticalUv = float2(i.objectPos.x - i.posWorld.x, i.objectPos.y - i.posWorld.y) * _Intensity;
                float2 verticalUv2 = float2(i.objectPos.z - i.posWorld.z, i.objectPos.y - i.posWorld.y) * _Intensity;
               float4 Texture_var = tex2D(_Texture, TRANSFORM_TEX(verticalUv, _Texture) );
               float4 Texture_var2 = tex2D(_Texture, TRANSFORM_TEX(verticalUv2, _Texture) );
               float4 finalColor = lerp(Texture_var ,Texture_var2 , abs(i.normalDir.x));
               return float4(finalColor*0.5);
}

            fixed4 frag (v2f i) : SV_Target
            {
                float3 emissive = (frac((_Time *_speed)));
float3 emissive2 = (frac((_Time *_speed)+0.5));
float3 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture)).rgb;
float3 _Texture_var2 = tex2D(_Texture,TRANSFORM_TEX(i.uv0 + float2(0.05,0.05), _Texture)).rgb;
float maskColor = saturate(1 - distance(emissive.r - _Texture_var.r,0.05)*20);
float maskColor2 = saturate(1 - distance(emissive2.r - _Texture_var2.r,0.05)*20);
float maskSwitch = saturate(abs(sin((_Time*0.5))));//两张图交替淡入
float3 finalColor = lerp(maskColor , maskColor2 ,maskSwitch );

float streamtime= saturate(_Time*20*_speed) ;
float3 streaming= tex2D(_Texture,TRANSFORM_TEX(saturate(i.uv0+float2(0,streamtime)), _TextureW)).rgb;

clip(1-(finalColor*streaming).r*10);
clip((finalColor*streaming).r-0.01);
float p=(finalColor)*streaming.r*20;
return float4(p,p,p,0.0);
            }
            ENDCG
        }
    }
}
