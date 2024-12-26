
//(c8

using UnityEngine;
using UnityEngine.UI;

namespace ElectricWire
{
    public class UISaveMenu : MonoBehaviour
    {
        public static UISaveMenu uiSaveMenu;

        public GameObject panel;
        public InputField saveNameInputField;

        private bool savedCursorActive = false;

        private void OnDisable()
        {
            // Return cursor to saved state
            if (savedCursorActive)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void Awake()
        {
            if (uiSaveMenu != null && uiSaveMenu != this) Destroy(gameObject); else uiSaveMenu = this;
        }

        private void Update()
        {
            // While open keep mouse cursor active
            if (panel.activeSelf)
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        public void SaveMenu(string fileName)
        {
            // Save cursor state
            if (Cursor.lockState == CursorLockMode.Locked)
                savedCursorActive = false;
            else
                savedCursorActive = true;

            saveNameInputField.text = fileName;

            panel.SetActive(true);

            // TODO : Select and edit input field
        }

        public void ClickAcceptSave()
        {
            ElectricManager.electricManager.WriteAllJsonData(saveNameInputField.text);

            // Closing the menu is like cancelling the save
            ClickCancelSave();
        }

        public void ClickCancelSave()
        {
            panel.SetActive(false);
        }
    }
}
