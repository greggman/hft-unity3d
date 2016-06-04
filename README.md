HappyFunTimes-Unity3D
=====================

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

