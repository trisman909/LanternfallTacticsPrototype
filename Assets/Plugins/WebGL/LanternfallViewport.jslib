mergeInto(LibraryManager.library, {
  LanternfallViewport_Revision: function () {
    var v = window.LanternfallViewport;
    return v ? (v.revision | 0) : 0;
  },
  LanternfallViewport_Width: function () {
    var v = window.LanternfallViewport;
    return v ? +v.width : 0;
  },
  LanternfallViewport_Height: function () {
    var v = window.LanternfallViewport;
    return v ? +v.height : 0;
  },
  LanternfallViewport_SafeLeft: function () {
    var v = window.LanternfallViewport;
    return v ? +v.safeLeft : 0;
  },
  LanternfallViewport_SafeTop: function () {
    var v = window.LanternfallViewport;
    return v ? +v.safeTop : 0;
  },
  LanternfallViewport_SafeRight: function () {
    var v = window.LanternfallViewport;
    return v ? +v.safeRight : 0;
  },
  LanternfallViewport_SafeBottom: function () {
    var v = window.LanternfallViewport;
    return v ? +v.safeBottom : 0;
  }
});
