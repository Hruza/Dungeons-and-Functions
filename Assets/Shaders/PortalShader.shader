﻿Shader "Custom/PortalView"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	}
		SubShader
	{

		Tags
		{
			"Queue" = "Transparent+1"

		}

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
                float4 screenPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos=ComputeScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				float2 screenSpaceUV = i.screenPos.xy/i.screenPos.w;
				return tex2D(_MainTex, screenSpaceUV);
			}
            ENDCG
        }
    }
}
