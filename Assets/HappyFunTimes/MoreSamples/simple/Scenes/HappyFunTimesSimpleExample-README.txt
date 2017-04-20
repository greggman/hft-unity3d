# The Simplest Example

This example is meant to show doing everything from scratch without using
the gamepad controller.

It uses a simple custom controller found in
`Assets/WebPlayerTemplates/HappyFunTimes/controllers/simple/controller.html`

Tersely, if you select the `ExampleSimpleSpawner` GameObject you'll see it has a `PlayerSpawner`
component set to spawn the `PrefabForExampleSimple` prefab anytime a player connects their phone.

The `PrefabForExampleSimple` prefab has the `ExampleSimplePlayer` script which uses the `NetPlayer`
object passed to it to send and receive messages from the phone.

Also please check out the clean example found in
`Assets/HappyFunTimes/MoreSamples/clean`

For more info see [http://docs.happyfuntimes.net/docs/unity/](http://docs.happyfuntimes.net/docs/unity/).
