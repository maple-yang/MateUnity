﻿//Only use for sprites
Shader "M8/Sprite/TextureOverlayWorld"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

		_Overlay("Overlay", 2D) = "white" {}
		_OverlayColor("Overlay Tint", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex SpriteVert_Overlay
			#pragma fragment SpriteFrag_Overlay
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile_local _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			struct v2fExt
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord2 : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _Overlay;
			float4 _Overlay_ST;
			fixed4 _OverlayColor;

			v2fExt SpriteVert_Overlay(appdata_t IN)
			{
				v2fExt OUT;

				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
				OUT.vertex = UnityObjectToClipPos(OUT.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color * _RendererColor;

				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				//coord for overlay to accomodate texture scale/offset
				OUT.texcoord2 = mul(unity_ObjectToWorld, IN.vertex) * _Overlay_ST.xy + _Overlay_ST.zw;

				return OUT;
			}

			fixed4 SpriteFrag_Overlay(v2fExt IN) : COLOR
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord);
				fixed4 o = tex2D(_Overlay, IN.texcoord2) * _OverlayColor;

				c.rgb = lerp(c.rgb, o.rgb, o.a);

				c *= IN.color;

				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
