
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class AdjustSun : MonoBehaviour
    {
        public void AdjustSunRotation(float value)
        {
            // TODO : At value 100 .. turn off light intensity

            transform.rotation = Quaternion.Euler(value, 0f, 0f);

            transform.GetChild(0).GetComponent<Light>().enabled = value < 90 && value > -90;
        }
    }
}
