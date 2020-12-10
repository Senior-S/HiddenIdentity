using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace HiddenIdentity
{
    public class HiddenIdentity : RocketPlugin<PluginConfiguration>
    {
        protected override void Load()
        {
            Logger.Log(" Plugin loaded correctly!");
            Logger.Log(" More plugins: www.dvtserver.xyz");
            Provider.onEnemyConnected += OnEnemyConnected;
            Provider.onEnemyDisconnected += OnEnemyDisconnected;
        }

        private void OnEnemyConnected(SteamPlayer player)
        {
            player.player.clothing.onMaskUpdated += (newMask, newMaskQuality, newMaskState) => OnMaskUpdated(player.player, newMask, newMaskQuality, newMaskState);
        }

        private void OnEnemyDisconnected(SteamPlayer player)
        {
            player.player.clothing.onMaskUpdated -= (newMask, newMaskQuality, newMaskState) => OnMaskUpdated(player.player, newMask, newMaskQuality, newMaskState);
        }

        private void OnMaskUpdated(Player player, ushort newMask, byte newMaskQuality, byte[] newMaskState)
        {
            var ids = Configuration.Instance.hidden_masks.Split(',');
            if (ids.Contains(newMask.ToString()))
            {
                List<Player> client = new List<Player>();
                PlayerTool.getPlayersInRadius(player.movement.move, 15f, client);
                for (int i = 0; i < client.Count; i++)
                {
                    if (client[i] == player) return;
                    Player victimPlayer = RaycastHelper.GetPlayer(client[i], 15);
                    if (victimPlayer == player)
                    {
                        var user = UnturnedPlayer.FromPlayer(client[i]);
                        if (user.HasPermission(Configuration.Instance.override_permission))
                        {
                            return;
                        }
                        client[i].setPluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy, false);
                        StartCoroutine(CheckPlayer(client[i], player));
                    }
                }
            }
        }

        public IEnumerator CheckPlayer(Player user, Player mask)
        {
            yield return new WaitForSeconds(3);
            if (user.look.player == mask)
            {
                StartCoroutine(CheckPlayer(user, mask));
            }
            else
            {
                user.setPluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy, true);
            }

        }
    }
}
