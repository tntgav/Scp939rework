using CommandSystem;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scp939rework
{

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class debugdmg : ICommand
    {
        public string Command => "damageme";
        public string[] Aliases => new string[] { "dmgme" };
        public string Description => "damages you";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            List<string> args = arguments.ToList();

            Player p = Player.Get(sender);
            float damage = float.Parse(args[0]);

            p.Damage(damage, "test damage");
            response = "done";
            return true;
        }
    }
}
