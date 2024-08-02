using PluginAPI.Core;
using RueI;
using RueI.Displays;
using RueI.Elements;
using RueI.Elements.Enums;
using RueI.Elements.Delegates;
using Display = RueI.Displays.Display;
using MEC;
using System;
using System.Linq;
using CustomPlayerEffects;
using UnityEngine;
using System.Collections.Generic;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items;
using Object = UnityEngine.Object;
using Footprinting;
using InventorySystem.Items.Pickups;
using Mirror;
using PlayerStatsSystem;
namespace Scp939rework
{
    public static class Extensions
    {
        public static void SendHint(this Player plr, int position, string content, float time)
        {
            ReferenceHub hub = plr.ReferenceHub;
            DisplayCore core = DisplayCore.Get(hub);
            IElemReference<SetElement> idk = DisplayCore.GetReference<SetElement>();
            SetElement element = core.GetElementOrNew(idk, () => new SetElement(position, content));
            Display disp = new Display(core);
            core.Update();

            Timing.CallDelayed(time, () =>
            {
                disp.Elements.Remove(element);
                element.Enabled = false;
                core.Update();
            });
        }

        public static float RoundToNearest(this float toRound, float nearest)
        {
            return (float)(nearest * Math.Round(toRound / nearest));
        }

        public static Custom939 Get939OrNew(this Player plr)
        {
            foreach (Custom939 instance in Custom939.Instances)
            {
                if (instance.Owner == plr) { return instance; }
            }
            return new Custom939(plr);
        }

        public static void Effect<T>(this Player plr, byte intensity, float duration) where T : StatusEffectBase
        {
            foreach (StatusEffectBase eff in plr.ReferenceHub.playerEffectsController.AllEffects)
            {
                if (eff.GetType() != typeof(T)) { continue; }
                eff.Intensity = intensity;
                eff.Duration = duration;
            }
        }

        public static void Effect<T>(this Player plr, bool enabled) where T : StatusEffectBase
        {
            foreach (StatusEffectBase eff in plr.ReferenceHub.playerEffectsController.AllEffects)
            {
                if (eff.GetType() != typeof(T)) { continue; }
                if (!enabled)
                {
                    eff.DisableEffect();
                    eff.Intensity = 0;
                    eff.Duration = 0;
                    return;
                }
                eff.Intensity = 1;
                eff.Duration = 0;
            }
        }

        public static Player NearestPlayer(this Player plr)
        {
            List<Player> allPlayers = Player.GetPlayers();
            List<Player> nearestPlayers = allPlayers
            .OrderBy(player => Vector3.Distance(player.Position, plr.Position))
            .ToList();

            float lowest = float.PositiveInfinity;
            Player nearest = null;
            if (nearestPlayers.Contains(plr)) { nearestPlayers.Remove(plr); }
            foreach (Player p in nearestPlayers) { if (Vector3.Distance(plr.Position, p.Position) < lowest) { nearest = p; lowest = Vector3.Distance(plr.Position, p.Position); } }

            return nearest;
        }

        public static void Detonate(this Player owner, int amount = 1) //player-only override
        {
            for (int i = 0; i < amount; i++)
            {
                ThrowableItem throwable = owner.ReferenceHub.inventory.CreateItemInstance(new ItemIdentifier(ItemType.GrenadeHE, ItemSerialGenerator.GenerateNext()), false) as ThrowableItem;

                TimeGrenade grenade = Object.Instantiate(throwable.Projectile, owner.Position, Quaternion.identity) as TimeGrenade;
                grenade.PreviousOwner = new Footprint(owner.ReferenceHub);
                grenade.NetworkInfo = new PickupSyncInfo(ItemType.GrenadeHE, throwable.Weight, throwable.ItemSerial);
                grenade._fuseTime = 0.05f;
                NetworkServer.Spawn(grenade.gameObject);
                grenade.ServerActivate();
            }
            
        }

        public static void AddAhp(this Player toahp, float amount)
        {
            toahp.GetStatModule<AhpStat>().ServerAddProcess(amount, float.MaxValue, 0, 1, 0, true);
        }
    }
}