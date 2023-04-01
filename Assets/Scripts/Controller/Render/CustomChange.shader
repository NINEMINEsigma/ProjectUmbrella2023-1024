Shader "Exberlahet_Effect/CustomChange(Blend)"
{
	Properties
	{
		_Translational("Translational",vector) = (0.0,0.0,0.0,0.0)
		_Scale("Scale",vector) = (1.0,1.0,1.0,1.0)
		_Rotation("Rotation",vector) = (0.0,0.0,0.0,1.0)
	}
		SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			//uniform float _Transform;
			uniform float4 _Translational;
			uniform float4 _Scale;
			uniform float4 _Rotation;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 color:COLOR;
				float4 vertex : SV_POSITION;
			};
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

			v2f vert(appdata v)
			{
				v2f o;
				//在模型空间计算
				v.vertex = mul(TRSBlend(_Rotation), v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);//将坐标从模型空间转到裁切空间
				o.color = float4(1, 0, 0, 1);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				return i.color;
			}
			ENDCG
		}
	}
}

