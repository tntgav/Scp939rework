using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginAPI.Events;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PlayerRoles.PlayableScps.Scp939;
using Mirror;
using PluginAPI.Core;
using HarmonyLib;
using PlayerStatsSystem;

namespace Scp939rework
{
    public class EventHandlers
    {

        [PluginEvent(ServerEventType.PlayerDamage)]
        public void PlayerDamage(PlayerDamageEvent ev)
        {
            if (ev.DamageHandler.GetType() == typeof(Scp939DamageHandler) && ev.Player != null)
            { //i think this works?
                if (ev.Player.Role == PlayerRoles.RoleTypeId.Scp939)
                {
                    Custom939 instance = ev.Player.Get939OrNew(); //Player = attacker not victim
                    Scp939DamageHandler handler = ev.DamageHandler as Scp939DamageHandler;
                    if (handler.Scp939DamageType == Scp939DamageType.Claw && instance.Stealth >= 30)
                    {
                        instance.ModStealth(-30);
                        handler.Damage *= 3f;
                    }
                }
            }
        }

        [PluginEvent(ServerEventType.PlayerChangeRole)]
        public void Scp939Check(PlayerChangeRoleEvent ev)
        {
            if (ev.NewRole == PlayerRoles.RoleTypeId.Scp939)
            {
                ev.Player.Get939OrNew(); // makes a new one if needed
            }
        }


        [PluginEvent(ServerEventType.PlayerDeath)]
        public void PlayerDeath(PlayerDeathEvent ev)
        {
            if (ev.DamageHandler.GetType() == typeof(Scp939DamageHandler) && ev.Attacker != null)
            {
                Custom939 instance = ev.Attacker.Get939OrNew();
                instance.ModMaxStealth(6); //+6 max stealth on kill
            }
        }

        [HarmonyPatch(typeof(Scp939LungeAbility), nameof(Scp939LungeAbility.OnKeyDown))]
        public static class lungemod
        {
            public static bool Prefix(Scp939LungeAbility __instance)
            {
                return true;
            }

            public static void Postfix(Scp939LungeAbility __instance)
            {
                Log.Info("detected?");
            }
        }
    }
}
