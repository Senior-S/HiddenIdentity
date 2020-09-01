using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Plugins;
using OpenMod.API.Plugins;
using SDG.Unturned;
using OpenMod.API.Permissions;
using System.Threading.Tasks;
using OpenMod.Core.Helpers;
using System.Linq;
using System.Collections.Generic;

[assembly: PluginMetadata("SS.HiddenIdentity", Author = "Senior S", DisplayName = "HiddenIdentity")]
namespace HiddenIdentity
{
    public class HiddenIdentity : OpenModUnturnedPlugin
    {
        private readonly ILogger<HiddenIdentity> ro_Logger;
        private readonly IConfiguration ro_configuration;
        private readonly IPermissionRegistry ro_permissionRegistry;

        public HiddenIdentity(
            ILogger<HiddenIdentity> logger,
            IConfiguration configuration,
            IPermissionRegistry permissionRegistry,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            ro_Logger = logger;
            ro_configuration = configuration;
            ro_permissionRegistry = permissionRegistry;
        }

        protected override async UniTask OnLoadAsync()
        {
			await UniTask.SwitchToMainThread();
            Provider.onEnemyConnected += OnEnemyConnected;
            Provider.onEnemyDisconnected += OnEnemyDisconnected;
            ro_permissionRegistry.RegisterPermission(this, ro_configuration.GetSection("plugin_configuration:override_permission").Get<string>());
            ro_Logger.LogInformation("Plugin loaded correctly!");
            ro_Logger.LogInformation("If you have any error you can contact the owner in discord: Senior S#9583");
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
            var ids = ro_configuration.GetSection("plugin_configuration:hidden_masks").Get<string>().Split(',');
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
                        AsyncHelper.Schedule("CheckPlayer", () => CheckPlayer(client[i], player));
                    }
                }
            }
        }

        public async Task CheckPlayer(Player user, Player mask)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            if (user.look.player == mask)
            {
                AsyncHelper.Schedule("CheckPlayer", () => CheckPlayer(user, mask));
            }
            else
            {
                user.setPluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy, true);
            }
        }

        protected override async UniTask OnUnloadAsync()
        {
            await UniTask.SwitchToMainThread();
            Provider.onEnemyConnected -= OnEnemyConnected;
            Provider.onEnemyDisconnected -= OnEnemyDisconnected;
            ro_Logger.LogInformation("Plugin unloaded correctly!");
        }
    }
}
