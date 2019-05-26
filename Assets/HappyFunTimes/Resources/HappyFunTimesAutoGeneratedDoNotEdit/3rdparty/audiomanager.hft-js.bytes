/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};

/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {

/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId])
/******/ 			return installedModules[moduleId].exports;

/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			exports: {},
/******/ 			id: moduleId,
/******/ 			loaded: false
/******/ 		};

/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);

/******/ 		// Flag the module as loaded
/******/ 		module.loaded = true;

/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}


/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;

/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;

/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";

/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(0);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [
	    __webpack_require__(1),
	  ], __WEBPACK_AMD_DEFINE_RESULT__ = function (
	    AudioManager
	  ) {
	  window.AudioManager = AudioManager;
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));



/***/ },
/* 1 */
/***/ function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;"use strict";

	!(__WEBPACK_AMD_DEFINE_ARRAY__ = [
	    __webpack_require__(2),
	  ], __WEBPACK_AMD_DEFINE_RESULT__ = function(
	     jsfxlib) {

	  var webAudioAPI = window.AudioContext || window.webkitAudioContext || window.mozAudioContext;

	  // To play a sound, simply call audio.playSound(id), where id is
	  // one of the keys of the g_sound_files array, e.g. "damage".

	  // options:
	  //   startedOnTouchCallback: on iOS no sounds can be played unless at least one is first initiated during
	  //       a use gesture. If a function is attached here it will be called when that user gesture has happened.
	  //       This is useful for situtations like jamjam where sounds 'should' start right from the beginning
	  //       even if the player as not touched the screen. In that case we put up a message, "touch the screen"
	  //       and remove that message when we get this callback
	  //
	  //   callback: called when all the sounds have loaded.
	  var AudioManager = function(sounds, options) {
	    options = options || {};
	    var g_context;
	    var g_audioMgr;
	    var g_soundBank = {};
	    var g_canPlay = false;
	    var g_canPlayOgg;
	    var g_canPlayMp3;
	    var g_canPlayWav;
	    var g_canPlayAif;
	    var g_createFromFileFn;
	    var g_createFromJSFXFn;

	    var changeExt = function(filename, ext) {
	      return filename.substring(0, filename.length - 3) + ext;
	    };

	    this.needUserGesture = (function() {
	      var iOS = ( navigator.userAgent.match(/(iPad|iPhone|iPod)/g) ? true : false );
	      var needUserGesture = iOS;
	      return function() {
	        return needUserGesture;
	      };
	    }());

	    var WebAudioBuffer = function() {
	    };

	    WebAudioBuffer.prototype.play = function(opt_when, opt_loop) {
	      if (!this.buffer) {
	        console.log(this.name, " not loaded");
	        return;
	      }
	      var src = g_context.createBufferSource();
	      src.buffer = this.buffer;
	      src.loop = opt_loop || false;
	      src.connect(g_context.destination);
	      if (src.start) {
	        src.start(opt_when);
	      } else {
	        src.noteOn(opt_when);
	      }
	      return src;
	    };

	    var WebAudioJSFX = function(name, data, samples, opt_callback) {
	      this.buffer = jsfxlib.createAudioBuffer(g_context, data);
	      if (opt_callback) {
	        setTimeout(opt_callback, 0);
	      }
	    };

	    WebAudioJSFX.prototype = new WebAudioBuffer();

	    function WebAudioSound(name, filename, samples, opt_callback) {
	      this.name = name;
	      var that = this;
	      var req = new XMLHttpRequest();
	      req.open("GET", filename, true);
	      req.responseType = "arraybuffer";
	      req.onload = function() {
	        g_context.decodeAudioData(req.response, function onSuccess(decodedBuffer) {
	          // Decoding was successful, do something useful with the audio buffer
	          that.buffer = decodedBuffer;
	          if (opt_callback) {
	            opt_callback(false);
	          }
	        }, function onFailure() {
	           console.error("failed to decoding audio buffer: " + filename);
	           if (opt_callback) {
	             opt_callback(true);
	           }
	        });
	      }
	      req.addEventListener("error", function(e) {
	        console.error("failed to load:", filename, " : ", e.target.status);
	      }, false);
	      req.send();
	    }

	    WebAudioSound.prototype = new WebAudioBuffer();

	    var AudioTagJSFX = function(name, data, samples, opt_callback) {
	      this.samples = samples || 1;
	      this.audio = {};
	      this.playNdx = 0;
	      for (var i = 0; i < samples; ++i) {
	        this.audio[i] = jsfxlib.createWave(data);
	      }
	      if (opt_callback) {
	        setTimeout(opt_callback, 0);
	      }
	    };

	    AudioTagJSFX.prototype.play = function(opt_when, opt_loop) {
	      this.playNdx = (this.playNdx + 1) % this.samples;
	      var a = this.audio[this.playNdx];
	      var b = new Audio();
	      b.src = a.src;
	      // TODO: use when
	      b.addEventListener("canplaythrough", function() {
	        b.play();
	        }, false);
	      b.load();
	    };

	    function AudioTagSound(name, filename, samples, opt_callback) {
	      this.waiting_on_load = samples;
	      this.samples = samples || 1;
	      this.name = name;
	      this.play_idx = 0;
	      this.audio = {};
	      for (var i = 0; i < samples; i++) {
	        var audio = new Audio();
	        var that = this;
	        var checkCallback = function(err) {
	          that.waiting_on_load--;
	          if (opt_callback) {
	            opt_callback(err);
	          }
	        };
	        audio.addEventListener("canplaythrough", function() {
	          checkCallback(false);
	        }, false);
	        audio.src = filename;
	        audio.onerror = function() {
	          checkCallback(true);
	        };
	        audio.load();
	        this.audio[i] = audio;
	      }
	    };

	    AudioTagSound.prototype.play = function(opt_when, opt_loop) {
	      if (this.waiting_on_load > 0) {
	        console.log(this.name, " not loaded");
	        return;
	      }
	      this.play_idx = (this.play_idx + 1) % this.samples;
	      var a = this.audio[this.play_idx];
	      // console.log(this.name, ":", this.play_idx, ":", a.src);
	      var b = new Audio();
	      b.src = a.src;
	      // TODO: use when
	      b.addEventListener("canplaythrough", function() {
	        b.play();
	        }, false);
	      b.load();
	    };

	    var handleError = function(filename, audio) {
	      return function(e) {
	        console.error("can't load ", filename);
	      }
	    };

	    this.playSound = function(name, opt_when, opt_loop) {
	      if (!g_canPlay)
	        return;
	      var sound = g_soundBank[name];
	      if (!sound) {
	        console.error("audio: '" + name + "' not known.");
	        return;
	      }
	      return sound.play(opt_when, opt_loop);
	    }.bind(this);

	    this.getTime = function() {
	      return g_context ? g_context.currentTime : Date.now() * 0.001;
	    }.bind(this);

	    // on iOS and possibly other devices you can't play any
	    // sounds in the browser unless you first play a sound
	    // in response to a user gesture. So, make something
	    // to respond to a user gesture.
	    var setupGesture = function() {
	      if (this.needUserGesture()) {
	        var count = 0;
	        var elem = window;
	        var that = this;
	        var eventNames = ['touchstart', 'mousedown'];
	        var playSoundToStartAudio = function() {
	          ++count;
	          if (count == 1) {
	            // just playing any sound does not seem to work.
	            var source = g_context.createOscillator();
	            var gain = g_context.createGain();
	            source.frequency.value = 1;
	            source.connect(gain);
	            gain.gain.value = 0;
	            gain.connect(g_context.destination);
	            if (source.start) {
	              source.start(0);
	            } else {
	              source.noteOn(0);
	            }
	            setTimeout(function() {
	              source.disconnect();
	            }, 100);
	            for (var ii = 0; ii < eventNames.length; ++ii) {
	              elem.removeEventListener(eventNames[ii], playSoundToStartAudio, false);
	            }
	            if (options.startedOnTouchCallback) {
	              options.startedOnTouchCallback();
	            }
	          }
	        }

	        for (var ii = 0; ii < eventNames.length; ++ii) {
	          elem.addEventListener(eventNames[ii], playSoundToStartAudio, false);
	        }
	      }
	    }.bind(this);

	    this.loadSound = function(soundName, filename, samples, opt_callback) {
	      var ext = filename.substring(filename.length - 3);
	      if (ext == 'ogg' && !g_canPlayOgg) {
	        filename = changeExt(filename, "mp3");
	      } else if (ext == 'mp3' && !g_canPlayMp3) {
	        filename = changeExt(filename, "ogg");
	      }
	      var s = new g_createFromFileFn(soundName, filename, samples, opt_callback);
	      g_soundBank[soundName] = s;
	      return s;
	    }.bind(this);

	    this.makeJSFXSound = function(soundName, data, samples, opt_callback) {
	      var s = new g_createFromJSFXFn(soundName, data, samples, opt_callback);
	      g_soundBank[soundName] = s;
	      return s;
	    }.bind(this);

	    this.loadSounds = function(sounds, opt_callback) {
	      var soundsPending = 1;
	      var soundsLoaded = function() {
	        --soundsPending;
	        if (soundsPending == 0 && opt_callback) {
	          opt_callback();
	        }
	      };

	      Object.keys(sounds).forEach(function(sound) {
	        var data = sounds[sound];
	        ++soundsPending;
	        if (data.jsfx) {
	          this.makeJSFXSound(sound, data.jsfx, data.samples, soundsLoaded);
	        } else {
	          this.loadSound(sound, data.filename, data.samples, soundsLoaded);
	        }
	      }.bind(this));

	      // so that we generate a callback even if there are no sounds.
	      // That way users don't have to restructure their code if they have no sounds or if they
	      // disable sounds by passing none in.
	      setTimeout(soundsLoaded, 0);
	    };

	    this.init = function(sounds) {
	      var a = new Audio()
	      g_canPlayOgg = a.canPlayType("audio/ogg");
	      g_canPlayMp3 = a.canPlayType("audio/mp3");
	      g_canPlayWav = a.canPlayType("audio/wav");
	      g_canPlayAif = a.canPlayType("audio/aif") || a.canPlayType("audio/aiff");
	      g_canPlay = g_canPlayOgg || g_canPlayMp3;
	      if (!g_canPlay)
	        return;

	      if (webAudioAPI) {
	        console.log("Using Web Audio API");
	        g_context = new webAudioAPI();

	        if (!g_context.createGain) { g_context.createGain = g_context.createGainNode.bind(g_context); }

	        g_createFromFileFn = WebAudioSound;
	        g_createFromJSFXFn = WebAudioJSFX;
	      } else {
	        console.log("Using Audio Tag");
	        g_createFromFileFn = AudioTagSound;
	        g_createFromJSFXFn = AudioTagJSFX;
	      }

	      if (sounds) {
	        this.loadSounds(sounds);
	      }

	      if (webAudioAPI) {
	        setupGesture();
	      }
	    }.bind(this);
	    this.init(sounds);

	    this.getSoundIds = function() {
	      return Object.keys(g_soundBank);
	    };
	  };

	  AudioManager.hasWebAudio = function() {
	    return webAudioAPI !== undefined;
	  };

	  return AudioManager;
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ },
/* 2 */
/***/ function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;"use strict";

	!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(3), __webpack_require__(4)], __WEBPACK_AMD_DEFINE_RESULT__ = function(audio, jsfx) {
	var jsfxlib = {};
	(function () {
	    // takes object with param arrays
	    // audiolib = {
	    //   Sound : ["sine", 1, 2, 4, 1...
	    // }
	    //
	    // returns object with audio samples
	    // p.Sound.play()
	    this.createWaves = function(lib){
	        var sounds = {};
	        for (var e in lib) {
	            var data = lib[e];
	            sounds[e] = this.createWave(data);
	        }
	        return sounds;
	    }

	    /* Create a single sound:
	       var p = jsfxlib.createWave(["sine", 1,2,3, etc.]);
	       p.play();
	   */
	    this.createWave = function(lib) {
	        var params = this.arrayToParams(lib),
	            data = jsfx.generate(params),
	            wave = audio.make(data);

	        return wave;
	    }

	    // takes object with param arrays
	    //
	    //     var audiolib = {
	    //       someSound : ["sine", 1, 2, 4, 1...
	    //     }
	    //
	    // returns object with AudioBuffers you can use
	    // with the Web Audio API
	    //
	    //     var sounds = jsfxlib.createAudioBuffers(ctx, audiolib);
	    //     var ctx = new AudioContext();
	    //     var src = ctx.createBufferSource();
	    //     src.buffer = sounds.someSound;
	    //     src.connect(ctx.destination);
	    //     src.start();
	    this.createAudioBuffers = function(ctx, lib) {
	        var sounds = {};
	        for (var e in lib) {
	            var data = lib[e];
	            sounds[e] = this.createAudioBuffer(ctx, data);
	        }
	        return sounds;
	    }

	    // Create a single AudioBuffer
	    //
	    //     var buffer = jsfxlib.createAudioBuffer(ctx, ["sine", 1,2,3, etc.]);
	    //     var ctx = new AudioContext();
	    //     var src = ctx.createBufferSource();
	    //     src.buffer = buffer;
	    //     src.connect(ctx.destination);
	    //     src.start();
	    this.createAudioBuffer = function(ctx, lib) {
	        var params = this.arrayToParams(lib),
	            data = jsfx.generate(params),
	            buffer = audio.makeAudioBuffer(ctx, data);

	        return buffer;
	    }

	    this.paramsToArray = function(params){
	        var pararr = [];
	        var len = jsfx.Parameters.length;
	        for(var i = 0; i < len; i++){
	            pararr.push(params[jsfx.Parameters[i].id]);
	        }
	        return pararr;
	    }

	    this.arrayToParams = function(pararr){
	        var params = {};
	        var len = jsfx.Parameters.length;
	        for(var i = 0; i < len; i++){
	            params[jsfx.Parameters[i].id] = pararr[i];
	        }
	        return params;
	    }
	}).apply(jsfxlib);

	return jsfxlib;
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));



/***/ },
/* 3 */
/***/ function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_RESULT__;"use strict";

	!(__WEBPACK_AMD_DEFINE_RESULT__ = function() {
	var audio = {};
	(function(samplerate){
	    this.SampleRate = samplerate || 44100;
	    var SampleRate = this.SampleRate;
	    
	    // Do not modify parameters without changing code!
	    var BitsPerSample = 16;
	    var NumChannels = 1;
	    var BlockAlign = NumChannels * BitsPerSample >> 3;
	    var ByteRate = SampleRate * BlockAlign;
	    
	    // helper functions
	    var chr = String.fromCharCode; // alias for getting converting int to char 
	    
	    //////////////////////
	    // Wave            ///
	    //////////////////////
	    
	    var waveTag="data:audio/wav;base64,";
	    // constructs a wave from sample array
	    var constructWave = function(data){
	        var l;
	        return pack( ["RIFF",36+(l=data.length),"WAVEfmt ",16,1,NumChannels,SampleRate,
	                       ByteRate,BlockAlign,BitsPerSample,"data",l,data],"s4s4224422s4s");
	    };
	    
	    // creates an audio object from sample data
	    this.make = function(arr){
	        return new Audio(waveTag + btoa(constructWave(arrayToData(arr))))
	    };
	    
	    // creates a wave file for downloading
	    this.makeWaveFile = function(arr){
	        dataToFile(waveTag + btoa(constructWave(arrayToData(arr))))
	    };
	    
	    this.makeAudioBuffer = function(ctx, data) {
	        var buffer = ctx.createBuffer(1, data.length, SampleRate);
	        var array = buffer.getChannelData(0);
	        for (var i = 0; i < data.length; ++i) {
	            array[i] = data[i];
	        }
	        return buffer;
	    };

	    //////////////////////
	    // General stuff   ///
	    //////////////////////
	     
	    // Converts an integer to String representation
	    //   a - number
	    //   i - number of bytes
	    var istr = function(a,i){
	        var m8 = 0xff; // 8 bit mask
	        return i?chr(a&m8)+istr(a>>8,i-1):"";
	    };
	    
	    // Packs array of data to a string
	    //   data   - array
	    //   format - s is for string, numbers denote bytes to store in
	    var pack = function(data,format){
	        var out="";
	        for(i=0;i<data.length;i++)
	            out+=format[i]=="s"?data[i]:istr(data[i],format.charCodeAt(i)-48);
	        return out;
	    }
	    
	    var dataToFile = function(data){
	        document.location.href = data;
	    }
	    
	    //////////////////////
	    // Audio Processing ///
	    //////////////////////
	    
	    // Utilities
	    //////////////////////
	    
	    // often used variables (just for convenience)
	    var count,out,i,sfreq;
	    var sin = Math.sin;
	    var TAU = 2*Math.PI;
	    var Arr = function(c){return new Array(c|0)}; // alias for creating a new array
	    
	    var clamp8bit  = function(a){return a<0?0:255<a?255:a}
	    var sample8bit = function(a){return clamp((a*127+127)|0)}
	    
	    this.toTime    = function(a){return a/SampleRate}
	    this.toSamples = function(a){return a*SampleRate}
	    
	    var arrayToData16bit = function(arr){
	        var out="";
	        var len = arr.length;
	        for( i=0 ; i < len ; i++){
	            var a = (arr[i] * 32767) | 0;
	            a = a < -32768 ? -32768 : 32767 < a ? 32767 : a; // clamp
	            a += a < 0 ? 65536 : 0;                       // 2-s complement
	            out += String.fromCharCode(a & 255, a >> 8);
	        };
	        return out;
	    }
	    
	    var arrayToData8bit = function(arr){
	        var out="";
	        var len = arr.length;
	        for( i=0 ; i < len ; i++){
	            var a = (arr[i] * 127 + 128) | 0;
	            a = a < 0 ? 0 : 255 < a ? 255 : a;
	            out += String.fromCharCode(a);
	        };
	        return out;
	    }
	    
	    var arrayToData = function(arr){
	        if( BitsPerSample == 16 )
	            return arrayToData16bit(arr);
	        else
	            return arrayToData8bit(arr);
	    }
	    
	    //////////////////////
	    // Processing
	    //////////////////////
	    
	    // adjusts volume of a buffer
	    this.adjustVolume = function(data, v){
	        for(i=0;i<data.length;i++)
	            data[i] *= v;
	        return data;
	    }
	    
	    // Filters
	    //////////////////////
	    
	    this.filter = function(data,func,from,to,A,B,C,D,E,F){
	        from = from ? from : 1;
	        to = to ? to : data.length;
	        out = data.slice(0);
	        for(i=from;i<to;i++)
	            out[i] = func(data, out, from,to,i,A,B,C,D,E,F)
	        return out;
	    };
	    var filter = this.filter;
	    
	    this.filters = {
	        lowpass  : 
	            function(data, out, from, to, pos, A){
	                return out[pos-1] + A * (data[pos] - out[pos-1]);
	            },
	        lowpassx : 
	            function(data, out, from, to, pos, A){
	                return out[pos-1] + A*(to - pos)/(to-from) * (data[pos] - out[pos-1]);
	            },
	        highpass :
	            function(data, out, from, to, pos, A){
	                return A * (out[pos-1] + data[pos] - data[pos-1])
	            }
	    };
	    var filters = this.filters;
	    
	    this.f = {
	        lowpass  : function(data, from, to, A){return filter(data, filters.lowpass, from, to, A);},
	        lowpassx : function(data, from, to, A){return filter(data, filters.lowpassx, from, to, A);},
	        highpass : function(data, from, to, A){return filter(data, filters.highpass, from, to, A);}
	    }
	    
	    // Generators
	    //////////////////////
	    
	    // general sound generation
	    // example:
	    // generate(3, 440, Math.sin);
	    this.generate = function(count, freq, func, A, B, C, D, E, F){
	        var sfreq=freq*TAU/SampleRate;
	        var out = Arr(count);
	        for(i=0; i < count;i++)
	            out[i] = func(i*sfreq,A,B,C,D,E,F);
	        return out;
	    }
	    
	    var lastNoise = 0;
	    
	    var generate = this.generate;
	    this.generators =  {
	        noise  : function(phase){
	                    if(phase % TAU < 4){
	                        lastNoise = Math.random() * 2 - 1;
	                    }
	                    return lastNoise;
	                },
	        uninoise : Math.random,
	        sine   : Math.sin,
	        synth  : function(phase){return sin(phase) + .5*sin(phase/2) + .3*sin(phase/4)},
	        saw    : function(phase){return 2*(phase/TAU - ((phase/TAU + 0.5)|0))},
	        square : function(phase,A){return sin(phase) > A ? 1.0 : sin(phase) < A ? -1.0 : A}
	    };
	    var generators = this.generators;
	    
	    this.g = {
	        noise  : function(count){ return generate(count,0, generators.noise) },
	        sine   : function(count, freq){ return generate(count, freq, generators.sine) },
	        synth  : function(count, freq){ return generate(count, freq, generators.synth) },
	        saw    : function(count, freq){ return generate(count, freq, generators.saw) },
	        square : function(count, freq, A){ return generate(count, freq, generators.square, A) }
	    };
	}).apply(audio);

	return audio;
	}.call(exports, __webpack_require__, exports, module), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ },
/* 4 */
/***/ function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;"use strict";

	!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(3)], __WEBPACK_AMD_DEFINE_RESULT__ = function(audio) {
	var jsfx = {};
	(function () {
	    this.Parameters = []; // will be constructed in the end

	    this.Generators = {
	        square : audio.generators.square,
	        saw    : audio.generators.saw,
	        sine   : audio.generators.sine,
	        noise  : audio.generators.noise,
	        synth  : audio.generators.synth
	    };
	    
	    this.getGeneratorNames = function(){
	        var names = [];
	        for(var e in this.Generators)
	            names.push(e);
	        return names;
	    }
	    
	    var nameToParam = function(name){
	        return name.replace(/ /g, "");
	    }
	    
	    this.getParameters = function () {
	        var params = [];
	        
	        var grp = 0;
	        
	        // add param
	        var ap = function (name, min, max, def, step) {
	            if (step === undefined)
	                step = (max - min) / 1000;
	            var param = { name: name, id: nameToParam(name),
	                          min: min, max: max, step:step, def: def, 
	                          type: "range", group: grp};
	            params.push(param);
	        };
	        
	        // add option
	        var ao = function(name, options, def){
	            var param = {name: name, id: nameToParam(name),
	                         options: options, def: def,
	                         type: "option", group: grp };
	            params.push(param);
	        }
	        
	        var gens = this.getGeneratorNames();
	        ao("Generator", gens, gens[0]);
	        ap("Super Sampling Quality", 0, 16, 0, 1);
	        ap("Master Volume",  0, 1, 0.4);
	        grp++;
	        
	        ap("Attack Time",    0, 1, 0.1); // seconds
	        ap("Sustain Time",   0, 2, 0.3); // seconds
	        ap("Sustain Punch",  0, 3, 2);
	        ap("Decay Time",     0, 2, 1); // seconds
	        grp++;
	        
	        ap("Min Frequency",   20, 2400, 0, 1);
	        ap("Start Frequency", 20, 2400, 440, 1);
	        ap("Max Frequency",   20, 2400, 2000, 1);
	        ap("Slide",           -1, 1, 0);
	        ap("Delta Slide",     -1, 1, 0);
	        
	        grp++;
	        ap("Vibrato Depth",     0, 1, 0);
	        ap("Vibrato Frequency", 0.01, 48, 8);
	        ap("Vibrato Depth Slide",   -0.3, 1, 0);
	        ap("Vibrato Frequency Slide", -1, 1, 0);
	        
	        grp++;
	        ap("Change Amount", -1, 1, 0);
	        ap("Change Speed",  0, 1, 0.1);
	        
	        grp++;
	        ap("Square Duty", 0, 0.5, 0);
	        ap("Square Duty Sweep", -1, 1, 0);
	        
	        grp++;
	        ap("Repeat Speed", 0, 0.8, 0);
	        
	        grp++;
	        ap("Phaser Offset", -1, 1, 0);
	        ap("Phaser Sweep", -1, 1, 0);
	        
	        grp++;
	        ap("LP Filter Cutoff", 0, 1, 1);
	        ap("LP Filter Cutoff Sweep", -1, 1, 0);
	        ap("LP Filter Resonance",    0, 1, 0);
	        ap("HP Filter Cutoff",       0, 1, 0);
	        ap("HP Filter Cutoff Sweep", -1, 1, 0);

	        return params;
	    };
	    
	    
	    /**
	     * Input params object has the same parameters as described above
	     * except all the spaces have been removed
	     *
	     * This returns an array of float values of the generated audio.
	     *
	     * To make it into a wave use:
	     *    data = jsfx.generate(params)
	     *    audio.make(data)
	     */
	    this.generate = function(params){
	        // useful consts/functions
	        var TAU = 2 * Math.PI,
	            sin = Math.sin,
	            cos = Math.cos,
	            pow = Math.pow,
	            abs = Math.abs;
	        var SampleRate = audio.SampleRate;
	        
	        // super sampling
	        var super_sampling_quality = params.SuperSamplingQuality | 0;
	        if(super_sampling_quality < 1) super_sampling_quality = 1;
	        SampleRate = SampleRate * super_sampling_quality;
	        
	        // enveloping initialization
	        var _ss = 1.0 + params.SustainPunch;
	        var envelopes = [ {from: 0.0, to: 1.0, time: params.AttackTime},
	                          {from: _ss, to: 1.0, time: params.SustainTime},
	                          {from: 1.0, to: 0.0, time: params.DecayTime}];
	        var envelopes_len = envelopes.length;
	        
	        // envelope sample calculation
	        for(var i = 0; i < envelopes_len; i++){
	            envelopes[i].samples = 1 + ((envelopes[i].time * SampleRate) | 0);
	        }
	        // envelope loop variables
	        var envelope = undefined;
	        var envelope_cur = 0.0;
	        var envelope_idx = -1;
	        var envelope_increment = 0.0;
	        var envelope_last = -1;
	        
	        // count total samples
	        var totalSamples = 0;
	        for(var i = 0; i < envelopes_len; i++){
	            totalSamples += envelopes[i].samples;
	        }
	        
	        // fix totalSample limit
	        if( totalSamples < SampleRate / 2){
	            totalSamples = SampleRate / 2;
	        }
	        
	        var outSamples = (totalSamples / super_sampling_quality)|0;
	        
	        // out data samples
	        var out = new Array(outSamples);
	        var sample = 0;
	        var sample_accumulator = 0;
	        
	        // main generator        
	        var generator = jsfx.Generators[params.Generator];
	        if (generator === undefined)
	            generator = this.Generators.square;
	        var generator_A = 0;
	        var generator_B = 0;
	        
	        // square generator
	        generator_A = params.SquareDuty;
	        var square_slide = params.SquareDutySweep / SampleRate;
	        
	        // phase calculation
	        var phase = 0;
	        var phase_speed = params.StartFrequency * TAU / SampleRate;
	        
	        // phase slide calculation        
	        var phase_slide = 1.0 + pow(params.Slide, 3.0) * 64.0 / SampleRate;
	        var phase_delta_slide = pow(params.DeltaSlide, 3.0) / (SampleRate * 1000); 
	        if (super_sampling_quality !== undefined)
	            phase_delta_slide /= super_sampling_quality; // correction
	        
	        // frequency limiter
	        if(params.MinFrequency > params.StartFrequency)
	            params.MinFrequency = params.StartFrequency;
	            
	        if(params.MaxFrequency < params.StartFrequency)
	            params.MaxFrequency = params.StartFrequency;
	        
	        var phase_min_speed = params.MinFrequency * TAU / SampleRate;
	        var phase_max_speed = params.MaxFrequency * TAU / SampleRate;
	        
	        // frequency vibrato
	        var vibrato_phase = 0;
	        var vibrato_phase_speed = params.VibratoFrequency * TAU / SampleRate;
	        var vibrato_amplitude = params.VibratoDepth;
	        
	        // frequency vibrato slide
	        var vibrato_phase_slide = 1.0 + pow(params.VibratoFrequencySlide, 3.0) * 3.0 / SampleRate;
	        var vibrato_amplitude_slide = params.VibratoDepthSlide / SampleRate;
	        
	        // arpeggiator
	        var arpeggiator_time = 0;
	        var arpeggiator_limit = params.ChangeSpeed * SampleRate;
	        var arpeggiator_mod   = pow(params.ChangeAmount, 2);
	        if (params.ChangeAmount > 0)
	            arpeggiator_mod = 1 + arpeggiator_mod * 10;
	        else
	            arpeggiator_mod = 1 - arpeggiator_mod * 0.9;
	        
	        // phaser
	        var phaser_max = 1024;
	        var phaser_mask = 1023;
	        var phaser_buffer = new Array(phaser_max);
	        for(var _i = 0; _i < phaser_max; _i++)
	            phaser_buffer[_i] = 0;
	        var phaser_pos = 0;
	        var phaser_offset = pow(params.PhaserOffset, 2.0) * (phaser_max - 4);
	        var phaser_offset_slide = pow(params.PhaserSweep, 3.0) * 4000 / SampleRate;
	        var phaser_enabled = (abs(phaser_offset_slide) > 0.00001) ||
	                             (abs(phaser_offset) > 0.00001);
	        
	        // lowpass filter
	        var filters_enabled = (params.HPFilterCutoff > 0.001) || (params.LPFilterCutoff < 0.999);
	        
	        var lowpass_pos = 0;
	        var lowpass_pos_slide = 0;
	        var lowpass_cutoff = pow(params.LPFilterCutoff, 3.0) / 10;
	        var lowpass_cutoff_slide = 1.0 + params.HPFilterCutoffSweep / 10000;
	        var lowpass_damping = 5.0 / (1.0 + pow(params.LPFilterResonance, 2) * 20 ) *
	                                    (0.01 + params.LPFilterCutoff);
	        if ( lowpass_damping > 0.8)
	            lowpass_damping = 0.8;
	        lowpass_damping = 1.0 - lowpass_damping;
	        var lowpass_enabled = params.LPFilterCutoff < 0.999;
	        
	        // highpass filter
	        var highpass_accumulator = 0;
	        var highpass_cutoff = pow(params.HPFilterCutoff, 2.0) / 10;
	        var highpass_cutoff_slide = 1.0 + params.HPFilterCutoffSweep / 10000;
	        
	        // repeat
	        var repeat_time  = 0;
	        var repeat_limit = totalSamples;
	        if (params.RepeatSpeed > 0){
	            repeat_limit = pow(1 - params.RepeatSpeed, 2.0) * SampleRate + 32;
	        }
	        
	        // master volume controller
	        var master_volume = params.MasterVolume;
	        
	        var k = 0;
	        for(var i = 0; i < totalSamples; i++){
	            // main generator
	            sample = generator(phase, generator_A, generator_B);
	            
	            // square generator
	            generator_A += square_slide;
	            if(generator_A < 0.0){
	                generator_A = 0.0;
	            } else if (generator_A > 0.5){
	                generator_A = 0.5;
	            }
	            
	            if( repeat_time > repeat_limit ){
	                // phase reset
	                phase = 0;
	                phase_speed = params.StartFrequency * TAU / SampleRate;
	                // phase slide reset
	                phase_slide = 1.0 + pow(params.Slide, 3.0) * 3.0 / SampleRate;
	                phase_delta_slide = pow(params.DeltaSlide, 3.0) / (SampleRate * 1000);
	                if (super_sampling_quality !== undefined)
	                    phase_delta_slide /= super_sampling_quality; // correction
	                // arpeggiator reset
	                arpeggiator_time = 0;
	                arpeggiator_limit = params.ChangeSpeed * SampleRate;
	                arpeggiator_mod   = 1 + (params.ChangeAmount | 0) / 12.0;                
	                // repeat reset
	                repeat_time = 0;
	            }
	            repeat_time += 1;
	            
	            // phase calculation
	            phase += phase_speed;
	            
	            // phase slide calculation
	            phase_slide += phase_delta_slide;
	            phase_speed *= phase_slide;
	            
	            // arpeggiator
	            if ( arpeggiator_time > arpeggiator_limit ){
	                phase_speed *= arpeggiator_mod;
	                arpeggiator_limit = totalSamples;
	            }
	            arpeggiator_time += 1;
	            
	            // frequency limiter
	            if (phase_speed > phase_max_speed){
	                phase_speed = phase_max_speed;
	            } else if(phase_speed < phase_min_speed){
	                phase_speed = phase_min_speed;
	            }
	            
	            // frequency vibrato
	            vibrato_phase += vibrato_phase_speed;
	            var _vibrato_phase_mod = phase_speed * sin(vibrato_phase) * vibrato_amplitude;
	            phase += _vibrato_phase_mod;
	            
	            // frequency vibrato slide
	            vibrato_phase_speed *= vibrato_phase_slide;
	            if(vibrato_amplitude_slide){
	                vibrato_amplitude += vibrato_amplitude_slide;
	                if(vibrato_amplitude < 0){
	                    vibrato_amplitude = 0;
	                    vibrato_amplitude_slide = 0;
	                } else if (vibrato_amplitude > 1){
	                    vibrato_amplitude = 1;
	                    vibrato_amplitude_slide = 0;
	                }
	            }
	            
	            // filters
	            if( filters_enabled ){
	                
	                if( abs(highpass_cutoff) > 0.001){
	                    highpass_cutoff *= highpass_cutoff_slide;
	                    if(highpass_cutoff < 0.00001){
	                        highpass_cutoff = 0.00001;
	                    } else if(highpass_cutoff > 0.1){
	                        highpass_cutoff = 0.1;
	                    }
	                }
	                
	                var _lowpass_pos_old = lowpass_pos;
	                lowpass_cutoff *= lowpass_cutoff_slide;
	                if(lowpass_cutoff < 0.0){
	                    lowpass_cutoff = 0.0;
	                } else if ( lowpass_cutoff > 0.1 ){
	                    lowpass_cutoff = 0.1;
	                }
	                if(lowpass_enabled){
	                    lowpass_pos_slide += (sample - lowpass_pos) * lowpass_cutoff;
	                    lowpass_pos_slide *= lowpass_damping;
	                } else {
	                    lowpass_pos = sample;
	                    lowpass_pos_slide = 0;
	                }
	                lowpass_pos += lowpass_pos_slide;
	                
	                highpass_accumulator += lowpass_pos - _lowpass_pos_old;
	                highpass_accumulator *= 1.0 - highpass_cutoff;
	                sample = highpass_accumulator;  
	            }
	            
	            // phaser
	            if (phaser_enabled) {
	                phaser_offset += phaser_offset_slide;
	                if( phaser_offset < 0){
	                    phaser_offset = -phaser_offset;
	                    phaser_offset_slide = -phaser_offset_slide;
	                }
	                if( phaser_offset > phaser_mask){
	                    phaser_offset = phaser_mask;
	                    phaser_offset_slide = 0;
	                }
	                
	                phaser_buffer[phaser_pos] = sample;
	                // phaser sample modification
	                var _p = (phaser_pos - (phaser_offset|0) + phaser_max) & phaser_mask;
	                sample += phaser_buffer[_p];
	                phaser_pos = (phaser_pos + 1) & phaser_mask;
	            }
	            
	            // envelope processing
	            if( i > envelope_last ){
	                envelope_idx += 1;
	                if(envelope_idx < envelopes_len) // fault protection
	                    envelope = envelopes[envelope_idx];
	                else // the trailing envelope is silence
	                    envelope = {from: 0, to: 0, samples: totalSamples};
	                envelope_cur = envelope.from;
	                envelope_increment = (envelope.to - envelope.from) / (envelope.samples + 1);
	                envelope_last += envelope.samples;
	            }
	            sample *= envelope_cur;
	            envelope_cur += envelope_increment;
	            
	            // master volume controller
	            sample *= master_volume;
	            
	            // prepare for next sample
	            if(super_sampling_quality > 1){
	                sample_accumulator += sample;
	                if( (i + 1) % super_sampling_quality === 0){
	                    out[k] = sample_accumulator / super_sampling_quality;
	                    k += 1;
	                    sample_accumulator = 0;
	                }
	            } else {
	                out[i] = sample;
	            }
	        }
	        
	        // return out;

	        // add padding 10ms
	        var len = (SampleRate / 100)|0;
	        var padding = new Array(len);
	        for(var i = 0; i < len; i++)
	            padding[i] = 0;
	        return padding.concat(out).concat(padding);
	    }
	    
	    this.Parameters = this.getParameters();
	    
	}).apply(jsfx);

	return jsfx;
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));



/***/ }
/******/ ]);