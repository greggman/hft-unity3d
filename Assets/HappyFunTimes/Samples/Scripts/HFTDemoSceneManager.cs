/*
 * Copyright 2014, Gregg Tavares.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 *
 *     * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above
 * copyright notice, this list of conditions and the following disclaimer
 * in the documentation and/or other materials provided with the
 * distribution.
 *     * Neither the name of Gregg Tavares. nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * This class is just a demo, used to load consequtive scenes
 */
public class HFTDemoSceneManager : MonoBehaviour
{
    public string[] scenePaths;
    private string m_currentSceneName = null;
    private int m_nextSceneNdx = 0;
    private AsyncOperation m_unloadOperation;
    private bool m_sceneJustLoaded = false;

    static private HFTDemoSceneManager s_instance = null;

    static public HFTDemoSceneManager GetInstance()
    {
      return s_instance;
    }

    void Awake()
    {
      if (s_instance)
      {
        throw new System.InvalidProgramException("there is more than one HFTDemoSceneManager component");
      }

      if (scenePaths.Length == 0)
      {
          throw new System.InvalidProgramException("No scenes are specified");
      }
      s_instance = this;
    }

    void Start()
    {
      LoadScene();
    }

    void Update()
    {
        if (m_sceneJustLoaded)
        {
          m_sceneJustLoaded = false;
          SceneManager.SetActiveScene(SceneManager.GetSceneByName(m_currentSceneName));
        }

        if (Input.GetKeyDown("tab"))
        {
          LoadScene();
        }
    }

    void LoadScene()
    {
      if (m_currentSceneName != null)
      {
        m_unloadOperation = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(m_currentSceneName));
      }

      if (m_unloadOperation != null)
      {
        if (!m_unloadOperation.isDone)
        {
          return;
        }
        m_unloadOperation = null;
      }

      m_currentSceneName = scenePaths[m_nextSceneNdx];
      m_nextSceneNdx = (m_nextSceneNdx + 1) % scenePaths.Length;
      SceneManager.LoadScene(m_currentSceneName, LoadSceneMode.Additive);
      m_sceneJustLoaded = true;
    }
}
