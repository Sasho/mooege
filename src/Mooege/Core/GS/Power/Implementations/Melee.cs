using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Core.GS.Skills;
using Mooege.Core.GS.Game;
using Mooege.Core.GS.Powers;

namespace Mooege.Core.GS.Powers.Implementations
{
    [PowerImplementationAttribute(0x00007780/*Skills.Skills.BasicAttack*/)]
    public class Melee : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowersManager fx)
        {
            if (fx.WillHitMeleeTarget(pp.User, pp.Target))
            {
                fx.PlayHitEffect(HitEffect.Flash, pp.User, pp.Target);
                fx.DoDamage(pp.Target, 25f, DamageType.Normal_fast);
            }
            yield break;
        }
    }
}
