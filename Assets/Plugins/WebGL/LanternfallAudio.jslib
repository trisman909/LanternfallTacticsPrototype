mergeInto(LibraryManager.library, {
  LanternfallAudio_Unlock: function(master,sfx,music,muted,boss) {
    var L=window.__lanternfallAudio||(window.__lanternfallAudio={});
    L.finite=L.finite||function(value,fallback,label){if(typeof value==='number'&&Number.isFinite(value))return value;if(!L.invalidValueWarned){console.warn('[Lanternfall Audio] Invalid '+label+'; using safe default.');L.invalidValueWarned=true;}return fallback;};
    L.volume=L.volume||function(value,fallback,label){return Math.min(1,Math.max(0,L.finite(value,fallback,label)));};
    L.ctx=L.ctx||new (window.AudioContext||window.webkitAudioContext)();
    if(L.ctx.state==='suspended')L.ctx.resume();
    L.master=L.volume(master,.8,'master volume');L.sfx=L.volume(sfx,.8,'SFX volume');L.music=L.volume(music,.45,'music volume');L.muted=L.finite(muted,0,'mute')?1:0;L.boss=L.finite(boss,0,'boss mode')?1:0;L.step=Math.max(0,Math.floor(L.finite(L.step,0,'music step')));
    if(!L.timer)L.timer=setInterval(function(){if(!L.ctx||L.muted||L.music<=0)return;var notes=L.boss?[110,138.59,164.81,123.47]:[146.83,174.61,220,196];var index=Math.max(0,Math.floor(L.finite(L.step,0,'music step')))%notes.length;L.step=index+1;var n=L.finite(notes[index],220,'oscillator frequency');var o=L.ctx.createOscillator(),g=L.ctx.createGain(),t=L.finite(L.ctx.currentTime,0,'audio time'),gain=L.volume(L.master*L.music*.035,0,'music gain');o.type=L.boss?'sawtooth':'sine';o.frequency.value=n;g.gain.setValueAtTime(0,t);g.gain.linearRampToValueAtTime(gain,t+.04);g.gain.exponentialRampToValueAtTime(.0001,t+.48);o.connect(g);g.connect(L.ctx.destination);o.start(t);o.stop(t+.5);},550);
  },
  LanternfallAudio_Play: function(cue,master,sfx,muted) {
    var L=window.__lanternfallAudio;if(!L||!L.ctx)return;master=L.volume(master,.8,'master volume');sfx=L.volume(sfx,.8,'SFX volume');muted=L.finite(muted,0,'mute')?1:0;if(muted)return;cue=Math.max(0,Math.floor(L.finite(cue,0,'audio cue')));var pitches=[260,330,130,220,170,150,280,360,420,190,165,120,105,440,145,110,392,350,180,523.25,92.5],t=L.finite(L.ctx.currentTime,0,'audio time'),o=L.ctx.createOscillator(),g=L.ctx.createGain(),frequency=L.finite(pitches[cue],220,'oscillator frequency'),gain=L.volume(master*sfx*.11,0,'SFX gain');o.type=(cue%3===0)?'sine':(cue%3===1)?'triangle':'square';o.frequency.value=frequency;g.gain.setValueAtTime(gain,t);g.gain.exponentialRampToValueAtTime(.0001,t+.08+(cue%3)*.03);o.connect(g);g.connect(L.ctx.destination);o.start(t);o.stop(t+.16);
  },
  LanternfallAudio_Set: function(master,sfx,music,muted,boss) {
    var L=window.__lanternfallAudio;if(!L)return;L.master=L.volume(master,.8,'master volume');L.sfx=L.volume(sfx,.8,'SFX volume');L.music=L.volume(music,.45,'music volume');L.muted=L.finite(muted,0,'mute')?1:0;L.boss=L.finite(boss,0,'boss mode')?1:0;
  }
});
