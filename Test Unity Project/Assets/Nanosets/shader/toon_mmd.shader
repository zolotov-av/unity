// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Toon/Toon MMD"
{
	Properties
	{
		_Color("Color(RGBA)", Color) = (1,1,1,1)
		_Specular("Specular", Color) = (1,1,1) // Memo: Postfix from material.(Revision>=0)
		_Ambient("Ambient", Color) = (1,1,1)
		
		_MainTex("Texture", 2D) = "white" {}
		_ToonTex("ToonTex", 2D) = "white" {}
		_SphereAddTex("Texture(Sphere)", 2D) = "black" {}
		
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
		#pragma surface surfMMD ToonMMD alphatest:_Cutoff
		#include "nanotoon_v1.cginc"
		
		half _LightAtten;
		half _DarkAtten;
		half _LightClamp;
		half _DarkClamp;
		half _LightFactor;
		
		sampler2D _ToonTex;
		sampler2D _SphereAddTex;
		
		half4 LightingToonMMD(ToonSurfaceOutput s, half3 lightDir, half atten)
		{
			half factor = dot(lightDir, s.Normal) * 0.5 + 0.5;
			
			half4 ramp = tex2D(_ToonTex, half2(factor, factor));
			
			
			half4 lightColor = s.Color * _LightColor0 * (atten * _LightAtten * _LightFactor);
			half4 darkColor = s.Color * _LightColor0 * (atten * _DarkAtten * _LightFactor);
			
			half4 color = lerp(darkColor, lightColor, ramp);
			
			color.a = s.Alpha;
			return color;
		}
		
		void surfMMD (Input IN, inout ToonSurfaceOutput o)
		{

			// Defaults
			o.Albedo = 0.0;
			o.Emission = 0.0;
			o.Gloss = 0.0;
			o.Specular = 0.0;
			
			
			// Sphere Map
			float3 viewNormal = normalize( mul( UNITY_MATRIX_V, float4(normalize(o.Normal), 0.0) ).xyz );
			
			float2 sphereUv = viewNormal.xy * 0.5 + 0.5;
			
			float4 sphereAdd = tex2D( _SphereAddTex, sphereUv );
			
			
			half4 c		= tex2D(_MainTex, IN.uv_MainTex);
			o.Color		= c;
			o.Color		+= sphereAdd;// * step(0, viewNormal.z);
			o.Alpha		= c.a;
		}
		
		ENDCG
	}
	
	// Other Environment
	Fallback "Diffuse"
}
