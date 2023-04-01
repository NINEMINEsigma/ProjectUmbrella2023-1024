Shader "Exberlahet_Effect/Cycline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color",vector)=(1,1,1,1)
        _Distance("Distance",Range(0,1))=0.1
        _Depth("Depth",Range(0,1))=0.01
		_Translational("Translational",vector) = (0.0,0.0,0.0,0.0)
		_Scale("Scale",vector) = (1.0,1.0,1.0,1.0)
		_Rotation("Rotation",vector) = (0.0,0.0,0.0,1.0)
    }
    SubShader
    {
    Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            fixed _Distance;
            fixed _Depth;
			//uniform float _Transform;
			uniform float4 _Translational;
			uniform float4 _Scale;
			uniform float4 _Rotation;
            
			//平移矩阵
			float4x4 Translational(float4 translational)
			{
				return float4x4(1.0, 0.0, 0.0, translational.x,
					0.0, 1.0, 0.0, translational.y,
					0.0, 0.0, 1.0, translational.z,
					0.0, 0.0, 0.0, 1.0);
			}
			//缩放矩阵
			float4x4 Scale(float4 scale)
			{
				return float4x4(scale.x, 0.0, 0.0, 0.0,
					0.0, scale.y, 0.0, 0.0,
					0.0, 0.0, scale.z, 0.0,
					0.0, 0.0, 0.0, 1.0);
			}
			//旋转矩阵
			float4x4 TRSBlend(float4 rotation)
			{
				float radX = radians(rotation.x);
				float radY = radians(rotation.y);
				float radZ = radians(rotation.z);
				float sinX = sin(radX);
				float cosX = cos(radX);
				float sinY = sin(radY);
				float cosY = cos(radY);
				float sinZ = sin(radZ);
				float cosZ = cos(radZ);
				float4x4 r= float4x4(cosY*cosZ, -cosY * sinZ, sinY, 0.0,
					cosX*sinZ + sinX * sinY*cosZ, cosX*cosZ - sinX * sinY*sinZ, -sinX * cosY, 0.0,
					sinX*sinZ - cosX * sinY*cosZ, sinX*cosZ + cosX * sinY*sinZ, cosX*cosY, 0.0,
					0.0, 0.0, 0.0, 1.0);
				float4x4 tr = mul(Translational(_Translational), r);
				return mul(tr, Scale(_Scale));
			}

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };


            v2f vert (appdata v)
            {
                v2f o;
				v.vertex = mul(TRSBlend(_Rotation), v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);//将坐标从模型空间转到裁切空间

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
				//在模型空间计算
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv)*_Color;
                float d=distance(i.uv,(0.5,0.5));
                
                if(d>_Distance||
                d<_Distance-_Depth||
                0<sin(_Time.w*i.uv.x-i.uv.y))discard;
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
