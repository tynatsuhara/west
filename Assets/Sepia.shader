Shader "Custom/Sepia" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_SepiaIntensity ("SepiaIntensity", Range(0, 1)) = .5
}

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
				
CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#include "UnityCG.cginc"

uniform sampler2D _MainTex;
fixed _SepiaIntensity;

fixed4 frag (v2f_img i) : SV_Target
{	
	fixed4 original = tex2D(_MainTex, i.uv);
	
	// get intensity value (Y part of YIQ color space)
	fixed Y = dot (fixed3(0.299, 0.587, 0.114), original.rgb) * _SepiaIntensity;

	// Convert to Sepia Tone by adding constant
	fixed4 sepiaConvert = float4 (0.191, -0.054, -0.221, 0.0) * _SepiaIntensity;
	fixed4 output = sepiaConvert + Y + float4(original.rgb.r, original.rgb.g, original.rgb.b, 0.0) * (1 - _SepiaIntensity);
	output.a = original.a;
	
	return output;
}
ENDCG

	}
}

Fallback off
}
