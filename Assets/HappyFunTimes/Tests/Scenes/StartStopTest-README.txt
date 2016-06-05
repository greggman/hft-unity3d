# StartStopTest

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

One thing to notice is the `PlayerSpawner` component is disabled.
This prevents HappyFunTimes from starting immediately. It is started
and stopped in `HFTStartStopTest` with the following code

```
PlayerSpawner m_spawner;
bool started = false;

void Start ()
{
    m_spawner = GetComponent<PlayerSpawner>();
}

void OnGUI()
{
    if (GUI.Button(new Rect(10, 10, 150, 100), started ? "Stop HFT" : "Start HFT"))
    {
        if (started)
        {
            m_spawner.StopHappyFunTimes();
        }
        else
        {
            m_spawner.StartHappyFunTimes();
        }
        started = !started;
    }
}
```



