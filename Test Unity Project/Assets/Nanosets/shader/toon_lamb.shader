// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Toon/Toon Lambert"
{
	Properties
	{
		_Color("Color(RGBA)", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_LightAtten("Light Atten(0.0:1.0)", Range(0.0, 1.0)) = 1.0
		_DarkAtten("Dark Atten(0.0:1.0)", Range(0.0, 1.0)) = 0.4
		_LightClamp("Light Clamp(-1.0:1.0)", Range(-1.0, 1.0)) = 1.0
		_DarkClamp("Dark Clamp(-1.0:1.0)", Range(-1.0, 1.0)) = 0.0
		_Shininess ("Shininess(0.0:)", Float) = 1.0
		
		_Cutoff ("Alpha cutoff", Float) = 0.9
	}
	
	SubShader
	{
		// Settings
		Tags {"Queue" = "Transparent" "IgnoreProjector"="False" "RenderType" = "TransparentCutout"}
		
		// Surface Shader Pass ( Front )
		Cull Back
		ZWrite On
		AlphaTest Greater 0.9
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma surface surf ToonLambert alphatest:_Cutoff
		#include "nanotoon_v1.cginc"
		sampler2D _SphereAddTex;
		
		half _LightAtten;
		half _DarkAtten;
		half _LightClamp;
		half _DarkClamp;
		
		half4 LightingToonLambert(ToonSurfaceOutput s, half3 lightDir, half atten)
		{
			half factor = (dot(lightDir, s.Normal) - _DarkClamp) / (_LightClamp - _DarkClamp);
			
			half4 lightColor = s.Color * _LightColor0 * (atten * _LightAtten * _Shininess);
			half4 darkColor = s.Color * _LightColor0 * (atten * _DarkAtten * _Shininess);
			
			half4 color = lerp(darkColor, lightColor, saturate(factor));
			
			color.a = s.Alpha;
			return color;
		}
		
		ENDCG
	}
	
	// Other Environment
	Fallback "Diffuse"
}
