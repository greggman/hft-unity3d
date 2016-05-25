using UnityEngine;
using System;
using System.Collections;

namespace HappyFunTimes {

    [ExecuteInEditMode]
    public class HFTReadmeFile : MonoBehaviour {

        public TextAsset file;
        public bool richText = true;
        public bool markdownish = true;

        #if UNITY_EDITOR
        void Awake() {
            ShowReadme();
        }

        void OnDestroy() {
            HFTReadmeUtils.CloseReadme(this);
        }

        public void ShowReadme(bool force = false)
        {
            if (file != null)
            {
                HFTReadmeUtils.ShowReadme(file.name, file.text, richText, markdownish, this, force);
            }
        }
        #endif
    }
}

