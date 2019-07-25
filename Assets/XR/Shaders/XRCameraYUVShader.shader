Shader "Unlit/XRCameraYUVShader" {
  Properties {
    _YTex ("Texture", 2D) = "white" {}
    _UVTex ("Texture", 2D) = "white" {}
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
      uniform sampler2D _YTex;
      uniform sampler2D _UVTex;

      fixed4 frag (v2f i) : SV_Target {
        float2 coords = i.texcoord;
        float y = tex2D(_YTex, coords).r;
        float2 uvCoords = float2(coords.x, coords.y);
        float2 uv = tex2D(_UVTex, uvCoords).rg - (0.5, 0.5);
        return float4(
          y + 1.13983 * uv.y,
          y - 0.39465 * uv.x - 0.58060 * uv.y,
          y + 2.03211 * uv.x,
          1.0
        );
      }
      ENDCG
    }
  }
}
