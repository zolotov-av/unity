
Shader "Toon/Toon SoftShadow"
{
	Properties
	{
		_Color("Color(RGBA)", Color) = (1,1,1,1)
		
		_MainTex("Texture", 2D) = "white" {}
		
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
		Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
		
		// Surface Shader Pass ( Front )
		Cull Back
		ZWrite On
		AlphaTest Greater 0.9
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma surface surf SoftShadow alphatest:_Cutoff fullforwardshadows
		
		half4 _Color;

		sampler2D _MainTex;
		
		half _LightFactor;

		half _LightAtten;
		half _DarkAtten;
		half _LightClamp;
		half _DarkClamp;

		struct ToonSurfaceOutput
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			//half Specular;
			//half3 Gloss;
			half Alpha;
		};

		struct Input
		{
			half2 uv_MainTex;
		};

		void surf(Input IN, inout ToonSurfaceOutput o)
		{
			half4 ct = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			
			o.Albedo = ct.rgb;
			o.Alpha = ct.a;
		}
		
		/**
		 * Lighting for forward side
		 */
		half4 LightingSoftShadow(ToonSurfaceOutput s, half3 lightDir, half atten)
		{
			half shadow;
			half t1 = 0.5;
			half t2 = 0.7;
			shadow = saturate( (atten-t1) / (t2 - t1) );
			
			half factor = (dot(lightDir, s.Normal) - _DarkClamp) / (_LightClamp - _DarkClamp);
			
			half3 lightColor = s.Albedo * (_LightFactor * _LightAtten);
			half3 darkColor = s.Albedo * (_LightFactor * _DarkAtten);
			
			half3 color = lerp(darkColor, lightColor, saturate(factor)) * _LightColor0 * shadow;
			return half4(color, s.Alpha);
		}

		ENDCG
	}
	
	// Other Environment
	Fallback "Diffuse"
}
