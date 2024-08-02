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
using static PlayerList;

namespace Scp939rework
{
    public class Custom939
    {
        public static List<Custom939> Instances = new List<Custom939>();

        public float Stealth { get { return _stealth; } set { _stealth = value; } }

        private float _stealth;
        public float MaxStealth { get { return _maxstealth; } }

        private float _maxstealth;

        public float LungeData { get; set; }
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
            if (change == 0) _stealth = Mathf.Clamp(_stealth + ( Time.deltaTime * ( _maxstealth * 0.09f ) ), 0, _maxstealth);
            _stealth = Mathf.Clamp(_stealth + (change * Time.deltaTime), 0, _maxstealth);

            if (_stealth >= _maxstealth * 0.5f) { Owner.Effect<MovementBoost>(20, 0); } else { Owner.Effect<MovementBoost>(false); }
            if (_stealth == _maxstealth) { Owner.Effect<Invisible>(true); Owner.Effect<MovementBoost>(50, 0); } else { Owner.Effect<Invisible>(false); }
            
        }

        public void HalfSecondCoroutine()
        {
            if (!Instances.Contains(this)) return;
            float progress = _stealth / _maxstealth; //0-1
            float lower = progress;
            float upper = 1 - lower;
            char c = '█';

            float width = 13;

            string ratio = $"{200 / MaxStealth}:1";

            

            string linebreaks = "<br><br><br><br><br><br><br><br><br><br><br><br><br>";

            string bar = $"<b>[<color=#ccccff><scale={lower * width}>{c}</scale></color><color=#cccccc><scale={upper * width}>{c}</scale></color>]</b>";
            string BuiltText = $"<b>{_stealth.RoundToNearest(0.5f)} / {MaxStealth.RoundToNearest(0.5f)} Stealth</b><br>{bar}";
            BuiltText += $"<br><size=20><align=left>+Passive: Invisibility {(_stealth == _maxstealth ? "<color=#00ff00>(Active)</color>" : "<color=#ff0000>(Inactive)</color>")}<br></size><align=left><size=15>Become invisible while at 100% stealth and not near any players.</size>";
            BuiltText += $"<br><br><size=20><align=left>+Passive: Sacrificial Heal <br></size><align=left><size=15>Using amnestic cloud converts all hume shield into HP, at a {ratio} ratio.</size>";
            BuiltText += $"<br><br><size=20><align=left>+Passive: Speed {(_stealth > _maxstealth * 0.5f ? "<color=#00ff00>(Active)</color>" : "<color=#ff0000>(Inactive)</color>")}<br></size><align=left><size=15>Move notably faster while above 50% stealth</size>{linebreaks}";
            if (_stealth > 30) BuiltText += $"<size=20><align=right>Sneak Attack <color=#00ff00>(Ready)</color><br></size><align=right><size=15>Claw attacks deal heavy damage, but consume 30 stealth.</size><br>";
            if (_stealth > 25 && _stealth < 50) BuiltText += $"<br><size=20><align=right>Super Pounce<color=#ff0000>(Level 1)</color><br></size><align=right><size=15>Your pounce slows all nearby players</size>";
            if (_stealth >= 50 && _stealth < 75) BuiltText += $"<br><size=20><align=right>Super Pounce<color=#cc4400>(Level 2)</color><br></size><align=right><size=15>Your pounce slows all nearby players, and gives you AHP on hit.</size>";
            if (_stealth >= 75 && _stealth < _maxstealth) BuiltText += $"<br><size=20><align=right>Super Pounce<color=#77aa00>(Level 3)</color><br></size><align=right><size=15>Your pounce slows all nearby players, and gives you AHP and HP on hit.</size>";
            if (_stealth == _maxstealth) BuiltText += $"<br><size=20><align=right>Super Pounce<color=#00ff00>(Level 4)</color><br></size><align=right><size=15>Your pounce is <b>explosive</b>, slows all nearby players, and gives you AHP and HP on hit.</size>";


            Owner.SendHint(975, BuiltText, 0.5f);
        }
    }
}
