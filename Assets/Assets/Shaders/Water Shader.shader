// Upgrade NOTE: upgraded instancing buffer 'CapstoneWater' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Capstone/Water"
{
	Properties
	{
		_WaterBaseColor("Water Base Color", Color) = (0,1,0.9333333,0.509804)
		_SkyTex("Sky Tex", 2D) = "white" {}
		_SpriteShape("Sprite Shape", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "bump" {}
		_Distortion("Distortion", Range( 0 , 1)) = 0
		_UV1PanSpeed("UV1 Pan Speed", Vector) = (0.1,0,0,0)
		_UVTiling("UV Tiling", Vector) = (0,0,0,0)
		_NormalMap2("Normal Map 2", 2D) = "bump" {}
		_Opacity("Opacity", Range( 0 , 1)) = 0
		_TexTiling("Tex Tiling", Vector) = (1,1,0,0)
		_XOffset("X Offset", Float) = 0
		_XAmplitude("X Amplitude", Float) = 0
		_XFrequency("X Frequency", Float) = 0
		_ZAmplitude("Z Amplitude", Float) = 0
		_ZOffset("Z Offset", Float) = 0
		_ZFrequency("Z Frequency", Float) = 0
		_UV2PanSpeed("UV2 Pan Speed", Vector) = (0.05,0,0,0)
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
		#pragma multi_compile_instancing
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
		uniform sampler2D _NormalMap;
		uniform float2 _UVTiling;
		uniform sampler2D _NormalMap2;
		uniform float4 _WaterBaseColor;
		uniform sampler2D _SpriteShape;
		uniform float4 _SpriteShape_ST;
		uniform float _Opacity;

		UNITY_INSTANCING_BUFFER_START(CapstoneWater)
			UNITY_DEFINE_INSTANCED_PROP(float2, _UV2PanSpeed)
#define _UV2PanSpeed_arr CapstoneWater
			UNITY_DEFINE_INSTANCED_PROP(float2, _UV1PanSpeed)
#define _UV1PanSpeed_arr CapstoneWater
			UNITY_DEFINE_INSTANCED_PROP(float, _Distortion)
#define _Distortion_arr CapstoneWater
		UNITY_INSTANCING_BUFFER_END(CapstoneWater)

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float temp_output_71_0 = ( ( sin( ( _XOffset * ( ( _Time.y * _XFrequency ) + ase_screenPosNorm.x ) ) ) * _XAmplitude ) + ( sin( ( ( ase_screenPosNorm.z + ( _ZFrequency * _Time.y ) ) * _ZOffset ) ) * _ZAmplitude ) );
			float4 appendResult56 = (float4(0.0 , temp_output_71_0 , 0.0 , 0.0));
			v.vertex.xyz += appendResult56.xyz;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float2 _UV1PanSpeed_Instance = UNITY_ACCESS_INSTANCED_PROP(_UV1PanSpeed_arr, _UV1PanSpeed);
			float2 panner12 = ( _Time.y * _UV1PanSpeed_Instance + i.uv_texcoord);
			float _Distortion_Instance = UNITY_ACCESS_INSTANCED_PROP(_Distortion_arr, _Distortion);
			float2 _UV2PanSpeed_Instance = UNITY_ACCESS_INSTANCED_PROP(_UV2PanSpeed_arr, _UV2PanSpeed);
			float2 panner34 = ( _Time.y * _UV2PanSpeed_Instance + i.uv_texcoord);
			float4 tex2DNode2 = tex2D( _SkyTex, ( float4( _TexTiling, 0.0 , 0.0 ) * ( ase_screenPos + float4( ( UnpackNormal( tex2D( _NormalMap, ( _UVTiling * panner12 ) ) ) * _Distortion_Instance ) , 0.0 ) + float4( ( _Distortion_Instance * UnpackNormal( tex2D( _NormalMap2, panner34 ) ) ) , 0.0 ) ) ).xy );
			o.Emission = ( tex2DNode2 * tex2DNode2 * _WaterBaseColor ).rgb;
			float2 uv_SpriteShape = i.uv_texcoord * _SpriteShape_ST.xy + _SpriteShape_ST.zw;
			float clampResult41 = clamp( tex2D( _SpriteShape, uv_SpriteShape ).a , 0.0 , _Opacity );
			o.Alpha = clampResult41;
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
206;92;1350;616;2364.124;599.0095;2.392709;True;True
Node;AmplifyShaderEditor.Vector2Node;19;-2226.869,213.7346;Float;False;InstancedProperty;_UV1PanSpeed;UV1 Pan Speed;5;0;Create;True;0;0;False;0;0.1,0;0.2,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;10;-2142.177,331.6471;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-2168.827,46.23799;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;12;-1867.673,280.678;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;72;-2241.396,444.4236;Float;False;InstancedProperty;_UV2PanSpeed;UV2 Pan Speed;16;0;Create;True;0;0;False;0;0.05,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;25;-1840.743,144.3435;Float;False;Property;_UVTiling;UV Tiling;6;0;Create;True;0;0;False;0;0,0;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;62;-696.9276,854.924;Float;False;Property;_ZFrequency;Z Frequency;15;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;63;-692.1281,936.5238;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;46;-701.9739,525.184;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-704.4737,595.1835;Float;False;Property;_XFrequency;X Frequency;12;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-514.8894,557.363;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-1680.262,161.4116;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;34;-1818.573,485.8773;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;58;-711.2953,681.4205;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-525.7278,870.9238;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1607.672,457.869;Float;False;InstancedProperty;_Distortion;Distortion;4;0;Create;True;0;0;False;0;0;0.08;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-383.3279,842.1238;Float;False;Property;_ZOffset;Z Offset;14;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-535.8894,467.363;Float;False;Property;_XOffset;X Offset;10;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-1620.126,257.8402;Float;True;Property;_NormalMap;Normal Map;3;0;Create;True;0;0;False;0;None;50de7d62666e88647be03746955f5bae;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;65;-437.7281,746.124;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;49;-431.8894,650.363;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;29;-1616.932,539.8926;Float;True;Property;_NormalMap2;Normal Map 2;7;0;Create;True;0;0;False;0;None;50de7d62666e88647be03746955f5bae;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-364.8894,493.363;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;45;-1851.673,-47.63255;Float;True;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1290.273,418.1057;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-309.7277,725.3242;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-1284.134,277.801;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SinOpNode;68;-188.1277,666.1239;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-168.9276,752.5239;Float;False;Property;_ZAmplitude;Z Amplitude;13;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;53;-219.1287,502.5142;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;43;-1169.578,-16.79162;Float;False;Property;_TexTiling;Tex Tiling;9;0;Create;True;0;0;False;0;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;54;-291.4453,586.9369;Float;False;Property;_XAmplitude;X Amplitude;11;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;-1120.846,115.3114;Float;False;3;3;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-28.12763,621.3241;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-977.028,100.1245;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-94.44531,513.9369;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-1077.455,298.2541;Float;True;Property;_SpriteShape;Sprite Shape;2;0;Create;True;0;0;False;0;4ddd7565e4b24b94794e4d8b4b727cf0;4ddd7565e4b24b94794e4d8b4b727cf0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;57;-158.8326,348.718;Float;False;Constant;_Zero;Zero;11;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;71;59.87244,512.5241;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-762.9039,-109.7791;Float;False;Property;_WaterBaseColor;Water Base Color;0;0;Create;True;0;0;False;0;0,1,0.9333333,0.509804;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;40;-1060.487,585.0467;Float;False;Property;_Opacity;Opacity;8;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-937.4715,500.6359;Float;False;Constant;_0;0;10;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-822.4387,81.84019;Float;True;Property;_SkyTex;Sky Tex;1;0;Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;60;20.30844,393.7797;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-167.0916,420.9797;Float;False;Constant;_One;One;14;0;Create;True;0;0;False;0;-0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;27;-2404.208,-23.86478;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;41;-611.1108,287.0747;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;56;201.1832,407.8962;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-446.8809,40.79059;Float;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;224.2967,12.24699;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Capstone/Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;1;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;11;0
WireConnection;12;2;19;0
WireConnection;12;1;10;0
WireConnection;48;0;46;0
WireConnection;48;1;47;0
WireConnection;26;0;25;0
WireConnection;26;1;12;0
WireConnection;34;0;11;0
WireConnection;34;2;72;0
WireConnection;34;1;10;0
WireConnection;64;0;62;0
WireConnection;64;1;63;0
WireConnection;5;1;26;0
WireConnection;65;0;58;3
WireConnection;65;1;64;0
WireConnection;49;0;48;0
WireConnection;49;1;58;1
WireConnection;29;1;34;0
WireConnection;52;0;51;0
WireConnection;52;1;49;0
WireConnection;30;0;15;0
WireConnection;30;1;29;0
WireConnection;67;0;65;0
WireConnection;67;1;66;0
WireConnection;14;0;5;0
WireConnection;14;1;15;0
WireConnection;68;0;67;0
WireConnection;53;0;52;0
WireConnection;13;0;45;0
WireConnection;13;1;14;0
WireConnection;13;2;30;0
WireConnection;70;0;68;0
WireConnection;70;1;69;0
WireConnection;44;0;43;0
WireConnection;44;1;13;0
WireConnection;55;0;53;0
WireConnection;55;1;54;0
WireConnection;71;0;55;0
WireConnection;71;1;70;0
WireConnection;2;1;44;0
WireConnection;60;0;59;0
WireConnection;60;1;71;0
WireConnection;41;0;4;4
WireConnection;41;1;42;0
WireConnection;41;2;40;0
WireConnection;56;0;57;0
WireConnection;56;1;71;0
WireConnection;56;2;57;0
WireConnection;3;0;2;0
WireConnection;3;1;2;0
WireConnection;3;2;1;0
WireConnection;0;2;3;0
WireConnection;0;9;41;0
WireConnection;0;11;56;0
ASEEND*/
//CHKSM=3815F8758DF08D4EA41B819B6A3358830E03835A