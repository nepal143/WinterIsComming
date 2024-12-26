
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class ElectricPanelButton : MonoBehaviour
    {
        public void OnClickTargetInfoButtonUI()
        {
            ElectricManager.electricManager.OnClickTargetInfoButtonUI();
        }

        public void OnClickRemoveButtonUI()
        {
            ElectricManager.electricManager.OnClickRemoveButtonUI();
        }

        public void OnClickWireButtonUI(GameObject objectPrefab)
        {
            ElectricManager.electricManager.OnClickWireButtonUI(objectPrefab);
        }

        public void OnClickComponentButtonUI(GameObject objectPrefab)
        {
            ElectricManager.electricManager.OnClickComponentButtonUI(objectPrefab);
        }
    }
}
