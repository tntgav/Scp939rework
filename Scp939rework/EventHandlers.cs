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
using UnityEngine;
using CustomPlayerEffects;
using System.Runtime.CompilerServices;
using MEC;

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

        [PluginEvent(ServerEventType.Scp939CreateAmnesticCloud)]
        public void HStoHP(Scp939CreateAmnesticCloudEvent ev)
        {
            Custom939 instance = ev.Player.Get939OrNew();
            float currenthume = ev.Player.GetStatModule<HumeShieldStat>().CurValue;
            ev.Player.GetStatModule<HumeShieldStat>().CurValue = 0;
            ev.Player.Health += currenthume / (200 / instance.MaxStealth);
            //starts off at 2:1 ratio and increases for more max stealth
        }

        [PluginEvent(ServerEventType.Scp939Lunge)]
        public void CustomLunge(Scp939LungeEvent ev)
        {
            if (ev.State == Scp939LungeState.Triggered)
            {
                Custom939 instance = ev.Player.Get939OrNew();
                float consumed = instance.Stealth;
                instance.ModStealth(instance.Stealth * -1); //consume all stealth
                instance.LungeData = consumed;
            }
            if (ev.State == Scp939LungeState.LandHit || ev.State == Scp939LungeState.LandRegular || ev.State == Scp939LungeState.LandHarsh)
            {
                Custom939 instance = ev.Player.Get939OrNew();
                float consumed = instance.LungeData;

                if (consumed > 25)
                {
                    Player.GetPlayers().Where(p => Vector3.Distance(p.Position, ev.Player.Position) < 5 && p != ev.Player).ToList().ForEach(p => { p.Effect<Slowness>(40, 5); });
                    // i know this is bad, but it just gives 40% slowness to all players within 5 meters
                }

                if (consumed > 50)
                {
                    Player.GetPlayers().Where(p => Vector3.Distance(p.Position, ev.Player.Position) < 10 && p != ev.Player).ToList().ForEach(p => { instance.Owner.AddAhp(30); });
                }

                if (consumed > 75)
                {
                    Player.GetPlayers().Where(p => Vector3.Distance(p.Position, ev.Player.Position) < 10).ToList().ForEach(p => { instance.Owner.Health += 25; });
                }

                if (consumed >= instance.MaxStealth)
                {
                    ev.Player.ReferenceHub.characterClassManager.GodMode = true;
                    ev.Player.Detonate(3);
                    Timing.CallDelayed(0.2f, () => { ev.Player.ReferenceHub.characterClassManager.GodMode = false; });
                }
            } 
        }
    }
}
