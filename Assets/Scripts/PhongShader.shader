
Shader "Unlit/PhongShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BlendFct ("Blend Factor", Float) = 0.5
		_PointLightColor ("Point Light Color", Color) = (0, 0, 0)
		_PointLightPosition ("Point Light Position", Vector) = (0.0, 0.0, 0.0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;	
			uniform float _BlendFct;
			uniform float3 _PointLightColor;
			uniform float3 _PointLightPosition;


			struct vertIn {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 color : COLOR;
				float2 uv : TEXCOORD2;
			};

			struct vertOut {
				float4 vertex : SV_POSITION;
				float4 worldVertex : TEXCOORD0;
   				float3 worldNormal : TEXCOORD1;
				float4 color : COLOR;
				float2 uv : TEXCOORD2;
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v) {
				vertOut o;

				// Convert Vertex position and corresponding normal into world coords
				// Note that we have to multiply the normal by the transposed inverse of the world 
				// transformation matrix (for cases where we have non-uniform scaling; we also don't
				// care about the "fourth" dimension, because translations don't affect the normal) 
				o.worldVertex = mul(_Object2World, v.vertex);
				o.worldNormal = normalize(mul(transpose((float3x3)_World2Object), v.normal.xyz));

				// Transform vertex in world coordinates to camera coordinates
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.uv = v.uv;
				return o;
			}
			
			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target {
				float3 worldNormal = v.worldNormal;
				float4 worldVertex = v.worldVertex;

				// Calculate ambient RGB intensities
				float Ka = 1;
				float3 amb = v.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;

				// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
				// (when calculating the reflected ray in our specular component)
				float fAtt = 1;
				float Kd = 1;
				float3 L = normalize(_PointLightPosition - worldVertex.xyz);
				float LdotN = dot(L, worldNormal.xyz);
				float3 dif = fAtt * _PointLightColor.rgb * Kd * v.color.rgb * saturate(LdotN);
				
				// Calculate specular reflections
				float Ks = 1;
				float specN = 5; // Values>>1 give tighter highlights
				float3 V = normalize(_WorldSpaceCameraPos - worldVertex.xyz);
				float3 R = reflect(-L, worldNormal.xyz);
				float3 RdotV = dot(R, V);
				float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(RdotV), specN);

				// Combine Phong illumination model components
				float4 finalColor = float4(amb.rgb + dif.rgb + spe.rgb, 1.0);
				fixed4 col = (tex2D(_MainTex, v.uv) * _BlendFct + finalColor * _BlendFct);
				return col;
			}
			ENDCG
		}
	}
}
