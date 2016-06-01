# Instructions

This is just a copy of the "TouchScene" scene except it has an `HFTInstructions`
script component on the `Instructions` GameObject

## How To Do it

Clicking it and checking `show` then running the game will show instructions

THAT'S NOT THE POINT. **Leave it unchecked!**. It's only for testing

Instead if you export the game you can run it from the command line and pass in the instructions

### On OSX, from a Terminal

```
nameofgame.app/Contents/MacOS/nameofgame --instructions="Connect To Wifi 'OurInstallation' then go to hft.net"
```

### On Windows

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
or event and it will be read to go.


