
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class WheelGeneratorTrigger : MonoBehaviour
    {
        public bool isForward = false;

        private ElectricWheelGenerator electricWheelGenerator;

        private void Start()
        {
            electricWheelGenerator = GetComponentInParent<ElectricWheelGenerator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<AvatarControlRigidBody>() != null)
            {
                electricWheelGenerator.RotateRotator(isForward);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponentInParent<AvatarControlRigidBody>() != null)
            {
                electricWheelGenerator.RotateRotator(isForward);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            electricWheelGenerator.RotateRotator(false, true);
        }
    }
}
