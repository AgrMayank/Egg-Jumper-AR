using UnityEngine;
using UnityEngine.Rendering;


public class XRVideoTextureController : MonoBehaviour {
  private XRController xr;
  bool initialized = false;
  private CommandBuffer buffer;
  bool isCBInit = false;
  Material rMat = null;
  private float lastRotation = 0.0f;
  float texAspect = 1.0f;

  void Start() {
    xr = GameObject.FindWithTag("XRController").GetComponent<XRController>();
    if (!xr.DisabledInEditor()) {
      Initialize();
    }
  }

  void Initialize() {
    initialized = true;

    // Set reality texture onto our material. Make sure it's unlit to avoid appearing washed out.
    // Note that this requires Unlit/Texture to be included in the unity project.
    Renderer r = GetComponent<Renderer>();
    rMat = r.material;
    rMat.shader = xr.GetVideoTextureShader();
  }

  void Update() {
    if (xr.DisabledInEditor()) {
      return;
    }

    if (!initialized) {
      Initialize();
    }

    if (!isCBInit) {
      buffer = new CommandBuffer();
      buffer.Blit(null, BuiltinRenderTextureType.CurrentActive, rMat);
      isCBInit = true;
    }

    if (xr.ShouldUseRealityRGBATexture()) {
      var tex = xr.GetRealityRGBATexture();
      if (tex == null) {
        return;
      }
      rMat.mainTexture = tex;
      texAspect = tex.width * 1.0f / tex.height;
    } else {
      var ytex = xr.GetRealityYTexture();
      rMat.SetTexture("_YTex", ytex);
      rMat.SetTexture("_UVTex", xr.GetRealityUVTexture());
      texAspect = ytex.width * 1.0f / ytex.height;
    }

    float scaleFactor = texAspect / xr.GetRealityTextureAspectRatio();
    float rotation = lastRotation;
    switch(xr.GetTextureRotation()) {
      case XRTextureRotation.R270:
        rotation = -90.0f;
        scaleFactor = texAspect * xr.GetRealityTextureAspectRatio();
        break;
      case XRTextureRotation.R0:
        rotation = 0.0f;
        break;
      case XRTextureRotation.R90:
        rotation = 90.0f;
        scaleFactor = texAspect* xr.GetRealityTextureAspectRatio();
        break;
      case XRTextureRotation.R180:
        rotation = 180.0f;
        break;
      default:
        break;
    }
    lastRotation = rotation;

    Matrix4x4 mWarp = Matrix4x4.identity;
    if (scaleFactor > 1 + 1e-2) {
      float invScaleFactor = 1.0f / scaleFactor;
      mWarp[1, 1] = invScaleFactor;
      mWarp[1, 3] = (1 - invScaleFactor) * .5f;
    } else if (scaleFactor < 1 - 1e-2) {
      mWarp[0, 0] = scaleFactor;
      mWarp[0, 3] = (1 - scaleFactor) * .5f;
    }

    Matrix4x4 rotate90 = Matrix4x4.zero;
    rotate90[0, 1] = -1;
    rotate90[0, 3] = 1;
    rotate90[1, 0] = 1;
    rotate90[2, 2] = 1;
    rotate90[3, 3] = 1;

    Matrix4x4 m = Matrix4x4.identity;
    while (rotation < 0) {
      rotation += 360;
    }
    while (rotation > 360) {
      rotation -= 360;
    }
    while (rotation > 0) {
      m = m * rotate90;
      rotation -= 90;
    }

    Matrix4x4 nm = m * mWarp;

    rMat.SetMatrix("_TextureWarp", nm);
  }

}

