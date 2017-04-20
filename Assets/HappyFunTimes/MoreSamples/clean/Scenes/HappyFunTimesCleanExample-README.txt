# The Cleanest Simple Example

This example is meant to show doing everything from scratch without using
without using any external libraries

Functionally it is exactly the same as the simple example found ount
`Assets/HappyFunTimes/MoreSamples/simple` except that one uses the
sample-ui library, this one does not.

It uses a simple custom controller found in
`Assets/WebPlayerTemplates/HappyFunTimes/controllers/clean/controller.html`

Tersely, if you select the `ExampleCleanSpawner` GameObject you'll see it has a `PlayerSpawner`
component set to spawn the `PrefabForExampleClean` prefab anytime a player connects their phone.

The `PrefabForExampleClean` prefab has the `ExampleCleanPlayer` script which uses the `NetPlayer`
object passed to it to send and receive messages from the phone.

For more info see [http://docs.happyfuntimes.net/docs/unity/](http://docs.happyfuntimes.net/docs/unity/).
