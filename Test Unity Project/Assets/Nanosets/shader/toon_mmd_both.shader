
Shader "Toon/MMD Both"
{
	Properties
	{
		_Color("Color(RGBA)", Color) = (1,1,1,1)
		
		_Diffuse("Diffuse", Color) = (1,1,1,1) // unused
		_Specular("Specular", Color) = (1,1,1) // unused
		_Ambient("Ambient", Color) = (1,1,1) // unused
		
		_MainTex("Texture", 2D) = "white" {}
		_ToonTex("ToonTex", 2D) = "white" {}
		_SphereTex("SphereTex", 2D) = "black" {}
		
		_LightFactor ("Ligth Factor(0.0:)", Float) = 1.0
		_LightAtten("Light Atten(0.0:1.0)", Range(0.0, 1.0)) = 1.0
		_DarkAtten("Dark Atten(0.0:1.0)", Range(0.0, 1.0)) = 0.0
		
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
		#pragma surface mmd_surf MMDFront alphatest:_Cutoff
		#include "nanotoon_mmd.cginc"
		ENDCG
		
		// Surface Shader Pass ( Back )
		Cull Front
		ZWrite On
		AlphaTest Greater 0.9
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma surface mmd_surf MMDBack alphatest:_Cutoff
		#include "nanotoon_mmd.cginc"
		ENDCG
	}
	
	// Other Environment
	Fallback "Diffuse"
}
