
//(c8

using System;
using UnityEngine;
using UnityEngine.UI;

namespace ElectricWire
{
    [Serializable]
    public class ElectricCounterJsonData
    {
        public int counter;

        public ElectricCounterJsonData(int newCounter)
        {
            counter = newCounter;
        }
    }

    public class ElectricCounter : ElectricComponent, ISaveJsonData
    {
        // Display
        public Text displayText;

        public int _counter = 0;

        public int Counter
        {
            get { return _counter; }
            set
            {
                _counter = value;
                displayText.text = _counter.ToString();
            }
        }

        #region ISaveJsonData interface

        public string GetJsonData()
        {
            string jsonData = JsonUtility.ToJson(new ElectricCounterJsonData(Counter));
            return jsonData;
        }

        public void SetupFromJsonData(string jsonData)
        {
            ElectricCounterJsonData electricCounterJsonData = JsonUtility.FromJson<ElectricCounterJsonData>(jsonData);
            if (electricCounterJsonData == null)
            {
                Debug.LogWarning("No json data found for: " + name + ". Resave could fix that.");
                return;
            }

            Counter = electricCounterJsonData.counter;
        }

        #endregion

        #region IWire interface

        public override bool IsOn()
        {
            // Counter do not have on/off state
            // When energized is on
            return GetSetIsEnergized;
        }

        public override void EnergizeByWire(bool onOff, int index)
        {
            if (index == 0)
            {
                // Toggle increase
                if (onOff)
                {
                    Counter++;
                    if (Counter > 99)
                        Counter = 99;
                }
            }
            else if (index == 1)
            {
                // Toggle decrease
                if (onOff)
                {
                    Counter--;
                    if (Counter < -99)
                        Counter = -99;
                }
            }
            else if (index == 2)
            {
                // Toggle reset
                if (onOff)
                    Counter = 0;
            }
        }

        #endregion
    }
}
