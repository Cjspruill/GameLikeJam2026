using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// Is this a Box Knife object?
    /// </summary>
    /// <param name="obj">The Collider</param>
    /// <returns>True or false</returns>
    public static bool IsBoxKnife(this Collider obj) => obj.CompareTag(Globals.BOX_KNIFE_TAG);

    /// <summary>
    /// Is this a Loot object?
    /// </summary>
    /// <param name="obj">The GameObject</param>
    /// <returns>True or false</returns>
    public static bool IsLoot(this GameObject obj) => obj.CompareTag(Globals.LOOT_BLOCK_TAG);

    /// <summary>
    /// Is this a Player object?
    /// </summary>
    /// <param name="obj">The Collider</param>
    /// <returns>True or false</returns>
    public static bool IsPlayer(this Collider obj) => obj.CompareTag(Globals.PLAYER_TAG);

    /// <summary>
    /// Is this a Punch object?
    /// </summary>
    /// <param name="obj">The Collider</param>
    /// <returns>True or false</returns>
    public static bool IsPunch(this Collider obj) => obj.CompareTag(Globals.PUNCH_TAG);

    /// <summary>
    /// Is this a Scrap object?
    /// </summary>
    /// <param name="obj">The Collider</param>
    /// <returns>True or false</returns>
    public static bool IsScrap(this Collider obj) => obj.CompareTag(Globals.SCRAP_BLOCK_TAG);

    /// <summary>
    /// IS this a Scrap object?
    /// </summary>
    /// <param name="obj">The GameObject</param>
    /// <returns>True or false</returns>
    public static bool IsScrap(this GameObject obj) => obj.CompareTag(Globals.SCRAP_BLOCK_TAG);

    /// <summary>
    /// Is this an Untagged object?
    /// </summary>
    /// <param name="obj">The Collider</param>
    /// <returns>True or false</returns>
    public static bool IsUntagged(this Collider obj) => obj.CompareTag(Globals.NO_TAG);

    /// <summary>
    /// Can this object be destroyed with BoxKnife only?
    /// </summary>
    /// <param name="obj">The Block</param>
    /// <returns>True or false</returns>
    public static bool CanBoxKnifeDestroy(this Block obj) => obj.blockDestructionType == Globals.BlockDestructionType.BoxKnife;

    /// <summary>
    /// Can this object be destroyed with Punch only?
    /// </summary>
    /// <param name="obj">The Block</param>
    /// <returns>True or false</returns>
    public static bool CanPunchDestroy(this Block obj) => obj.blockDestructionType == Globals.BlockDestructionType.Punch;

    /// <summary>
    /// Can this object be destroyed with Punch and BoxKnife?
    /// </summary>
    /// <param name="obj">The Block</param>
    /// <returns>True or false</returns>
    public static bool CanPunchAndBoxKnifeDestroy(this Block obj) => obj.blockDestructionType == Globals.BlockDestructionType.PunchAndBoxKnife;
}
