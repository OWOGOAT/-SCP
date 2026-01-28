using System.Collections.Generic;
using PlayerRoles;

namespace DuplicateScpPlugin
{
    public sealed class DuplicateScpConfig
    {
        public bool IsEnabled { get; set; } = true;
        public int MinimumPlayerCount { get; set; } = 40;
        public int AdditionalPlayersPerDuplicate { get; set; } = 10;
        public List<RoleTypeId> ExcludedScps { get; set; } = new()
        {
            RoleTypeId.Scp079,
        };
    }
}
