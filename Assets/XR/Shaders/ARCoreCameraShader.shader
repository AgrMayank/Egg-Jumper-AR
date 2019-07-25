Shader "Unlit/ARCoreCameraShader" {
  Properties {
    // Default to portrait mode if none is specified.
    _ScreenOrientation("Screen Orientation", Int) = 1
  }
  SubShader {
    Pass {
      ZWrite Off
      GLSLPROGRAM

      #pragma only_renderers gles3

      #ifdef SHADER_API_GLES3
      #extension GL_OES_EGL_image_external_essl3 : require
      #endif

      uniform vec4 _MainTex_ST;

      #ifdef VERTEX

      #define PORTRAIT 1
      #define PORTRAIT_UPSIDE_DOWN 2
      #define LANDSCAPE_LEFT 3
      #define LANDSCAPE_RIGHT 4

      varying vec2 v_TexCoord;
      uniform int _ScreenOrientation;
      uniform mat4 _TextureWarp;

      void main() {
        #ifdef SHADER_API_GLES3
        gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
        v_TexCoord = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
        v_TexCoord = (_TextureWarp * vec4(v_TexCoord,0,1)).xy;

        switch (_ScreenOrientation) {
          case PORTRAIT_UPSIDE_DOWN:
            v_TexCoord = vec2(v_TexCoord.y, v_TexCoord.x);
            break;
          case LANDSCAPE_LEFT:
            v_TexCoord.y = 1.0 - v_TexCoord.y;
            break;
          case LANDSCAPE_RIGHT:
            v_TexCoord.x = 1.0 - v_TexCoord.x;
            break;
          case PORTRAIT:
          default:
            v_TexCoord = vec2(1.0 - v_TexCoord.y, 1.0 - v_TexCoord.x);
            break;
        }
        #endif
      }
      #endif

      #ifdef FRAGMENT
      varying vec2 v_TexCoord;
      uniform samplerExternalOES sTexture;

      void main() {
        #ifdef SHADER_API_GLES3
        gl_FragColor = texture2D(sTexture, v_TexCoord);
        #endif
      }
      #endif

      ENDGLSL
    }
  }
  Fallback "Unlit/XRCameraRGBAShader"
}
