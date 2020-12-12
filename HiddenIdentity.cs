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
            StartCoroutine(CheckPlayer());
        }

        public IEnumerator CheckPlayer()
        {
            yield return new WaitForFixedUpdate();
            List<string> ids = Configuration.Instance.hidden_masks.Split(',').ToList();
            foreach (var client in Provider.clients)
            {
                Player user = client.player;
                Player victimPlayer = RaycastHelper.GetPlayer(user, 15);
                if (ids.Contains(victimPlayer.clothing.mask.ToString()))
                {
                    user.disablePluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy);
                }
                else
                {
                    user.enablePluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy);
                }
            }
        }
    }
}
