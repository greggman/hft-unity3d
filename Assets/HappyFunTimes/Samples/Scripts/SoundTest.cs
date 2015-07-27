using UnityEngine;
using System.Collections;

public class SoundTest : MonoBehaviour {

    void Awake ()
    {
        m_soundPlayer = GetComponent<HFTSoundPlayer>();
    }

    void Start()
    {
        StartCoroutine("Play");
    }

    IEnumerator Play()
    {
        HFTSounds.Sounds sounds = HFTGlobalSoundHelper.GetSounds();
        string[] soundNames = new string[sounds.Keys.Count];
        sounds.Keys.CopyTo(soundNames, 0);

        int index = 0;
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            m_soundPlayer.PlaySound(soundNames[index]);
            index = (index + 1) % soundNames.Length;
        }
    }

    private HFTSoundPlayer m_soundPlayer;
}
