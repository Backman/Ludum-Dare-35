﻿Shader "Hidden/ImageDistort"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			uniform float _Distortion;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 pos = i.uv;

				pos -= float2(0.5, 0.5);
				float p = pow(length(pos), _Distortion);
				pos *= float2(p, p);
				pos += float2(0.5, 0.5);

				return tex2D(_MainTex, pos);
			}
			ENDCG
		}
	}
}
