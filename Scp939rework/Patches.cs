using HarmonyLib;
using PlayerRoles.PlayableScps.Scp939;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Scp939rework
{
    public static class Patches
    {

        [HarmonyPatch(typeof(Scp939AmnesticCloudAbility), nameof(Scp939AmnesticCloudAbility.ServerProcessCmd))]
        public static class NoCooldowns
        {
            public static bool Prefix(Scp939AmnesticCloudAbility __instance)
            {
                return true;
            }

            public static void Postfix(Scp939AmnesticCloudAbility __instance)
            {
                __instance.Cooldown.Clear();
            }
        }
    }
}
