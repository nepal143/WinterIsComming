
//(c8

using UnityEngine;

namespace ElectricWire
{
    // The wire connector should go on the connector socket on the machine
    public class WireConnector : MonoBehaviour
    {
        // true is input
        [HideInInspector] public bool isInput;
        // index to connector 0, 1, 2, ...
        [HideInInspector] public int index = 0;

        public void SetupInit(bool newIsInput, int newIndex)
        {
            isInput = newIsInput;
            index = newIndex;
        }

        public void EnableGlow()
        {
            // To use Halo need to add Halo component directly on each connector
            //((Behaviour)GetComponent("Halo")).enabled = true;

            // TODO : Should cache material? Check if unity instantiate new each time we make it glow?
            GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

            CancelInvoke();
            Invoke(nameof(DisableGlow), 0.1f);
        }

        public void DisableGlow()
        {
            // To use Halo need to add Halo component directly on each connector
            //((Behaviour)GetComponent("Halo")).enabled = false;

            // TODO : Should cache material? Check if unity instantiate new each time we make it glow?
            GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }

        public void ConnectWire(GameObject wire)
        {
            IWire iwire = GetComponentInParent<IWire>();
            if (iwire != null)
                iwire.ConnectWire(wire, isInput, index);
        }

        public void DisconnectWire()
        {
            IWire iwire = GetComponentInParent<IWire>();
            if (iwire != null)
                iwire.DisconnectWire(isInput, index);
        }

        public bool IsWireConnected()
        {
            IWire iwire = GetComponentInParent<IWire>();
            if (iwire != null)
                return iwire.IsWireConnected(isInput, index);

            return false;
        }

        public bool IsEnergized()
        {
            IWire iwire = GetComponentInParent<IWire>();
            if (iwire != null)
                return iwire.IsEnergized();

            return false;
        }

        public bool IsOn()
        {
            IWire iwire = GetComponentInParent<IWire>();
            if (iwire != null)
                return iwire.IsOn();

            return false;
        }

        public float IsGenerateEnergy()
        {
            IWire iwire = GetComponentInParent<IWire>();
            if (iwire != null)
                return iwire.IsGenerateEnergy(index);

            return 0f;
        }

        public float IsDrainEnergy()
        {
            IWire iwire = GetComponentInParent<IWire>();
            if (iwire != null)
                return iwire.IsDrainEnergy(index);

            return 0f;
        }

        public void ActivateByWire(string data = "")
        {
            IWire iwire = GetComponentInParent<IWire>();
            if (iwire != null)
                iwire.ActivateByWire(data);
        }

        public void EnergizeByWire(bool isEnergized)
        {
            IWire iwire = GetComponentInParent<IWire>();
            if (iwire != null)
                iwire.EnergizeByWire(isEnergized, index);
        }
    }
}
