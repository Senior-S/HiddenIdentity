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
        internal bool Active = false;

        protected override void Load()
        {
            Logger.Log(" Plugin loaded correctly!");
            Logger.Log(" More plugins: www.dvtserver.xyz");
            Active = true;
            StartCoroutine(CheckPlayer());
        }

        public IEnumerator CheckPlayer()
        {
            while (Active)
            {
                yield return new WaitForFixedUpdate();
                List<string> ids = Configuration.Instance.hidden_masks.Split(',').ToList();
                foreach (var client in Provider.clients)
                {
                    Transform trans = RaycastHelper.Raycast(UnturnedPlayer.FromSteamPlayer(client), 5f);
                    if (trans != null)
                    {
                        Player mas = trans.GetComponent<Player>();
                        if (ids.Contains(mas.clothing.mask.ToString()))
                        {
                            if (!UnturnedPlayer.FromSteamPlayer(client).HasPermission(Configuration.Instance.override_permission))
                            {
                                client.player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy);
                            }
                            else
                            {
                                client.player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy);
                            }
                        }
                    }
                }
            }
        }

        protected override void Unload()
        {
            Logger.Log(" Plugin unloaded correctly!");
            Logger.Log(" More plugins: www.dvtserver.xyz");
            Active = false;
            StopCoroutine(CheckPlayer());
        }
    }
}
