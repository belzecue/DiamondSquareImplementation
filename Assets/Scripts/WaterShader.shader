
Shader "WaterShader" {
	Properties {
		_MainTex ("Diffuse(RGB) Spec(A)", 2D) = "white" {}
       	_BumpMap ("Bumpmap", 2D) = "bump" {}
       	_Color ("Color", Color) = (1,1,1,1)
     }
     SubShader {
       	Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
       	CGPROGRAM
 
       	#pragma surface surf SimpleSpecular alpha
    	float _Shininess;
    	sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _Color;
 
       	half4 LightingSimpleSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
			half4 c;
			c.rgb = s.Albedo * _Color;
			c.a = s.Alpha;
			return c;
       	}
 
       	struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
       	};

       	void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
			o.Alpha = c.a;
       	}
       	ENDCG
     } 
     Fallback "Diffuse"
}