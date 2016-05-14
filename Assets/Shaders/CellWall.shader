Shader "Custom/CellWall" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
        _RimColor ("RimColor", Color) = (0,0,1,1)
		_MainTex ("Normal Map", 2D) = "bump" {}
        _NoiseTex("Noise", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
        _RippleScale( "RippleScale", Range(0.001, 1)) = 1.0
	}
	SubShader {	
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        ZWRITE OFF
        Pass{
            ZWrite On
            ColorMask 0
        }
        CULL OFF
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

        #include "UnityCG.cginc"

		sampler2D _MainTex;
        uniform float4 _MainTex_ST; // tiling and offset stored in here
        sampler2D _NoiseTex;

		struct Input {
            float2 uvs;
            float3 norm;
            float3 viewDir;
		};

        //float4 _Time;
        half _RippleScale;
        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            o.norm = normalize(v.vertex.xyz);
            float uvn = float4(sin(v.normal.x + _Time.y/5.0)/2.0, cos(v.normal.y + _Time.y/7.3)/2.0, 0, 0);
            fixed4 noise = tex2Dlod(_NoiseTex, uvn) * 0.75;
            v.vertex.xyz += noise * v.normal.xyz * _RippleScale;
            
        }

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        fixed4 _RimColor;

		void surf (Input IN, inout SurfaceOutputStandard o) {
            const float kOneOverPi = 1.0 / 3.14159265;
            float uu = 0.5 - 0.5 * atan2(IN.norm.x, -IN.norm.z) * kOneOverPi;
            float vv = 1.0 - acos(IN.norm.y) * kOneOverPi;
            IN.uvs = float2(uu,vv) * _MainTex_ST.xy + _MainTex_ST.zw;

			// Albedo comes from a texture tinted by color
			//fixed4 c = tex2D (_MainTex, IN.uvs);
            //o.Albedo = c.rgb * _Color;
            o.Albedo = _Color;
            o.Normal = UnpackNormal(tex2D(_MainTex, IN.uvs));
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

            float dotp = 1.0 - saturate(dot(IN.viewDir, o.Normal));
            o.Emission = _RimColor.rgb * smoothstep(1 - 0.7, 1.0, dotp);

            //o.Alpha = c.r * _Color.a;
            o.Alpha = _Color.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
