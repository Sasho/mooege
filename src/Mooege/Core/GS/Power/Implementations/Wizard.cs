﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Core.GS.Actors;
using Mooege.Net.GS;
using Mooege.Net.GS.Message.Fields;
using Mooege.Core.GS.Game;
using Mooege.Core.GS.Powers;

namespace Mooege.Core.GS.Powers.Implementations
{
    [PowerImplementationAttribute(0x00010E46/*Skills.Skills.Wizard.Offensive.Meteor*/)]
    public class WizardMeteor : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager fx)
        {
            /*fx.SpawnEffect(pp.User, 86790, pp.TargetPosition);
            yield return 2000;
            fx.SpawnEffect(pp.User, 86769, pp.TargetPosition);
            fx.SpawnEffect(pp.User, 90364, pp.TargetPosition, -1, 4000);

            IList<Actor> hits = fx.FindActorsInRadius(pp.TargetPosition, 13f);
            fx.DoDamage(pp.User, hits, 150f, 0);*/
            yield break;
        }
    }

    [PowerImplementationAttribute(0x000006E5/*Skills.Skills.Wizard.Signature.Electrocute*/)]
    public class WizardElectrocute : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager fx)
        {
            /*fx.RegisterChannelingPower(pp.User, 150);

            fx.ActorLookAt(pp.User, pp.TargetPosition);

            // if throttling only update proxy if needed, then exit
            if (pp.ThrottledCast)
            {
                if (pp.Target == null)
                    fx.GetChanneledProxy(pp.User, 0, pp.TargetPosition);
                yield break;
            }

            IList<Actor> targets;
            if (pp.Target == null)
            {
                targets = new List<Actor>();
                fx.PlayRopeEffectActorToActor(0x78c0, pp.User, fx.GetChanneledProxy(pp.User, 0, pp.TargetPosition));
            }
            else
            {
                targets = fx.FindActorsInRadius(pp.TargetPosition, 15f, 1);
                targets.Insert(0, pp.Target);
                Actor effect_source = pp.User;
                foreach (Actor actor in targets)
                {
                    fx.PlayHitEffect(2, effect_source, actor);
                    fx.PlayRopeEffectActorToActor(0x78c0, effect_source, actor);
                    effect_source = actor;
                }
            }

            fx.DoDamage(pp.User, targets, 12, 0);*/
            yield break;
        }
    }

    [PowerImplementationAttribute(0x00007818/*Skills.Skills.Wizard.Signature.MagicMissile*/)]
    public class WizardMagicMissile : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager fx)
        {
            // HACK: made up spell, not real magic missile
            /*for (int step = 1; step < 10; ++step)
            {
                var spos = new Vector3D();
                spos.X = pp.User.Position.X + ((pp.TargetPosition.X - pp.User.Position.X) * (step * 0.10f));
                spos.Y = pp.User.Position.Y + ((pp.TargetPosition.Y - pp.User.Position.Y) * (step * 0.10f));
                spos.Z = pp.User.Position.Z + ((pp.TargetPosition.Z - pp.User.Position.Z) * (step * 0.10f));

                fx.SpawnEffect(pp.User, 61419, spos);

                IList<Actor> hits = fx.FindActorsInRadius(spos, 6f);
                fx.DoDamage(pp.User, hits, 60f, 0);
                yield return 100;
            }*/
            yield break;
        }
    }

    [PowerImplementationAttribute(0x00007805/*Skills.Skills.Wizard.Offensive.Hydra*/)]
    public class WizardHydra : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager fx)
        {
            // HACK: made up demonic meteor spell, not real hydra
            /*fx.SpawnEffect(pp.User, 185366, pp.TargetPosition);
            yield return 400;

            IList<Actor> hits = fx.FindActorsInRadius(pp.TargetPosition, 10f);
            fx.DoDamage(pp.User, hits, 100f, 0);*/
            yield break;
        }
    }

    [PowerImplementationAttribute(0x0001659D/*Skills.Skills.Wizard.Offensive.Disintegrate*/)]
    public class WizardDisintegrate : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager fx)
        {
            /*fx.RegisterChannelingPower(pp.User);

            // TODO: hit effects
            Effect pid = fx.GetChanneledProxy(pp.User, 0, pp.TargetPosition);
            if (! pp.UserIsChanneling)
            {
                fx.PlayRopeEffectActorToActor(30888, pp.User, pid);
            }

            // TODO: beam damage
            //DoDamage(user, target, 12, 0);*/
            yield break;
        }
    }

    [PowerImplementationAttribute(0x0000784C/*Skills.Skills.Wizard.Offensive.WaveOfForce*/)]
    public class WizardWaveOfForce : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager fx)
        {
            /*yield return 350;
            fx.PlayEffectGroupActorToActor(19356, pp.User, fx.SpawnTempProxy(pp.User, pp.User.Position));

            IList<Actor> hits = fx.FindActorsInRadius(pp.User.Position, 20);
            foreach (Actor actor in hits)
            {
                fx.DoKnockback(pp.User, actor, 10f);
                fx.DoDamage(pp.User, actor, 20, 0);
            }*/
            yield break;
        }
    }

    [PowerImplementationAttribute(0x00020D38/*Skills.Skills.Wizard.Offensive.ArcaneTorrent*/)]
    public class WizardArcaneTorrent : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager fx)
        {
           /* fx.RegisterChannelingPower(pp.User, 200);

            if (!pp.UserIsChanneling)
            {
                Actor targetProxy = fx.GetChanneledProxy(pp.User, 0, pp.TargetPosition);
                Actor userProxy = fx.GetChanneledProxy(pp.User, 1, pp.User.Position);
                fx.ActorLookAt(userProxy, pp.TargetPosition);
                fx.PlayEffectGroupActorToActor(97385, userProxy, userProxy);
                fx.PlayEffectGroupActorToActor(134442, userProxy, targetProxy);
            }

            if (!pp.ThrottledCast)
            {
                yield return 800;
                // update proxy target location laggy
                if (fx.ChannelingCanceled(pp.User))
                    fx.GetChanneledProxy(pp.User, 0, pp.TargetPosition);

                fx.SpawnEffect(pp.User, 97821, pp.TargetPosition);
                fx.DoDamage(pp.User, fx.FindActorsInRadius(pp.TargetPosition, 6f), 20, 0);
            }*/
            yield break;
        }
    }
}
