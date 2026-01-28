using System;
using System.Collections.Generic;
using System.Linq;
using LabAPI.Features;
using LabAPI.Features.Wrappers;
using LabAPI.Events;
using PlayerRoles;

namespace DuplicateScpPlugin
{
    public sealed class DuplicateScpPlugin : Plugin<DuplicateScpConfig>
    {
        public override string Name => "Duplicate SCPs";
        public override string Description => "Spawns extra SCP duplicates at round start based on player count.";
        public override string Author => "YourName";
        public override Version Version => new(1, 0, 0);

        private readonly Random _random = new();

        public override void OnEnabled()
        {
            base.OnEnabled();
            EventManager.RoundStarted += OnRoundStarted;
        }

        public override void OnDisabled()
        {
            EventManager.RoundStarted -= OnRoundStarted;
            base.OnDisabled();
        }

        private void OnRoundStarted()
        {
            if (!Config.IsEnabled)
            {
                return;
            }

            List<Player> players = Player.GetPlayers().ToList();
            int totalPlayers = players.Count;

            if (totalPlayers < Config.MinimumPlayerCount)
            {
                return;
            }

            int duplicates = 1;
            if (Config.AdditionalPlayersPerDuplicate > 0)
            {
                duplicates += (totalPlayers - Config.MinimumPlayerCount) / Config.AdditionalPlayersPerDuplicate;
            }

            List<RoleTypeId> eligibleScps = players
                .Where(player => player.Team == Team.SCPs)
                .Select(player => player.Role)
                .Distinct()
                .Where(role => !Config.ExcludedScps.Contains(role))
                .ToList();

            if (eligibleScps.Count == 0)
            {
                return;
            }

            List<Player> eligibleTargets = players
                .Where(player => player.Team != Team.SCPs)
                .ToList();

            for (int i = 0; i < duplicates && eligibleTargets.Count > 0; i++)
            {
                RoleTypeId chosenScp = eligibleScps[_random.Next(eligibleScps.Count)];
                Player chosenTarget = eligibleTargets[_random.Next(eligibleTargets.Count)];
                eligibleTargets.Remove(chosenTarget);

                chosenTarget.SetRole(chosenScp, RoleChangeReason.RoundStart);
            }
        }
    }
}
