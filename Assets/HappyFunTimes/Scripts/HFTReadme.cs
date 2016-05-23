using UnityEngine;
using System;
using System.Collections;

namespace HappyFunTimes {

    [ExecuteInEditMode]
    public class HFTReadme : MonoBehaviour {

        [Multiline(20)]
        public string message = "";
        public bool richText = false;

        #if UNITY_EDITOR
        void Awake() {
            ShowReadme();
        }

        void OnDestroy() {
            HFTReadmeUtils.CloseReadme(this);
        }

        public void ShowReadme(bool force = false)
        {
            if (message != null)
            {
                HFTReadmeUtils.ShowReadme("**README**", message, richText, this, force);
            }
        }
        #endif
    }
}

