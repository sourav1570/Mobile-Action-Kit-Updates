// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AmplifyWater"
{
	Properties
	{
		_ColorDeep("ColorDeep", Color) = (0,0,0,0)
		_ColorShallow("ColorShallow", Color) = (0,0,0,0)
		_NormalMap("Normal Map", 2D) = "bump" {}
		_Glossiness("Glossiness", Range( 0 , 1)) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_NormalMap1strength("NormalMap1strength", Range( 0 , 1)) = 0
		_NormalMap2strength("NormalMap2strength", Range( 0 , 1)) = 0
		_UV1Animator("UV1 Animator", Vector) = (0,0,0,0)
		_UV2Animator("UV2 Animator", Vector) = (0,0,0,0)
		_UVscale("UVscale", Float) = 0
		_NormalBlendStrength("NormalBlendStrength", Range( 0 , 1)) = 0
		_UV2Tiling("UV2 Tiling", Range( 0 , 100)) = 0
		_UV1Tiling("UV1 Tiling", Range( 1 , 100)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "ForceNoShadowCasting" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 2.0
		#pragma surface surf Standard keepalpha noshadow 
		struct Input
		{
			float3 worldPos;
		};

		uniform sampler2D _NormalMap;
		uniform float _NormalMap1strength;
		uniform float2 _UV1Animator;
		uniform float _UVscale;
		uniform float _UV1Tiling;
		uniform float _NormalMap2strength;
		uniform float2 _UV2Animator;
		uniform float _UV2Tiling;
		uniform float _NormalBlendStrength;
		uniform float4 _ColorDeep;
		uniform float4 _ColorShallow;
		uniform float _Metallic;
		uniform float _Glossiness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float2 appendResult2 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 WorldUV5 = ( appendResult2 / _UVscale );
			float2 panner28 = ( _Time.x * _UV1Animator + ( WorldUV5 * _UV1Tiling ));
			float2 _UV137 = panner28;
			float2 panner29 = ( _Time.x * _UV2Animator + ( WorldUV5 * _UV2Tiling ));
			float2 _UV238 = panner29;
			float3 lerpResult9 = lerp( UnpackScaleNormal( tex2D( _NormalMap, _UV137 ), _NormalMap1strength ) , UnpackScaleNormal( tex2D( _NormalMap, _UV238 ), _NormalMap2strength ) , _NormalBlendStrength);
			float3 NormalMap16 = lerpResult9;
			o.Normal = NormalMap16;
			float4 lerpResult15 = lerp( _ColorDeep , _ColorShallow , float4( NormalMap16 , 0.0 ));
			float4 _color20 = lerpResult15;
			o.Albedo = _color20.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
0;92;1005;966;4204.788;1178.017;2.736277;False;False
Node;AmplifyShaderEditor.CommentaryNode;41;-3906.156,-108.6165;Inherit;False;1585.488;734.6997;UV 1 and 2;13;34;38;37;31;32;25;35;30;27;33;29;28;1;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1;-2551.783,475.9561;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;24;-2581.524,370.247;Inherit;False;1075;339;WorldUV;4;2;4;3;5;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;2;-2158.524,469.2466;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2165.524,594.2458;Inherit;False;Property;_UVscale;UVscale;9;0;Create;True;0;0;False;0;False;0;209.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;3;-1925.526,538.2457;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;5;-1749.527,525.2457;Float;False;WorldUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;-3878.812,80.19981;Inherit;False;5;WorldUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-3589.553,71.7298;Float;False;Property;_UV1Tiling;UV1 Tiling;12;0;Create;True;0;0;False;0;False;0;8.5;1;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-3588.724,204.7861;Float;False;Property;_UV2Tiling;UV2 Tiling;11;0;Create;True;0;0;False;0;False;0;32.9;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-3293.629,195.4222;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;27;-3877.705,191.286;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-3231.805,-43.51312;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;35;-3271.323,49.18682;Inherit;False;Property;_UV1Animator;UV1 Animator;7;0;Create;True;0;0;False;0;False;0,0;0,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;34;-3470.113,304.9881;Float;False;Property;_UV2Animator;UV2 Animator;8;0;Create;True;0;0;False;0;False;0,0;0,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;28;-2983.711,17.30071;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;29;-3114.109,243.7221;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;22;-3921.507,-813.1964;Inherit;False;1547.069;618.4875;NormalMap;10;9;7;8;6;10;11;12;16;40;39;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;38;-2730.782,167.7052;Inherit;False;_UV2;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;-2719.23,51.00087;Inherit;False;_UV1;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;8;-3882.896,-588.4547;Inherit;True;Property;_NormalMap;Normal Map;2;0;Create;True;0;0;False;0;False;None;dd2fd2df93418444c8e280f1d34deeb5;True;bump;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;39;-3864.574,-757.7903;Inherit;False;37;_UV1;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-3909.321,-383.9643;Inherit;False;Property;_NormalMap1strength;NormalMap1strength;5;0;Create;True;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-3910.845,-304.3047;Inherit;False;Property;_NormalMap2strength;NormalMap2strength;6;0;Create;True;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;40;-3869.151,-673.9528;Inherit;False;38;_UV2;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;6;-3511.703,-772.126;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;12;-3495.478,-318.639;Float;False;Property;_NormalBlendStrength;NormalBlendStrength;10;0;Create;True;0;0;False;0;False;0;0.469;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;7;-3502.703,-535.1263;Inherit;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;9;-3140.303,-654.6863;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;23;-2719.901,-791.7316;Inherit;False;1035;651.7464;Color;7;13;14;15;19;18;20;44;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-2955.541,-721.7343;Inherit;False;NormalMap;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;14;-2516.826,-594.5739;Inherit;False;Property;_ColorShallow;ColorShallow;1;0;Create;True;0;0;False;0;False;0,0,0,0;0.1727941,0.3455882,0.3110293,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;13;-2518.913,-758.6312;Inherit;False;Property;_ColorDeep;ColorDeep;0;0;Create;True;0;0;False;0;False;0,0,0,0;0.03921569,0.3137254,0.2588235,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;44;-2505.332,-413.1678;Inherit;False;16;NormalMap;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;15;-2118.358,-674.5507;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-1927.903,-699.9854;Float;False;_color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;21;-2216.046,16.23279;Inherit;False;20;_color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;17;-2222.184,87.84799;Inherit;False;16;NormalMap;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-2217.765,231.6635;Inherit;False;Property;_Glossiness;Glossiness;3;0;Create;True;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;19;-2202.825,-353.3125;Inherit;False;16;NormalMap;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;18;-1979.358,-358.0725;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-2219.765,160.6637;Inherit;False;Property;_Metallic;Metallic;4;0;Create;True;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1887.671,-97.71992;Float;False;True;-1;0;ASEMaterialInspector;0;0;Standard;AmplifyWater;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;0;1;1
WireConnection;2;1;1;3
WireConnection;3;0;2;0
WireConnection;3;1;4;0
WireConnection;5;0;3;0
WireConnection;33;0;25;0
WireConnection;33;1;32;0
WireConnection;30;0;25;0
WireConnection;30;1;31;0
WireConnection;28;0;30;0
WireConnection;28;2;35;0
WireConnection;28;1;27;1
WireConnection;29;0;33;0
WireConnection;29;2;34;0
WireConnection;29;1;27;1
WireConnection;38;0;29;0
WireConnection;37;0;28;0
WireConnection;6;0;8;0
WireConnection;6;1;39;0
WireConnection;6;5;10;0
WireConnection;7;0;8;0
WireConnection;7;1;40;0
WireConnection;7;5;11;0
WireConnection;9;0;6;0
WireConnection;9;1;7;0
WireConnection;9;2;12;0
WireConnection;16;0;9;0
WireConnection;15;0;13;0
WireConnection;15;1;14;0
WireConnection;15;2;44;0
WireConnection;20;0;15;0
WireConnection;18;0;19;0
WireConnection;0;0;21;0
WireConnection;0;1;17;0
WireConnection;0;3;42;0
WireConnection;0;4;43;0
ASEEND*/
//CHKSM=603A49B02BFFC92B5DF7DFBC4EF05C8F3F9EA7FA