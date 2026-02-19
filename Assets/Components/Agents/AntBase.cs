using UnityEngine;
using Antymology.Terrain;

namespace Antymology.Agents
{
    public class AntBase : MonoBehaviour
    {
        [Header("Health")]
        public float maxHealth = 30f;
        public float health = 30f;
        public float decayPerSecond = 1f;

        [Header("Position")]
        public Vector3Int gridPos;

    void Start()
{
    health = maxHealth;

    int x = Mathf.RoundToInt(transform.position.x);
    int z = Mathf.RoundToInt(transform.position.z);

    int y = GetSurfaceHeight(x, z);

    gridPos = new Vector3Int(x, y, z);

    SyncTransform();
}



       protected virtual void Update()

        {
            // === HEALTH DECAY (REQUIRED) ===
            float decay = decayPerSecond * Time.deltaTime;

            if (IsStandingOnAcid())
                decay *= 2f; // Acid doubles decay (required)

            health -= decay;

            if (health <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            // === MULCH CONSUMPTION ===
            TryConsumeMulch();

            // === TEMP RANDOM MOVEMENT (until we add neural net) ===
            if (Random.value < 0.05f)
            {
                TryRandomMove();
            }
        }

        // ===========================
        // MOVEMENT
        // ===========================

        void TryRandomMove()
        {
            Vector3Int[] dirs = new[]
            {
                Vector3Int.forward,
                Vector3Int.back,
                Vector3Int.left,
                Vector3Int.right
            };

            Vector3Int target = gridPos + dirs[Random.Range(0, dirs.Length)];
            TryMoveTo(target);
        }

        void TryMoveTo(Vector3Int target)

        {
            if (target.x <= 1 || target.z <= 1)
    return;

            int currentHeight = GetSurfaceHeight(gridPos.x, gridPos.z);
            int targetHeight = GetSurfaceHeight(target.x, target.z);

            // Required rule: height difference <= 2
            if (Mathf.Abs(targetHeight - currentHeight) > 2)
                return;

            gridPos = new Vector3Int(target.x, targetHeight, target.z);
            SyncTransform();
        }

        void SyncTransform()
        {
            transform.position = new Vector3(
                gridPos.x,
                gridPos.y,
                gridPos.z
            );
        }

        // ===========================
        // BLOCK INTERACTION
        // ===========================

        bool IsStandingOnAcid()
        {
            var block = WorldManager.Instance.GetBlock(
                gridPos.x,
                gridPos.y,
                gridPos.z
            );

            return block is AcidicBlock;
        }

        bool IsStandingOnMulch()
        {
            var block = WorldManager.Instance.GetBlock(
                gridPos.x,
                gridPos.y,
                gridPos.z
            );

            return block is MulchBlock;
        }
void TryConsumeMulch()
{
    if (!IsStandingOnMulch())
        return;

    int x = gridPos.x;
    int y = gridPos.y;
    int z = gridPos.z;

    int worldHeight = WorldManager.Instance
        .GetType()
        .GetField("Blocks",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance)
        .GetValue(WorldManager.Instance) is AbstractBlock[,,] blocks
        ? blocks.GetLength(1)
        : 0;

    if (y < 1 || y >= worldHeight - 1)
        return;

    WorldManager.Instance.SetBlock(x, y, z, new AirBlock());

    health = Mathf.Min(maxHealth, health + maxHealth * 0.6f);
}



        public void TryDig()
        {
            var block = WorldManager.Instance.GetBlock(
                gridPos.x,
                gridPos.y,
                gridPos.z
            );

            // Required rule: cannot dig container block
            if (block is ContainerBlock)
                return;

            WorldManager.Instance.SetBlock(
                gridPos.x,
                gridPos.y,
                gridPos.z,
                new AirBlock()
            );

            // Move down after digging
            gridPos += Vector3Int.down;
            SyncTransform();
        }


        // ===========================
        // HELPERS
        // ===========================
int GetSurfaceHeight(int x, int z)
{
    int maxY =  WorldManager.Instance
        .GetType()
        .GetField("Blocks", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        .GetValue(WorldManager.Instance) is AbstractBlock[,,] blocks
        ? blocks.GetLength(1) - 1
        : 200;

    for (int y = maxY; y >= 1; y--)
    {
        var block = WorldManager.Instance.GetBlock(x, y, z);

        if (!(block is AirBlock))
            return y;
    }

    return 1;
}


        Vector3Int FindSurfacePosition(int x, int z)
        {
            int y = GetSurfaceHeight(x, z);
            return new Vector3Int(x, y, z);
        }
        
protected bool IsStandingOnMulch_Public() => IsStandingOnMulch();
protected bool IsStandingOnAcid_Public() => IsStandingOnAcid();
protected void TryMoveTo_Public(Vector3Int target) => TryMoveTo(target);


    }


}
