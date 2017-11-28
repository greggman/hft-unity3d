/*
 * Copyright 2014, Gregg Tavares.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 *
 *     * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above
 * copyright notice, this list of conditions and the following disclaimer
 * in the documentation and/or other materials provided with the
 * distribution.
 *     * Neither the name of Gregg Tavares. nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
"use strict";

// Start the main app logic.
var commonUI = sampleUI.commonUI;
var input = sampleUI.input;
var misc = sampleUI.misc;
var mobileHacks = sampleUI.mobileHacks;
var strings = sampleUI.strings;
var touch = sampleUI.touch;

var globals = {
  debug: false,
  //orientation: "landscape-primary",
};
misc.applyUrlSettings(globals);
mobileHacks.fixHeightHack();
mobileHacks.disableContextMenu();

var score = 0;
var statusElem = document.getElementById("gamestatus");
var inputElem = document.getElementById("inputarea");
var colorElem = document.getElementById("display");
var picFrameElem = document.getElementById("picture");
var picElem = document.querySelector("#picture input[type=file]");
var client = new hft.GameClient();

commonUI.setupStandardControllerUI(client, globals);
commonUI.askForNameOnce();
commonUI.showMenu(true);

function randInt(range) {
  return Math.floor(Math.random() * range);
};

// Sends a move command to the game.
//
// This will generate a 'move' event in the corresponding
// NetPlayer object in the game.
function sendMoveCmd(position, target) {
  client.sendCmd('move', {
    x: position.x / target.clientWidth,
    y: position.y / target.clientHeight,
  });
};

// Send a message to the game when the screen is touched
inputElem.addEventListener('pointermove', function(event) {
  var position = input.getRelativeCoordinates(event.target, event);
  sendMoveCmd(position, event.target);
  event.preventDefault();
});

// Send selected picture to game
picElem.addEventListener('change', function(event) {
  if (this.files && this.files[0]) {
    // NOTE: Older iOS could not handle camera images
    // in web pages
    picFrameElem.style.display = "none";
    // make image to load picture
    var img = new Image();
    // call function when done loading
    img.onload = function() {
      // create a 256x256 canvas
      var canvas = document.createElement("canvas");
      canvas.width = 256;
      canvas.height = 256;
      var ctx = canvas.getContext("2d");
      // scale the image using a css "cover" algo
      var aspect = img.width / img.height;
      var dstHeight = 256
      var dstWidth = dstHeight * aspect;
      if (dstWidth < 256) {
        dstWidth = 256;
        dstHeight = dstWidth / aspect;
      }
      var dstX = (256 - dstWidth) / 2;
      var dstY = (256 - dstHeight) / 2;
      ctx.drawImage(img, dstX, dstY, dstWidth, dstHeight);
      // send the image as a dataUrl to theg game
      client.sendCmd('pic', {
        dataUrl: canvas.toDataURL(),
      });
      // tell the browser we're done
      URL.revokeObjectURL(img.src);
    };
    // load the image
    img.src = URL.createObjectURL(this.files[0]);
  }
});

