Shader "Unlit/XRTextureYUVShader" {
  Properties {
    _YTex ("Texture", 2D) = "white" {}
    _UVTex ("Texture", 2D) = "white" {}
  }

  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma target 2.0
      #pragma multi_compile_fog

      #include "UnityCG.cginc"

      float4x4 _TextureWarp;

      struct appdata_t {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
      };

      struct v2f {
        float4 vertex : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        UNITY_FOG_COORDS(1)
        UNITY_VERTEX_OUTPUT_STEREO
      };

      sampler2D _YTex;
      sampler2D _UVTex;
      float4 _YTex_ST;

      v2f vert (appdata_t v) {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.texcoord = TRANSFORM_TEX(v.texcoord, _YTex);
        o.texcoord = mul(_TextureWarp, float4(o.texcoord,0,1)).xy;
        UNITY_TRANSFER_FOG(o,o.vertex);
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        // fixed4 col = tex2D(_MainTex, i.texcoord);
        float2 coords = i.texcoord;
        float y = tex2D(_YTex, coords).r;
        float2 uvCoords = float2(coords.x, coords.y);
        float2 uv = tex2D(_UVTex, uvCoords).rg - (0.5, 0.5);
        fixed4 col = float4(
          y + 1.13983 * uv.y,
          y - 0.39465 * uv.x - 0.58060 * uv.y,
          y + 2.03211 * uv.x,
          1.0
        );
        UNITY_APPLY_FOG(i.fogCoord, col);
        UNITY_OPAQUE_ALPHA(col.a);
        return col;
      }
      ENDCG
    }
  }
}
