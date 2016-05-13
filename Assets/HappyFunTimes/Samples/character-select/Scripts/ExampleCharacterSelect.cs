using UnityEngine;
using System;
using HappyFunTimes;

namespace HappyFunTimesExample {

class ExampleCharacterSelect : MonoBehaviour {

    public GameObject[] characterPrefabs = null;

    private class MessageCharacter : MessageCmdData {
        public int id = 0;
    }

    void InitializeNetPlayer(SpawnInfo spawnInfo) {
        // Save the netplayer object so we can use it send messages to the phone
        m_netPlayer = spawnInfo.netPlayer;

        // Register handler to call if the player disconnects from the game.
        m_netPlayer.OnDisconnect += Remove;

        // Track name changes
        m_playerNameManager = new HFTPlayerNameManager(m_netPlayer);

        // Setup events for the different messages.
        m_netPlayer.RegisterCmdHandler<MessageCharacter>("character", OnCharacter);
    }

    private void Remove(object sender, EventArgs e) {
        Destroy(gameObject);
    }

    private void OnCharacter(MessageCharacter data) {
        int ndx = data.id;
        if (ndx < 0 || ndx >= characterPrefabs.Length) {
            Debug.LogError("invalid charater selection: " + ndx);
            return;
        }

        // The ExampleCharacterSelect GameObject no longer
        // needs to care about m_netPlayer
        m_netPlayer.OnDisconnect -= Remove;
        m_netPlayer.UnregisterCmdHandler("character");

        // Create the character
        GameObject newGameObject = (GameObject)Instantiate(characterPrefabs[ndx]);
        // Send the netplayer to the character. We use a message
        // because then every character can have a differnet script if we want.
        ExampleSimplePlayer sp = newGameObject.GetComponent<ExampleSimplePlayer>();
        sp.InitializeNetPlayer(m_netPlayer, m_playerNameManager);
//        newGameObject.SendMessage("InitializeNetPlayer", m_netPlayer);
        // We're done. Destory ourselves
        Destroy(gameObject);
    }

    private NetPlayer m_netPlayer;
    private HFTPlayerNameManager m_playerNameManager;
}

}  // namespace HappyFunTimesExample

