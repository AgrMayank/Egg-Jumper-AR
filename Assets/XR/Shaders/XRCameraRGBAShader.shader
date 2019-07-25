Shader "Unlit/XRCameraRGBAShader" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
  }

  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    Pass {
      ZWrite Off
      CGPROGRAM

      // use "vert" function as the vertex shader
      #pragma vertex vert
      // use "frag" function as the pixel (fragment) shader
      #pragma fragment frag

      #include "UnityCG.cginc"

      float4x4 _TextureWarp;

      // vertex shader inputs
      struct appdata {
        float4 position : POSITION; // vertex position
        float2 texcoord : TEXCOORD0; // texture coordinate
      };

      // vertex shader outputs ("vertex to fragment")
      struct v2f {
        float2 texcoord : TEXCOORD0; // texture coordinate
        float4 position : SV_POSITION; // clip space position
      };

      v2f vert (appdata vertex) {
        v2f o;
        o.position = UnityObjectToClipPos(vertex.position);
        o.texcoord = float2(vertex.texcoord.x, (1 - vertex.texcoord.y) );
        o.texcoord = mul(_TextureWarp, float4(o.texcoord,0,1)).xy;
        return o;
      }

      // texture we will sample
      sampler2D _MainTex;

      fixed4 frag (v2f i) : SV_Target {
        fixed4 col = tex2D(_MainTex, i.texcoord);
        return col;
      }
      ENDCG
    }
  }
}
