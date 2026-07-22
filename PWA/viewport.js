(function () {
  var viewportTimer = 0;
  var safeProbe = document.createElement('div');
  safeProbe.setAttribute('aria-hidden', 'true');
  safeProbe.style.cssText = 'position:fixed;visibility:hidden;pointer-events:none;padding:env(safe-area-inset-top) env(safe-area-inset-right) env(safe-area-inset-bottom) env(safe-area-inset-left)';
  document.body.appendChild(safeProbe);

  function updateLanternfallViewportMode() {
    viewportTimer = 0;
    var vv = window.visualViewport;
    var w = Math.max(1, Math.round((vv && vv.width) || window.innerWidth || document.documentElement.clientWidth || 1));
    var h = Math.max(1, Math.round((vv && vv.height) || window.innerHeight || document.documentElement.clientHeight || 1));
    var safe = getComputedStyle(safeProbe);
    var next = { width:w, height:h, safeLeft:parseFloat(safe.paddingLeft)||0, safeTop:parseFloat(safe.paddingTop)||0, safeRight:parseFloat(safe.paddingRight)||0, safeBottom:parseFloat(safe.paddingBottom)||0 };
    var prior = window.LanternfallViewport;
    next.revision = prior ? prior.revision : 1;
    if (!prior || Math.abs(prior.width-next.width)>1 || Math.abs(prior.height-next.height)>1 || Math.abs(prior.safeLeft-next.safeLeft)>1 || Math.abs(prior.safeTop-next.safeTop)>1 || Math.abs(prior.safeRight-next.safeRight)>1 || Math.abs(prior.safeBottom-next.safeBottom)>1) next.revision++;
    window.LanternfallViewport = next;

    var coarse = window.matchMedia && window.matchMedia('(pointer: coarse)').matches;
    var touch = (navigator.maxTouchPoints || 0) > 0 || 'ontouchstart' in window;
    var mobileUA = /Android|iPhone|iPod|Mobile|Windows Phone/i.test(navigator.userAgent || '');
    var likelyPhone = (coarse || touch || mobileUA) && Math.min(w, h) <= 700 && Math.max(w, h) <= 1200;
    document.body.classList.toggle('lanternfall-phone-portrait', likelyPhone && h > w);
    document.body.classList.toggle('lanternfall-phone-landscape', likelyPhone && w > h);
    document.body.classList.toggle('lanternfall-desktop', !likelyPhone);
    document.documentElement.style.setProperty('--lf-vw', w + 'px');
    document.documentElement.style.setProperty('--lf-vh', h + 'px');
  }

  function scheduleLanternfallViewportUpdate() {
    clearTimeout(viewportTimer);
    viewportTimer = setTimeout(updateLanternfallViewportMode, 80);
  }

  updateLanternfallViewportMode();
  window.addEventListener('resize', scheduleLanternfallViewportUpdate, { passive: true });
  window.addEventListener('orientationchange', scheduleLanternfallViewportUpdate, { passive: true });
  document.addEventListener('visibilitychange', scheduleLanternfallViewportUpdate);
  document.addEventListener('fullscreenchange', scheduleLanternfallViewportUpdate);
  if (window.visualViewport) {
    window.visualViewport.addEventListener('resize', scheduleLanternfallViewportUpdate, { passive: true });
    window.visualViewport.addEventListener('scroll', scheduleLanternfallViewportUpdate, { passive: true });
  }
  if ('serviceWorker' in navigator) window.addEventListener('load', function () { navigator.serviceWorker.register('./service-worker.js', { scope: './' }); });
})();
