// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Capstone/Spring Tile"
{
	Properties
	{
		_WaterBaseColor("Water Base Color", Color) = (0,1,0.931602,0.509804)
		_SkyTex("Sky Tex", 2D) = "white" {}
		_SpriteShape("Sprite Shape", 2D) = "white" {}
		_Distortion("Distortion", Range( 0 , 1)) = 0.41634
		_Opacity("Opacity", Range( 0 , 1)) = 0
		_TexTiling("Tex Tiling", Vector) = (1,1,0,0)
		_TextureSample1("Texture Sample 1", 2D) = "bump" {}
		_Offset("Offset", Vector) = (0.5,0.5,0,0)
		_XOffset("X Offset", Float) = 0
		_XAmplitude("X Amplitude", Float) = 0
		_XFrequency("X Frequency", Float) = 0
		_ZAmplitude("Z Amplitude", Float) = 0
		_ZOffset("Z Offset", Float) = 0
		_ZFrequency("Z Frequency", Float) = 0
		_TextureSample4("Texture Sample 4", 2D) = "bump" {}
		_TextureSample0("Texture Sample 0", 2D) = "bump" {}
		_TextureSample3("Texture Sample 3", 2D) = "bump" {}
		_TextureSample2("Texture Sample 2", 2D) = "bump" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
		};

		uniform float _XOffset;
		uniform float _XFrequency;
		uniform float _XAmplitude;
		uniform float _ZFrequency;
		uniform float _ZOffset;
		uniform float _ZAmplitude;
		uniform sampler2D _SkyTex;
		uniform float2 _TexTiling;
		uniform float _Distortion;
		uniform sampler2D _TextureSample1;
		uniform sampler2D _Sampler6095;
		uniform float2 _Offset;
		uniform sampler2D _TextureSample0;
		uniform sampler2D _TextureSample2;
		uniform sampler2D _TextureSample4;
		uniform sampler2D _TextureSample3;
		uniform float4 _WaterBaseColor;
		uniform sampler2D _SpriteShape;
		uniform float4 _SpriteShape_ST;
		uniform float _Opacity;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float temp_output_122_0 = ( ( sin( ( _XOffset * ( ( _Time.y * _XFrequency ) + ase_screenPosNorm.x ) ) ) * _XAmplitude ) + ( sin( ( ( ase_screenPosNorm.z + ( _ZFrequency * _Time.y ) ) * _ZOffset ) ) * _ZAmplitude ) );
			float4 appendResult123 = (float4(0.0 , temp_output_122_0 , 0.0 , 0.0));
			v.vertex.xyz += appendResult123.xyz;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float2 temp_output_1_0_g1 = float2( 1,1 );
			float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * i.uv_texcoord.x ) , ( i.uv_texcoord.y * (temp_output_1_0_g1).y )));
			float2 temp_output_11_0_g1 = float2( 0,0 );
			float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _Time.y ) * float2( 1,0 ) + i.uv_texcoord);
			float2 panner19_g1 = ( ( _Time.y * (temp_output_11_0_g1).y ) * float2( 0,1 ) + i.uv_texcoord);
			float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
			float2 temp_cast_1 = (-0.5).xx;
			float2 temp_output_47_0_g1 = temp_cast_1;
			float2 temp_output_31_0_g1 = ( ( i.uv_texcoord + _Offset ) - float2( 1,1 ) );
			float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / 6.28318548202515 ) ) , length( temp_output_31_0_g1 )));
			float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _Time.y ) * float2( 1,0 ) + appendResult39_g1);
			float2 panner55_g1 = ( ( _Time.y * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
			float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
			float2 panner128 = ( 1.0 * _Time.y * float2( 1,1 ) + i.uv_texcoord);
			float temp_output_192_0 = ( ( 1.0 - i.uv_texcoord.x ) * ( 1.0 - i.uv_texcoord.y ) );
			float3 lerpResult131 = lerp( float3( 0,0,0 ) , UnpackNormal( tex2D( _TextureSample0, panner128 ) ) , temp_output_192_0);
			float2 panner137 = ( 1.0 * _Time.y * float2( -1,-1 ) + i.uv_texcoord);
			float temp_output_133_0 = ( i.uv_texcoord.x * i.uv_texcoord.y );
			float3 lerpResult138 = lerp( float3( 0,0,0 ) , UnpackNormal( tex2D( _TextureSample2, panner137 ) ) , temp_output_133_0);
			float3 temp_output_139_0 = ( lerpResult131 + lerpResult138 );
			float2 panner151 = ( 1.0 * _Time.y * float2( 1,-1 ) + i.uv_texcoord);
			float clampResult179 = clamp( ( i.uv_texcoord.y - i.uv_texcoord.x ) , 0.0 , 1.0 );
			float3 lerpResult152 = lerp( float3( 0,0,0 ) , UnpackNormal( tex2D( _TextureSample4, panner151 ) ) , clampResult179);
			float2 panner143 = ( 1.0 * _Time.y * float2( -1,1 ) + i.uv_texcoord);
			float clampResult171 = clamp( ( i.uv_texcoord.x - i.uv_texcoord.y ) , 0.0 , 1.0 );
			float3 lerpResult149 = lerp( float3( 0,0,0 ) , UnpackNormal( tex2D( _TextureSample3, panner143 ) ) , clampResult171);
			float3 temp_output_148_0 = ( lerpResult152 + lerpResult149 );
			float clampResult202 = clamp( ( 0.5 - ( temp_output_192_0 * temp_output_133_0 * 8.0 ) ) , 0.0 , 1.0 );
			float3 lerpResult206 = lerp( UnpackNormal( tex2D( _TextureSample1, ( ( (tex2D( _Sampler6095, ( appendResult10_g1 + appendResult24_g1 ) )).rg * 1.0 ) + ( float2( 1,1 ) * appendResult58_g1 ) ) ) ) , ( temp_output_139_0 + temp_output_148_0 ) , clampResult202);
			float4 tex2DNode22 = tex2D( _SkyTex, ( float4( _TexTiling, 0.0 , 0.0 ) * ( ase_screenPos + float4( ( _Distortion * lerpResult206 ) , 0.0 ) ) ).xy );
			o.Emission = ( tex2DNode22 * tex2DNode22 * _WaterBaseColor ).rgb;
			float2 uv_SpriteShape = i.uv_texcoord * _SpriteShape_ST.xy + _SpriteShape_ST.zw;
			float clampResult24 = clamp( tex2D( _SpriteShape, uv_SpriteShape ).a , 0.0 , _Opacity );
			o.Alpha = clampResult24;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.screenPos = IN.screenPos;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15900
212;100;1339;532;2420.721;671.1594;1.809881;True;True
Node;AmplifyShaderEditor.Vector2Node;136;-2121.497,189.8804;Float;False;Constant;_Normal2Speed;Normal 2 Speed;16;0;Create;True;0;0;False;0;-1,-1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;144;-1595.095,501.9726;Float;False;Constant;_Normal3Speed;Normal 3 Speed;15;0;Create;True;0;0;False;0;1,-1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;190;-1859.49,-74.21186;Float;False;Constant;_Float0;Float 0;18;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;147;-1590.136,965.8749;Float;False;Constant;_Vector1;Vector 1;16;0;Create;True;0;0;False;0;-1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;129;-2157.981,-151.0223;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;154;-1613.261,682.8265;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;130;-2126.456,-274.0222;Float;False;Constant;_Normal1Speed;Normal 1 Speed;15;0;Create;True;0;0;False;0;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;191;-1672.209,85.32407;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;143;-1385.733,955.3926;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;173;-1366.16,780.1255;Float;False;Constant;_Float5;Float 5;18;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;128;-1899.874,-182.129;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;172;-1366.16,857.6392;Float;False;Constant;_Float4;Float 4;18;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;189;-1674.982,-9.010234;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;178;-1030.969,637.7949;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;151;-1368.513,593.8654;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;166;-1236.97,775.2809;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;137;-1917.094,179.3981;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;153;-1175.833,450.8643;Float;True;Property;_TextureSample4;Texture Sample 4;14;0;Create;True;0;0;False;0;50de7d62666e88647be03746955f5bae;50de7d62666e88647be03746955f5bae;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;179;-973.2737,660.3777;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;201;-541.6281,317.675;Float;False;Constant;_Float3;Float 3;18;0;Create;True;0;0;False;0;8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;135;-1709.139,234.552;Float;True;Property;_TextureSample2;Texture Sample 2;17;0;Create;True;0;0;False;0;50de7d62666e88647be03746955f5bae;50de7d62666e88647be03746955f5bae;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;133;-1893.981,2.347986;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;127;-1717.799,-210.252;Float;True;Property;_TextureSample0;Texture Sample 0;15;0;Create;True;0;0;False;0;50de7d62666e88647be03746955f5bae;50de7d62666e88647be03746955f5bae;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;142;-1175.476,963.5688;Float;True;Property;_TextureSample3;Texture Sample 3;16;0;Create;True;0;0;False;0;50de7d62666e88647be03746955f5bae;50de7d62666e88647be03746955f5bae;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;192;-1502.831,-21.61332;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;171;-905.0087,867.1061;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;100;-1488.341,-810.3782;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;101;-1440.341,-682.3782;Float;False;Property;_Offset;Offset;7;0;Create;True;0;0;False;0;0.5,0.5;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;199;-328.8549,317.6392;Float;False;Constant;_Float2;Float 2;18;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;152;-644.3103,620.4399;Float;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;197;-384.9543,397.1847;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;138;-1390.434,204.2004;Float;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;131;-1386.006,-149.8245;Float;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;149;-646.3636,848.428;Float;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;102;-1200.341,-778.3782;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-1248.341,-874.3782;Float;False;Constant;_Speed;Speed;7;0;Create;True;0;0;False;0;-0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;105;74.20071,1102.102;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;103;64.35497,690.7625;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;106;69.40121,1020.503;Float;False;Property;_ZFrequency;Z Frequency;13;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;148;-731.6104,383.4746;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;104;61.85515,760.762;Float;False;Property;_XFrequency;X Frequency;10;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;95;-1056.341,-938.3782;Float;False;RadialUVDistortion;-1;;1;051d65e7699b41a4c800363fd0e822b2;0;7;60;SAMPLER2D;_Sampler6095;False;1;FLOAT2;1,1;False;11;FLOAT2;0,0;False;65;FLOAT;1;False;68;FLOAT2;1,1;False;47;FLOAT2;1,1;False;29;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;139;-1130.718,-68.08069;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;200;-169.4679,106.2241;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;203;-65.89091,313.7085;Float;False;Constant;_Float6;Float 6;18;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;240.601,1036.502;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;107;55.03358,846.999;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;202;74.35743,88.79887;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;108;251.4395,722.9415;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;196;-473.0088,5.998536;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;96;-509.1121,-199.5179;Float;True;Property;_TextureSample1;Texture Sample 1;6;0;Create;True;0;0;False;0;50de7d62666e88647be03746955f5bae;50de7d62666e88647be03746955f5bae;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-531.2681,-311.0519;Float;False;Property;_Distortion;Distortion;3;0;Create;True;0;0;False;0;0.41634;0.08;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;206;-60.25749,-138.7976;Float;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;111;334.4395,815.9415;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;383.0009,1007.702;Float;False;Property;_ZOffset;Z Offset;12;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;113;328.6008,911.7026;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;110;230.4394,632.9417;Float;False;Property;_XOffset;X Offset;8;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;401.4395,658.9416;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;14;-206.2737,-609.4287;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;207;-58.13767,-302.9263;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;456.6011,890.9028;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;17;176.4991,-483.3917;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SinOpNode;118;578.2012,831.7025;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;116;474.8835,752.5154;Float;False;Property;_XAmplitude;X Amplitude;9;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;117;597.4013,918.1025;Float;False;Property;_ZAmplitude;Z Amplitude;11;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;16;127.7671,-615.4952;Float;False;Property;_TexTiling;Tex Tiling;5;0;Create;True;0;0;False;0;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SinOpNode;119;547.2001,668.0927;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;738.2014,786.9026;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;671.8836,679.5154;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;179.8978,-281.1557;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;22;430.5663,-263.1481;Float;True;Property;_SkyTex;Sky Tex;1;0;Create;False;0;0;False;0;8aad9d9563a70ee4a9f86dd5b027a106;8aad9d9563a70ee4a9f86dd5b027a106;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;20;769.127,344.3925;Float;False;Property;_Opacity;Opacity;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;892.1422,259.9818;Float;False;Constant;_0;0;10;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;122;826.2013,678.1026;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;19;490.1011,-454.7674;Float;False;Property;_WaterBaseColor;Water Base Color;0;0;Create;True;0;0;False;0;0,1,0.931602,0.509804;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;735.1581,73.60037;Float;True;Property;_SpriteShape;Sprite Shape;2;0;Create;True;0;0;False;0;4ddd7565e4b24b94794e4d8b4b727cf0;4ddd7565e4b24b94794e4d8b4b727cf0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;126;607.4963,514.2968;Float;False;Constant;_Zero;Zero;11;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;123;967.5121,573.475;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ClampOpNode;24;1189.675,32.76529;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;193;-739.0686,197.5346;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;141;-1082.277,145.2654;Float;False;Constant;_Float1;Float 1;16;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;140;-894.1627,-67.44868;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;124;786.6373,559.3585;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;125;599.2374,586.5585;Float;False;Constant;_One;One;14;0;Create;True;0;0;False;0;-0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;806.1241,-304.1977;Float;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1153.806,-370.7315;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Capstone/Spring Tile;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;191;0;190;0
WireConnection;191;1;129;2
WireConnection;143;0;154;0
WireConnection;143;2;147;0
WireConnection;128;0;129;0
WireConnection;128;2;130;0
WireConnection;189;0;190;0
WireConnection;189;1;129;1
WireConnection;178;0;154;2
WireConnection;178;1;154;1
WireConnection;151;0;154;0
WireConnection;151;2;144;0
WireConnection;166;0;154;1
WireConnection;166;1;154;2
WireConnection;137;0;129;0
WireConnection;137;2;136;0
WireConnection;153;1;151;0
WireConnection;179;0;178;0
WireConnection;179;1;173;0
WireConnection;179;2;172;0
WireConnection;135;1;137;0
WireConnection;133;0;129;1
WireConnection;133;1;129;2
WireConnection;127;1;128;0
WireConnection;142;1;143;0
WireConnection;192;0;189;0
WireConnection;192;1;191;0
WireConnection;171;0;166;0
WireConnection;171;1;173;0
WireConnection;171;2;172;0
WireConnection;152;1;153;0
WireConnection;152;2;179;0
WireConnection;197;0;192;0
WireConnection;197;1;133;0
WireConnection;197;2;201;0
WireConnection;138;1;135;0
WireConnection;138;2;133;0
WireConnection;131;1;127;0
WireConnection;131;2;192;0
WireConnection;149;1;142;0
WireConnection;149;2;171;0
WireConnection;102;0;100;0
WireConnection;102;1;101;0
WireConnection;148;0;152;0
WireConnection;148;1;149;0
WireConnection;95;47;97;0
WireConnection;95;29;102;0
WireConnection;139;0;131;0
WireConnection;139;1;138;0
WireConnection;200;0;199;0
WireConnection;200;1;197;0
WireConnection;109;0;106;0
WireConnection;109;1;105;0
WireConnection;202;0;200;0
WireConnection;202;1;203;0
WireConnection;108;0;103;0
WireConnection;108;1;104;0
WireConnection;196;0;139;0
WireConnection;196;1;148;0
WireConnection;96;1;95;0
WireConnection;206;0;96;0
WireConnection;206;1;196;0
WireConnection;206;2;202;0
WireConnection;111;0;108;0
WireConnection;111;1;107;1
WireConnection;113;0;107;3
WireConnection;113;1;109;0
WireConnection;115;0;110;0
WireConnection;115;1;111;0
WireConnection;207;0;11;0
WireConnection;207;1;206;0
WireConnection;114;0;113;0
WireConnection;114;1;112;0
WireConnection;17;0;14;0
WireConnection;17;1;207;0
WireConnection;118;0;114;0
WireConnection;119;0;115;0
WireConnection;120;0;118;0
WireConnection;120;1;117;0
WireConnection;121;0;119;0
WireConnection;121;1;116;0
WireConnection;18;0;16;0
WireConnection;18;1;17;0
WireConnection;22;1;18;0
WireConnection;122;0;121;0
WireConnection;122;1;120;0
WireConnection;123;0;126;0
WireConnection;123;1;122;0
WireConnection;123;2;126;0
WireConnection;24;0;23;4
WireConnection;24;1;21;0
WireConnection;24;2;20;0
WireConnection;193;0;148;0
WireConnection;193;1;141;0
WireConnection;140;0;139;0
WireConnection;140;1;141;0
WireConnection;124;0;125;0
WireConnection;124;1;122;0
WireConnection;25;0;22;0
WireConnection;25;1;22;0
WireConnection;25;2;19;0
WireConnection;0;2;25;0
WireConnection;0;9;24;0
WireConnection;0;11;123;0
ASEEND*/
//CHKSM=0C2A5AB182BF834489DDBB425E1535D3F917FA22