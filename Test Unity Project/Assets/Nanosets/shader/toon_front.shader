
Shader "Toon/Toon Front"
{
	Properties
	{
		_Color("Color(RGBA)", Color) = (1,1,1,1)
		
		_MainTex("Texture", 2D) = "white" {}
		_SphereTex("SphereTex", 2D) = "black" {}
		
		_LightFactor ("Ligth Factor(0.0:)", Float) = 1.0
		
		_LightAtten("Light Atten(0.0:1.0)", Range(0.0, 1.0)) = 1.0
		_DarkAtten("Dark Atten(0.0:1.0)", Range(0.0, 1.0)) = 0.4
		_LightClamp("Light Clamp(-1.0:1.0)", Range(-1.0, 1.0)) = 1.0
		_DarkClamp("Dark Clamp(-1.0:1.0)", Range(-1.0, 1.0)) = 0.0
		
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
		#pragma surface surf ToonFront alphatest:_Cutoff
		#include "nanotoon_v1.cginc"
		ENDCG
	}
	
	// Other Environment
	Fallback "Diffuse"
}
