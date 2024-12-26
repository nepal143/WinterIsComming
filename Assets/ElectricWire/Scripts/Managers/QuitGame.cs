
//(c8

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ElectricWire
{
    public class QuitGame : MonoBehaviour
    {
        public void ClickQuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
