Shader "Unlit/ARCoreTextureShader" {
  Properties {
    // Default to portrait mode if none is specified.
    _ScreenOrientation("Screen Orientation", Int) = 1
  }
  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    Pass {
      ZWrite On
      GLSLPROGRAM

      #pragma only_renderers gles3

      #ifdef SHADER_API_GLES3
      #extension GL_OES_EGL_image_external_essl3 : require
      #endif

      #ifdef VERTEX

      #define PORTRAIT 1
      #define PORTRAIT_UPSIDE_DOWN 2
      #define LANDSCAPE_LEFT 3
      #define LANDSCAPE_RIGHT 4

      varying vec2 v_TexCoord;
      uniform int _ScreenOrientation;

      void main() {
        #ifdef SHADER_API_GLES3
        gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
        switch (_ScreenOrientation) {
          case PORTRAIT_UPSIDE_DOWN:
            v_TexCoord = vec2(1.0 - gl_MultiTexCoord0.y, gl_MultiTexCoord0.x);
            break;
          case LANDSCAPE_LEFT:
            v_TexCoord = gl_MultiTexCoord0.xy;
            break;
          case LANDSCAPE_RIGHT:
            v_TexCoord = vec2(1.0 - gl_MultiTexCoord0.x, 1.0 - gl_MultiTexCoord0.y);
            break;
          case PORTRAIT:
          default:
            v_TexCoord = vec2(gl_MultiTexCoord0.y, 1.0 - gl_MultiTexCoord0.x);
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
  Fallback "Unlit/Texture"
}
