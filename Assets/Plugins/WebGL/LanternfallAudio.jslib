mergeInto(LibraryManager.library, {
  LanternfallAudio_Unlock: function(master,sfx,music,muted,boss) {
    var L=window.__lanternfallAudio||(window.__lanternfallAudio={});
    L.ctx=L.ctx||new (window.AudioContext||window.webkitAudioContext)();
    if(L.ctx.state==='suspended')L.ctx.resume();
    L.master=master;L.sfx=sfx;L.music=music;L.muted=muted;L.boss=boss;
    if(!L.timer)L.timer=setInterval(function(){if(!L.ctx||L.muted||L.music<=0)return;var notes=L.boss?[110,138.59,164.81,123.47]:[146.83,174.61,220,196];var n=notes[L.step++%notes.length];var o=L.ctx.createOscillator(),g=L.ctx.createGain(),t=L.ctx.currentTime;o.type=L.boss?'sawtooth':'sine';o.frequency.value=n;g.gain.setValueAtTime(0,t);g.gain.linearRampToValueAtTime(L.master*L.music*.035,t+.04);g.gain.exponentialRampToValueAtTime(.0001,t+.48);o.connect(g);g.connect(L.ctx.destination);o.start(t);o.stop(t+.5);},550);
  },
  LanternfallAudio_Play: function(cue,master,sfx,muted) {
    var L=window.__lanternfallAudio;if(!L||!L.ctx||muted)return;var pitches=[260,330,130,220,170,150,280,360,420,190,165,120,105,440,145,110,392,350,180,523.25,92.5],t=L.ctx.currentTime,o=L.ctx.createOscillator(),g=L.ctx.createGain();o.type=(cue%3===0)?'sine':(cue%3===1)?'triangle':'square';o.frequency.value=pitches[cue]||220;g.gain.setValueAtTime(master*sfx*.11,t);g.gain.exponentialRampToValueAtTime(.0001,t+.08+(cue%3)*.03);o.connect(g);g.connect(L.ctx.destination);o.start(t);o.stop(t+.16);
  },
  LanternfallAudio_Set: function(master,sfx,music,muted,boss) {
    var L=window.__lanternfallAudio;if(!L)return;L.master=master;L.sfx=sfx;L.music=music;L.muted=muted;L.boss=boss;
  }
});
