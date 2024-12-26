
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class AdjustWindOnTerrainGrass : MonoBehaviour
    {
        private Terrain terrain;

        private void Awake()
        {
            terrain = GetComponent<Terrain>();
        }

        public void AdjustWindGrass(float value)
        {
            terrain.terrainData.wavingGrassSpeed = value / 2;
            terrain.terrainData.wavingGrassStrength = value / 2;
        }
    }
}
