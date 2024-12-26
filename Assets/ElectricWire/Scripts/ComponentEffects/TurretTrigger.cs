
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class TurretTrigger : MonoBehaviour
    {
        private ElectricTurret electricTurret;

        private void Start()
        {
            electricTurret = GetComponentInParent<ElectricTurret>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<TurretTarget>() != null &&
                electricTurret.IsEnergized() && electricTurret.IsOn())
            {
                if (!electricTurret.targets.Contains(other.transform))
                    electricTurret.targets.Add(other.transform);
                if (!electricTurret.activated)
                    StartCoroutine(electricTurret.TurretLoop());
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<TurretTarget>() != null &&
                electricTurret.IsEnergized() && electricTurret.IsOn())
            {
                if (!electricTurret.targets.Contains(other.transform))
                    electricTurret.targets.Add(other.transform);
                if (!electricTurret.activated)
                    StartCoroutine(electricTurret.TurretLoop());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (electricTurret.targets.Contains(other.transform))
                electricTurret.targets.Remove(other.transform);
        }
    }
}
