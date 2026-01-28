using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            Log("Plugin enabled.");
        }

        public override void OnDisabled()
        {
            EventManager.RoundStarted -= OnRoundStarted;
            Log("Plugin disabled.");
            base.OnDisabled();
        }

        private async void OnRoundStarted()
        {
            if (!Config.IsEnabled)
            {
                return;
            }

            if (Config.DelaySeconds > 0f)
            {
                await Task.Delay(TimeSpan.FromSeconds(Config.DelaySeconds));
            }

            List<Player> players = Player.GetPlayers().ToList();
            int totalPlayers = players.Count;
            Log($"Players: {totalPlayers}");

            if (totalPlayers < Config.MinimumPlayerCount)
            {
                return;
            }

            int duplicates = 1;
            if (Config.AdditionalPlayersPerDuplicate > 0)
            {
                duplicates += (totalPlayers - Config.MinimumPlayerCount) / Config.AdditionalPlayersPerDuplicate;
            }

            duplicates = Math.Max(0, Math.Min(duplicates, Config.MaxDuplicatesPerRound));
            Log($"Calculated duplicates: {duplicates}");

            HashSet<RoleTypeId> existingScps = players
                .Where(player => player.Team == Team.SCPs)
                .Select(player => player.Role)
                .ToHashSet();

            Log($"Current SCP count: {existingScps.Count}");

            List<RoleTypeId> eligibleExistingScps = existingScps
                .Where(role => !Config.ExcludedScps.Contains(role))
                .ToList();

            Log($"Eligible SCPs: {eligibleExistingScps.Count}");

            List<Player> eligibleTargets = players
                .Where(player => player.Team != Team.SCPs)
                .Where(player => player.Role != RoleTypeId.Spectator)
                .Where(player => player.Role != RoleTypeId.Tutorial)
                .Where(player => Config.AllowConvertGuards || player.Role != RoleTypeId.FacilityGuard)
                .ToList();

            Log($"Eligible targets: {eligibleTargets.Count}");

            List<RoleTypeId> missingUnique = new();
            List<RoleTypeId> eligibleDuplicatePool = new();

            if (Config.PreferFillAllScpsOnce)
            {
                missingUnique = Config.UniqueScpPool
                    .Where(role => !existingScps.Contains(role))
                    .Where(role => !Config.ExcludedScps.Contains(role))
                    .ToList();

                eligibleDuplicatePool = Config.DuplicateScpPool
                    .Where(role => !Config.ExcludedScps.Contains(role))
                    .ToList();

                string missingUniqueSummary = missingUnique.Count > 0
                    ? string.Join(", ", missingUnique)
                    : "None";
                Log($"Missing unique SCPs: {missingUniqueSummary}");
                Log($"Eligible duplicate pool: {eligibleDuplicatePool.Count}");
            }

            for (int i = 0; i < duplicates && eligibleTargets.Count > 0; i++)
            {
                RoleTypeId chosenScp;

                if (Config.PreferFillAllScpsOnce)
                {
                    if (missingUnique.Count > 0)
                    {
                        chosenScp = missingUnique[0];
                        missingUnique.RemoveAt(0);
                    }
                    else if (eligibleDuplicatePool.Count > 0)
                    {
                        chosenScp = eligibleDuplicatePool[_random.Next(eligibleDuplicatePool.Count)];
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (eligibleExistingScps.Count == 0)
                    {
                        return;
                    }

                    chosenScp = eligibleExistingScps[_random.Next(eligibleExistingScps.Count)];
                }

                Player chosenTarget = eligibleTargets[_random.Next(eligibleTargets.Count)];
                eligibleTargets.Remove(chosenTarget);

                TrySetRole(chosenTarget, chosenScp);
            }
        }

        private void TrySetRole(Player target, RoleTypeId role)
        {
            try
            {
                target.SetRole(role, RoleChangeReason.RoundStart);
                Log($"Converted {target.Nickname} to {role}.");
            }
            catch (Exception ex)
            {
                Log($"Failed to convert {target.Nickname} to {role}. Exception: {ex}");
            }
        }

        private void Log(string message)
        {
            if (!Config.Debug)
            {
                return;
            }

            Logger.Info($"[DuplicateScp] {message}");
        }
    }
}
