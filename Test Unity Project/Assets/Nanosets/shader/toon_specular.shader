// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Toon/Toon Specular"
{
	Properties
	{
		_Color("Color(RGBA)", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_SphereAddTex("Texture(Sphere)", 2D) = "black" {}
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
		
		// Surface Shader Pass ( Front )
		Cull Back
		ZWrite On
		AlphaTest Greater 0.9
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma surface surfHair ToonForward alphatest:_Cutoff
		#include "nanotoon_v1.cginc"
		sampler2D _SphereAddTex;
		
		void surfHair (Input IN, inout ToonSurfaceOutput o)
		{

			// Defaults
			o.Albedo = 0.0;
			o.Emission = 0.0;
			o.Gloss = 0.0;
			o.Specular = 0.0;
			
			
			// Sphere Map
			float3 viewNormal = normalize( mul( UNITY_MATRIX_V, float4(normalize(o.Normal), 0.0) ).xyz );
			
			float2 sphereUv = viewNormal.xy * 0.4 + 0.5;
			
			float4 sphereAdd = tex2D( _SphereAddTex, sphereUv );
			
			
			half4 c		= tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Color		= c;
			o.Color		+= sphereAdd ;//* step(0, viewNormal.z);
			o.Alpha		= c.a;
		}
		
		ENDCG
	}
	
	// Other Environment
	Fallback "Diffuse"
}
