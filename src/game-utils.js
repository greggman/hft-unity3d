define([
    './hft-utils/dist/audio',
    './hft-utils/dist/imageloader',
    './hft-utils/dist/imageutils',
  ], function (
    AudioManager,
    imageLoader,
    imageUtils
  ) {
  window.AudioManager = AudioManager;
  window.imageLoader = imageLoader;
  window.imageUtils = imageUtils;
});

