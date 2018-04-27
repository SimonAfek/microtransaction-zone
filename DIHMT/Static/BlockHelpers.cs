using System;
using DIHMT.Models;

namespace DIHMT.Static
{
    public class BlockHelpers
    {
        public static BlockType GetBlockStatus(string ip)
        {
            var rawVal = DbAccess.IsBlocked(ip);

            if (rawVal == null || rawVal.Expiration < DateTime.UtcNow)
            {
                return BlockType.Unblocked;
            }

            return rawVal.Explicit ? BlockType.ExplicitBlocked : BlockType.HiddenBlocked;
        }

        public static void BlockIp(BlockModel input)
        {
            var dbModel = new BlockList
            {
                BlockedIp = input.BlockedIp,
                Explicit = input.Explicit,
                Reason = input.Reason,
                Expiration = input.BlockType.ToLower() == "timeout" ? DateTime.UtcNow.AddDays(1).Date : DateTime.MaxValue
            };

            DbAccess.BlockIp(dbModel);
        }
    }

    public enum BlockType
    {
        Unblocked = 1,
        HiddenBlocked = 2,
        ExplicitBlocked = 3
    }
}