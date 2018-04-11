// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Inception-Fx/FlowInsideOut" {
	Properties{
		_MainTex("Base Texture (RGB)", 2D) = "white" {}
		_FlowColor("Flow Color", Color) = (1, 1, 1, 1)
			_MainColor("Base Color", Color) = (1, 1, 1, 1)
		_Period("Period (Seconds)", float) = 1
		_FlowWidth("Flow Width", Range(0, 1)) = 0.1

	    [Toggle] _InvertDirection("Invert Direction?", float) = 0

		_FlowHighlight("Flow Highlight", float) = 1

			_FlowColorAtCenter("Flow Color At Center", Color) = (1, 1, 1, 1)

			_FlowColorAtEdge("Flow Color At Edge", Color) = (1, 1, 1, 1)


	}
		SubShader{
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Blend One One

			Pass {
				CGPROGRAM
				#pragma vertex vert  
				#pragma fragment frag  
				#include "UnityCG.cginc"  

				sampler2D _MainTex;
				fixed4 _FlowColor;
				float _Period;
				float _FlowWidth;
				fixed4 _FlowColorAtEdge;
				fixed4 _FlowColorAtCenter;
				float _FlowHighlight;
				float _InvertDirection;
				fixed4 _MainColor;


				struct appdata_t {
					float4 vertex : POSITION;
					half2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 pos : POSITION;
					half2 mainTex : TEXCOORD0;
				};

				v2f vert(appdata_t v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.mainTex = v.texcoord;
					return o;
				}

				fixed4 frag(v2f i) : COLOR
				{
					fixed4 baseColor = tex2D(_MainTex, i.mainTex) * _MainColor;


					float2 center = float2(0.5, 0.5);


					float r = distance(i.mainTex, center);


					float radiusMax = 0.5;

					float timeProgress = fmod(_Time.y, _Period) / _Period;


					float flowProgress = _InvertDirection * (1 - timeProgress) + (1 - _InvertDirection) * timeProgress;

					float flowRadiusMax = flowProgress * (radiusMax + _FlowWidth);


					float flowRadiusMin = flowRadiusMax - _FlowWidth;

					float isInFlow = step(flowRadiusMin, r) - step(flowRadiusMax, r);

					


					fixed4 flowColor = _FlowColorAtCenter + flowProgress * (_FlowColorAtEdge - _FlowColorAtCenter);




					



					
					fixed4 finalColor = baseColor + isInFlow * _FlowHighlight* _FlowColor * baseColor;
					return finalColor;
				}
				ENDCG
			}
		}
			FallBack "Diffuse"
}