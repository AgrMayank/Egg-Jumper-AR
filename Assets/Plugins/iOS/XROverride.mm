extern "C" {
  // If you want 8th Wall to use a custom ARSession instead of creating one for you, override this
  // method. For example:
  //   auto session = [ARSession new];
  //   return (__bridge_retained void*) session;
  void* c8_getCustomARSession() {
    return nullptr;
  }

  // If you want a custom ARWorldTrackingConfiguration (whether 8th Wall makes an ARSession for you
  // or not), override this method. For example:
  //   auto config = [ARWorldTrackingConfiguration new];
  //   return (__bridge_retained void*)config;
  void* c8_getCustomARSessionConfig() {
    return nullptr;
  }

  // If you want custom ARSessionDelegate methods for the existing ARSession, override this method
  // to create NSObject<ARSessionDelegate> and return its pointer. For example:
  //   auto delegate = [CustomDelegate new];
  //   return (__bridge_retained void*) delegate;
  void* c8_getCustomARSessionDelegate() {
    return nullptr;
  }
  /*
   * CustomDelegate example
   * 
   * @interface CustomDelegate : NSObject<ARSessionDelegate>
   * - (void)session:(ARSession *)session didUpdateFrame:(ARFrame *)frame;
   * @end
   *
   * @implementation CustomDelegate
   * - (void)session:(ARSession *)session didUpdateFrame:(ARFrame *)frame {
   *   NSLog(@"CustomDelegate::didUpdateFrame");
   * }

   * @end
   */
}
