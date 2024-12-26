
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class ElectricPresenceSensor : ElectricComponent
    {
        public void TryTurnOnOff(bool newIsOn)
        {
            if (newIsOn)
            {
                if (!IsOn())
                    TurningOnOff(newIsOn);
            }
            else if (!newIsOn)
            {
                if (IsOn())
                    TurningOnOff(newIsOn);
            }
        }

        public void TurningOnOff(bool newIsOn)
        {
            GetSetIsOn = newIsOn;
            ActivateOutput();
        }

        #region IWire interface

        public override void ConnectWire(GameObject wire, bool isInput, int index)
        {
            base.ConnectWire(wire, isInput, index);

            ActivateOutput();
        }

        public override void DisconnectWire(bool isInput, int index)
        {
            base.DisconnectWire(isInput, index);

            if (isInput)
            {
                GetSetIsEnergized = false;
                ActivateOutput();
            }
        }

        public override void EnergizeByWire(bool onOff, int index)
        {
            base.EnergizeByWire(onOff, index);

            ActivateOutput();
        }

        #endregion
    }
}
