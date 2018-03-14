
half4 _Color;

half _LightAtten;
half _DarkAtten;
half _LightFactor;

sampler2D _MainTex;
sampler2D _ToonTex;
sampler2D _SphereTex;

struct ToonSurfaceOutput
{
	half3 Albedo;
	half3 Normal;
	half3 Emission;
	//half3 Gloss;
	//half Specular;
	half Alpha;
	half4 Color;
};

struct Input
{
	half2 uv_MainTex;
};

half4 LightingMMDFront(ToonSurfaceOutput s, half3 lightDir, half atten)
{
	half factor = dot(lightDir, s.Normal) * 0.5h + 0.5h;
	
	half4 ramp = tex2D(_ToonTex, half2(factor, factor));
	
	half4 lightColor = s.Color * _LightColor0 * (_LightAtten);
	half4 darkColor = s.Color * _LightColor0 * (_DarkAtten);
	
	half4 color = lerp(darkColor, lightColor, ramp) * atten * _LightFactor;
	color.a = s.Alpha;
	return color;
}

half4 LightingMMDBack(ToonSurfaceOutput s, half3 lightDir, half atten)
{
	half factor = dot(lightDir, s.Normal) * (-0.5h) + 0.5h;
	
	half4 ramp = tex2D(_ToonTex, half2(factor, factor));
	
	half4 lightColor = s.Color * _LightColor0 * (_LightAtten);
	half4 darkColor = s.Color * _LightColor0 * (_DarkAtten);
	
	half4 color = lerp(darkColor, lightColor, ramp) * atten * _LightFactor;
	color.a = s.Alpha;
	return color;
}

void mmd_surf(Input IN, inout ToonSurfaceOutput o)
{
	// texture color
	half4 ct = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	
	// sphere reflection color
	half3 viewNormal = normalize(mul(UNITY_MATRIX_V, half4(o.Normal, 0.0h)).xyz);
	half2 suv = viewNormal.xy * 0.5h + 0.5h;
	half4 cs = tex2D(_SphereTex, suv);
	
	//o.Albedo = 0.0h;
	//o.Emission = 0.0h;
	//o.Gloss = 0.0h;
	//o.Specular = 0.0h;
	
	o.Color = ct + cs;
	o.Alpha = ct.a;
}
