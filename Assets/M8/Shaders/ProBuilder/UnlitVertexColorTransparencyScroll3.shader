// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "M8/ProBuilder/Unlit Vertex Color Transparency Scroll 3" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
	_MainTex2 ("Texture2", 2D) = "white" {}
	
	speedX ("Speed X", Float) = 1
	speedY ("Speed Y", Float) = 1
	
	speed2X ("Speed 2 X", Float) = 1
	speed2Y ("Speed 2 Y", Float) = 1
	
	colorMod1 ("Color", Color) = (1,1,1,1)
	colorMod2 ("Color 2", Color) = (1,1,1,1)
  }
  SubShader {
	Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}

    //ZWrite Off // on might hide behind pixels, off might miss order
    //Blend SrcAlpha OneMinusSrcAlpha
	//Blend SrcAlpha OneMinusSrcAlpha
	Blend One One
    ColorMask RGB
	Lighting Off Fog { Mode Off }

	Pass {
		CGPROGRAM
		#pragma vertex vert_vct
		#pragma fragment frag_mult
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		
		float speedX;
		float speedY;
		
		float speed2X;
		float speed2Y;
		
		fixed4 colorMod1;
		fixed4 colorMod2;

		struct vin_vct 
		{
			float4 vertex : POSITION;
			float4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f_vct
		{
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		v2f_vct vert_vct(vin_vct v)
		{
			v2f_vct o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.color = v.color;
			
			o.texcoord = v.texcoord;
			o.texcoord.x += speedX * _Time.y;
			o.texcoord.y += speedY * _Time.y;
			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR
		{
			return tex2D(_MainTex, i.texcoord) * i.color * colorMod1;
		}

		ENDCG
	}
	
	Pass {
		CGPROGRAM
		#pragma vertex vert_vct
		#pragma fragment frag_mult
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		sampler2D _MainTex2;
		
		float speedX;
		float speedY;
		
		float speed2X;
		float speed2Y;
		
		fixed4 colorMod1;
		fixed4 colorMod2;

		struct vin_vct 
		{
			float4 vertex : POSITION;
			float4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f_vct
		{
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		v2f_vct vert_vct(vin_vct v)
		{
			v2f_vct o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.color = v.color;
			
			o.texcoord = v.texcoord;
			o.texcoord.x += speed2X * _Time.y;
			o.texcoord.y += speed2Y * _Time.y;
			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR
		{
			return tex2D(_MainTex2, i.texcoord) * colorMod2;
		}

		ENDCG
	}
  }
  Fallback "Transparent/Diffuse"
}
