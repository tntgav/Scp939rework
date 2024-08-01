using CustomPlayerEffects;
using MEC;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Scp939rework
{
    public class Custom939
    {
        public static List<Custom939> Instances = new List<Custom939>();

        public float Stealth { get { return _stealth; } set { _stealth = value; } }

        private float _stealth;
        public float MaxStealth { get { return _maxstealth; } }

        private float _maxstealth;
        public Player Owner { get; }

        public Custom939(Player owner)
        {
            _stealth = 100f;
            _maxstealth = 100f;
            Owner = owner;

            Instances.Add(this);

            float Infinite = float.MaxValue / 2; // will end after a few octillion years but thats ok
            Timing.CallContinuously(Infinite, Update);
            Timing.CallPeriodically(Infinite, 0.5f, HalfSecondCoroutine);
        }

        public void ModMaxStealth(float change)
        {
            _maxstealth += change;
        }

        public void ModStealth(float change)
        {
            _stealth = Mathf.Clamp(_stealth + change, 0, _maxstealth);
        }

        public void Update()
        {
            if (!Instances.Contains(this)) return;
            if (Owner.Role != PlayerRoles.RoleTypeId.Scp939)
            {
                Instances.Remove(this); //cuts it off from the main loop, still exists tho, thanks c# :3
                Log.Info($"cutoff {Owner.Nickname} from normal coroutines");
            }

            float change = 0;
            foreach (Player p in Player.GetPlayers())
            {
                if (p == Owner) continue;
                if (Vector3.Distance(Owner.Position, p.Position) < 7.5f)
                {
                    change -= 3;
                }
            }
            if (change == 0) _stealth = Mathf.Clamp(_stealth + ( Time.deltaTime * ( _maxstealth * (0.06f) ) ), 0, _maxstealth);
            _stealth = Mathf.Clamp(_stealth + (change * Time.deltaTime), 0, _maxstealth);

            if (_stealth == _maxstealth) { Owner.Effect<Invisible>(true); } else { Owner.Effect<Invisible>(false); }
            if (_stealth > _maxstealth * 0.5f) { Owner.Effect<MovementBoost>(50, 0); } else { Owner.Effect<MovementBoost>(false); }
        }

        public void HalfSecondCoroutine()
        {
            if (!Instances.Contains(this)) return;
            //Log.Info($"updating text of {Owner.Nickname}");
            float progress = _stealth / _maxstealth; //0-1
            float lower = progress;
            float upper = 1 - lower;
            char c = '█';
            //char c = ':';

            


            string bar = $"<b>[<color=#ccccff><scale={lower * 9}>{c}</scale></color><color=#cccccc><scale={upper * 9}>{c}</scale></color>]</b>";
            string BuiltText = $"<b>{_stealth.RoundToNearest(0.5f)} / {MaxStealth.RoundToNearest(0.5f)} Stealth</b><br>{bar}";
            BuiltText += $"<br><size=20><align=left>+Passive: Invisibility {(_stealth == _maxstealth ? "<color=#00ff00>(Active)</color>" : "<color=#ff0000>(Inactive)</color>")}<br></size><align=left><size=15>Become invisible while at 100% stealth and not near any players.</size>";
            BuiltText += $"<br><br><size=20><align=left>+Passive: Speed {(_stealth > _maxstealth * 0.5f ? "<color=#00ff00>(Active)</color>" : "<color=#ff0000>(Inactive)</color>")}<br></size><align=left><size=15>Move notably faster while above 50% stealth</size>";
            BuiltText += $"<br><br><size=20><align=left>+Active: Sneak attack {(_stealth >= 30 ? "<color=#00ff00>(Ready)</color>" : "<color=#ff0000>(Insufficient Stealth)</color>")}<br></size><align=left><size=15>Claw attacks deal heavy damage, but consume 30 stealth.</size>";
            Owner.SendHint(975, BuiltText, 0.5f);
        }
    }
}
