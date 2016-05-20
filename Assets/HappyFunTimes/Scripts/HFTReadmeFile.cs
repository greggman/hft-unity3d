using UnityEngine;
using System;
using System.Collections;

namespace HappyFunTimes {

    [ExecuteInEditMode]
    public class HFTReadmeFile : MonoBehaviour {

        public TextAsset file;
        public bool richText = false;

        #if UNITY_EDITOR
        void Awake() {
            ShowReadme();
        }

        void OnDestroy() {
            HFTReadmeUtils.CloseReadme(this);
        }

        public void ShowReadme()
        {
            if (file != null)
            {
                HFTReadmeUtils.ShowReadme(file.name, file.text, richText, this);
            }
        }
        #endif
    }
}

