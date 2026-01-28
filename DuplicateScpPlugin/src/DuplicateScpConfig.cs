using System.Collections.Generic;
using PlayerRoles;

namespace DuplicateScpPlugin
{
    public sealed class DuplicateScpConfig
    {
        public bool IsEnabled { get; set; } = true;
        public int MinimumPlayerCount { get; set; } = 40;
        public int AdditionalPlayersPerDuplicate { get; set; } = 10;
        public float DelaySeconds { get; set; } = 2.0f;
        public int MaxDuplicatesPerRound { get; set; } = 3;
        public bool Debug { get; set; } = true;
        public bool AllowConvertGuards { get; set; } = true;
        public bool PreferFillAllScpsOnce { get; set; } = true;
        public List<RoleTypeId> UniqueScpPool { get; set; } = new()
        {
            RoleTypeId.Scp173,
            RoleTypeId.Scp106,
            RoleTypeId.Scp049,
            RoleTypeId.Scp096,
            RoleTypeId.Scp939,
            RoleTypeId.Scp3114,
        };
        public List<RoleTypeId> DuplicateScpPool { get; set; } = new()
        {
            RoleTypeId.Scp173,
            RoleTypeId.Scp106,
            RoleTypeId.Scp049,
            RoleTypeId.Scp096,
            RoleTypeId.Scp939,
            RoleTypeId.Scp3114,
        };
        public List<RoleTypeId> ExcludedScps { get; set; } = new()
        {
            RoleTypeId.Scp079,
        };
    }
}
