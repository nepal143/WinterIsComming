
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class SwitchRotator : MonoBehaviour
    {
        public void TurnOnOff(bool isOn)
        {
            if (isOn)
                transform.localRotation = new Quaternion(0.3826834f, 0f, 0f, 0.9238796f);
            else
                transform.localRotation = new Quaternion(0.9238796f, 0f, 0f, 0.3826834f);
        }
    }
}
