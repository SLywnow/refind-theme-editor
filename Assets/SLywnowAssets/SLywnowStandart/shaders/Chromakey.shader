Shader "SLywnow/ChromaKey" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
		_I("Intensity",Range(0,100)) = 0
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		fixed4 _Color;
		float _I;

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		half4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb;

		if (c.g > _Color.g*(1-0.01*_I) && c.r < _Color.r*(1 - 0.01*_I) && c.b < _Color.b*(1 - 0.01*_I))
		{
			o.Alpha = 0.0;
		}
		else
		{
			o.Alpha = c.a;
		}
	}
	ENDCG
	}
		FallBack "Diffuse"
}
