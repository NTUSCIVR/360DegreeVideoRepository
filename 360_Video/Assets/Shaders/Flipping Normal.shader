Shader "Flipping Normal" {
	Properties {
		_MainTex("Albedo (RGB)", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Cull Off

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 color : COLOR;
		};

		// Flips the normal
		void vert(inout appdata_full v)
		{
			v.normal.xyz = v.normal * -1;
		}

		void surf(Input In, inout SurfaceOutput o) 
		{
			fixed3 result = tex2D(_MainTex, In.uv_MainTex);
			o.Albedo = result.rgb;
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
