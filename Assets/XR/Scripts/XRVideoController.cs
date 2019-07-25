using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using XRInternal;

public class XRVideoController : MonoBehaviour {

  private XRController xr;

  private Material xrMat;
  private CommandBuffer buffer;
  private bool isCBInit;
  private Camera cam;
  private CameraClearFlags camOriginalClearFlag_;
  private bool initialized = false;
  private Shader shader;
  private float lastRotation = 0.0f;
  private Camera sceneCamera;

  public void Start() {
    cam = GetComponent<Camera>();
    xr = GameObject.FindWithTag("XRController").GetComponent<XRController>();
    if (!xr.DisabledInEditor()) {
      Initialize();
    }
  }

  private void Initialize() {
    initialized = true;
    camOriginalClearFlag_ = cam.clearFlags;
    cam.clearFlags = CameraClearFlags.Depth;
    isCBInit = false;
    shader = xr.GetVideoShader();
    xrMat = new Material(shader);
    sceneCamera = GetComponent<Camera>();
  }

  void OnEnable() {
    // did we get re-enabled after disabled?
    if (initialized) {
      camOriginalClearFlag_ = cam.clearFlags;
      cam.clearFlags = CameraClearFlags.Depth;
    }
  }

  void OnDisable() {
    if (isCBInit) {
      cam.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, buffer);
      cam.clearFlags = camOriginalClearFlag_;
      isCBInit = false;
    }
  }

  void OnDestroy() {
    if (xr.DisabledInEditor()) {
      return;
    }
    if (isCBInit) {
      cam.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, buffer);
    }
  }

  public void OnPreRender() {
    if (xr.DisabledInEditor()) {
      return;
    }

    if (!initialized || xr.GetVideoShader() != shader) {
      Initialize();
    }
    if (!isCBInit) {
      buffer = new CommandBuffer();
      buffer.Blit(null, BuiltinRenderTextureType.CurrentActive, xrMat);
      cam.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, buffer);
      isCBInit = true;
    }

    if (xr.ShouldUseRealityRGBATexture()) {
      var tex = xr.GetRealityRGBATexture();
      if (tex == null) {
        return;
      }
      xrMat.mainTexture = tex;
    } else {
      xrMat.SetTexture("_YTex", xr.GetRealityYTexture());
      xrMat.SetTexture("_UVTex", xr.GetRealityUVTexture());
    }

    float scaleFactor = cam.aspect / xr.GetRealityTextureAspectRatio();
    float rotation = lastRotation;
    switch(xr.GetTextureRotation()) {
      case XRTextureRotation.R270:
        rotation = -90.0f;
        scaleFactor = cam.aspect * xr.GetRealityTextureAspectRatio();
        break;
      case XRTextureRotation.R0:
        rotation = 0.0f;
        break;
      case XRTextureRotation.R90:
        rotation = 90.0f;
        scaleFactor = cam.aspect * xr.GetRealityTextureAspectRatio();
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

    xrMat.SetMatrix("_TextureWarp", nm);
  }

  void Update() {
    if (!initialized) {
      return;
    }
    sceneCamera.projectionMatrix = xr.GetCameraIntrinsics();
  }
}
