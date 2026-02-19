using UnityEngine;
using Antymology.Terrain;


namespace Antymology.Agents
{
    public class QueenAnt : AntBase
    {
       

        public float nestCostFraction = 0.33f;
        public NeuralNetwork brain;

        // Called by EvolutionManager after instantiation
        public void SetBrain(NeuralNetwork nn)
        {
            brain = nn;
        }

        protected override void Update()
        {
            base.Update(); // keeps health decay + mulch consumption + random worker movement if any
            if (brain == null) return;

            ActWithBrain();
        }

        void ActWithBrain()
        {
            float healthNorm = Mathf.Clamp01(health / maxHealth);
            float onMulch = IsStandingOnMulch_Public() ? 1f : 0f;
            float onAcid = IsStandingOnAcid_Public() ? 1f : 0f;
            float noise = Random.value * 2f - 1f;

            bool canPlace = CanPlaceNestHere();
            float canPlaceF = canPlace ? 1f : 0f;

            float ready = (health >= maxHealth * nestCostFraction) ? 1f : 0f;

            float[] inp = { healthNorm, onMulch, onAcid, noise, canPlaceF, ready };
            float[] outp = brain.Forward(inp);

            int a = ArgMax(outp);

            switch (a)
            {
                case 0: TryMoveDir(Vector3Int.forward); break;
                case 1: TryMoveDir(Vector3Int.back); break;
                case 2: TryMoveDir(Vector3Int.left); break;
                case 3: TryMoveDir(Vector3Int.right); break;
                case 4: TryDig(); break;
                case 5: TryPlaceNest(); break;
            }
        }
int ArgMax(float[] arr)
{
    if (arr == null || arr.Length == 0)
        return 0;

    int best = 0;
    float bestVal = arr[0];

    for (int i = 1; i < arr.Length; i++)
    {
        if (arr[i] > bestVal)
        {
            bestVal = arr[i];
            best = i;
        }
    }

    return best;
}


        void TryMoveDir(Vector3Int dir)
        {
            // Uses AntBase movement logic
            TryMoveTo_Public(gridPos + dir);
        }

        bool CanPlaceNestHere()
        {
            // Place nest only in AIR at current position
            var block = WorldManager.Instance.GetBlock(gridPos.x, gridPos.y, gridPos.z);
            return block is AirBlock;
        }

       void TryPlaceNest()
{
    if (health < maxHealth * nestCostFraction)
        return;

    int x = gridPos.x;
    int y = gridPos.y;
    int z = gridPos.z;

    // HARD world safety guard
    if (x <= 1 || y <= 1 || z <= 1)
        return;

    if (!CanPlaceNestHere())
        return;

    WorldManager.Instance.SetBlock(x, y, z, new NestBlock());
    WorldManager.Instance.NestCount += 1;

    health -= maxHealth * nestCostFraction;
}

    }
}
