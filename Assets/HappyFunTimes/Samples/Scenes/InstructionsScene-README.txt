# Instructions

This is just a copy of the "TouchScene" scene except it has an `HFTInstructions`
script component on the `Instructions` GameObject

## How To Use it

### Use Window->HappyFunTimes->Settings

Pick `Window->HappyFunTimes->Settings` from the menu. Check the `show instructions` button. Run

You should see the a message about how to use happyfuntimes come across the screen.

The settings here are for running in the editor without effecting the scene file.

### Use the command line

If you export the game you can run it from the command line and pass in the instructions

### On OSX, from a Terminal

```
nameofgame.app/Contents/MacOS/nameofgame --show-instructions
```

or

```
nameofgame.app/Contents/MacOS/nameofgame --instructions="Connect To Wifi 'OurInstallation' then go to hft.net"
```

### On Windows

```
nameofgame.exe --show-instructions
```

or

```
nameofgame.exe --instructions="Connect To Wifi 'OurInstallation' then go to hft.net"
```

You can also do this by setting environment variables

### On OSX, from a Terminal

```
export INSTRUCTIONS "Connect To Wifi 'OurInstallation' then go to hft.net"
nameofgame.app/Contents/MacOS/nameofgame
```

### On Windows

```
set INSTRUCTIONS="Connect To Wifi 'OurInstallation' then go to hft.net"
nameofgame.exe
```

The point is you can set the details at your location. Of course if you wanted to get fancy
you might create GUI to set this stuff when your game starts but for many situations this
is enough and/or possibly better. Make a script to launch the game at some installation
or event and it will be ready to go.

## Command Line Arguments

`--show-instuctions`    turn on the instructions as set in the scene

`--instruction="msg"`   sets the instructions to "msg"

`--bottom`     makes them appear at the bottom of the screen

