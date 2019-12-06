HappyFunTimes-Unity3D
=====================

> ## **DEPECATED**
>
> I'm deprecating happyfuntimes. I'll keep the rendevous server running for a while
> longer but I think it's unfortunately time to mostly depreciate this project
>
> i'll consider accepting PRs still if you want to fix something but otherwise just
> fork it if you want to keep using it
>
> ## Issues
>
> There are a few major issues on why
>
> 1. **device orientation is no longer usable**
>
>    Both Safari and Chrome have made getting device orientation require HTTPS
>    which is something HFT can't provide at the moment. It would require
>    $$$$$$$. If a end-user friendly solution comes up maybe I'll revisit
>
> 2. **Browsers break stuff**
>
>    Every year or so a browser changes something or breaks something. Over the course
>    of HTF browser broke fullscreen support, audio support, touch support, orientation support,
>    and other things. It's no fun to keep up on that
>
> 3. **OSes break stuff**
>
>    For whatever reason networking that work before stops working. HFT has to do
>    some things to find out all the ways your phone might connect to the game
>    and that stuff seems to break every 2 years or so
>
> 4. **Offline Support breaks**
>
>    Using HFT without internet breaks every few year and will likely eventually
>    be unfixable. Both iOS and Android ping Apple and Google respectively when you
>    connect to WiFi to check if you're acutally on the internet. HFT tries to send
>    them *fake* data so they believe they are, otherwise they'll stop using the WiFi
>    and switch to mobile.
>
>    How they do this changes every few years so HFT has to figure out how to change its
>    faking. It is within Apple and Google's power to make this faking impossible and
>    I supsect they might at sometime which is scary because if they do then there is
>    no solution (well, short of acutally providing internet access)
>
> 5. **More browser features require HTTPS**
>
>    This is really #1 and #2 repeated but more and more browser features require HTTPS
>    and as it says above there is no way for hft to provide HTTPS at the moment.
>
> ## Good news
>
> If you don't need device orientation and you have no need for offline mode/installation mode
> then HFT still works. Write your own controllers to keep up to date with the latest changes
> in the browsers.
>
> ## Bad news
>
> I don't really have time to keep it running.

<img src="src/images/screenshot.png" />

[HappyFunTimes](http://docs.happyfuntimes.net) is a system for making game with 10-100+ players.
Players connect using their phones. Nothing needs to be installed on there phone. They can join
immediately.

The controllers are made with HTML5 so you can customize them all you want. There is a simple gamepad
controller included so you can get started without making a custom controller.

## Install

[Clear here to get it from the unity asset store](http://assetstore.unity3d.com/en/#!/content/19668).

## Docs

[Some more docs can be found here](http://docs.happyfuntimes.net/docs/unity/).

You can get this from the [Unity Asset Store](http://u3d.as/content/greggman/happy-fun-times)
or you can [download it directly here](http://docs.happyfuntimes.net/docs/unity/samples.html?owner=greggman&repo=hft-unity3d)

## Changelist

[See here](http://docs.happyfuntimes.net/docs/unity/changelist.html)

## Cloning

This repo uses [`git lfs`](https://git-lfs.github.com/). To clone install [`git lfs`](https://git-lfs.github.com/)
then

    git lfs clone https://github.com/greggman/hft-unity3d.git

## Building

If you want to use this repo directly you should be aware of a few requirements

### hft.js and hft.min.hs

These are generated from files in `src/hft` using webpack.

#### Prerequisites

make sure you have node.js installed and type

    cd path/to/repo
    npm install

This will download the required libraries and tools

#### building

    cd path/to/repo
    ./src/build

Note I have no made build scripts for windows. Feel free to submit a PR.

### sample-ui

The sample-ui is also in `src/hft` and is also generated with webpack

#### Building

Follow the instructions for `hft.js` above

### The OSX installation mode server

To run installation mode requires administrator priviledges because it needs ports 80
and 53. To achieve this on OSX we generate a separate web server program that is
only used in installation mode. This program is in `hft-server`.

#### Prerequisites

Install the [Xamarin Platform](https://www.xamarin.com/platform)

#### Building

    cd path/to/repo
    ./hft-server/build

Note I have no made build scripts for windows. Feel free to submit a PR.

## Dev Notes

### macOS

For whatever reason I can't seem to get the output of the macOS external server
to deliver debug messages. To test that you can put a `return;` at the top of
`HFTManager.StartExternalServer`. Then run the external server manually something
like this

    sudo  ~/Library/Application\ Support/Greggman/hft-unity3d/hft-server --hft-args="{\"debug\":\"*\",\"dns\":true,\"installationMode\":true,\"controllerFilename\":\"controllers/gamepad/controller.html\"}"


