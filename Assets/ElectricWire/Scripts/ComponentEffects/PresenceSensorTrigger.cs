
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class PresenceSensorTrigger : MonoBehaviour
    {
        private ElectricPresenceSensor electricPresenceSensor;

        private void Start()
        {
            electricPresenceSensor = GetComponentInParent<ElectricPresenceSensor>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<AvatarControlRigidBody>() != null &&
                electricPresenceSensor.IsEnergized())
            {
                electricPresenceSensor.TryTurnOnOff(true);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponentInParent<AvatarControlRigidBody>() != null &&
                electricPresenceSensor.IsEnergized())
            {
                electricPresenceSensor.TryTurnOnOff(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            electricPresenceSensor.TryTurnOnOff(false);
        }
    }
}
