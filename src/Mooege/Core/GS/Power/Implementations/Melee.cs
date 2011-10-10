using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Core.GS.Skills;
using Mooege.Core.GS.Game;
using Mooege.Core.GS.Powers;
using Mooege.Net.GS.Message.Definitions.Misc;

namespace Mooege.Core.GS.Powers.Implementations
{
    [PowerImplementationAttribute(0x00007780/*Skills.Skills.BasicAttack*/)]
    public class Melee : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager pm)
        {
            if (pm.fx.getDistance(pp.User.Position, pp.Target.Position) > pm.meleeRange)
            {
                pm.fx.PlayHitEffect(HitEffect.Flash, pp.User, pp.Target);
                pm.DoDamage(pp.Target, 25f, FloatingNumberMessage.FloatType.White);
            }
            yield break;
        }
    }
}
