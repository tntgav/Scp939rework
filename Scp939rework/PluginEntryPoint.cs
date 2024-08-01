using HarmonyLib;
using MEC;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using RueI;
using RueI.Events;
using System;

namespace Scp939rework
{

    
    public class PluginEntry
    {

        internal static PluginEntry Instance { get; private set; }

        private static EventHandlers EventHandlers { get; set; }

        [PluginEntryPoint("does anyone", "1.0.0", "read these", "cat")]
        private void EntryPoint()
        {
            Instance = this;

            EventHandlers = new EventHandlers();
            EventManager.RegisterEvents(EventHandlers);
            //EventManager.RegisterEvents<mapeditorunborn>(this);
            RueIMain.EnsureInit();
            Harmony _harmony;
            _harmony = new Harmony("com.tpd.patches");
            _harmony.PatchAll();
        }

    }
}
