using System;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using SDG.Unturned;

namespace HiddenIdentity
{
    public class PluginConfiguration : IRocketPluginConfiguration
    {
        public string hidden_masks;
        public string override_permission;
        public void LoadDefaults()
        {
            hidden_masks = "1048,1270,434,441";
            override_permission = "ss.overridepermission";
        }
    }
}
