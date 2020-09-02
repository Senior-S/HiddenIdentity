using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Core.Utils;
using SDG.Unturned;

namespace HiddenIdentity
{
    public class HiddenIdentity : RocketPlugin<PluginConfiguration>
    {
        protected override void Load()
        {
            Logger.Log("[HiddenIdentity] Plugin loaded correctly!");
            Logger.Log("If you have any error you can contact the owner in discord: Senior S#9583");
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
                        client[i].setPluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy, false);
                        TaskDispatcher.QueueOnMainThread(CheckPlayer(client[i], player));
                    }
                }
            }
        }

        public System.Action CheckPlayer(Player user, Player mask)
        {
            Task.Delay(TimeSpan.FromSeconds(3));
            if (user.look.player == mask)
            {
                TaskDispatcher.QueueOnMainThread(CheckPlayer(user, mask));
                return null;
            }
            else
            {
                user.setPluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy, true);
                return null;
            }
        }
    }
}
