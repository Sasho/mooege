﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Core.GS.Skills;
using Mooege.Net.GS.Message.Fields;
using Mooege.Core.GS.Actors;
using Mooege.Core.GS.Game;
using Mooege.Core.GS.Powers;

namespace Mooege.Core.GS.Powers.Implementations
{
    [PowerImplementationAttribute(0x00017713/*Skills.Skills.Monk.SpiritGenerator.DeadlyReach*/)]
    public class MonkDeadlyReach : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager fx)
        {
            /*if (pp.Message.Field5 == 0)
                fx.PlayEffectGroupActorToActor(71921, pp.User, fx.SpawnTempProxy(pp.User, pp.TargetPosition));
            else if (pp.Message.Field5 == 1)
                fx.PlayEffectGroupActorToActor(72134, pp.User, fx.SpawnTempProxy(pp.User, pp.TargetPosition));
            else if (pp.Message.Field5 == 2)
                fx.PlayEffectGroupActorToActor(72331, pp.User, fx.SpawnTempProxy(pp.User, pp.TargetPosition));

            if (fx.WillHitMeleeTarget(pp.User, pp.Target))
            {
                fx.PlayHitEffect(5, pp.User, pp.Target);
                fx.DoDamage(pp.User, pp.Target, 25f, 0);
            }*/

            yield break;
        }
    }

    [PowerImplementationAttribute(0x000176C4/*Skills.Skills.Monk.SpiritGenerator.FistsOfThunder*/)]
    public class MonkFistsOfThunder : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager fx)
        {
            /*if (pp.Message.Field5 == 0)
                fx.PlayEffectGroupActorToActor(96176, pp.User, fx.SpawnTempProxy(pp.User, pp.TargetPosition));
            else if (pp.Message.Field5 == 1)
                fx.PlayEffectGroupActorToActor(96176, pp.User, fx.SpawnTempProxy(pp.User, pp.TargetPosition));
            else if (pp.Message.Field5 == 2)
                fx.PlayEffectGroupActorToActor(96178, pp.User, fx.SpawnTempProxy(pp.User, pp.TargetPosition));

            if (fx.WillHitMeleeTarget(pp.User, pp.Target))
            {
                fx.PlayHitEffect(2, pp.User, pp.Target);
                fx.DoDamage(pp.User, pp.Target, 25f, 0);
            }*/

            yield break;
        }
    }

    [PowerImplementationAttribute(0x000179B6/*Skills.Skills.Monk.SpiritSpenders.SevenSidedStrike*/)]
    public class MonkSevenSidedStrike : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager fx)
        {
            /*Vector3D startpos;
            if (pp.Target == null)
                startpos = pp.User.Position;
            else
                startpos = pp.TargetPosition;
            
            for (int n = 0; n < 7; ++n)
            {
                IList<Actor> nearby = fx.FindActorsInRadius(startpos, 20f, 1);
                if (nearby.Count > 0)
                {
                    fx.SpawnEffect(pp.User, 99063, nearby[0].Position);
                    fx.DoDamage(pp.User, nearby[0], 100f, 0);
                    yield return 100;
                }
                else
                {
                    break;
                }
            }*/
            yield break;
        }
    }

    [PowerImplementationAttribute(0x00017837/*Skills.Skills.Monk.SpiritGenerator.CripplingWave*/)]
    public class MonkCripplingWave : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager fx)
        {
            /*if (pp.Message.Field5 == 0)
                fx.PlayEffectGroupActorToActor(18987, pp.User, fx.SpawnTempProxy(pp.User, pp.TargetPosition));
            else if (pp.Message.Field5 == 1)
                fx.PlayEffectGroupActorToActor(18988, pp.User, fx.SpawnTempProxy(pp.User, pp.TargetPosition));
            else if (pp.Message.Field5 == 2)
                fx.PlayEffectGroupActorToActor(96519, pp.User, fx.SpawnTempProxy(pp.User, pp.TargetPosition));

            if (pp.Message.Field5 != 2)
            {
                if (fx.WillHitMeleeTarget(pp.User, pp.Target))
                {
                    fx.PlayHitEffect(6, pp.User, pp.Target);
                    fx.DoDamage(pp.User, pp.Target, 25f, 0);
                }
            }
            else
            {
                IList<Actor> hits = fx.FindActorsInRadius(pp.User.Position, 10);
                foreach (Actor hit in hits)
                {
                    fx.PlayHitEffect(6, pp.User, hit);
                    fx.DoDamage(pp.User, hit, 25f, 0);
                }
            }*/
            yield break;
        }
    }

    [PowerImplementationAttribute(0x00017C30/*Skills.Skills.Monk.SpiritGenerator.ExplodingPalm*/)]
    public class MonkExplodingPalm : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager fx)
        {
            /*if (pp.Message.Field5 == 0)
                fx.PlayEffectGroupActorToActor(142471, pp.User, fx.SpawnTempProxy(pp.User, pp.TargetPosition));
            else if (pp.Message.Field5 == 1)
                fx.PlayEffectGroupActorToActor(142471, pp.User, fx.SpawnTempProxy(pp.User, pp.TargetPosition));
            else if (pp.Message.Field5 == 2)
                fx.PlayEffectGroupActorToActor(142473, pp.User, fx.SpawnTempProxy(pp.User, pp.TargetPosition));

            if (fx.WillHitMeleeTarget(pp.User, pp.Target))
            {
                fx.PlayHitEffect(0, pp.User, pp.Target);
                fx.DoDamage(pp.User, pp.Target, 25f, 0);
            }*/

            yield break;
        }
    }

    [PowerImplementationAttribute(0x0001775A/*Skills.Skills.Monk.SpiritGenerator.SweepingWind*/)]
    public class MonkSweepingWind : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager fx)
        {
            // TODO: make buffs disappear so skill can be implemented
            //if (pp.Message.Field5 == 0)
            //    fx.PlayEffectGroupActorToActor(73953, pp.User, fx.SpawnTempProxy(pp.User, pp.TargetPosition));
            //else if (pp.Message.Field5 == 1)
            //    fx.PlayEffectGroupActorToActor(73953, pp.User, fx.SpawnTempProxy(pp.User, pp.TargetPosition));
            //else if (pp.Message.Field5 == 2)
            //    fx.PlayEffectGroupActorToActor(73953, pp.User, fx.SpawnTempProxy(pp.User, pp.TargetPosition));

            yield break;
        }
    }
}
