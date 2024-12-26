
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class UIButtons : MonoBehaviour
    {
        public void ClickSaveButton()
        {
            ElectricManager.electricManager.SaveAllJsonData();
        }

        public void ClickLoadButton()
        {
            ElectricManager.electricManager.LoadAllJsonData();
        }

        public void ClickClearButton()
        {
            ElectricManager.electricManager.ClearAllJsonData();
        }
    }
}
