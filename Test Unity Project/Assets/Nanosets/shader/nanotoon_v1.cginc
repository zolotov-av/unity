
half4 _Color;

sampler2D _MainTex;
sampler2D _SphereTex;

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
	half4 Color;
};

struct Input
{
	half2 uv_MainTex;
};

void surf(Input IN, inout ToonSurfaceOutput o)
{
	// texture color
	half4 ct = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	
	// sphere reflection color
	half3 vn = normalize(mul(UNITY_MATRIX_V, half4(o.Normal, 0.0h)).xyz);
	half4 cs = tex2D(_SphereTex, vn.xy * 0.5h + 0.5h);
	
	o.Color = ct + cs;
	o.Alpha = ct.a;
}

/**
 * Lighting for forward side
 */
half4 LightingToonFront(ToonSurfaceOutput s, half3 lightDir, half atten)
{
	half factor = (dot(lightDir, s.Normal) - _DarkClamp) / (_LightClamp - _DarkClamp);
	
	half4 lightColor = s.Color * _LightColor0 * (atten * _LightAtten * _LightFactor);
	half4 darkColor = s.Color * _LightColor0 * (atten * _DarkAtten * _LightFactor);
	
	half4 color = lerp(darkColor, lightColor, saturate(factor));
	color.a = s.Alpha;
	return color;
}

/**
 * Lighting for backward side
 */
half4 LightingToonBack(ToonSurfaceOutput s, half3 lightDir, half atten)
{
	half factor = (-dot(lightDir, s.Normal) - _DarkClamp) / (_LightClamp - _DarkClamp);
	
	half4 lightColor = s.Color * _LightColor0 * (atten * _LightAtten * _LightFactor);
	half4 darkColor = s.Color * _LightColor0 * (atten * _DarkAtten * _LightFactor);
	
	half4 color = lerp(darkColor, lightColor, saturate(factor));
	color.a = s.Alpha;
	return color;
}
