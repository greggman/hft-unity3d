# Camera Example

This sample is based of the Simple Example in `Assets/HappyFunTimes/MoreSamples/Simple`
It shows letting the user take or select an image on their camera then
sending that image to the game and using it as a texture.

It uses a custom controller found in
`Assets/WebPlayerTemplates/HappyFunTimes/controllers/camera/controller.html`

Tersely, if you select the `ExampleCameraSpawner` GameObject you'll see it has a `PlayerSpawner`
component set to spawn the `PrefabForExampleCamera` prefab anytime a player connects their phone.

The `PrefabForExampleCamera` prefab has the `ExampleCameraPlayer` script which uses the `NetPlayer`
object passed to it to send and receive messages from the phone.

The code in  `Assets/WebPlayerTemplates/HappyFunTimes/controllers/camera/controller.html`
uses an input type=file tag to ask the user to select an image. It then scales that image to 256x256,
using a 2D canvas, converts it to a dataUrl and sends it to the game. The game converts the dataUrl
to binary and passes it to `Texture2D.Load` in unity.

For more info see [http://docs.happyfuntimes.net/docs/unity/](http://docs.happyfuntimes.net/docs/unity/).
