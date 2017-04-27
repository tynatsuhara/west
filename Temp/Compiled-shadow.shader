// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 88.6KB

// Skipping shader variants that would not be included into build of current scene.

Shader "shadow" {
SubShader { 
 Tags { "QUEUE"="Background" "IGNOREPROJECTOR"="False" "RenderType"="Opaque" }


 // Stats for Vertex shader:
 //       metal : 50 avg math (35..66), 1 branch
 //      opengl : 2 math
 // Stats for Fragment shader:
 //       metal : 3 math
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Background" "IGNOREPROJECTOR"="False" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }
  ZTest Less
  ZWrite Off
  GpuProgramID 49792
Program "vp" {
// Platform glcore had shader errors
//   Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
//   Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
//   Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
//   Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
SubProgram "opengl " {
// Stats: 2 math
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
"#version 120

#ifdef VERTEX
uniform vec4 unity_SHAr;
uniform vec4 unity_SHAg;
uniform vec4 unity_SHAb;
uniform vec4 unity_SHBr;
uniform vec4 unity_SHBg;
uniform vec4 unity_SHBb;
uniform vec4 unity_SHC;

uniform mat4 _Object2World;
uniform mat4 _World2Object;
uniform vec4 unity_WorldTransformParams;
uniform vec4 unity_ColorSpaceLuminance;
attribute vec4 TANGENT;
varying vec4 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec3 xlv_TEXCOORD3;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * gl_Vertex);
  vec3 tmpvar_2;
  tmpvar_2 = (_Object2World * gl_Vertex).xyz;
  vec4 v_3;
  v_3.x = _World2Object[0].x;
  v_3.y = _World2Object[1].x;
  v_3.z = _World2Object[2].x;
  v_3.w = _World2Object[3].x;
  vec4 v_4;
  v_4.x = _World2Object[0].y;
  v_4.y = _World2Object[1].y;
  v_4.z = _World2Object[2].y;
  v_4.w = _World2Object[3].y;
  vec4 v_5;
  v_5.x = _World2Object[0].z;
  v_5.y = _World2Object[1].z;
  v_5.z = _World2Object[2].z;
  v_5.w = _World2Object[3].z;
  vec3 tmpvar_6;
  tmpvar_6 = normalize(((
    (v_3.xyz * gl_Normal.x)
   + 
    (v_4.xyz * gl_Normal.y)
  ) + (v_5.xyz * gl_Normal.z)));
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  vec3 tmpvar_8;
  tmpvar_8 = normalize((tmpvar_7 * TANGENT.xyz));
  vec3 tmpvar_9;
  tmpvar_9 = (((tmpvar_6.yzx * tmpvar_8.zxy) - (tmpvar_6.zxy * tmpvar_8.yzx)) * (TANGENT.w * unity_WorldTransformParams.w));
  vec4 tmpvar_10;
  tmpvar_10.x = tmpvar_8.x;
  tmpvar_10.y = tmpvar_9.x;
  tmpvar_10.z = tmpvar_6.x;
  tmpvar_10.w = tmpvar_2.x;
  vec4 tmpvar_11;
  tmpvar_11.x = tmpvar_8.y;
  tmpvar_11.y = tmpvar_9.y;
  tmpvar_11.z = tmpvar_6.y;
  tmpvar_11.w = tmpvar_2.y;
  vec4 tmpvar_12;
  tmpvar_12.x = tmpvar_8.z;
  tmpvar_12.y = tmpvar_9.z;
  tmpvar_12.z = tmpvar_6.z;
  tmpvar_12.w = tmpvar_2.z;
  vec4 tmpvar_13;
  tmpvar_13.w = 1.0;
  tmpvar_13.xyz = tmpvar_6;
  vec3 res_14;
  vec3 x_15;
  x_15.x = dot (unity_SHAr, tmpvar_13);
  x_15.y = dot (unity_SHAg, tmpvar_13);
  x_15.z = dot (unity_SHAb, tmpvar_13);
  vec3 x1_16;
  vec4 tmpvar_17;
  tmpvar_17 = (tmpvar_6.xyzz * tmpvar_6.yzzx);
  x1_16.x = dot (unity_SHBr, tmpvar_17);
  x1_16.y = dot (unity_SHBg, tmpvar_17);
  x1_16.z = dot (unity_SHBb, tmpvar_17);
  res_14 = (x_15 + (x1_16 + (unity_SHC.xyz * 
    ((tmpvar_6.x * tmpvar_6.x) - (tmpvar_6.y * tmpvar_6.y))
  )));
  if ((unity_ColorSpaceLuminance.w == 0.0)) {
    res_14 = max (((1.055 * 
      pow (max (res_14, vec3(0.0, 0.0, 0.0)), vec3(0.4166667, 0.4166667, 0.4166667))
    ) - 0.055), vec3(0.0, 0.0, 0.0));
  };
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = tmpvar_10;
  xlv_TEXCOORD1 = tmpvar_11;
  xlv_TEXCOORD2 = tmpvar_12;
  xlv_TEXCOORD3 = res_14;
}


#endif
#ifdef FRAGMENT
void main ()
{
  vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 1.0;
  gl_FragData[0] = c_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 35 math, 1 branches
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
Bind "tangent" ATTR0
Bind "vertex" ATTR1
Bind "normal" ATTR2
ConstBuffer "$Globals" 280
Matrix 64 [glstate_matrix_mvp]
Matrix 128 [_Object2World]
Matrix 192 [_World2Object]
VectorHalf 0 [unity_SHAr] 4
VectorHalf 8 [unity_SHAg] 4
VectorHalf 16 [unity_SHAb] 4
VectorHalf 24 [unity_SHBr] 4
VectorHalf 32 [unity_SHBg] 4
VectorHalf 40 [unity_SHBb] 4
VectorHalf 48 [unity_SHC] 4
Vector 256 [unity_WorldTransformParams]
VectorHalf 272 [unity_ColorSpaceLuminance] 4
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesTANGENT [[attribute(0)]];
  float4 _glesVertex [[attribute(1)]];
  float3 _glesNormal [[attribute(2)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  float4 xlv_TEXCOORD0;
  float4 xlv_TEXCOORD1;
  float4 xlv_TEXCOORD2;
  half3 xlv_TEXCOORD3;
};
struct xlatMtlShaderUniform {
  half4 unity_SHAr;
  half4 unity_SHAg;
  half4 unity_SHAb;
  half4 unity_SHBr;
  half4 unity_SHBg;
  half4 unity_SHBb;
  half4 unity_SHC;
  float4x4 glstate_matrix_mvp;
  float4x4 _Object2World;
  float4x4 _World2Object;
  float4 unity_WorldTransformParams;
  half4 unity_ColorSpaceLuminance;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  float3 shlight_1;
  half tangentSign_2;
  half3 worldTangent_3;
  half3 worldNormal_4;
  float4 tmpvar_5;
  half3 tmpvar_6;
  tmpvar_5 = (_mtl_u.glstate_matrix_mvp * _mtl_i._glesVertex);
  float3 tmpvar_7;
  tmpvar_7 = (_mtl_u._Object2World * _mtl_i._glesVertex).xyz;
  float4 v_8;
  v_8.x = _mtl_u._World2Object[0].x;
  v_8.y = _mtl_u._World2Object[1].x;
  v_8.z = _mtl_u._World2Object[2].x;
  v_8.w = _mtl_u._World2Object[3].x;
  float4 v_9;
  v_9.x = _mtl_u._World2Object[0].y;
  v_9.y = _mtl_u._World2Object[1].y;
  v_9.z = _mtl_u._World2Object[2].y;
  v_9.w = _mtl_u._World2Object[3].y;
  float4 v_10;
  v_10.x = _mtl_u._World2Object[0].z;
  v_10.y = _mtl_u._World2Object[1].z;
  v_10.z = _mtl_u._World2Object[2].z;
  v_10.w = _mtl_u._World2Object[3].z;
  float3 tmpvar_11;
  tmpvar_11 = normalize(((
    (v_8.xyz * _mtl_i._glesNormal.x)
   + 
    (v_9.xyz * _mtl_i._glesNormal.y)
  ) + (v_10.xyz * _mtl_i._glesNormal.z)));
  worldNormal_4 = half3(tmpvar_11);
  float3x3 tmpvar_12;
  tmpvar_12[0] = _mtl_u._Object2World[0].xyz;
  tmpvar_12[1] = _mtl_u._Object2World[1].xyz;
  tmpvar_12[2] = _mtl_u._Object2World[2].xyz;
  float3 tmpvar_13;
  tmpvar_13 = normalize((tmpvar_12 * _mtl_i._glesTANGENT.xyz));
  worldTangent_3 = half3(tmpvar_13);
  float tmpvar_14;
  tmpvar_14 = (_mtl_i._glesTANGENT.w * _mtl_u.unity_WorldTransformParams.w);
  tangentSign_2 = half(tmpvar_14);
  half3 tmpvar_15;
  tmpvar_15 = (((worldNormal_4.yzx * worldTangent_3.zxy) - (worldNormal_4.zxy * worldTangent_3.yzx)) * tangentSign_2);
  float4 tmpvar_16;
  tmpvar_16.x = float(worldTangent_3.x);
  tmpvar_16.y = float(tmpvar_15.x);
  tmpvar_16.z = float(worldNormal_4.x);
  tmpvar_16.w = tmpvar_7.x;
  float4 tmpvar_17;
  tmpvar_17.x = float(worldTangent_3.y);
  tmpvar_17.y = float(tmpvar_15.y);
  tmpvar_17.z = float(worldNormal_4.y);
  tmpvar_17.w = tmpvar_7.y;
  float4 tmpvar_18;
  tmpvar_18.x = float(worldTangent_3.z);
  tmpvar_18.y = float(tmpvar_15.z);
  tmpvar_18.z = float(worldNormal_4.z);
  tmpvar_18.w = tmpvar_7.z;
  half4 tmpvar_19;
  tmpvar_19.w = half(1.0);
  tmpvar_19.xyz = worldNormal_4;
  half4 normal_20;
  normal_20 = tmpvar_19;
  half3 res_21;
  half3 x_22;
  x_22.x = dot (_mtl_u.unity_SHAr, normal_20);
  x_22.y = dot (_mtl_u.unity_SHAg, normal_20);
  x_22.z = dot (_mtl_u.unity_SHAb, normal_20);
  half3 x1_23;
  half4 tmpvar_24;
  tmpvar_24 = (normal_20.xyzz * normal_20.yzzx);
  x1_23.x = dot (_mtl_u.unity_SHBr, tmpvar_24);
  x1_23.y = dot (_mtl_u.unity_SHBg, tmpvar_24);
  x1_23.z = dot (_mtl_u.unity_SHBb, tmpvar_24);
  res_21 = (x_22 + (x1_23 + (_mtl_u.unity_SHC.xyz * 
    ((normal_20.x * normal_20.x) - (normal_20.y * normal_20.y))
  )));
  bool tmpvar_25;
  tmpvar_25 = (_mtl_u.unity_ColorSpaceLuminance.w == (half)0.0);
  if (tmpvar_25) {
    res_21 = max ((((half)1.055 * 
      pow (max (res_21, (half3)float3(0.0, 0.0, 0.0)), (half3)float3(0.4166667, 0.4166667, 0.4166667))
    ) - (half)0.055), (half3)float3(0.0, 0.0, 0.0));
  };
  shlight_1 = float3(res_21);
  tmpvar_6 = half3(shlight_1);
  _mtl_o.gl_Position = tmpvar_5;
  _mtl_o.xlv_TEXCOORD0 = tmpvar_16;
  _mtl_o.xlv_TEXCOORD1 = tmpvar_17;
  _mtl_o.xlv_TEXCOORD2 = tmpvar_18;
  _mtl_o.xlv_TEXCOORD3 = tmpvar_6;
  return _mtl_o;
}

"
}
SubProgram "opengl " {
// Stats: 2 math
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
"#version 120

#ifdef VERTEX
uniform vec4 _ProjectionParams;
uniform vec4 unity_SHAr;
uniform vec4 unity_SHAg;
uniform vec4 unity_SHAb;
uniform vec4 unity_SHBr;
uniform vec4 unity_SHBg;
uniform vec4 unity_SHBb;
uniform vec4 unity_SHC;

uniform mat4 _Object2World;
uniform mat4 _World2Object;
uniform vec4 unity_WorldTransformParams;
uniform vec4 unity_ColorSpaceLuminance;
attribute vec4 TANGENT;
varying vec4 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec3 xlv_TEXCOORD3;
varying vec4 xlv_TEXCOORD4;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * gl_Vertex);
  vec3 tmpvar_2;
  tmpvar_2 = (_Object2World * gl_Vertex).xyz;
  vec4 v_3;
  v_3.x = _World2Object[0].x;
  v_3.y = _World2Object[1].x;
  v_3.z = _World2Object[2].x;
  v_3.w = _World2Object[3].x;
  vec4 v_4;
  v_4.x = _World2Object[0].y;
  v_4.y = _World2Object[1].y;
  v_4.z = _World2Object[2].y;
  v_4.w = _World2Object[3].y;
  vec4 v_5;
  v_5.x = _World2Object[0].z;
  v_5.y = _World2Object[1].z;
  v_5.z = _World2Object[2].z;
  v_5.w = _World2Object[3].z;
  vec3 tmpvar_6;
  tmpvar_6 = normalize(((
    (v_3.xyz * gl_Normal.x)
   + 
    (v_4.xyz * gl_Normal.y)
  ) + (v_5.xyz * gl_Normal.z)));
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  vec3 tmpvar_8;
  tmpvar_8 = normalize((tmpvar_7 * TANGENT.xyz));
  vec3 tmpvar_9;
  tmpvar_9 = (((tmpvar_6.yzx * tmpvar_8.zxy) - (tmpvar_6.zxy * tmpvar_8.yzx)) * (TANGENT.w * unity_WorldTransformParams.w));
  vec4 tmpvar_10;
  tmpvar_10.x = tmpvar_8.x;
  tmpvar_10.y = tmpvar_9.x;
  tmpvar_10.z = tmpvar_6.x;
  tmpvar_10.w = tmpvar_2.x;
  vec4 tmpvar_11;
  tmpvar_11.x = tmpvar_8.y;
  tmpvar_11.y = tmpvar_9.y;
  tmpvar_11.z = tmpvar_6.y;
  tmpvar_11.w = tmpvar_2.y;
  vec4 tmpvar_12;
  tmpvar_12.x = tmpvar_8.z;
  tmpvar_12.y = tmpvar_9.z;
  tmpvar_12.z = tmpvar_6.z;
  tmpvar_12.w = tmpvar_2.z;
  vec4 tmpvar_13;
  tmpvar_13.w = 1.0;
  tmpvar_13.xyz = tmpvar_6;
  vec3 res_14;
  vec3 x_15;
  x_15.x = dot (unity_SHAr, tmpvar_13);
  x_15.y = dot (unity_SHAg, tmpvar_13);
  x_15.z = dot (unity_SHAb, tmpvar_13);
  vec3 x1_16;
  vec4 tmpvar_17;
  tmpvar_17 = (tmpvar_6.xyzz * tmpvar_6.yzzx);
  x1_16.x = dot (unity_SHBr, tmpvar_17);
  x1_16.y = dot (unity_SHBg, tmpvar_17);
  x1_16.z = dot (unity_SHBb, tmpvar_17);
  res_14 = (x_15 + (x1_16 + (unity_SHC.xyz * 
    ((tmpvar_6.x * tmpvar_6.x) - (tmpvar_6.y * tmpvar_6.y))
  )));
  if ((unity_ColorSpaceLuminance.w == 0.0)) {
    res_14 = max (((1.055 * 
      pow (max (res_14, vec3(0.0, 0.0, 0.0)), vec3(0.4166667, 0.4166667, 0.4166667))
    ) - 0.055), vec3(0.0, 0.0, 0.0));
  };
  vec4 o_18;
  vec4 tmpvar_19;
  tmpvar_19 = (tmpvar_1 * 0.5);
  vec2 tmpvar_20;
  tmpvar_20.x = tmpvar_19.x;
  tmpvar_20.y = (tmpvar_19.y * _ProjectionParams.x);
  o_18.xy = (tmpvar_20 + tmpvar_19.w);
  o_18.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = tmpvar_10;
  xlv_TEXCOORD1 = tmpvar_11;
  xlv_TEXCOORD2 = tmpvar_12;
  xlv_TEXCOORD3 = res_14;
  xlv_TEXCOORD4 = o_18;
}


#endif
#ifdef FRAGMENT
void main ()
{
  vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 1.0;
  gl_FragData[0] = c_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 38 math, 1 branches
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
Bind "tangent" ATTR0
Bind "vertex" ATTR1
Bind "normal" ATTR2
ConstBuffer "$Globals" 296
Matrix 80 [glstate_matrix_mvp]
Matrix 144 [_Object2World]
Matrix 208 [_World2Object]
Vector 0 [_ProjectionParams]
VectorHalf 16 [unity_SHAr] 4
VectorHalf 24 [unity_SHAg] 4
VectorHalf 32 [unity_SHAb] 4
VectorHalf 40 [unity_SHBr] 4
VectorHalf 48 [unity_SHBg] 4
VectorHalf 56 [unity_SHBb] 4
VectorHalf 64 [unity_SHC] 4
Vector 272 [unity_WorldTransformParams]
VectorHalf 288 [unity_ColorSpaceLuminance] 4
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesTANGENT [[attribute(0)]];
  float4 _glesVertex [[attribute(1)]];
  float3 _glesNormal [[attribute(2)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  float4 xlv_TEXCOORD0;
  float4 xlv_TEXCOORD1;
  float4 xlv_TEXCOORD2;
  half3 xlv_TEXCOORD3;
  half4 xlv_TEXCOORD4;
};
struct xlatMtlShaderUniform {
  float4 _ProjectionParams;
  half4 unity_SHAr;
  half4 unity_SHAg;
  half4 unity_SHAb;
  half4 unity_SHBr;
  half4 unity_SHBg;
  half4 unity_SHBb;
  half4 unity_SHC;
  float4x4 glstate_matrix_mvp;
  float4x4 _Object2World;
  float4x4 _World2Object;
  float4 unity_WorldTransformParams;
  half4 unity_ColorSpaceLuminance;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  float3 shlight_1;
  half tangentSign_2;
  half3 worldTangent_3;
  half3 worldNormal_4;
  float4 tmpvar_5;
  half3 tmpvar_6;
  half4 tmpvar_7;
  tmpvar_5 = (_mtl_u.glstate_matrix_mvp * _mtl_i._glesVertex);
  float3 tmpvar_8;
  tmpvar_8 = (_mtl_u._Object2World * _mtl_i._glesVertex).xyz;
  float4 v_9;
  v_9.x = _mtl_u._World2Object[0].x;
  v_9.y = _mtl_u._World2Object[1].x;
  v_9.z = _mtl_u._World2Object[2].x;
  v_9.w = _mtl_u._World2Object[3].x;
  float4 v_10;
  v_10.x = _mtl_u._World2Object[0].y;
  v_10.y = _mtl_u._World2Object[1].y;
  v_10.z = _mtl_u._World2Object[2].y;
  v_10.w = _mtl_u._World2Object[3].y;
  float4 v_11;
  v_11.x = _mtl_u._World2Object[0].z;
  v_11.y = _mtl_u._World2Object[1].z;
  v_11.z = _mtl_u._World2Object[2].z;
  v_11.w = _mtl_u._World2Object[3].z;
  float3 tmpvar_12;
  tmpvar_12 = normalize(((
    (v_9.xyz * _mtl_i._glesNormal.x)
   + 
    (v_10.xyz * _mtl_i._glesNormal.y)
  ) + (v_11.xyz * _mtl_i._glesNormal.z)));
  worldNormal_4 = half3(tmpvar_12);
  float3x3 tmpvar_13;
  tmpvar_13[0] = _mtl_u._Object2World[0].xyz;
  tmpvar_13[1] = _mtl_u._Object2World[1].xyz;
  tmpvar_13[2] = _mtl_u._Object2World[2].xyz;
  float3 tmpvar_14;
  tmpvar_14 = normalize((tmpvar_13 * _mtl_i._glesTANGENT.xyz));
  worldTangent_3 = half3(tmpvar_14);
  float tmpvar_15;
  tmpvar_15 = (_mtl_i._glesTANGENT.w * _mtl_u.unity_WorldTransformParams.w);
  tangentSign_2 = half(tmpvar_15);
  half3 tmpvar_16;
  tmpvar_16 = (((worldNormal_4.yzx * worldTangent_3.zxy) - (worldNormal_4.zxy * worldTangent_3.yzx)) * tangentSign_2);
  float4 tmpvar_17;
  tmpvar_17.x = float(worldTangent_3.x);
  tmpvar_17.y = float(tmpvar_16.x);
  tmpvar_17.z = float(worldNormal_4.x);
  tmpvar_17.w = tmpvar_8.x;
  float4 tmpvar_18;
  tmpvar_18.x = float(worldTangent_3.y);
  tmpvar_18.y = float(tmpvar_16.y);
  tmpvar_18.z = float(worldNormal_4.y);
  tmpvar_18.w = tmpvar_8.y;
  float4 tmpvar_19;
  tmpvar_19.x = float(worldTangent_3.z);
  tmpvar_19.y = float(tmpvar_16.z);
  tmpvar_19.z = float(worldNormal_4.z);
  tmpvar_19.w = tmpvar_8.z;
  half4 tmpvar_20;
  tmpvar_20.w = half(1.0);
  tmpvar_20.xyz = worldNormal_4;
  half4 normal_21;
  normal_21 = tmpvar_20;
  half3 res_22;
  half3 x_23;
  x_23.x = dot (_mtl_u.unity_SHAr, normal_21);
  x_23.y = dot (_mtl_u.unity_SHAg, normal_21);
  x_23.z = dot (_mtl_u.unity_SHAb, normal_21);
  half3 x1_24;
  half4 tmpvar_25;
  tmpvar_25 = (normal_21.xyzz * normal_21.yzzx);
  x1_24.x = dot (_mtl_u.unity_SHBr, tmpvar_25);
  x1_24.y = dot (_mtl_u.unity_SHBg, tmpvar_25);
  x1_24.z = dot (_mtl_u.unity_SHBb, tmpvar_25);
  res_22 = (x_23 + (x1_24 + (_mtl_u.unity_SHC.xyz * 
    ((normal_21.x * normal_21.x) - (normal_21.y * normal_21.y))
  )));
  bool tmpvar_26;
  tmpvar_26 = (_mtl_u.unity_ColorSpaceLuminance.w == (half)0.0);
  if (tmpvar_26) {
    res_22 = max ((((half)1.055 * 
      pow (max (res_22, (half3)float3(0.0, 0.0, 0.0)), (half3)float3(0.4166667, 0.4166667, 0.4166667))
    ) - (half)0.055), (half3)float3(0.0, 0.0, 0.0));
  };
  shlight_1 = float3(res_22);
  tmpvar_6 = half3(shlight_1);
  float4 o_27;
  float4 tmpvar_28;
  tmpvar_28 = (tmpvar_5 * 0.5);
  float2 tmpvar_29;
  tmpvar_29.x = tmpvar_28.x;
  tmpvar_29.y = (tmpvar_28.y * _mtl_u._ProjectionParams.x);
  o_27.xy = (tmpvar_29 + tmpvar_28.w);
  o_27.zw = tmpvar_5.zw;
  tmpvar_7 = half4(o_27);
  _mtl_o.gl_Position = tmpvar_5;
  _mtl_o.xlv_TEXCOORD0 = tmpvar_17;
  _mtl_o.xlv_TEXCOORD1 = tmpvar_18;
  _mtl_o.xlv_TEXCOORD2 = tmpvar_19;
  _mtl_o.xlv_TEXCOORD3 = tmpvar_6;
  _mtl_o.xlv_TEXCOORD4 = tmpvar_7;
  return _mtl_o;
}

"
}
SubProgram "opengl " {
// Stats: 2 math
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"#version 120

#ifdef VERTEX
uniform vec4 unity_4LightPosX0;
uniform vec4 unity_4LightPosY0;
uniform vec4 unity_4LightPosZ0;
uniform vec4 unity_4LightAtten0;
uniform vec4 unity_LightColor[8];
uniform vec4 unity_SHAr;
uniform vec4 unity_SHAg;
uniform vec4 unity_SHAb;
uniform vec4 unity_SHBr;
uniform vec4 unity_SHBg;
uniform vec4 unity_SHBb;
uniform vec4 unity_SHC;

uniform mat4 _Object2World;
uniform mat4 _World2Object;
uniform vec4 unity_WorldTransformParams;
uniform vec4 unity_ColorSpaceLuminance;
attribute vec4 TANGENT;
varying vec4 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec3 xlv_TEXCOORD3;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * gl_Vertex);
  vec3 tmpvar_2;
  tmpvar_2 = (_Object2World * gl_Vertex).xyz;
  vec4 v_3;
  v_3.x = _World2Object[0].x;
  v_3.y = _World2Object[1].x;
  v_3.z = _World2Object[2].x;
  v_3.w = _World2Object[3].x;
  vec4 v_4;
  v_4.x = _World2Object[0].y;
  v_4.y = _World2Object[1].y;
  v_4.z = _World2Object[2].y;
  v_4.w = _World2Object[3].y;
  vec4 v_5;
  v_5.x = _World2Object[0].z;
  v_5.y = _World2Object[1].z;
  v_5.z = _World2Object[2].z;
  v_5.w = _World2Object[3].z;
  vec3 tmpvar_6;
  tmpvar_6 = normalize(((
    (v_3.xyz * gl_Normal.x)
   + 
    (v_4.xyz * gl_Normal.y)
  ) + (v_5.xyz * gl_Normal.z)));
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  vec3 tmpvar_8;
  tmpvar_8 = normalize((tmpvar_7 * TANGENT.xyz));
  vec3 tmpvar_9;
  tmpvar_9 = (((tmpvar_6.yzx * tmpvar_8.zxy) - (tmpvar_6.zxy * tmpvar_8.yzx)) * (TANGENT.w * unity_WorldTransformParams.w));
  vec4 tmpvar_10;
  tmpvar_10.x = tmpvar_8.x;
  tmpvar_10.y = tmpvar_9.x;
  tmpvar_10.z = tmpvar_6.x;
  tmpvar_10.w = tmpvar_2.x;
  vec4 tmpvar_11;
  tmpvar_11.x = tmpvar_8.y;
  tmpvar_11.y = tmpvar_9.y;
  tmpvar_11.z = tmpvar_6.y;
  tmpvar_11.w = tmpvar_2.y;
  vec4 tmpvar_12;
  tmpvar_12.x = tmpvar_8.z;
  tmpvar_12.y = tmpvar_9.z;
  tmpvar_12.z = tmpvar_6.z;
  tmpvar_12.w = tmpvar_2.z;
  vec4 tmpvar_13;
  tmpvar_13.w = 1.0;
  tmpvar_13.xyz = tmpvar_6;
  vec3 res_14;
  vec3 x_15;
  x_15.x = dot (unity_SHAr, tmpvar_13);
  x_15.y = dot (unity_SHAg, tmpvar_13);
  x_15.z = dot (unity_SHAb, tmpvar_13);
  vec3 x1_16;
  vec4 tmpvar_17;
  tmpvar_17 = (tmpvar_6.xyzz * tmpvar_6.yzzx);
  x1_16.x = dot (unity_SHBr, tmpvar_17);
  x1_16.y = dot (unity_SHBg, tmpvar_17);
  x1_16.z = dot (unity_SHBb, tmpvar_17);
  res_14 = (x_15 + (x1_16 + (unity_SHC.xyz * 
    ((tmpvar_6.x * tmpvar_6.x) - (tmpvar_6.y * tmpvar_6.y))
  )));
  if ((unity_ColorSpaceLuminance.w == 0.0)) {
    res_14 = max (((1.055 * 
      pow (max (res_14, vec3(0.0, 0.0, 0.0)), vec3(0.4166667, 0.4166667, 0.4166667))
    ) - 0.055), vec3(0.0, 0.0, 0.0));
  };
  vec3 col_18;
  vec4 ndotl_19;
  vec4 lengthSq_20;
  vec4 tmpvar_21;
  tmpvar_21 = (unity_4LightPosX0 - tmpvar_2.x);
  vec4 tmpvar_22;
  tmpvar_22 = (unity_4LightPosY0 - tmpvar_2.y);
  vec4 tmpvar_23;
  tmpvar_23 = (unity_4LightPosZ0 - tmpvar_2.z);
  lengthSq_20 = (tmpvar_21 * tmpvar_21);
  lengthSq_20 = (lengthSq_20 + (tmpvar_22 * tmpvar_22));
  lengthSq_20 = (lengthSq_20 + (tmpvar_23 * tmpvar_23));
  ndotl_19 = (tmpvar_21 * tmpvar_6.x);
  ndotl_19 = (ndotl_19 + (tmpvar_22 * tmpvar_6.y));
  ndotl_19 = (ndotl_19 + (tmpvar_23 * tmpvar_6.z));
  vec4 tmpvar_24;
  tmpvar_24 = max (vec4(0.0, 0.0, 0.0, 0.0), (ndotl_19 * inversesqrt(lengthSq_20)));
  ndotl_19 = tmpvar_24;
  vec4 tmpvar_25;
  tmpvar_25 = (tmpvar_24 * (1.0/((1.0 + 
    (lengthSq_20 * unity_4LightAtten0)
  ))));
  col_18 = (unity_LightColor[0].xyz * tmpvar_25.x);
  col_18 = (col_18 + (unity_LightColor[1].xyz * tmpvar_25.y));
  col_18 = (col_18 + (unity_LightColor[2].xyz * tmpvar_25.z));
  col_18 = (col_18 + (unity_LightColor[3].xyz * tmpvar_25.w));
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = tmpvar_10;
  xlv_TEXCOORD1 = tmpvar_11;
  xlv_TEXCOORD2 = tmpvar_12;
  xlv_TEXCOORD3 = (res_14 + col_18);
}


#endif
#ifdef FRAGMENT
void main ()
{
  vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 1.0;
  gl_FragData[0] = c_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 63 math, 1 branches
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "tangent" ATTR0
Bind "vertex" ATTR1
Bind "normal" ATTR2
ConstBuffer "$Globals" 392
Matrix 176 [glstate_matrix_mvp]
Matrix 240 [_Object2World]
Matrix 304 [_World2Object]
Vector 0 [unity_4LightPosX0]
Vector 16 [unity_4LightPosY0]
Vector 32 [unity_4LightPosZ0]
VectorHalf 48 [unity_4LightAtten0] 4
VectorHalf 56 [unity_LightColor0] 4
VectorHalf 64 [unity_LightColor1] 4
VectorHalf 72 [unity_LightColor2] 4
VectorHalf 80 [unity_LightColor3] 4
VectorHalf 88 [unity_LightColor4] 4
VectorHalf 96 [unity_LightColor5] 4
VectorHalf 104 [unity_LightColor6] 4
VectorHalf 112 [unity_LightColor7] 4
VectorHalf 120 [unity_SHAr] 4
VectorHalf 128 [unity_SHAg] 4
VectorHalf 136 [unity_SHAb] 4
VectorHalf 144 [unity_SHBr] 4
VectorHalf 152 [unity_SHBg] 4
VectorHalf 160 [unity_SHBb] 4
VectorHalf 168 [unity_SHC] 4
Vector 368 [unity_WorldTransformParams]
VectorHalf 384 [unity_ColorSpaceLuminance] 4
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesTANGENT [[attribute(0)]];
  float4 _glesVertex [[attribute(1)]];
  float3 _glesNormal [[attribute(2)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  float4 xlv_TEXCOORD0;
  float4 xlv_TEXCOORD1;
  float4 xlv_TEXCOORD2;
  half3 xlv_TEXCOORD3;
};
struct xlatMtlShaderUniform {
  float4 unity_4LightPosX0;
  float4 unity_4LightPosY0;
  float4 unity_4LightPosZ0;
  half4 unity_4LightAtten0;
  half4 unity_LightColor[8];
  half4 unity_SHAr;
  half4 unity_SHAg;
  half4 unity_SHAb;
  half4 unity_SHBr;
  half4 unity_SHBg;
  half4 unity_SHBb;
  half4 unity_SHC;
  float4x4 glstate_matrix_mvp;
  float4x4 _Object2World;
  float4x4 _World2Object;
  float4 unity_WorldTransformParams;
  half4 unity_ColorSpaceLuminance;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  float3 shlight_1;
  half tangentSign_2;
  half3 worldTangent_3;
  half3 worldNormal_4;
  float4 tmpvar_5;
  half3 tmpvar_6;
  tmpvar_5 = (_mtl_u.glstate_matrix_mvp * _mtl_i._glesVertex);
  float3 tmpvar_7;
  tmpvar_7 = (_mtl_u._Object2World * _mtl_i._glesVertex).xyz;
  float4 v_8;
  v_8.x = _mtl_u._World2Object[0].x;
  v_8.y = _mtl_u._World2Object[1].x;
  v_8.z = _mtl_u._World2Object[2].x;
  v_8.w = _mtl_u._World2Object[3].x;
  float4 v_9;
  v_9.x = _mtl_u._World2Object[0].y;
  v_9.y = _mtl_u._World2Object[1].y;
  v_9.z = _mtl_u._World2Object[2].y;
  v_9.w = _mtl_u._World2Object[3].y;
  float4 v_10;
  v_10.x = _mtl_u._World2Object[0].z;
  v_10.y = _mtl_u._World2Object[1].z;
  v_10.z = _mtl_u._World2Object[2].z;
  v_10.w = _mtl_u._World2Object[3].z;
  float3 tmpvar_11;
  tmpvar_11 = normalize(((
    (v_8.xyz * _mtl_i._glesNormal.x)
   + 
    (v_9.xyz * _mtl_i._glesNormal.y)
  ) + (v_10.xyz * _mtl_i._glesNormal.z)));
  worldNormal_4 = half3(tmpvar_11);
  float3x3 tmpvar_12;
  tmpvar_12[0] = _mtl_u._Object2World[0].xyz;
  tmpvar_12[1] = _mtl_u._Object2World[1].xyz;
  tmpvar_12[2] = _mtl_u._Object2World[2].xyz;
  float3 tmpvar_13;
  tmpvar_13 = normalize((tmpvar_12 * _mtl_i._glesTANGENT.xyz));
  worldTangent_3 = half3(tmpvar_13);
  float tmpvar_14;
  tmpvar_14 = (_mtl_i._glesTANGENT.w * _mtl_u.unity_WorldTransformParams.w);
  tangentSign_2 = half(tmpvar_14);
  half3 tmpvar_15;
  tmpvar_15 = (((worldNormal_4.yzx * worldTangent_3.zxy) - (worldNormal_4.zxy * worldTangent_3.yzx)) * tangentSign_2);
  float4 tmpvar_16;
  tmpvar_16.x = float(worldTangent_3.x);
  tmpvar_16.y = float(tmpvar_15.x);
  tmpvar_16.z = float(worldNormal_4.x);
  tmpvar_16.w = tmpvar_7.x;
  float4 tmpvar_17;
  tmpvar_17.x = float(worldTangent_3.y);
  tmpvar_17.y = float(tmpvar_15.y);
  tmpvar_17.z = float(worldNormal_4.y);
  tmpvar_17.w = tmpvar_7.y;
  float4 tmpvar_18;
  tmpvar_18.x = float(worldTangent_3.z);
  tmpvar_18.y = float(tmpvar_15.z);
  tmpvar_18.z = float(worldNormal_4.z);
  tmpvar_18.w = tmpvar_7.z;
  half4 tmpvar_19;
  tmpvar_19.w = half(1.0);
  tmpvar_19.xyz = worldNormal_4;
  half4 normal_20;
  normal_20 = tmpvar_19;
  half3 res_21;
  half3 x_22;
  x_22.x = dot (_mtl_u.unity_SHAr, normal_20);
  x_22.y = dot (_mtl_u.unity_SHAg, normal_20);
  x_22.z = dot (_mtl_u.unity_SHAb, normal_20);
  half3 x1_23;
  half4 tmpvar_24;
  tmpvar_24 = (normal_20.xyzz * normal_20.yzzx);
  x1_23.x = dot (_mtl_u.unity_SHBr, tmpvar_24);
  x1_23.y = dot (_mtl_u.unity_SHBg, tmpvar_24);
  x1_23.z = dot (_mtl_u.unity_SHBb, tmpvar_24);
  res_21 = (x_22 + (x1_23 + (_mtl_u.unity_SHC.xyz * 
    ((normal_20.x * normal_20.x) - (normal_20.y * normal_20.y))
  )));
  bool tmpvar_25;
  tmpvar_25 = (_mtl_u.unity_ColorSpaceLuminance.w == (half)0.0);
  if (tmpvar_25) {
    res_21 = max ((((half)1.055 * 
      pow (max (res_21, (half3)float3(0.0, 0.0, 0.0)), (half3)float3(0.4166667, 0.4166667, 0.4166667))
    ) - (half)0.055), (half3)float3(0.0, 0.0, 0.0));
  };
  shlight_1 = float3(res_21);
  tmpvar_6 = half3(shlight_1);
  float3 lightColor0_26;
  lightColor0_26 = float3(_mtl_u.unity_LightColor[0].xyz);
  float3 lightColor1_27;
  lightColor1_27 = float3(_mtl_u.unity_LightColor[1].xyz);
  float3 lightColor2_28;
  lightColor2_28 = float3(_mtl_u.unity_LightColor[2].xyz);
  float3 lightColor3_29;
  lightColor3_29 = float3(_mtl_u.unity_LightColor[3].xyz);
  float4 lightAttenSq_30;
  lightAttenSq_30 = float4(_mtl_u.unity_4LightAtten0);
  float3 normal_31;
  normal_31 = float3(worldNormal_4);
  float3 col_32;
  float4 ndotl_33;
  float4 lengthSq_34;
  float4 tmpvar_35;
  tmpvar_35 = (_mtl_u.unity_4LightPosX0 - tmpvar_7.x);
  float4 tmpvar_36;
  tmpvar_36 = (_mtl_u.unity_4LightPosY0 - tmpvar_7.y);
  float4 tmpvar_37;
  tmpvar_37 = (_mtl_u.unity_4LightPosZ0 - tmpvar_7.z);
  lengthSq_34 = (tmpvar_35 * tmpvar_35);
  lengthSq_34 = (lengthSq_34 + (tmpvar_36 * tmpvar_36));
  lengthSq_34 = (lengthSq_34 + (tmpvar_37 * tmpvar_37));
  ndotl_33 = (tmpvar_35 * normal_31.x);
  ndotl_33 = (ndotl_33 + (tmpvar_36 * normal_31.y));
  ndotl_33 = (ndotl_33 + (tmpvar_37 * normal_31.z));
  float4 tmpvar_38;
  tmpvar_38 = max (float4(0.0, 0.0, 0.0, 0.0), (ndotl_33 * rsqrt(lengthSq_34)));
  ndotl_33 = tmpvar_38;
  float4 tmpvar_39;
  tmpvar_39 = (tmpvar_38 * (1.0/((1.0 + 
    (lengthSq_34 * lightAttenSq_30)
  ))));
  col_32 = (lightColor0_26 * tmpvar_39.x);
  col_32 = (col_32 + (lightColor1_27 * tmpvar_39.y));
  col_32 = (col_32 + (lightColor2_28 * tmpvar_39.z));
  col_32 = (col_32 + (lightColor3_29 * tmpvar_39.w));
  tmpvar_6 = half3(((float3)tmpvar_6 + col_32));
  _mtl_o.gl_Position = tmpvar_5;
  _mtl_o.xlv_TEXCOORD0 = tmpvar_16;
  _mtl_o.xlv_TEXCOORD1 = tmpvar_17;
  _mtl_o.xlv_TEXCOORD2 = tmpvar_18;
  _mtl_o.xlv_TEXCOORD3 = tmpvar_6;
  return _mtl_o;
}

"
}
SubProgram "opengl " {
// Stats: 2 math
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"#version 120

#ifdef VERTEX
uniform vec4 _ProjectionParams;
uniform vec4 unity_4LightPosX0;
uniform vec4 unity_4LightPosY0;
uniform vec4 unity_4LightPosZ0;
uniform vec4 unity_4LightAtten0;
uniform vec4 unity_LightColor[8];
uniform vec4 unity_SHAr;
uniform vec4 unity_SHAg;
uniform vec4 unity_SHAb;
uniform vec4 unity_SHBr;
uniform vec4 unity_SHBg;
uniform vec4 unity_SHBb;
uniform vec4 unity_SHC;

uniform mat4 _Object2World;
uniform mat4 _World2Object;
uniform vec4 unity_WorldTransformParams;
uniform vec4 unity_ColorSpaceLuminance;
attribute vec4 TANGENT;
varying vec4 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec3 xlv_TEXCOORD3;
varying vec4 xlv_TEXCOORD4;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * gl_Vertex);
  vec3 tmpvar_2;
  tmpvar_2 = (_Object2World * gl_Vertex).xyz;
  vec4 v_3;
  v_3.x = _World2Object[0].x;
  v_3.y = _World2Object[1].x;
  v_3.z = _World2Object[2].x;
  v_3.w = _World2Object[3].x;
  vec4 v_4;
  v_4.x = _World2Object[0].y;
  v_4.y = _World2Object[1].y;
  v_4.z = _World2Object[2].y;
  v_4.w = _World2Object[3].y;
  vec4 v_5;
  v_5.x = _World2Object[0].z;
  v_5.y = _World2Object[1].z;
  v_5.z = _World2Object[2].z;
  v_5.w = _World2Object[3].z;
  vec3 tmpvar_6;
  tmpvar_6 = normalize(((
    (v_3.xyz * gl_Normal.x)
   + 
    (v_4.xyz * gl_Normal.y)
  ) + (v_5.xyz * gl_Normal.z)));
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  vec3 tmpvar_8;
  tmpvar_8 = normalize((tmpvar_7 * TANGENT.xyz));
  vec3 tmpvar_9;
  tmpvar_9 = (((tmpvar_6.yzx * tmpvar_8.zxy) - (tmpvar_6.zxy * tmpvar_8.yzx)) * (TANGENT.w * unity_WorldTransformParams.w));
  vec4 tmpvar_10;
  tmpvar_10.x = tmpvar_8.x;
  tmpvar_10.y = tmpvar_9.x;
  tmpvar_10.z = tmpvar_6.x;
  tmpvar_10.w = tmpvar_2.x;
  vec4 tmpvar_11;
  tmpvar_11.x = tmpvar_8.y;
  tmpvar_11.y = tmpvar_9.y;
  tmpvar_11.z = tmpvar_6.y;
  tmpvar_11.w = tmpvar_2.y;
  vec4 tmpvar_12;
  tmpvar_12.x = tmpvar_8.z;
  tmpvar_12.y = tmpvar_9.z;
  tmpvar_12.z = tmpvar_6.z;
  tmpvar_12.w = tmpvar_2.z;
  vec4 tmpvar_13;
  tmpvar_13.w = 1.0;
  tmpvar_13.xyz = tmpvar_6;
  vec3 res_14;
  vec3 x_15;
  x_15.x = dot (unity_SHAr, tmpvar_13);
  x_15.y = dot (unity_SHAg, tmpvar_13);
  x_15.z = dot (unity_SHAb, tmpvar_13);
  vec3 x1_16;
  vec4 tmpvar_17;
  tmpvar_17 = (tmpvar_6.xyzz * tmpvar_6.yzzx);
  x1_16.x = dot (unity_SHBr, tmpvar_17);
  x1_16.y = dot (unity_SHBg, tmpvar_17);
  x1_16.z = dot (unity_SHBb, tmpvar_17);
  res_14 = (x_15 + (x1_16 + (unity_SHC.xyz * 
    ((tmpvar_6.x * tmpvar_6.x) - (tmpvar_6.y * tmpvar_6.y))
  )));
  if ((unity_ColorSpaceLuminance.w == 0.0)) {
    res_14 = max (((1.055 * 
      pow (max (res_14, vec3(0.0, 0.0, 0.0)), vec3(0.4166667, 0.4166667, 0.4166667))
    ) - 0.055), vec3(0.0, 0.0, 0.0));
  };
  vec3 col_18;
  vec4 ndotl_19;
  vec4 lengthSq_20;
  vec4 tmpvar_21;
  tmpvar_21 = (unity_4LightPosX0 - tmpvar_2.x);
  vec4 tmpvar_22;
  tmpvar_22 = (unity_4LightPosY0 - tmpvar_2.y);
  vec4 tmpvar_23;
  tmpvar_23 = (unity_4LightPosZ0 - tmpvar_2.z);
  lengthSq_20 = (tmpvar_21 * tmpvar_21);
  lengthSq_20 = (lengthSq_20 + (tmpvar_22 * tmpvar_22));
  lengthSq_20 = (lengthSq_20 + (tmpvar_23 * tmpvar_23));
  ndotl_19 = (tmpvar_21 * tmpvar_6.x);
  ndotl_19 = (ndotl_19 + (tmpvar_22 * tmpvar_6.y));
  ndotl_19 = (ndotl_19 + (tmpvar_23 * tmpvar_6.z));
  vec4 tmpvar_24;
  tmpvar_24 = max (vec4(0.0, 0.0, 0.0, 0.0), (ndotl_19 * inversesqrt(lengthSq_20)));
  ndotl_19 = tmpvar_24;
  vec4 tmpvar_25;
  tmpvar_25 = (tmpvar_24 * (1.0/((1.0 + 
    (lengthSq_20 * unity_4LightAtten0)
  ))));
  col_18 = (unity_LightColor[0].xyz * tmpvar_25.x);
  col_18 = (col_18 + (unity_LightColor[1].xyz * tmpvar_25.y));
  col_18 = (col_18 + (unity_LightColor[2].xyz * tmpvar_25.z));
  col_18 = (col_18 + (unity_LightColor[3].xyz * tmpvar_25.w));
  vec4 o_26;
  vec4 tmpvar_27;
  tmpvar_27 = (tmpvar_1 * 0.5);
  vec2 tmpvar_28;
  tmpvar_28.x = tmpvar_27.x;
  tmpvar_28.y = (tmpvar_27.y * _ProjectionParams.x);
  o_26.xy = (tmpvar_28 + tmpvar_27.w);
  o_26.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = tmpvar_10;
  xlv_TEXCOORD1 = tmpvar_11;
  xlv_TEXCOORD2 = tmpvar_12;
  xlv_TEXCOORD3 = (res_14 + col_18);
  xlv_TEXCOORD4 = o_26;
}


#endif
#ifdef FRAGMENT
void main ()
{
  vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 1.0;
  gl_FragData[0] = c_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 66 math, 1 branches
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "tangent" ATTR0
Bind "vertex" ATTR1
Bind "normal" ATTR2
ConstBuffer "$Globals" 408
Matrix 192 [glstate_matrix_mvp]
Matrix 256 [_Object2World]
Matrix 320 [_World2Object]
Vector 0 [_ProjectionParams]
Vector 16 [unity_4LightPosX0]
Vector 32 [unity_4LightPosY0]
Vector 48 [unity_4LightPosZ0]
VectorHalf 64 [unity_4LightAtten0] 4
VectorHalf 72 [unity_LightColor0] 4
VectorHalf 80 [unity_LightColor1] 4
VectorHalf 88 [unity_LightColor2] 4
VectorHalf 96 [unity_LightColor3] 4
VectorHalf 104 [unity_LightColor4] 4
VectorHalf 112 [unity_LightColor5] 4
VectorHalf 120 [unity_LightColor6] 4
VectorHalf 128 [unity_LightColor7] 4
VectorHalf 136 [unity_SHAr] 4
VectorHalf 144 [unity_SHAg] 4
VectorHalf 152 [unity_SHAb] 4
VectorHalf 160 [unity_SHBr] 4
VectorHalf 168 [unity_SHBg] 4
VectorHalf 176 [unity_SHBb] 4
VectorHalf 184 [unity_SHC] 4
Vector 384 [unity_WorldTransformParams]
VectorHalf 400 [unity_ColorSpaceLuminance] 4
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesTANGENT [[attribute(0)]];
  float4 _glesVertex [[attribute(1)]];
  float3 _glesNormal [[attribute(2)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  float4 xlv_TEXCOORD0;
  float4 xlv_TEXCOORD1;
  float4 xlv_TEXCOORD2;
  half3 xlv_TEXCOORD3;
  half4 xlv_TEXCOORD4;
};
struct xlatMtlShaderUniform {
  float4 _ProjectionParams;
  float4 unity_4LightPosX0;
  float4 unity_4LightPosY0;
  float4 unity_4LightPosZ0;
  half4 unity_4LightAtten0;
  half4 unity_LightColor[8];
  half4 unity_SHAr;
  half4 unity_SHAg;
  half4 unity_SHAb;
  half4 unity_SHBr;
  half4 unity_SHBg;
  half4 unity_SHBb;
  half4 unity_SHC;
  float4x4 glstate_matrix_mvp;
  float4x4 _Object2World;
  float4x4 _World2Object;
  float4 unity_WorldTransformParams;
  half4 unity_ColorSpaceLuminance;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  float3 shlight_1;
  half tangentSign_2;
  half3 worldTangent_3;
  half3 worldNormal_4;
  float4 tmpvar_5;
  half3 tmpvar_6;
  half4 tmpvar_7;
  tmpvar_5 = (_mtl_u.glstate_matrix_mvp * _mtl_i._glesVertex);
  float3 tmpvar_8;
  tmpvar_8 = (_mtl_u._Object2World * _mtl_i._glesVertex).xyz;
  float4 v_9;
  v_9.x = _mtl_u._World2Object[0].x;
  v_9.y = _mtl_u._World2Object[1].x;
  v_9.z = _mtl_u._World2Object[2].x;
  v_9.w = _mtl_u._World2Object[3].x;
  float4 v_10;
  v_10.x = _mtl_u._World2Object[0].y;
  v_10.y = _mtl_u._World2Object[1].y;
  v_10.z = _mtl_u._World2Object[2].y;
  v_10.w = _mtl_u._World2Object[3].y;
  float4 v_11;
  v_11.x = _mtl_u._World2Object[0].z;
  v_11.y = _mtl_u._World2Object[1].z;
  v_11.z = _mtl_u._World2Object[2].z;
  v_11.w = _mtl_u._World2Object[3].z;
  float3 tmpvar_12;
  tmpvar_12 = normalize(((
    (v_9.xyz * _mtl_i._glesNormal.x)
   + 
    (v_10.xyz * _mtl_i._glesNormal.y)
  ) + (v_11.xyz * _mtl_i._glesNormal.z)));
  worldNormal_4 = half3(tmpvar_12);
  float3x3 tmpvar_13;
  tmpvar_13[0] = _mtl_u._Object2World[0].xyz;
  tmpvar_13[1] = _mtl_u._Object2World[1].xyz;
  tmpvar_13[2] = _mtl_u._Object2World[2].xyz;
  float3 tmpvar_14;
  tmpvar_14 = normalize((tmpvar_13 * _mtl_i._glesTANGENT.xyz));
  worldTangent_3 = half3(tmpvar_14);
  float tmpvar_15;
  tmpvar_15 = (_mtl_i._glesTANGENT.w * _mtl_u.unity_WorldTransformParams.w);
  tangentSign_2 = half(tmpvar_15);
  half3 tmpvar_16;
  tmpvar_16 = (((worldNormal_4.yzx * worldTangent_3.zxy) - (worldNormal_4.zxy * worldTangent_3.yzx)) * tangentSign_2);
  float4 tmpvar_17;
  tmpvar_17.x = float(worldTangent_3.x);
  tmpvar_17.y = float(tmpvar_16.x);
  tmpvar_17.z = float(worldNormal_4.x);
  tmpvar_17.w = tmpvar_8.x;
  float4 tmpvar_18;
  tmpvar_18.x = float(worldTangent_3.y);
  tmpvar_18.y = float(tmpvar_16.y);
  tmpvar_18.z = float(worldNormal_4.y);
  tmpvar_18.w = tmpvar_8.y;
  float4 tmpvar_19;
  tmpvar_19.x = float(worldTangent_3.z);
  tmpvar_19.y = float(tmpvar_16.z);
  tmpvar_19.z = float(worldNormal_4.z);
  tmpvar_19.w = tmpvar_8.z;
  half4 tmpvar_20;
  tmpvar_20.w = half(1.0);
  tmpvar_20.xyz = worldNormal_4;
  half4 normal_21;
  normal_21 = tmpvar_20;
  half3 res_22;
  half3 x_23;
  x_23.x = dot (_mtl_u.unity_SHAr, normal_21);
  x_23.y = dot (_mtl_u.unity_SHAg, normal_21);
  x_23.z = dot (_mtl_u.unity_SHAb, normal_21);
  half3 x1_24;
  half4 tmpvar_25;
  tmpvar_25 = (normal_21.xyzz * normal_21.yzzx);
  x1_24.x = dot (_mtl_u.unity_SHBr, tmpvar_25);
  x1_24.y = dot (_mtl_u.unity_SHBg, tmpvar_25);
  x1_24.z = dot (_mtl_u.unity_SHBb, tmpvar_25);
  res_22 = (x_23 + (x1_24 + (_mtl_u.unity_SHC.xyz * 
    ((normal_21.x * normal_21.x) - (normal_21.y * normal_21.y))
  )));
  bool tmpvar_26;
  tmpvar_26 = (_mtl_u.unity_ColorSpaceLuminance.w == (half)0.0);
  if (tmpvar_26) {
    res_22 = max ((((half)1.055 * 
      pow (max (res_22, (half3)float3(0.0, 0.0, 0.0)), (half3)float3(0.4166667, 0.4166667, 0.4166667))
    ) - (half)0.055), (half3)float3(0.0, 0.0, 0.0));
  };
  shlight_1 = float3(res_22);
  tmpvar_6 = half3(shlight_1);
  float3 lightColor0_27;
  lightColor0_27 = float3(_mtl_u.unity_LightColor[0].xyz);
  float3 lightColor1_28;
  lightColor1_28 = float3(_mtl_u.unity_LightColor[1].xyz);
  float3 lightColor2_29;
  lightColor2_29 = float3(_mtl_u.unity_LightColor[2].xyz);
  float3 lightColor3_30;
  lightColor3_30 = float3(_mtl_u.unity_LightColor[3].xyz);
  float4 lightAttenSq_31;
  lightAttenSq_31 = float4(_mtl_u.unity_4LightAtten0);
  float3 normal_32;
  normal_32 = float3(worldNormal_4);
  float3 col_33;
  float4 ndotl_34;
  float4 lengthSq_35;
  float4 tmpvar_36;
  tmpvar_36 = (_mtl_u.unity_4LightPosX0 - tmpvar_8.x);
  float4 tmpvar_37;
  tmpvar_37 = (_mtl_u.unity_4LightPosY0 - tmpvar_8.y);
  float4 tmpvar_38;
  tmpvar_38 = (_mtl_u.unity_4LightPosZ0 - tmpvar_8.z);
  lengthSq_35 = (tmpvar_36 * tmpvar_36);
  lengthSq_35 = (lengthSq_35 + (tmpvar_37 * tmpvar_37));
  lengthSq_35 = (lengthSq_35 + (tmpvar_38 * tmpvar_38));
  ndotl_34 = (tmpvar_36 * normal_32.x);
  ndotl_34 = (ndotl_34 + (tmpvar_37 * normal_32.y));
  ndotl_34 = (ndotl_34 + (tmpvar_38 * normal_32.z));
  float4 tmpvar_39;
  tmpvar_39 = max (float4(0.0, 0.0, 0.0, 0.0), (ndotl_34 * rsqrt(lengthSq_35)));
  ndotl_34 = tmpvar_39;
  float4 tmpvar_40;
  tmpvar_40 = (tmpvar_39 * (1.0/((1.0 + 
    (lengthSq_35 * lightAttenSq_31)
  ))));
  col_33 = (lightColor0_27 * tmpvar_40.x);
  col_33 = (col_33 + (lightColor1_28 * tmpvar_40.y));
  col_33 = (col_33 + (lightColor2_29 * tmpvar_40.z));
  col_33 = (col_33 + (lightColor3_30 * tmpvar_40.w));
  tmpvar_6 = half3(((float3)tmpvar_6 + col_33));
  float4 o_41;
  float4 tmpvar_42;
  tmpvar_42 = (tmpvar_5 * 0.5);
  float2 tmpvar_43;
  tmpvar_43.x = tmpvar_42.x;
  tmpvar_43.y = (tmpvar_42.y * _mtl_u._ProjectionParams.x);
  o_41.xy = (tmpvar_43 + tmpvar_42.w);
  o_41.zw = tmpvar_5.zw;
  tmpvar_7 = half4(o_41);
  _mtl_o.gl_Position = tmpvar_5;
  _mtl_o.xlv_TEXCOORD0 = tmpvar_17;
  _mtl_o.xlv_TEXCOORD1 = tmpvar_18;
  _mtl_o.xlv_TEXCOORD2 = tmpvar_19;
  _mtl_o.xlv_TEXCOORD3 = tmpvar_6;
  _mtl_o.xlv_TEXCOORD4 = tmpvar_7;
  return _mtl_o;
}

"
}
}
Program "fp" {
// Platform glcore skipped due to earlier errors
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
"// shader disassembly not supported on opengl"
}
SubProgram "metal " {
// Stats: 3 math
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 c_1;
  half4 c_2;
  c_2.xyz = half3(float3(0.0, 0.0, 0.0));
  c_2.w = half(1.0);
  c_1 = c_2;
  c_1.xyz = c_1.xyz;
  c_1.w = half(1.0);
  _mtl_o._glesFragData_0 = c_1;
  return _mtl_o;
}

"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
"// shader disassembly not supported on opengl"
}
SubProgram "metal " {
// Stats: 3 math
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 c_1;
  half4 c_2;
  c_2.xyz = half3(float3(0.0, 0.0, 0.0));
  c_2.w = half(1.0);
  c_1 = c_2;
  c_1.xyz = c_1.xyz;
  c_1.w = half(1.0);
  _mtl_o._glesFragData_0 = c_1;
  return _mtl_o;
}

"
}
}
 }


 // Stats for Vertex shader:
 //       metal : 15 math
 //      opengl : 2 math
 // Stats for Fragment shader:
 //       metal : 2 math
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "QUEUE"="Background" "IGNOREPROJECTOR"="False" "RenderType"="Opaque" }
  ZTest Less
  ZWrite Off
  Blend One One
  GpuProgramID 80358
Program "vp" {
// Platform glcore had shader errors
//   Keywords { "POINT" }
//   Keywords { "DIRECTIONAL" }
//   Keywords { "SPOT" }
//   Keywords { "POINT_COOKIE" }
//   Keywords { "DIRECTIONAL_COOKIE" }
SubProgram "opengl " {
// Stats: 2 math
Keywords { "POINT" }
"#version 120

#ifdef VERTEX

uniform mat4 _Object2World;
uniform mat4 _World2Object;
uniform vec4 unity_WorldTransformParams;
attribute vec4 TANGENT;
varying vec3 xlv_TEXCOORD0;
varying vec3 xlv_TEXCOORD1;
varying vec3 xlv_TEXCOORD2;
varying vec3 xlv_TEXCOORD3;
void main ()
{
  vec4 v_1;
  v_1.x = _World2Object[0].x;
  v_1.y = _World2Object[1].x;
  v_1.z = _World2Object[2].x;
  v_1.w = _World2Object[3].x;
  vec4 v_2;
  v_2.x = _World2Object[0].y;
  v_2.y = _World2Object[1].y;
  v_2.z = _World2Object[2].y;
  v_2.w = _World2Object[3].y;
  vec4 v_3;
  v_3.x = _World2Object[0].z;
  v_3.y = _World2Object[1].z;
  v_3.z = _World2Object[2].z;
  v_3.w = _World2Object[3].z;
  vec3 tmpvar_4;
  tmpvar_4 = normalize(((
    (v_1.xyz * gl_Normal.x)
   + 
    (v_2.xyz * gl_Normal.y)
  ) + (v_3.xyz * gl_Normal.z)));
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  vec3 tmpvar_6;
  tmpvar_6 = normalize((tmpvar_5 * TANGENT.xyz));
  vec3 tmpvar_7;
  tmpvar_7 = (((tmpvar_4.yzx * tmpvar_6.zxy) - (tmpvar_4.zxy * tmpvar_6.yzx)) * (TANGENT.w * unity_WorldTransformParams.w));
  vec3 tmpvar_8;
  tmpvar_8.x = tmpvar_6.x;
  tmpvar_8.y = tmpvar_7.x;
  tmpvar_8.z = tmpvar_4.x;
  vec3 tmpvar_9;
  tmpvar_9.x = tmpvar_6.y;
  tmpvar_9.y = tmpvar_7.y;
  tmpvar_9.z = tmpvar_4.y;
  vec3 tmpvar_10;
  tmpvar_10.x = tmpvar_6.z;
  tmpvar_10.y = tmpvar_7.z;
  tmpvar_10.z = tmpvar_4.z;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_8;
  xlv_TEXCOORD1 = tmpvar_9;
  xlv_TEXCOORD2 = tmpvar_10;
  xlv_TEXCOORD3 = (_Object2World * gl_Vertex).xyz;
}


#endif
#ifdef FRAGMENT
void main ()
{
  vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 1.0;
  gl_FragData[0] = c_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 15 math
Keywords { "POINT" }
Bind "tangent" ATTR0
Bind "vertex" ATTR1
Bind "normal" ATTR2
ConstBuffer "$Globals" 208
Matrix 0 [glstate_matrix_mvp]
Matrix 64 [_Object2World]
Matrix 128 [_World2Object]
Vector 192 [unity_WorldTransformParams]
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesTANGENT [[attribute(0)]];
  float4 _glesVertex [[attribute(1)]];
  float3 _glesNormal [[attribute(2)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  half3 xlv_TEXCOORD0;
  half3 xlv_TEXCOORD1;
  half3 xlv_TEXCOORD2;
  float3 xlv_TEXCOORD3;
};
struct xlatMtlShaderUniform {
  float4x4 glstate_matrix_mvp;
  float4x4 _Object2World;
  float4x4 _World2Object;
  float4 unity_WorldTransformParams;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half tangentSign_1;
  half3 worldTangent_2;
  half3 worldNormal_3;
  float4 v_4;
  v_4.x = _mtl_u._World2Object[0].x;
  v_4.y = _mtl_u._World2Object[1].x;
  v_4.z = _mtl_u._World2Object[2].x;
  v_4.w = _mtl_u._World2Object[3].x;
  float4 v_5;
  v_5.x = _mtl_u._World2Object[0].y;
  v_5.y = _mtl_u._World2Object[1].y;
  v_5.z = _mtl_u._World2Object[2].y;
  v_5.w = _mtl_u._World2Object[3].y;
  float4 v_6;
  v_6.x = _mtl_u._World2Object[0].z;
  v_6.y = _mtl_u._World2Object[1].z;
  v_6.z = _mtl_u._World2Object[2].z;
  v_6.w = _mtl_u._World2Object[3].z;
  float3 tmpvar_7;
  tmpvar_7 = normalize(((
    (v_4.xyz * _mtl_i._glesNormal.x)
   + 
    (v_5.xyz * _mtl_i._glesNormal.y)
  ) + (v_6.xyz * _mtl_i._glesNormal.z)));
  worldNormal_3 = half3(tmpvar_7);
  float3x3 tmpvar_8;
  tmpvar_8[0] = _mtl_u._Object2World[0].xyz;
  tmpvar_8[1] = _mtl_u._Object2World[1].xyz;
  tmpvar_8[2] = _mtl_u._Object2World[2].xyz;
  float3 tmpvar_9;
  tmpvar_9 = normalize((tmpvar_8 * _mtl_i._glesTANGENT.xyz));
  worldTangent_2 = half3(tmpvar_9);
  float tmpvar_10;
  tmpvar_10 = (_mtl_i._glesTANGENT.w * _mtl_u.unity_WorldTransformParams.w);
  tangentSign_1 = half(tmpvar_10);
  half3 tmpvar_11;
  tmpvar_11 = (((worldNormal_3.yzx * worldTangent_2.zxy) - (worldNormal_3.zxy * worldTangent_2.yzx)) * tangentSign_1);
  half3 tmpvar_12;
  tmpvar_12.x = worldTangent_2.x;
  tmpvar_12.y = tmpvar_11.x;
  tmpvar_12.z = worldNormal_3.x;
  half3 tmpvar_13;
  tmpvar_13.x = worldTangent_2.y;
  tmpvar_13.y = tmpvar_11.y;
  tmpvar_13.z = worldNormal_3.y;
  half3 tmpvar_14;
  tmpvar_14.x = worldTangent_2.z;
  tmpvar_14.y = tmpvar_11.z;
  tmpvar_14.z = worldNormal_3.z;
  _mtl_o.gl_Position = (_mtl_u.glstate_matrix_mvp * _mtl_i._glesVertex);
  _mtl_o.xlv_TEXCOORD0 = tmpvar_12;
  _mtl_o.xlv_TEXCOORD1 = tmpvar_13;
  _mtl_o.xlv_TEXCOORD2 = tmpvar_14;
  _mtl_o.xlv_TEXCOORD3 = (_mtl_u._Object2World * _mtl_i._glesVertex).xyz;
  return _mtl_o;
}

"
}
SubProgram "opengl " {
// Stats: 2 math
Keywords { "DIRECTIONAL" }
"#version 120

#ifdef VERTEX

uniform mat4 _Object2World;
uniform mat4 _World2Object;
uniform vec4 unity_WorldTransformParams;
attribute vec4 TANGENT;
varying vec3 xlv_TEXCOORD0;
varying vec3 xlv_TEXCOORD1;
varying vec3 xlv_TEXCOORD2;
varying vec3 xlv_TEXCOORD3;
void main ()
{
  vec4 v_1;
  v_1.x = _World2Object[0].x;
  v_1.y = _World2Object[1].x;
  v_1.z = _World2Object[2].x;
  v_1.w = _World2Object[3].x;
  vec4 v_2;
  v_2.x = _World2Object[0].y;
  v_2.y = _World2Object[1].y;
  v_2.z = _World2Object[2].y;
  v_2.w = _World2Object[3].y;
  vec4 v_3;
  v_3.x = _World2Object[0].z;
  v_3.y = _World2Object[1].z;
  v_3.z = _World2Object[2].z;
  v_3.w = _World2Object[3].z;
  vec3 tmpvar_4;
  tmpvar_4 = normalize(((
    (v_1.xyz * gl_Normal.x)
   + 
    (v_2.xyz * gl_Normal.y)
  ) + (v_3.xyz * gl_Normal.z)));
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  vec3 tmpvar_6;
  tmpvar_6 = normalize((tmpvar_5 * TANGENT.xyz));
  vec3 tmpvar_7;
  tmpvar_7 = (((tmpvar_4.yzx * tmpvar_6.zxy) - (tmpvar_4.zxy * tmpvar_6.yzx)) * (TANGENT.w * unity_WorldTransformParams.w));
  vec3 tmpvar_8;
  tmpvar_8.x = tmpvar_6.x;
  tmpvar_8.y = tmpvar_7.x;
  tmpvar_8.z = tmpvar_4.x;
  vec3 tmpvar_9;
  tmpvar_9.x = tmpvar_6.y;
  tmpvar_9.y = tmpvar_7.y;
  tmpvar_9.z = tmpvar_4.y;
  vec3 tmpvar_10;
  tmpvar_10.x = tmpvar_6.z;
  tmpvar_10.y = tmpvar_7.z;
  tmpvar_10.z = tmpvar_4.z;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_8;
  xlv_TEXCOORD1 = tmpvar_9;
  xlv_TEXCOORD2 = tmpvar_10;
  xlv_TEXCOORD3 = (_Object2World * gl_Vertex).xyz;
}


#endif
#ifdef FRAGMENT
void main ()
{
  vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 1.0;
  gl_FragData[0] = c_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 15 math
Keywords { "DIRECTIONAL" }
Bind "tangent" ATTR0
Bind "vertex" ATTR1
Bind "normal" ATTR2
ConstBuffer "$Globals" 208
Matrix 0 [glstate_matrix_mvp]
Matrix 64 [_Object2World]
Matrix 128 [_World2Object]
Vector 192 [unity_WorldTransformParams]
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesTANGENT [[attribute(0)]];
  float4 _glesVertex [[attribute(1)]];
  float3 _glesNormal [[attribute(2)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  half3 xlv_TEXCOORD0;
  half3 xlv_TEXCOORD1;
  half3 xlv_TEXCOORD2;
  float3 xlv_TEXCOORD3;
};
struct xlatMtlShaderUniform {
  float4x4 glstate_matrix_mvp;
  float4x4 _Object2World;
  float4x4 _World2Object;
  float4 unity_WorldTransformParams;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half tangentSign_1;
  half3 worldTangent_2;
  half3 worldNormal_3;
  float4 v_4;
  v_4.x = _mtl_u._World2Object[0].x;
  v_4.y = _mtl_u._World2Object[1].x;
  v_4.z = _mtl_u._World2Object[2].x;
  v_4.w = _mtl_u._World2Object[3].x;
  float4 v_5;
  v_5.x = _mtl_u._World2Object[0].y;
  v_5.y = _mtl_u._World2Object[1].y;
  v_5.z = _mtl_u._World2Object[2].y;
  v_5.w = _mtl_u._World2Object[3].y;
  float4 v_6;
  v_6.x = _mtl_u._World2Object[0].z;
  v_6.y = _mtl_u._World2Object[1].z;
  v_6.z = _mtl_u._World2Object[2].z;
  v_6.w = _mtl_u._World2Object[3].z;
  float3 tmpvar_7;
  tmpvar_7 = normalize(((
    (v_4.xyz * _mtl_i._glesNormal.x)
   + 
    (v_5.xyz * _mtl_i._glesNormal.y)
  ) + (v_6.xyz * _mtl_i._glesNormal.z)));
  worldNormal_3 = half3(tmpvar_7);
  float3x3 tmpvar_8;
  tmpvar_8[0] = _mtl_u._Object2World[0].xyz;
  tmpvar_8[1] = _mtl_u._Object2World[1].xyz;
  tmpvar_8[2] = _mtl_u._Object2World[2].xyz;
  float3 tmpvar_9;
  tmpvar_9 = normalize((tmpvar_8 * _mtl_i._glesTANGENT.xyz));
  worldTangent_2 = half3(tmpvar_9);
  float tmpvar_10;
  tmpvar_10 = (_mtl_i._glesTANGENT.w * _mtl_u.unity_WorldTransformParams.w);
  tangentSign_1 = half(tmpvar_10);
  half3 tmpvar_11;
  tmpvar_11 = (((worldNormal_3.yzx * worldTangent_2.zxy) - (worldNormal_3.zxy * worldTangent_2.yzx)) * tangentSign_1);
  half3 tmpvar_12;
  tmpvar_12.x = worldTangent_2.x;
  tmpvar_12.y = tmpvar_11.x;
  tmpvar_12.z = worldNormal_3.x;
  half3 tmpvar_13;
  tmpvar_13.x = worldTangent_2.y;
  tmpvar_13.y = tmpvar_11.y;
  tmpvar_13.z = worldNormal_3.y;
  half3 tmpvar_14;
  tmpvar_14.x = worldTangent_2.z;
  tmpvar_14.y = tmpvar_11.z;
  tmpvar_14.z = worldNormal_3.z;
  _mtl_o.gl_Position = (_mtl_u.glstate_matrix_mvp * _mtl_i._glesVertex);
  _mtl_o.xlv_TEXCOORD0 = tmpvar_12;
  _mtl_o.xlv_TEXCOORD1 = tmpvar_13;
  _mtl_o.xlv_TEXCOORD2 = tmpvar_14;
  _mtl_o.xlv_TEXCOORD3 = (_mtl_u._Object2World * _mtl_i._glesVertex).xyz;
  return _mtl_o;
}

"
}
SubProgram "opengl " {
// Stats: 2 math
Keywords { "SPOT" }
"#version 120

#ifdef VERTEX

uniform mat4 _Object2World;
uniform mat4 _World2Object;
uniform vec4 unity_WorldTransformParams;
attribute vec4 TANGENT;
varying vec3 xlv_TEXCOORD0;
varying vec3 xlv_TEXCOORD1;
varying vec3 xlv_TEXCOORD2;
varying vec3 xlv_TEXCOORD3;
void main ()
{
  vec4 v_1;
  v_1.x = _World2Object[0].x;
  v_1.y = _World2Object[1].x;
  v_1.z = _World2Object[2].x;
  v_1.w = _World2Object[3].x;
  vec4 v_2;
  v_2.x = _World2Object[0].y;
  v_2.y = _World2Object[1].y;
  v_2.z = _World2Object[2].y;
  v_2.w = _World2Object[3].y;
  vec4 v_3;
  v_3.x = _World2Object[0].z;
  v_3.y = _World2Object[1].z;
  v_3.z = _World2Object[2].z;
  v_3.w = _World2Object[3].z;
  vec3 tmpvar_4;
  tmpvar_4 = normalize(((
    (v_1.xyz * gl_Normal.x)
   + 
    (v_2.xyz * gl_Normal.y)
  ) + (v_3.xyz * gl_Normal.z)));
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  vec3 tmpvar_6;
  tmpvar_6 = normalize((tmpvar_5 * TANGENT.xyz));
  vec3 tmpvar_7;
  tmpvar_7 = (((tmpvar_4.yzx * tmpvar_6.zxy) - (tmpvar_4.zxy * tmpvar_6.yzx)) * (TANGENT.w * unity_WorldTransformParams.w));
  vec3 tmpvar_8;
  tmpvar_8.x = tmpvar_6.x;
  tmpvar_8.y = tmpvar_7.x;
  tmpvar_8.z = tmpvar_4.x;
  vec3 tmpvar_9;
  tmpvar_9.x = tmpvar_6.y;
  tmpvar_9.y = tmpvar_7.y;
  tmpvar_9.z = tmpvar_4.y;
  vec3 tmpvar_10;
  tmpvar_10.x = tmpvar_6.z;
  tmpvar_10.y = tmpvar_7.z;
  tmpvar_10.z = tmpvar_4.z;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_8;
  xlv_TEXCOORD1 = tmpvar_9;
  xlv_TEXCOORD2 = tmpvar_10;
  xlv_TEXCOORD3 = (_Object2World * gl_Vertex).xyz;
}


#endif
#ifdef FRAGMENT
void main ()
{
  vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 1.0;
  gl_FragData[0] = c_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 15 math
Keywords { "SPOT" }
Bind "tangent" ATTR0
Bind "vertex" ATTR1
Bind "normal" ATTR2
ConstBuffer "$Globals" 208
Matrix 0 [glstate_matrix_mvp]
Matrix 64 [_Object2World]
Matrix 128 [_World2Object]
Vector 192 [unity_WorldTransformParams]
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesTANGENT [[attribute(0)]];
  float4 _glesVertex [[attribute(1)]];
  float3 _glesNormal [[attribute(2)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  half3 xlv_TEXCOORD0;
  half3 xlv_TEXCOORD1;
  half3 xlv_TEXCOORD2;
  float3 xlv_TEXCOORD3;
};
struct xlatMtlShaderUniform {
  float4x4 glstate_matrix_mvp;
  float4x4 _Object2World;
  float4x4 _World2Object;
  float4 unity_WorldTransformParams;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half tangentSign_1;
  half3 worldTangent_2;
  half3 worldNormal_3;
  float4 v_4;
  v_4.x = _mtl_u._World2Object[0].x;
  v_4.y = _mtl_u._World2Object[1].x;
  v_4.z = _mtl_u._World2Object[2].x;
  v_4.w = _mtl_u._World2Object[3].x;
  float4 v_5;
  v_5.x = _mtl_u._World2Object[0].y;
  v_5.y = _mtl_u._World2Object[1].y;
  v_5.z = _mtl_u._World2Object[2].y;
  v_5.w = _mtl_u._World2Object[3].y;
  float4 v_6;
  v_6.x = _mtl_u._World2Object[0].z;
  v_6.y = _mtl_u._World2Object[1].z;
  v_6.z = _mtl_u._World2Object[2].z;
  v_6.w = _mtl_u._World2Object[3].z;
  float3 tmpvar_7;
  tmpvar_7 = normalize(((
    (v_4.xyz * _mtl_i._glesNormal.x)
   + 
    (v_5.xyz * _mtl_i._glesNormal.y)
  ) + (v_6.xyz * _mtl_i._glesNormal.z)));
  worldNormal_3 = half3(tmpvar_7);
  float3x3 tmpvar_8;
  tmpvar_8[0] = _mtl_u._Object2World[0].xyz;
  tmpvar_8[1] = _mtl_u._Object2World[1].xyz;
  tmpvar_8[2] = _mtl_u._Object2World[2].xyz;
  float3 tmpvar_9;
  tmpvar_9 = normalize((tmpvar_8 * _mtl_i._glesTANGENT.xyz));
  worldTangent_2 = half3(tmpvar_9);
  float tmpvar_10;
  tmpvar_10 = (_mtl_i._glesTANGENT.w * _mtl_u.unity_WorldTransformParams.w);
  tangentSign_1 = half(tmpvar_10);
  half3 tmpvar_11;
  tmpvar_11 = (((worldNormal_3.yzx * worldTangent_2.zxy) - (worldNormal_3.zxy * worldTangent_2.yzx)) * tangentSign_1);
  half3 tmpvar_12;
  tmpvar_12.x = worldTangent_2.x;
  tmpvar_12.y = tmpvar_11.x;
  tmpvar_12.z = worldNormal_3.x;
  half3 tmpvar_13;
  tmpvar_13.x = worldTangent_2.y;
  tmpvar_13.y = tmpvar_11.y;
  tmpvar_13.z = worldNormal_3.y;
  half3 tmpvar_14;
  tmpvar_14.x = worldTangent_2.z;
  tmpvar_14.y = tmpvar_11.z;
  tmpvar_14.z = worldNormal_3.z;
  _mtl_o.gl_Position = (_mtl_u.glstate_matrix_mvp * _mtl_i._glesVertex);
  _mtl_o.xlv_TEXCOORD0 = tmpvar_12;
  _mtl_o.xlv_TEXCOORD1 = tmpvar_13;
  _mtl_o.xlv_TEXCOORD2 = tmpvar_14;
  _mtl_o.xlv_TEXCOORD3 = (_mtl_u._Object2World * _mtl_i._glesVertex).xyz;
  return _mtl_o;
}

"
}
SubProgram "opengl " {
// Stats: 2 math
Keywords { "POINT_COOKIE" }
"#version 120

#ifdef VERTEX

uniform mat4 _Object2World;
uniform mat4 _World2Object;
uniform vec4 unity_WorldTransformParams;
attribute vec4 TANGENT;
varying vec3 xlv_TEXCOORD0;
varying vec3 xlv_TEXCOORD1;
varying vec3 xlv_TEXCOORD2;
varying vec3 xlv_TEXCOORD3;
void main ()
{
  vec4 v_1;
  v_1.x = _World2Object[0].x;
  v_1.y = _World2Object[1].x;
  v_1.z = _World2Object[2].x;
  v_1.w = _World2Object[3].x;
  vec4 v_2;
  v_2.x = _World2Object[0].y;
  v_2.y = _World2Object[1].y;
  v_2.z = _World2Object[2].y;
  v_2.w = _World2Object[3].y;
  vec4 v_3;
  v_3.x = _World2Object[0].z;
  v_3.y = _World2Object[1].z;
  v_3.z = _World2Object[2].z;
  v_3.w = _World2Object[3].z;
  vec3 tmpvar_4;
  tmpvar_4 = normalize(((
    (v_1.xyz * gl_Normal.x)
   + 
    (v_2.xyz * gl_Normal.y)
  ) + (v_3.xyz * gl_Normal.z)));
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  vec3 tmpvar_6;
  tmpvar_6 = normalize((tmpvar_5 * TANGENT.xyz));
  vec3 tmpvar_7;
  tmpvar_7 = (((tmpvar_4.yzx * tmpvar_6.zxy) - (tmpvar_4.zxy * tmpvar_6.yzx)) * (TANGENT.w * unity_WorldTransformParams.w));
  vec3 tmpvar_8;
  tmpvar_8.x = tmpvar_6.x;
  tmpvar_8.y = tmpvar_7.x;
  tmpvar_8.z = tmpvar_4.x;
  vec3 tmpvar_9;
  tmpvar_9.x = tmpvar_6.y;
  tmpvar_9.y = tmpvar_7.y;
  tmpvar_9.z = tmpvar_4.y;
  vec3 tmpvar_10;
  tmpvar_10.x = tmpvar_6.z;
  tmpvar_10.y = tmpvar_7.z;
  tmpvar_10.z = tmpvar_4.z;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_8;
  xlv_TEXCOORD1 = tmpvar_9;
  xlv_TEXCOORD2 = tmpvar_10;
  xlv_TEXCOORD3 = (_Object2World * gl_Vertex).xyz;
}


#endif
#ifdef FRAGMENT
void main ()
{
  vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 1.0;
  gl_FragData[0] = c_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 15 math
Keywords { "POINT_COOKIE" }
Bind "tangent" ATTR0
Bind "vertex" ATTR1
Bind "normal" ATTR2
ConstBuffer "$Globals" 208
Matrix 0 [glstate_matrix_mvp]
Matrix 64 [_Object2World]
Matrix 128 [_World2Object]
Vector 192 [unity_WorldTransformParams]
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesTANGENT [[attribute(0)]];
  float4 _glesVertex [[attribute(1)]];
  float3 _glesNormal [[attribute(2)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  half3 xlv_TEXCOORD0;
  half3 xlv_TEXCOORD1;
  half3 xlv_TEXCOORD2;
  float3 xlv_TEXCOORD3;
};
struct xlatMtlShaderUniform {
  float4x4 glstate_matrix_mvp;
  float4x4 _Object2World;
  float4x4 _World2Object;
  float4 unity_WorldTransformParams;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half tangentSign_1;
  half3 worldTangent_2;
  half3 worldNormal_3;
  float4 v_4;
  v_4.x = _mtl_u._World2Object[0].x;
  v_4.y = _mtl_u._World2Object[1].x;
  v_4.z = _mtl_u._World2Object[2].x;
  v_4.w = _mtl_u._World2Object[3].x;
  float4 v_5;
  v_5.x = _mtl_u._World2Object[0].y;
  v_5.y = _mtl_u._World2Object[1].y;
  v_5.z = _mtl_u._World2Object[2].y;
  v_5.w = _mtl_u._World2Object[3].y;
  float4 v_6;
  v_6.x = _mtl_u._World2Object[0].z;
  v_6.y = _mtl_u._World2Object[1].z;
  v_6.z = _mtl_u._World2Object[2].z;
  v_6.w = _mtl_u._World2Object[3].z;
  float3 tmpvar_7;
  tmpvar_7 = normalize(((
    (v_4.xyz * _mtl_i._glesNormal.x)
   + 
    (v_5.xyz * _mtl_i._glesNormal.y)
  ) + (v_6.xyz * _mtl_i._glesNormal.z)));
  worldNormal_3 = half3(tmpvar_7);
  float3x3 tmpvar_8;
  tmpvar_8[0] = _mtl_u._Object2World[0].xyz;
  tmpvar_8[1] = _mtl_u._Object2World[1].xyz;
  tmpvar_8[2] = _mtl_u._Object2World[2].xyz;
  float3 tmpvar_9;
  tmpvar_9 = normalize((tmpvar_8 * _mtl_i._glesTANGENT.xyz));
  worldTangent_2 = half3(tmpvar_9);
  float tmpvar_10;
  tmpvar_10 = (_mtl_i._glesTANGENT.w * _mtl_u.unity_WorldTransformParams.w);
  tangentSign_1 = half(tmpvar_10);
  half3 tmpvar_11;
  tmpvar_11 = (((worldNormal_3.yzx * worldTangent_2.zxy) - (worldNormal_3.zxy * worldTangent_2.yzx)) * tangentSign_1);
  half3 tmpvar_12;
  tmpvar_12.x = worldTangent_2.x;
  tmpvar_12.y = tmpvar_11.x;
  tmpvar_12.z = worldNormal_3.x;
  half3 tmpvar_13;
  tmpvar_13.x = worldTangent_2.y;
  tmpvar_13.y = tmpvar_11.y;
  tmpvar_13.z = worldNormal_3.y;
  half3 tmpvar_14;
  tmpvar_14.x = worldTangent_2.z;
  tmpvar_14.y = tmpvar_11.z;
  tmpvar_14.z = worldNormal_3.z;
  _mtl_o.gl_Position = (_mtl_u.glstate_matrix_mvp * _mtl_i._glesVertex);
  _mtl_o.xlv_TEXCOORD0 = tmpvar_12;
  _mtl_o.xlv_TEXCOORD1 = tmpvar_13;
  _mtl_o.xlv_TEXCOORD2 = tmpvar_14;
  _mtl_o.xlv_TEXCOORD3 = (_mtl_u._Object2World * _mtl_i._glesVertex).xyz;
  return _mtl_o;
}

"
}
SubProgram "opengl " {
// Stats: 2 math
Keywords { "DIRECTIONAL_COOKIE" }
"#version 120

#ifdef VERTEX

uniform mat4 _Object2World;
uniform mat4 _World2Object;
uniform vec4 unity_WorldTransformParams;
attribute vec4 TANGENT;
varying vec3 xlv_TEXCOORD0;
varying vec3 xlv_TEXCOORD1;
varying vec3 xlv_TEXCOORD2;
varying vec3 xlv_TEXCOORD3;
void main ()
{
  vec4 v_1;
  v_1.x = _World2Object[0].x;
  v_1.y = _World2Object[1].x;
  v_1.z = _World2Object[2].x;
  v_1.w = _World2Object[3].x;
  vec4 v_2;
  v_2.x = _World2Object[0].y;
  v_2.y = _World2Object[1].y;
  v_2.z = _World2Object[2].y;
  v_2.w = _World2Object[3].y;
  vec4 v_3;
  v_3.x = _World2Object[0].z;
  v_3.y = _World2Object[1].z;
  v_3.z = _World2Object[2].z;
  v_3.w = _World2Object[3].z;
  vec3 tmpvar_4;
  tmpvar_4 = normalize(((
    (v_1.xyz * gl_Normal.x)
   + 
    (v_2.xyz * gl_Normal.y)
  ) + (v_3.xyz * gl_Normal.z)));
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  vec3 tmpvar_6;
  tmpvar_6 = normalize((tmpvar_5 * TANGENT.xyz));
  vec3 tmpvar_7;
  tmpvar_7 = (((tmpvar_4.yzx * tmpvar_6.zxy) - (tmpvar_4.zxy * tmpvar_6.yzx)) * (TANGENT.w * unity_WorldTransformParams.w));
  vec3 tmpvar_8;
  tmpvar_8.x = tmpvar_6.x;
  tmpvar_8.y = tmpvar_7.x;
  tmpvar_8.z = tmpvar_4.x;
  vec3 tmpvar_9;
  tmpvar_9.x = tmpvar_6.y;
  tmpvar_9.y = tmpvar_7.y;
  tmpvar_9.z = tmpvar_4.y;
  vec3 tmpvar_10;
  tmpvar_10.x = tmpvar_6.z;
  tmpvar_10.y = tmpvar_7.z;
  tmpvar_10.z = tmpvar_4.z;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_8;
  xlv_TEXCOORD1 = tmpvar_9;
  xlv_TEXCOORD2 = tmpvar_10;
  xlv_TEXCOORD3 = (_Object2World * gl_Vertex).xyz;
}


#endif
#ifdef FRAGMENT
void main ()
{
  vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 1.0;
  gl_FragData[0] = c_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 15 math
Keywords { "DIRECTIONAL_COOKIE" }
Bind "tangent" ATTR0
Bind "vertex" ATTR1
Bind "normal" ATTR2
ConstBuffer "$Globals" 208
Matrix 0 [glstate_matrix_mvp]
Matrix 64 [_Object2World]
Matrix 128 [_World2Object]
Vector 192 [unity_WorldTransformParams]
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesTANGENT [[attribute(0)]];
  float4 _glesVertex [[attribute(1)]];
  float3 _glesNormal [[attribute(2)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  half3 xlv_TEXCOORD0;
  half3 xlv_TEXCOORD1;
  half3 xlv_TEXCOORD2;
  float3 xlv_TEXCOORD3;
};
struct xlatMtlShaderUniform {
  float4x4 glstate_matrix_mvp;
  float4x4 _Object2World;
  float4x4 _World2Object;
  float4 unity_WorldTransformParams;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half tangentSign_1;
  half3 worldTangent_2;
  half3 worldNormal_3;
  float4 v_4;
  v_4.x = _mtl_u._World2Object[0].x;
  v_4.y = _mtl_u._World2Object[1].x;
  v_4.z = _mtl_u._World2Object[2].x;
  v_4.w = _mtl_u._World2Object[3].x;
  float4 v_5;
  v_5.x = _mtl_u._World2Object[0].y;
  v_5.y = _mtl_u._World2Object[1].y;
  v_5.z = _mtl_u._World2Object[2].y;
  v_5.w = _mtl_u._World2Object[3].y;
  float4 v_6;
  v_6.x = _mtl_u._World2Object[0].z;
  v_6.y = _mtl_u._World2Object[1].z;
  v_6.z = _mtl_u._World2Object[2].z;
  v_6.w = _mtl_u._World2Object[3].z;
  float3 tmpvar_7;
  tmpvar_7 = normalize(((
    (v_4.xyz * _mtl_i._glesNormal.x)
   + 
    (v_5.xyz * _mtl_i._glesNormal.y)
  ) + (v_6.xyz * _mtl_i._glesNormal.z)));
  worldNormal_3 = half3(tmpvar_7);
  float3x3 tmpvar_8;
  tmpvar_8[0] = _mtl_u._Object2World[0].xyz;
  tmpvar_8[1] = _mtl_u._Object2World[1].xyz;
  tmpvar_8[2] = _mtl_u._Object2World[2].xyz;
  float3 tmpvar_9;
  tmpvar_9 = normalize((tmpvar_8 * _mtl_i._glesTANGENT.xyz));
  worldTangent_2 = half3(tmpvar_9);
  float tmpvar_10;
  tmpvar_10 = (_mtl_i._glesTANGENT.w * _mtl_u.unity_WorldTransformParams.w);
  tangentSign_1 = half(tmpvar_10);
  half3 tmpvar_11;
  tmpvar_11 = (((worldNormal_3.yzx * worldTangent_2.zxy) - (worldNormal_3.zxy * worldTangent_2.yzx)) * tangentSign_1);
  half3 tmpvar_12;
  tmpvar_12.x = worldTangent_2.x;
  tmpvar_12.y = tmpvar_11.x;
  tmpvar_12.z = worldNormal_3.x;
  half3 tmpvar_13;
  tmpvar_13.x = worldTangent_2.y;
  tmpvar_13.y = tmpvar_11.y;
  tmpvar_13.z = worldNormal_3.y;
  half3 tmpvar_14;
  tmpvar_14.x = worldTangent_2.z;
  tmpvar_14.y = tmpvar_11.z;
  tmpvar_14.z = worldNormal_3.z;
  _mtl_o.gl_Position = (_mtl_u.glstate_matrix_mvp * _mtl_i._glesVertex);
  _mtl_o.xlv_TEXCOORD0 = tmpvar_12;
  _mtl_o.xlv_TEXCOORD1 = tmpvar_13;
  _mtl_o.xlv_TEXCOORD2 = tmpvar_14;
  _mtl_o.xlv_TEXCOORD3 = (_mtl_u._Object2World * _mtl_i._glesVertex).xyz;
  return _mtl_o;
}

"
}
}
Program "fp" {
// Platform glcore skipped due to earlier errors
SubProgram "opengl " {
Keywords { "POINT" }
"// shader disassembly not supported on opengl"
}
SubProgram "metal " {
// Stats: 2 math
Keywords { "POINT" }
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 c_1;
  c_1.xyz = half3(float3(0.0, 0.0, 0.0));
  c_1.w = half(1.0);
  _mtl_o._glesFragData_0 = c_1;
  return _mtl_o;
}

"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" }
"// shader disassembly not supported on opengl"
}
SubProgram "metal " {
// Stats: 2 math
Keywords { "DIRECTIONAL" }
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 c_1;
  c_1.xyz = half3(float3(0.0, 0.0, 0.0));
  c_1.w = half(1.0);
  _mtl_o._glesFragData_0 = c_1;
  return _mtl_o;
}

"
}
SubProgram "opengl " {
Keywords { "SPOT" }
"// shader disassembly not supported on opengl"
}
SubProgram "metal " {
// Stats: 2 math
Keywords { "SPOT" }
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 c_1;
  c_1.xyz = half3(float3(0.0, 0.0, 0.0));
  c_1.w = half(1.0);
  _mtl_o._glesFragData_0 = c_1;
  return _mtl_o;
}

"
}
SubProgram "opengl " {
Keywords { "POINT_COOKIE" }
"// shader disassembly not supported on opengl"
}
SubProgram "metal " {
// Stats: 2 math
Keywords { "POINT_COOKIE" }
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 c_1;
  c_1.xyz = half3(float3(0.0, 0.0, 0.0));
  c_1.w = half(1.0);
  _mtl_o._glesFragData_0 = c_1;
  return _mtl_o;
}

"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL_COOKIE" }
"// shader disassembly not supported on opengl"
}
SubProgram "metal " {
// Stats: 2 math
Keywords { "DIRECTIONAL_COOKIE" }
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 c_1;
  c_1.xyz = half3(float3(0.0, 0.0, 0.0));
  c_1.w = half(1.0);
  _mtl_o._glesFragData_0 = c_1;
  return _mtl_o;
}

"
}
}
 }


 // Stats for Vertex shader:
 //       metal : 14 math
 //      opengl : 3 math
 // Stats for Fragment shader:
 //       metal : 3 math
 Pass {
  Name "PREPASS"
  Tags { "LIGHTMODE"="PrePassBase" "QUEUE"="Background" "IGNOREPROJECTOR"="False" "RenderType"="Opaque" }
  ZTest Less
  ZWrite Off
  GpuProgramID 193041
Program "vp" {
// Platform glcore skipped due to earlier errors
// Platform glcore had shader errors
//   <no keywords>
SubProgram "opengl " {
// Stats: 3 math
"#version 120

#ifdef VERTEX

uniform mat4 _Object2World;
uniform mat4 _World2Object;
uniform vec4 unity_WorldTransformParams;
attribute vec4 TANGENT;
varying vec3 xlv_TEXCOORD0;
varying vec3 xlv_TEXCOORD1;
varying vec3 xlv_TEXCOORD2;
void main ()
{
  vec4 v_1;
  v_1.x = _World2Object[0].x;
  v_1.y = _World2Object[1].x;
  v_1.z = _World2Object[2].x;
  v_1.w = _World2Object[3].x;
  vec4 v_2;
  v_2.x = _World2Object[0].y;
  v_2.y = _World2Object[1].y;
  v_2.z = _World2Object[2].y;
  v_2.w = _World2Object[3].y;
  vec4 v_3;
  v_3.x = _World2Object[0].z;
  v_3.y = _World2Object[1].z;
  v_3.z = _World2Object[2].z;
  v_3.w = _World2Object[3].z;
  vec3 tmpvar_4;
  tmpvar_4 = normalize(((
    (v_1.xyz * gl_Normal.x)
   + 
    (v_2.xyz * gl_Normal.y)
  ) + (v_3.xyz * gl_Normal.z)));
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  vec3 tmpvar_6;
  tmpvar_6 = normalize((tmpvar_5 * TANGENT.xyz));
  vec3 tmpvar_7;
  tmpvar_7 = (((tmpvar_4.yzx * tmpvar_6.zxy) - (tmpvar_4.zxy * tmpvar_6.yzx)) * (TANGENT.w * unity_WorldTransformParams.w));
  vec3 tmpvar_8;
  tmpvar_8.x = tmpvar_6.x;
  tmpvar_8.y = tmpvar_7.x;
  tmpvar_8.z = tmpvar_4.x;
  vec3 tmpvar_9;
  tmpvar_9.x = tmpvar_6.y;
  tmpvar_9.y = tmpvar_7.y;
  tmpvar_9.z = tmpvar_4.y;
  vec3 tmpvar_10;
  tmpvar_10.x = tmpvar_6.z;
  tmpvar_10.y = tmpvar_7.z;
  tmpvar_10.z = tmpvar_4.z;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_8;
  xlv_TEXCOORD1 = tmpvar_9;
  xlv_TEXCOORD2 = tmpvar_10;
}


#endif
#ifdef FRAGMENT
varying vec3 xlv_TEXCOORD0;
varying vec3 xlv_TEXCOORD1;
varying vec3 xlv_TEXCOORD2;
void main ()
{
  vec4 res_1;
  vec3 worldN_2;
  worldN_2.x = xlv_TEXCOORD0.z;
  worldN_2.y = xlv_TEXCOORD1.z;
  worldN_2.z = xlv_TEXCOORD2.z;
  res_1.xyz = ((worldN_2 * 0.5) + 0.5);
  res_1.w = 0.0;
  gl_FragData[0] = res_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 14 math
Bind "tangent" ATTR0
Bind "vertex" ATTR1
Bind "normal" ATTR2
ConstBuffer "$Globals" 208
Matrix 0 [glstate_matrix_mvp]
Matrix 64 [_Object2World]
Matrix 128 [_World2Object]
Vector 192 [unity_WorldTransformParams]
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesTANGENT [[attribute(0)]];
  float4 _glesVertex [[attribute(1)]];
  float3 _glesNormal [[attribute(2)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  half3 xlv_TEXCOORD0;
  half3 xlv_TEXCOORD1;
  half3 xlv_TEXCOORD2;
};
struct xlatMtlShaderUniform {
  float4x4 glstate_matrix_mvp;
  float4x4 _Object2World;
  float4x4 _World2Object;
  float4 unity_WorldTransformParams;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half tangentSign_1;
  half3 worldTangent_2;
  half3 worldNormal_3;
  float4 v_4;
  v_4.x = _mtl_u._World2Object[0].x;
  v_4.y = _mtl_u._World2Object[1].x;
  v_4.z = _mtl_u._World2Object[2].x;
  v_4.w = _mtl_u._World2Object[3].x;
  float4 v_5;
  v_5.x = _mtl_u._World2Object[0].y;
  v_5.y = _mtl_u._World2Object[1].y;
  v_5.z = _mtl_u._World2Object[2].y;
  v_5.w = _mtl_u._World2Object[3].y;
  float4 v_6;
  v_6.x = _mtl_u._World2Object[0].z;
  v_6.y = _mtl_u._World2Object[1].z;
  v_6.z = _mtl_u._World2Object[2].z;
  v_6.w = _mtl_u._World2Object[3].z;
  float3 tmpvar_7;
  tmpvar_7 = normalize(((
    (v_4.xyz * _mtl_i._glesNormal.x)
   + 
    (v_5.xyz * _mtl_i._glesNormal.y)
  ) + (v_6.xyz * _mtl_i._glesNormal.z)));
  worldNormal_3 = half3(tmpvar_7);
  float3x3 tmpvar_8;
  tmpvar_8[0] = _mtl_u._Object2World[0].xyz;
  tmpvar_8[1] = _mtl_u._Object2World[1].xyz;
  tmpvar_8[2] = _mtl_u._Object2World[2].xyz;
  float3 tmpvar_9;
  tmpvar_9 = normalize((tmpvar_8 * _mtl_i._glesTANGENT.xyz));
  worldTangent_2 = half3(tmpvar_9);
  float tmpvar_10;
  tmpvar_10 = (_mtl_i._glesTANGENT.w * _mtl_u.unity_WorldTransformParams.w);
  tangentSign_1 = half(tmpvar_10);
  half3 tmpvar_11;
  tmpvar_11 = (((worldNormal_3.yzx * worldTangent_2.zxy) - (worldNormal_3.zxy * worldTangent_2.yzx)) * tangentSign_1);
  half3 tmpvar_12;
  tmpvar_12.x = worldTangent_2.x;
  tmpvar_12.y = tmpvar_11.x;
  tmpvar_12.z = worldNormal_3.x;
  half3 tmpvar_13;
  tmpvar_13.x = worldTangent_2.y;
  tmpvar_13.y = tmpvar_11.y;
  tmpvar_13.z = worldNormal_3.y;
  half3 tmpvar_14;
  tmpvar_14.x = worldTangent_2.z;
  tmpvar_14.y = tmpvar_11.z;
  tmpvar_14.z = worldNormal_3.z;
  _mtl_o.gl_Position = (_mtl_u.glstate_matrix_mvp * _mtl_i._glesVertex);
  _mtl_o.xlv_TEXCOORD0 = tmpvar_12;
  _mtl_o.xlv_TEXCOORD1 = tmpvar_13;
  _mtl_o.xlv_TEXCOORD2 = tmpvar_14;
  return _mtl_o;
}

"
}
}
Program "fp" {
// Platform glcore skipped due to earlier errors
// Platform glcore skipped due to earlier errors
SubProgram "opengl " {
"// shader disassembly not supported on opengl"
}
SubProgram "metal " {
// Stats: 3 math
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  half3 xlv_TEXCOORD0;
  half3 xlv_TEXCOORD1;
  half3 xlv_TEXCOORD2;
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 res_1;
  half3 worldN_2;
  half3 tmpvar_3;
  half tmpvar_4;
  tmpvar_4 = _mtl_i.xlv_TEXCOORD0.z;
  worldN_2.x = tmpvar_4;
  half tmpvar_5;
  tmpvar_5 = _mtl_i.xlv_TEXCOORD1.z;
  worldN_2.y = tmpvar_5;
  half tmpvar_6;
  tmpvar_6 = _mtl_i.xlv_TEXCOORD2.z;
  worldN_2.z = tmpvar_6;
  tmpvar_3 = worldN_2;
  res_1.xyz = ((tmpvar_3 * (half)0.5) + (half)0.5);
  res_1.w = half(0.0);
  _mtl_o._glesFragData_0 = res_1;
  return _mtl_o;
}

"
}
}
 }


 // Stats for Vertex shader:
 //       metal : 32 math, 1 branch
 //      opengl : 2 math
 // Stats for Fragment shader:
 //       metal : 2 math
 Pass {
  Name "PREPASS"
  Tags { "LIGHTMODE"="PrePassFinal" "QUEUE"="Background" "IGNOREPROJECTOR"="False" "RenderType"="Opaque" }
  ZTest Less
  ZWrite Off
  GpuProgramID 245920
Program "vp" {
// Platform glcore had shader errors
//   Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_OFF" }
//   Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "UNITY_HDR_ON" }
SubProgram "opengl " {
// Stats: 2 math
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_OFF" }
"#version 120

#ifdef VERTEX
uniform vec4 _ProjectionParams;
uniform vec4 unity_SHAr;
uniform vec4 unity_SHAg;
uniform vec4 unity_SHAb;
uniform vec4 unity_SHBr;
uniform vec4 unity_SHBg;
uniform vec4 unity_SHBb;
uniform vec4 unity_SHC;

uniform mat4 _World2Object;
uniform vec4 unity_ColorSpaceLuminance;
varying vec4 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec3 xlv_TEXCOORD2;
void main ()
{
  vec4 tmpvar_1;
  vec4 tmpvar_2;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * gl_Vertex);
  vec4 o_3;
  vec4 tmpvar_4;
  tmpvar_4 = (tmpvar_1 * 0.5);
  vec2 tmpvar_5;
  tmpvar_5.x = tmpvar_4.x;
  tmpvar_5.y = (tmpvar_4.y * _ProjectionParams.x);
  o_3.xy = (tmpvar_5 + tmpvar_4.w);
  o_3.zw = tmpvar_1.zw;
  tmpvar_2.zw = vec2(0.0, 0.0);
  tmpvar_2.xy = vec2(0.0, 0.0);
  vec4 v_6;
  v_6.x = _World2Object[0].x;
  v_6.y = _World2Object[1].x;
  v_6.z = _World2Object[2].x;
  v_6.w = _World2Object[3].x;
  vec4 v_7;
  v_7.x = _World2Object[0].y;
  v_7.y = _World2Object[1].y;
  v_7.z = _World2Object[2].y;
  v_7.w = _World2Object[3].y;
  vec4 v_8;
  v_8.x = _World2Object[0].z;
  v_8.y = _World2Object[1].z;
  v_8.z = _World2Object[2].z;
  v_8.w = _World2Object[3].z;
  vec3 tmpvar_9;
  tmpvar_9 = normalize(((
    (v_6.xyz * gl_Normal.x)
   + 
    (v_7.xyz * gl_Normal.y)
  ) + (v_8.xyz * gl_Normal.z)));
  vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_9;
  vec3 res_11;
  vec3 x_12;
  x_12.x = dot (unity_SHAr, tmpvar_10);
  x_12.y = dot (unity_SHAg, tmpvar_10);
  x_12.z = dot (unity_SHAb, tmpvar_10);
  vec3 x1_13;
  vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_9.xyzz * tmpvar_9.yzzx);
  x1_13.x = dot (unity_SHBr, tmpvar_14);
  x1_13.y = dot (unity_SHBg, tmpvar_14);
  x1_13.z = dot (unity_SHBb, tmpvar_14);
  res_11 = (x_12 + (x1_13 + (unity_SHC.xyz * 
    ((tmpvar_9.x * tmpvar_9.x) - (tmpvar_9.y * tmpvar_9.y))
  )));
  if ((unity_ColorSpaceLuminance.w == 0.0)) {
    res_11 = max (((1.055 * 
      pow (max (res_11, vec3(0.0, 0.0, 0.0)), vec3(0.4166667, 0.4166667, 0.4166667))
    ) - 0.055), vec3(0.0, 0.0, 0.0));
  };
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_3;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = res_11;
}


#endif
#ifdef FRAGMENT
void main ()
{
  vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 1.0;
  gl_FragData[0] = c_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 32 math, 1 branches
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_OFF" }
Bind "vertex" ATTR0
Bind "normal" ATTR1
ConstBuffer "$Globals" 216
Matrix 80 [glstate_matrix_mvp]
Matrix 144 [_World2Object]
Vector 0 [_ProjectionParams]
VectorHalf 16 [unity_SHAr] 4
VectorHalf 24 [unity_SHAg] 4
VectorHalf 32 [unity_SHAb] 4
VectorHalf 40 [unity_SHBr] 4
VectorHalf 48 [unity_SHBg] 4
VectorHalf 56 [unity_SHBb] 4
VectorHalf 64 [unity_SHC] 4
VectorHalf 208 [unity_ColorSpaceLuminance] 4
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesVertex [[attribute(0)]];
  float3 _glesNormal [[attribute(1)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  float4 xlv_TEXCOORD0;
  float4 xlv_TEXCOORD1;
  float3 xlv_TEXCOORD2;
};
struct xlatMtlShaderUniform {
  float4 _ProjectionParams;
  half4 unity_SHAr;
  half4 unity_SHAg;
  half4 unity_SHAb;
  half4 unity_SHBr;
  half4 unity_SHBg;
  half4 unity_SHBb;
  half4 unity_SHC;
  float4x4 glstate_matrix_mvp;
  float4x4 _World2Object;
  half4 unity_ColorSpaceLuminance;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  float4 tmpvar_1;
  float4 tmpvar_2;
  float3 tmpvar_3;
  tmpvar_1 = (_mtl_u.glstate_matrix_mvp * _mtl_i._glesVertex);
  float4 o_4;
  float4 tmpvar_5;
  tmpvar_5 = (tmpvar_1 * 0.5);
  float2 tmpvar_6;
  tmpvar_6.x = tmpvar_5.x;
  tmpvar_6.y = (tmpvar_5.y * _mtl_u._ProjectionParams.x);
  o_4.xy = (tmpvar_6 + tmpvar_5.w);
  o_4.zw = tmpvar_1.zw;
  tmpvar_2.zw = float2(0.0, 0.0);
  tmpvar_2.xy = float2(0.0, 0.0);
  float4 v_7;
  v_7.x = _mtl_u._World2Object[0].x;
  v_7.y = _mtl_u._World2Object[1].x;
  v_7.z = _mtl_u._World2Object[2].x;
  v_7.w = _mtl_u._World2Object[3].x;
  float4 v_8;
  v_8.x = _mtl_u._World2Object[0].y;
  v_8.y = _mtl_u._World2Object[1].y;
  v_8.z = _mtl_u._World2Object[2].y;
  v_8.w = _mtl_u._World2Object[3].y;
  float4 v_9;
  v_9.x = _mtl_u._World2Object[0].z;
  v_9.y = _mtl_u._World2Object[1].z;
  v_9.z = _mtl_u._World2Object[2].z;
  v_9.w = _mtl_u._World2Object[3].z;
  float4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = normalize(((
    (v_7.xyz * _mtl_i._glesNormal.x)
   + 
    (v_8.xyz * _mtl_i._glesNormal.y)
  ) + (v_9.xyz * _mtl_i._glesNormal.z)));
  half4 normal_11;
  normal_11 = half4(tmpvar_10);
  half3 res_12;
  half3 x_13;
  x_13.x = dot (_mtl_u.unity_SHAr, normal_11);
  x_13.y = dot (_mtl_u.unity_SHAg, normal_11);
  x_13.z = dot (_mtl_u.unity_SHAb, normal_11);
  half3 x1_14;
  half4 tmpvar_15;
  tmpvar_15 = (normal_11.xyzz * normal_11.yzzx);
  x1_14.x = dot (_mtl_u.unity_SHBr, tmpvar_15);
  x1_14.y = dot (_mtl_u.unity_SHBg, tmpvar_15);
  x1_14.z = dot (_mtl_u.unity_SHBb, tmpvar_15);
  res_12 = (x_13 + (x1_14 + (_mtl_u.unity_SHC.xyz * 
    ((normal_11.x * normal_11.x) - (normal_11.y * normal_11.y))
  )));
  bool tmpvar_16;
  tmpvar_16 = (_mtl_u.unity_ColorSpaceLuminance.w == (half)0.0);
  if (tmpvar_16) {
    res_12 = max ((((half)1.055 * 
      pow (max (res_12, (half3)float3(0.0, 0.0, 0.0)), (half3)float3(0.4166667, 0.4166667, 0.4166667))
    ) - (half)0.055), (half3)float3(0.0, 0.0, 0.0));
  };
  tmpvar_3 = float3(res_12);
  _mtl_o.gl_Position = tmpvar_1;
  _mtl_o.xlv_TEXCOORD0 = o_4;
  _mtl_o.xlv_TEXCOORD1 = tmpvar_2;
  _mtl_o.xlv_TEXCOORD2 = tmpvar_3;
  return _mtl_o;
}

"
}
SubProgram "opengl " {
// Stats: 2 math
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "UNITY_HDR_ON" }
"#version 120

#ifdef VERTEX
uniform vec4 _ProjectionParams;
uniform vec4 unity_SHAr;
uniform vec4 unity_SHAg;
uniform vec4 unity_SHAb;
uniform vec4 unity_SHBr;
uniform vec4 unity_SHBg;
uniform vec4 unity_SHBb;
uniform vec4 unity_SHC;

uniform mat4 _World2Object;
uniform vec4 unity_ColorSpaceLuminance;
varying vec4 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec3 xlv_TEXCOORD2;
void main ()
{
  vec4 tmpvar_1;
  vec4 tmpvar_2;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * gl_Vertex);
  vec4 o_3;
  vec4 tmpvar_4;
  tmpvar_4 = (tmpvar_1 * 0.5);
  vec2 tmpvar_5;
  tmpvar_5.x = tmpvar_4.x;
  tmpvar_5.y = (tmpvar_4.y * _ProjectionParams.x);
  o_3.xy = (tmpvar_5 + tmpvar_4.w);
  o_3.zw = tmpvar_1.zw;
  tmpvar_2.zw = vec2(0.0, 0.0);
  tmpvar_2.xy = vec2(0.0, 0.0);
  vec4 v_6;
  v_6.x = _World2Object[0].x;
  v_6.y = _World2Object[1].x;
  v_6.z = _World2Object[2].x;
  v_6.w = _World2Object[3].x;
  vec4 v_7;
  v_7.x = _World2Object[0].y;
  v_7.y = _World2Object[1].y;
  v_7.z = _World2Object[2].y;
  v_7.w = _World2Object[3].y;
  vec4 v_8;
  v_8.x = _World2Object[0].z;
  v_8.y = _World2Object[1].z;
  v_8.z = _World2Object[2].z;
  v_8.w = _World2Object[3].z;
  vec3 tmpvar_9;
  tmpvar_9 = normalize(((
    (v_6.xyz * gl_Normal.x)
   + 
    (v_7.xyz * gl_Normal.y)
  ) + (v_8.xyz * gl_Normal.z)));
  vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_9;
  vec3 res_11;
  vec3 x_12;
  x_12.x = dot (unity_SHAr, tmpvar_10);
  x_12.y = dot (unity_SHAg, tmpvar_10);
  x_12.z = dot (unity_SHAb, tmpvar_10);
  vec3 x1_13;
  vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_9.xyzz * tmpvar_9.yzzx);
  x1_13.x = dot (unity_SHBr, tmpvar_14);
  x1_13.y = dot (unity_SHBg, tmpvar_14);
  x1_13.z = dot (unity_SHBb, tmpvar_14);
  res_11 = (x_12 + (x1_13 + (unity_SHC.xyz * 
    ((tmpvar_9.x * tmpvar_9.x) - (tmpvar_9.y * tmpvar_9.y))
  )));
  if ((unity_ColorSpaceLuminance.w == 0.0)) {
    res_11 = max (((1.055 * 
      pow (max (res_11, vec3(0.0, 0.0, 0.0)), vec3(0.4166667, 0.4166667, 0.4166667))
    ) - 0.055), vec3(0.0, 0.0, 0.0));
  };
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_3;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = res_11;
}


#endif
#ifdef FRAGMENT
void main ()
{
  vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 1.0;
  gl_FragData[0] = c_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 32 math, 1 branches
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "UNITY_HDR_ON" }
Bind "vertex" ATTR0
Bind "normal" ATTR1
ConstBuffer "$Globals" 216
Matrix 80 [glstate_matrix_mvp]
Matrix 144 [_World2Object]
Vector 0 [_ProjectionParams]
VectorHalf 16 [unity_SHAr] 4
VectorHalf 24 [unity_SHAg] 4
VectorHalf 32 [unity_SHAb] 4
VectorHalf 40 [unity_SHBr] 4
VectorHalf 48 [unity_SHBg] 4
VectorHalf 56 [unity_SHBb] 4
VectorHalf 64 [unity_SHC] 4
VectorHalf 208 [unity_ColorSpaceLuminance] 4
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesVertex [[attribute(0)]];
  float3 _glesNormal [[attribute(1)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  float4 xlv_TEXCOORD0;
  float4 xlv_TEXCOORD1;
  float3 xlv_TEXCOORD2;
};
struct xlatMtlShaderUniform {
  float4 _ProjectionParams;
  half4 unity_SHAr;
  half4 unity_SHAg;
  half4 unity_SHAb;
  half4 unity_SHBr;
  half4 unity_SHBg;
  half4 unity_SHBb;
  half4 unity_SHC;
  float4x4 glstate_matrix_mvp;
  float4x4 _World2Object;
  half4 unity_ColorSpaceLuminance;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  float4 tmpvar_1;
  float4 tmpvar_2;
  float3 tmpvar_3;
  tmpvar_1 = (_mtl_u.glstate_matrix_mvp * _mtl_i._glesVertex);
  float4 o_4;
  float4 tmpvar_5;
  tmpvar_5 = (tmpvar_1 * 0.5);
  float2 tmpvar_6;
  tmpvar_6.x = tmpvar_5.x;
  tmpvar_6.y = (tmpvar_5.y * _mtl_u._ProjectionParams.x);
  o_4.xy = (tmpvar_6 + tmpvar_5.w);
  o_4.zw = tmpvar_1.zw;
  tmpvar_2.zw = float2(0.0, 0.0);
  tmpvar_2.xy = float2(0.0, 0.0);
  float4 v_7;
  v_7.x = _mtl_u._World2Object[0].x;
  v_7.y = _mtl_u._World2Object[1].x;
  v_7.z = _mtl_u._World2Object[2].x;
  v_7.w = _mtl_u._World2Object[3].x;
  float4 v_8;
  v_8.x = _mtl_u._World2Object[0].y;
  v_8.y = _mtl_u._World2Object[1].y;
  v_8.z = _mtl_u._World2Object[2].y;
  v_8.w = _mtl_u._World2Object[3].y;
  float4 v_9;
  v_9.x = _mtl_u._World2Object[0].z;
  v_9.y = _mtl_u._World2Object[1].z;
  v_9.z = _mtl_u._World2Object[2].z;
  v_9.w = _mtl_u._World2Object[3].z;
  float4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = normalize(((
    (v_7.xyz * _mtl_i._glesNormal.x)
   + 
    (v_8.xyz * _mtl_i._glesNormal.y)
  ) + (v_9.xyz * _mtl_i._glesNormal.z)));
  half4 normal_11;
  normal_11 = half4(tmpvar_10);
  half3 res_12;
  half3 x_13;
  x_13.x = dot (_mtl_u.unity_SHAr, normal_11);
  x_13.y = dot (_mtl_u.unity_SHAg, normal_11);
  x_13.z = dot (_mtl_u.unity_SHAb, normal_11);
  half3 x1_14;
  half4 tmpvar_15;
  tmpvar_15 = (normal_11.xyzz * normal_11.yzzx);
  x1_14.x = dot (_mtl_u.unity_SHBr, tmpvar_15);
  x1_14.y = dot (_mtl_u.unity_SHBg, tmpvar_15);
  x1_14.z = dot (_mtl_u.unity_SHBb, tmpvar_15);
  res_12 = (x_13 + (x1_14 + (_mtl_u.unity_SHC.xyz * 
    ((normal_11.x * normal_11.x) - (normal_11.y * normal_11.y))
  )));
  bool tmpvar_16;
  tmpvar_16 = (_mtl_u.unity_ColorSpaceLuminance.w == (half)0.0);
  if (tmpvar_16) {
    res_12 = max ((((half)1.055 * 
      pow (max (res_12, (half3)float3(0.0, 0.0, 0.0)), (half3)float3(0.4166667, 0.4166667, 0.4166667))
    ) - (half)0.055), (half3)float3(0.0, 0.0, 0.0));
  };
  tmpvar_3 = float3(res_12);
  _mtl_o.gl_Position = tmpvar_1;
  _mtl_o.xlv_TEXCOORD0 = o_4;
  _mtl_o.xlv_TEXCOORD1 = tmpvar_2;
  _mtl_o.xlv_TEXCOORD2 = tmpvar_3;
  return _mtl_o;
}

"
}
}
Program "fp" {
// Platform glcore skipped due to earlier errors
SubProgram "opengl " {
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_OFF" }
"// shader disassembly not supported on opengl"
}
SubProgram "metal " {
// Stats: 2 math
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_OFF" }
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 tmpvar_1;
  half4 c_2;
  c_2.xyz = half3(float3(0.0, 0.0, 0.0));
  c_2.w = half(1.0);
  tmpvar_1 = c_2;
  _mtl_o._glesFragData_0 = tmpvar_1;
  return _mtl_o;
}

"
}
SubProgram "opengl " {
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "UNITY_HDR_ON" }
"// shader disassembly not supported on opengl"
}
SubProgram "metal " {
// Stats: 2 math
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "UNITY_HDR_ON" }
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 tmpvar_1;
  half4 c_2;
  c_2.xyz = half3(float3(0.0, 0.0, 0.0));
  c_2.w = half(1.0);
  tmpvar_1 = c_2;
  _mtl_o._glesFragData_0 = tmpvar_1;
  return _mtl_o;
}

"
}
}
 }
}
Fallback "Diffuse"
}