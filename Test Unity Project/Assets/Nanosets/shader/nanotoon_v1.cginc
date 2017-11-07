
float4 _Color;
sampler2D _MainTex;
float	_ShadowThreshold;
float4	_ShadowColor;
float	_ShadowSharpness;
float	_Shininess;

struct ToonSurfaceOutput
{
	half3 Albedo;
	half3 Normal;
	half3 Emission;
	half3 Gloss;
	half Specular;
	half Alpha;
	half4 Color;
};

struct Input
{
	float2 uv_MainTex;
};

void surf(Input IN, inout ToonSurfaceOutput o)
{
	half4 c		= tex2D(_MainTex, IN.uv_MainTex) * _Color;
	
	o.Albedo = 0.0;
	o.Emission = 0.0;
	o.Gloss = 0.0;
	o.Specular = 0.0;
	o.Color = c;
	o.Alpha = c.a;
}

/**
 * Lighting for forward side
 */
half4 LightingToonForward(ToonSurfaceOutput s, half3 lightDir, half atten)
{
	half lightStrength = dot(lightDir, s.Normal) * 0.5 + 0.5;
	half shadowRate = abs(max(-1, (min(lightStrength, _ShadowThreshold) - _ShadowThreshold) * _ShadowSharpness)) * _ShadowColor.a;
	
	half4 lightColor = s.Color * _LightColor0 * (atten * _Shininess);
	
	half4 color = lerp(lightColor, _ShadowColor, shadowRate);
	
	color.a = s.Alpha;
	return color;
}

/**
 * Lighting for backward side
 */
half4 LightingToonBackward(ToonSurfaceOutput s, half3 lightDir, half atten)
{
	half lightStrength = dot(lightDir, s.Normal) * (-0.5) + 0.5;
	half shadowRate = abs(max(-1, (min(lightStrength, _ShadowThreshold) - _ShadowThreshold) * _ShadowSharpness)) * _ShadowColor.a;
	
	half4 lightColor = s.Color * _LightColor0 * (atten * _Shininess);
	
	half4 color = lerp(lightColor, _ShadowColor, shadowRate);
	
	color.a = s.Alpha;
	return color;
}
