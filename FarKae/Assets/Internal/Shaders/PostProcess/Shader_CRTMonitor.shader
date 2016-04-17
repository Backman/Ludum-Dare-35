Shader "Hidden/CRTMonitor"
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

			uniform float _LineOneFrequency;
			uniform float _LineOneSpeed;
			uniform float _LineOneThickness;
			uniform float _LineTwoFrequency;
			uniform float _LineTwoSpeed;
			uniform float _LineTwoThickness;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				col.rgb -= abs(sin(i.uv.y * _LineOneFrequency + _Time.y * _LineOneSpeed)) * _LineOneThickness * 0.01; // (1)
				col.rgb -= abs(sin(i.uv.y * _LineTwoFrequency - _Time.y * _LineTwoSpeed)) * _LineTwoThickness * 0.01; // (2)

				return col;
			}
			ENDCG
		}
	}
}
