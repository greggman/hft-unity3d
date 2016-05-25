A very simple scene showing the using the Touch controller mode of the HFTGamepad.

**IMPORTANT!!!**

The touch controller is very data intensive since it is sending the position of the
fingers from every player very quickly. For a small number of players, say 4 to 8
this is unlikely to be an issue. For a large number of players you should consider
making a custom controller.

As an example lets say you wanted to make a hockey game where users touch the screen
and then drag their finger in the direction they want their player to go. You send
all the data to Unity and then compute things in Unity. It works but it's very
inefficient

Ideally in this case you would create a custom controller in JavaScript that reads the
player's touch events, computes a direction and only sends the direction to Unity.

One direction per player is at least 50% less data than x and y per finger.
You could even decide that for such mushy controls you only need to send the data
at 10hz or even 5hz and players are unlikely to notice the difference. They are on
ice after all.

Even further you could possibly quantize the directions to 8 or 16 values instead
of a fluid angle and send the direction if it's different from the last time you
checked.

All of these optimizations would go a long way toward making sure your game can
support more players.

