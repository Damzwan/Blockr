using UnityEngine;

namespace Blocks
{
    public interface IBlock
    {
        void onBlockSpawned(GameObject block);
    }
}