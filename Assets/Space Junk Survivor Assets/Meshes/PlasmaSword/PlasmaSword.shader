Shader "Custom/PlasmaSword"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_Amount ("JitterAmount", Range(-1,1)) = 0.5
		_DispTex("Displacement Texture", 2D) = "gray" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
		Blend One One

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        //#pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
		#pragma surface surf Standard vertex:vert

        sampler2D _MainTex;
		sampler2D _DispTex;
		float _Amount;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_DispTex;
        };



		void vert(inout appdata_full v){
			float2 uv = float2(_Time.x * 50 + v.texcoord.x, v.texcoord.y);
			float4 d = tex2Dlod(_DispTex, float4(uv, 0, 0)).r * _Amount;
			v.vertex.xyz += v.normal * d;
		}

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
			o.Emission = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
