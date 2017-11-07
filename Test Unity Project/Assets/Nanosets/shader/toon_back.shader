Shader "Toon/Toon Back"
{
	Properties
	{
		_Color("Color(RGBA)", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_Shininess ("Shininess(0.0:)", Float) = 1.0
		
		_ShadowThreshold ("Shadow Threshold(0.0:1.0)", Range(0.0, 1.0)) = 0.5
		_ShadowColor ("Shadow Color(RGBA)", Color) = (0,0,0,0.5)
		_ShadowSharpness ("Shadow Sharpness(0.0:)", Range(0, 100)) = 100
        _Cutoff ("Alpha cutoff", Float) = 0.9
	}
	
	SubShader
	{
		// Settings
		Tags {"Queue" = "Transparent" "IgnoreProjector"="False" "RenderType" = "TransparentCutout"}
		
		// Surface Shader Pass ( Back )
		Cull Front
		ZWrite On
		AlphaTest Greater 0.9
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma surface surf ToonBackward alphatest:_Cutoff
		#include "nanotoon_v1.cginc"
		ENDCG
	}
	
	// Other Environment
	Fallback "Diffuse"
}
