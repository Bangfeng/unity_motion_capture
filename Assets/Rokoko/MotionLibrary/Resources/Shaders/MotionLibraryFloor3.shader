// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MotionLibrary/MotionLibraryFloor3"
{
	Properties
	{
		[NoScaleOffset]_MainTexture("MainTexture", 2D) = "white" {}
		_Tiles("Tiles", Range( 0.2 , 4)) = 0.04
		_SubTiles("SubTiles", Int) = 4
		_Radius("Radius", Float) = 2
		_FadeDistance("FadeDistance", Float) = 2
		_MainColor("MainColor", Color) = (0.5849056,0.5849056,0.5849056,0)
		_GridColor("GridColor", Color) = (1,1,1,0)
		_FloorOpacity("FloorOpacity", Range( 0 , 1)) = 0
		_GridStrength("GridStrength", Range( 0 , 1)) = 0
		_GridOpacity("GridOpacity", Range( 0 , 1)) = 0
		_ExtraOpacity("ExtraOpacity", Range( 0 , 1)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
		};

		uniform sampler2D _GrabTexture;
		uniform float _FloorOpacity;
		uniform float _GridOpacity;
		uniform sampler2D _MainTexture;
		uniform float _Tiles;
		uniform int _SubTiles;
		uniform float _FadeDistance;
		uniform float _Radius;
		uniform float _ExtraOpacity;
		uniform float4 _MainColor;
		uniform float4 _GridColor;
		uniform float _GridStrength;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 screenColor26 = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD( ase_grabScreenPos ) );
			float4 _black = float4(0,0,0,0);
			float3 ase_worldPos = i.worldPos;
			float4 appendResult6 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float temp_output_22_0 = max( tex2D( _MainTexture, ( appendResult6 * _Tiles * 0.5 ).xy ).r , tex2D( _MainTexture, ( appendResult6 * _SubTiles * _Tiles * 0.5 ).xy ).r );
			float gridMask51 = temp_output_22_0;
			float lerpResult53 = lerp( _FloorOpacity , _GridOpacity , gridMask51);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float clampResult21 = clamp( (0.0 + (length( ( ase_vertex3Pos * ase_objectScale ) ) - ( _FadeDistance + _Radius )) * (1.0 - 0.0) / (_Radius - ( _FadeDistance + _Radius ))) , 0.0 , 1.0 );
			float Mask224 = ( lerpResult53 * clampResult21 * _ExtraOpacity );
			float4 lerpResult30 = lerp( screenColor26 , _black , Mask224);
			float4 lerpResult28 = lerp( _MainColor , _GridColor , ( _GridStrength * temp_output_22_0 ));
			float4 lerpResult33 = lerp( _black , lerpResult28 , Mask224);
			float4 lerpResult40 = lerp( lerpResult30 , lerpResult33 , Mask224);
			o.Emission = lerpResult40.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15600
-1686;263;1420;890;1450.76;-12.19821;1;True;True
Node;AmplifyShaderEditor.WorldPosInputsNode;1;-2977.985,-219.1988;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;6;-2374.984,-357.1988;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.IntNode;7;-2242.984,-108.1988;Float;False;Property;_SubTiles;SubTiles;2;0;Create;True;0;0;False;0;4;1;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-2184.115,-492.3999;Float;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-2363.984,-192.1988;Float;False;Property;_Tiles;Tiles;1;0;Create;True;0;0;False;0;0.04;2;0.2;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectScaleNode;3;-2309.984,178.8011;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TexturePropertyNode;15;-2665.029,-469.3017;Float;True;Property;_MainTexture;MainTexture;0;1;[NoScaleOffset];Create;True;0;0;False;0;bb452507b4ba94349a2d3002dfff6a7f;b6362d040c2678643832b7603ae12b0e;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-1978.984,-395.1989;Float;False;3;3;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-1968.685,-159.3794;Float;False;4;4;0;FLOAT4;0,0,0,0;False;1;INT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PosVertexDataNode;5;-2316.984,24.80121;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;17;-1787.666,-470.0018;Float;True;Property;_TextureSample0;Texture Sample 0;9;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;18;-1799.046,-224.4995;Float;True;Property;_TextureSample1;Texture Sample 1;9;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;9;-1766.514,743.2321;Float;False;Property;_Radius;Radius;4;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-2002.984,87.80122;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-2012.748,665.586;Float;False;Property;_FadeDistance;FadeDistance;5;0;Create;True;0;0;False;0;2;9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;22;-1433.456,-203.0353;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;12;-1740.984,190.8011;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-1564.33,568.4604;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-1150.731,322.5305;Float;False;Property;_GridOpacity;GridOpacity;10;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1152.124,242.3643;Float;False;Property;_FloorOpacity;FloorOpacity;8;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;51;-1261.953,-122.3893;Float;False;gridMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;-1079.168,410.044;Float;False;51;gridMask;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;19;-1303.798,534.6246;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;53;-813.168,302.044;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;21;-1019.893,556.9217;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-1447.283,-383.4536;Float;False;Property;_GridStrength;GridStrength;9;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-862.7603,637.1982;Float;False;Property;_ExtraOpacity;ExtraOpacity;11;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;20;-1347.648,-586.8648;Float;False;Property;_GridColor;GridColor;7;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-1379.655,-819.2106;Float;False;Property;_MainColor;MainColor;6;0;Create;True;0;0;False;0;0.5849056,0.5849056,0.5849056,0;0.1176471,0.1098039,0.1137255,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-1167.094,-370.0589;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-622.0007,506.6701;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;28;-1009.769,-432.2131;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-524.2685,208.2581;Float;False;24;Mask2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;25;-1002.181,19.50735;Float;False;Constant;_black;black;9;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;23;-1043.668,-216.7832;Float;False;24;Mask2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-328.1891,524.9524;Float;False;Mask2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;26;-636.1374,-131.5954;Float;False;Global;_GrabScreen0;Grab Screen 0;8;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;30;-394.1553,-1.062281;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;33;-611.1817,-308.5102;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;40;-184.2837,-88.81354;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldPosInputsNode;36;-2122.815,471.3381;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;31;-2128.118,333.4843;Float;False;Property;_Center;Center;3;1;[HideInInspector];Create;True;0;0;False;0;0.6,0.8;0.6,0.8;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;35;-1894.705,454.8475;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-2671.745,125.1009;Float;False;Constant;_ff;ff;3;0;Create;True;0;0;False;0;4;4;1;16;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;29;-1737.884,398.716;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;34;-223.5462,163.3178;Float;False;24;Mask2;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;37;-2990.078,-435.8256;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldToObjectTransfNode;4;-2633.984,-165.1988;Float;False;1;0;FLOAT4;20,20,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;42;16,-134;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;MotionLibrary/MotionLibraryFloor3;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;0;1;1
WireConnection;6;1;1;3
WireConnection;13;0;6;0
WireConnection;13;1;8;0
WireConnection;13;2;38;0
WireConnection;14;0;6;0
WireConnection;14;1;7;0
WireConnection;14;2;8;0
WireConnection;14;3;38;0
WireConnection;17;0;15;0
WireConnection;17;1;13;0
WireConnection;18;0;15;0
WireConnection;18;1;14;0
WireConnection;10;0;5;0
WireConnection;10;1;3;0
WireConnection;22;0;17;1
WireConnection;22;1;18;1
WireConnection;12;0;10;0
WireConnection;16;0;11;0
WireConnection;16;1;9;0
WireConnection;51;0;22;0
WireConnection;19;0;12;0
WireConnection;19;1;16;0
WireConnection;19;2;9;0
WireConnection;53;0;44;0
WireConnection;53;1;54;0
WireConnection;53;2;52;0
WireConnection;21;0;19;0
WireConnection;50;0;46;0
WireConnection;50;1;22;0
WireConnection;49;0;53;0
WireConnection;49;1;21;0
WireConnection;49;2;56;0
WireConnection;28;0;2;0
WireConnection;28;1;20;0
WireConnection;28;2;50;0
WireConnection;24;0;49;0
WireConnection;30;0;26;0
WireConnection;30;1;25;0
WireConnection;30;2;27;0
WireConnection;33;0;25;0
WireConnection;33;1;28;0
WireConnection;33;2;23;0
WireConnection;40;0;30;0
WireConnection;40;1;33;0
WireConnection;40;2;27;0
WireConnection;35;0;36;1
WireConnection;35;1;36;3
WireConnection;29;0;31;0
WireConnection;29;1;35;0
WireConnection;4;0;1;0
WireConnection;42;2;40;0
ASEEND*/
//CHKSM=D9796C0A095F62933C735E99EAA6DA62D8AFBD11