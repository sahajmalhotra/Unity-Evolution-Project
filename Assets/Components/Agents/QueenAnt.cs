using UnityEngine;
using Antymology.Terrain;

namespace Antymology.Agents
{
    public class QueenAnt : AntBase
    {
        public float nestCostFraction = 0.33f;

        void Update()
        {
            base.Update();

            TryPlaceNest();
        }

        void TryPlaceNest()
        {
            if (health < maxHealth * nestCostFraction)
                return;

            // Only place nest if standing on air
            var block = WorldManager.Instance.GetBlock(
                gridPos.x,
                gridPos.y,
                gridPos.z
            );

            if (block is AirBlock)
            {
                WorldManager.Instance.SetBlock(
                    gridPos.x,
                    gridPos.y,
                    gridPos.z,
                    new GrassBlock()   // TEMP nest placeholder
                );

                health -= maxHealth * nestCostFraction;
            }
        }
    }
}
