StartStopTest
=============

This example shows a script that starts and stops HappyFunTimes on demand.

The idea is maybe you'd like to make a standard 1-4 player game that uses
gamepads but you want to add an option to allow more players using HappyFunTimes.

Well, you could put something on your main menu like

```
             Awesome Fight!
   -----------------------------------
   * 1-4 Players
   * 1-100 Players using HappyFunTimes

```

When players pick the second option you'd start HappyFunTimes. If they exit
that mode you'd stop HappyFunTimes

Note: Because HappyFunTimes runs over the network many parts of it are asynchronous.
There may be issues. Let's work through them together at http://github.com/greggman/hft-unity3d/issues

