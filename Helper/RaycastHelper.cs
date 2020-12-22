using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace HiddenIdentity
{
    public class RaycastHelper
    {
        public static Transform Raycast(IRocketPlayer rocketPlayer, float distance)
        {
            UnturnedPlayer player = (UnturnedPlayer)rocketPlayer;
            if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out RaycastHit hit, distance, RayMasks.PLAYER_INTERACT | RayMasks.PLAYER))
            {
                Transform transform = hit.transform;


                return transform;
            }
            return null;
        }
    }
}