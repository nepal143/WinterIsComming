
//(c8

using UnityEngine;

namespace ElectricWire
{
    public interface IWire
    {
        void ConnectWire(GameObject wire, bool isInput, int index = 0);

        void DisconnectWire(bool isInput, int index = 0);

        bool IsWireConnected(bool isInput, int index = 0);

        bool IsEnergized();

        bool IsOn();

        float IsGenerateEnergy(int index = 0);

        float IsDrainEnergy(int index = 0);

        void ActivateByWire(string data = "");

        void EnergizeByWire(bool onOff, int index = 0);
    }
}
