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
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            m_soundPlayer.PlaySound("explosion");
            yield return new WaitForSeconds(1.0f);
            m_soundPlayer.PlaySound("gameover");
            yield return new WaitForSeconds(1.0f);
            m_soundPlayer.PlaySound("launch");
        }
    }

    private HFTSoundPlayer m_soundPlayer;
}
