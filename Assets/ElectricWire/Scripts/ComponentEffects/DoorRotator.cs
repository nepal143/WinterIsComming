
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class DoorRotator : MonoBehaviour
    {
        public void TurnOnOff(bool isOn)
        {
            if (isOn)
                transform.localRotation = new Quaternion(0f, -0.7071068f, 0f, 0.7071068f);
            else
                transform.localRotation = Quaternion.identity;
        }
    }
}
