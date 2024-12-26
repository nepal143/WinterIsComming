
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class AdjustWind : MonoBehaviour
    {
        public void AdjustWindRotation(float value)
        {
            transform.rotation = Quaternion.Euler(0f, value, 0f);
        }
    }
}
