Shader "Custom/AlphaBlend"
{
	Properties
	{
		_Color("Main Tint", Color) = (1,1,1,1)
		_MainTex("Main Tex", 2D) = "white" {}
	//使用新的属性_AlphaScale来代替原先的_Cutoff属性，用以控制透明纹理的整体透明度
	_AlphaScale("Alpha Scale", Range(0, 1)) = 1
	}
		SubShader
	{
		//如综述中所示，需要注意，透明度混合用到的渲染队列和类型应该是Transparent
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		//在pass中，为透明度混合进行合适的混合状态设置
		Pass {
			Tags { "LightMode" = "ForwardBase" }
			Cull Off//双面
			ZWrite Off //关闭深度写入
			Blend SrcAlpha OneMinusSrcAlpha //开启Blend混合，设置源颜色和目标颜色混合因子

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _AlphaScale;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD2;
			};
			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				return o;
			}
			//移除透明度测试的代码，并设置该片元着色器返回值中的透明通道值为纹理像素透明通道的值玉与材质参数_AlphaScale的乘积
			fixed4 frag(v2f i) : SV_Target {
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

				fixed4 texColor = tex2D(_MainTex, i.uv);

				//使用纹理去采样漫反射颜色
				fixed3 albedo = texColor.rgb * _Color.rgb;
				// 环境光
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				//漫反射
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir)); // 颜色融合用乘法

				// 最终颜色 = 漫反射 + 环境光 + 高光反射
				return fixed4(diffuse + ambient, texColor.a * _AlphaScale);
			}
			ENDCG
		}
	}
		FallBack "Transparent/VertexLit"
}