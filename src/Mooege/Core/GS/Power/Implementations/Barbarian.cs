using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Core.GS.Actors;
using Mooege.Net.GS;
using Mooege.Net.GS.Message.Fields;
using Mooege.Net.GS.Message.Definitions.Attribute;
using Mooege.Net.GS.Message.Definitions.Misc;
using Mooege.Net.GS.Message;
using Mooege.Core.GS.Game;
using Mooege.Core.GS.Powers;


namespace Mooege.Core.GS.Powers.Implementations
{
    [PowerImplementationAttribute(Skills.Skills.Barbarian.FuryGenerators.Bash)]
    public class BarbarianBash : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager pm)
        {
            //Need in range selected target
            if (pp.Target == null || pm.fx.getDistance(pp.User.Position, pp.Target.Position) > pm.meleeRange) { yield break; }

            yield return 100; //Synchronize with weapon swing and give time to player position to update

            //Add 20% knockback chance
            if (pm._rand.Next(0, 100) < 60) { pm.fx.DoKnockback(pp.User, pp.Target, 1); }

            //Effect may depend on rune/gender not implemented for the moment
            pm.fx.PlayEffectGroupActorToActor(18662, pp.User, pp.User);
            pm.fx.PlayHitEffect(HitEffect.Flash, pp.Target, pp.User);

            //Tmp dmg
            pm.DoDamage(pp.Target, 30f, FloatingNumberMessage.FloatType.White);
                        
            //Regenerate ressource
            pm.generateRessource(pp.User, 6);

            //Flush all player buffer
            pm.flushAll(pp.User);
        }
    }

    [PowerImplementationAttribute(Skills.Skills.Barbarian.FuryGenerators.Cleave)]
    public class BarbarianCleave : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager pm)
        {
            //Need in range selected target
            if (pp.Target == null || pm.fx.getDistance(pp.User.Position, pp.Target.Position) > pm.meleeRange) { yield break; }

            yield return 200; //Synchronize with weapon swing

            //pp.Message.Field6.Field0 = swing side
            if(pp.Message.Field6.Field0 == 3) {
                pm.fx.PlayEffectGroupActorToActor(18671, pp.User, pp.Target);
            } else {
                pm.fx.PlayEffectGroupActorToActor(18672, pp.User, pp.Target);
            }

            IList<Actor> hits = pm.FindActorsInFront(pp.User, pp.Target.Position, 180f, 10f);
            foreach (Actor actor in hits)
            {
                //Must not be other player either will need fix
                if(actor.DynamicID != pp.User.DynamicID)
                    pm.DoDamage(actor, 20, FloatingNumberMessage.FloatType.White);
            }

            //Regenerate ressource
            pm.generateRessource(pp.User, 4);

            //Flush all player buffer
            pm.flushAll(pp.User);
        }
    }

    [PowerImplementationAttribute(Skills.Skills.Barbarian.FuryGenerators.GroundStomp)]
    public class BarbarianGroundStomp : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager pm)
        {
            yield return 150;

            pm.fx.PlayEffectGroupActorToActor(18685, pp.User, pp.User);

            //Regenerate ressource
            pm.generateRessource(pp.User, 15);

            List<Actor> targetList = pp.User.World.GetActorsInRange(pp.User.Position, 17f);

            //Add stunt effect
            foreach (Actor actor in targetList)
            {
                if (actor.DynamicID != pp.User.DynamicID)
                {
                    pm.DoDamage(actor, 20f, FloatingNumberMessage.FloatType.White);
                    actor.setAttribute(GameAttribute.Stunned, new GameAttributeValue(true));
                }
            }

            //Set skill on Colldown
            pp.User.setAttribute(GameAttribute.Power_Disabled, new GameAttributeValue(true), Skills.Skills.Barbarian.FuryGenerators.GroundStomp);
            pm.SendDWordTick(pp.User.InGameClient);
            pm.flushAll(pp.User);

            yield return 3000;
            
            //Remove stunt effect after 3sec
            foreach (Actor actor in targetList)
            {
                if (actor.DynamicID != pp.User.DynamicID)
                {
                    actor.setAttribute(GameAttribute.Stunned, new GameAttributeValue(false));
                }
            }

            pm.SendDWordTick(pp.User.InGameClient);

            yield return 9000;

            //Remove skill cooldown
            pp.User.setAttribute(GameAttribute.Power_Disabled, new GameAttributeValue(false), Skills.Skills.Barbarian.FuryGenerators.GroundStomp);
            pm.SendDWordTick(pp.User.InGameClient);
            pm.flushAll(pp.User);
            
        }
    }

    /*
    //On hold, need to figure out how arctranslate work
    //[PowerImplementationAttribute(0x00016CE1/*Skills.Skills.Barbarian.FuryGenerator.Leap*)]
    /*public class BarbarianLeap : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowersManager fx)
        {
            //Play barbarian animation ?
            
            //Barbarian arctranslate
            fx.MoveActorArc(pp.User, pp.TargetPosition);
            
            //Put skill on cooldown for 10 sec


            IList<Actor> hits = fx.FindActorsInRadius(pp.TargetPosition, 15);
            foreach (Actor actor in hits)
            {
                fx.DoDamage(pp.User, actor, 20, 0);
            }

            yield break;
            
        }
    }*/
    

    [PowerImplementationAttribute(Skills.Skills.Barbarian.FuryGenerators.Frenzy)]
    public class BarbarianFrenzy : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowerManager pm)
        {
            Console.Write("Fury stack : " + pp.User.Properties.furyStack + "\r\n");

            //Need in range selected target
            if (pp.Target == null || pm.fx.getDistance(pp.User.Position, pp.Target.Position) > pm.meleeRange) { yield break; }

            //Spawn frenzy effect
            //fx.SpawnEffect(pp.User, 3291, pp.User.Position, -1, 500);
            if (pp.User.Properties.furyStack == 0)
            {
                pm.fx.PlayEffectGroupActorToActor(18678, pp.User, pp.User);
            }

            pm.DoDamage(pp.Target, 30f, FloatingNumberMessage.FloatType.White);

            //Add 15% aps bonus for 4 sec
            if (pp.User.Properties.furyStack < 5)
            {
                pp.User.setAttribute(GameAttribute.Attacks_Per_Second_Total, new GameAttributeValue(pp.User.Attributes.GetAttributeValue(GameAttribute.Attacks_Per_Second_Total, null).ValueF + 0.15f));
                pm.SendDWordTick(pp.User.InGameClient);
                pp.User.Properties.furyStack++;
            }

            yield return 4000;

            if (pp.User.Properties.furyStack > 0)
            {

                pp.User.setAttribute(GameAttribute.Attacks_Per_Second_Total, new GameAttributeValue(pp.User.Attributes.GetAttributeValue(GameAttribute.Attacks_Per_Second_Total, null).ValueF - 0.15f));
                pm.SendDWordTick(pp.User.InGameClient);
                pp.User.Properties.furyStack--;

            }

            Console.Write("Fury stack : " + pp.User.Properties.furyStack);

        }
    }

    /*
    //[PowerImplementationAttribute(0x00013ECC/*Skills.Skills.Barbarian.FuryGenerator.Warcry*)]
    /*public class BarbarianWarCry : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowersManager fx)
        {
            //Warcry effect
            fx.PlayEffectGroupActorToActor(18664, pp.User, pp.User);

            foreach (Actor actor in fx.FindHeroInRadius(pp.User, 15f))
            {                
                //Add 100% armor bonus
                fx.ActorSetAttribute(actor, GameAttribute.Armor_Total.Id, actor.gameAttributes[GameAttribute.Armor_Total].Value * 2, 60000, actor.gameAttributes[GameAttribute.Armor_Total].Value / 2);
                actor.gameAttributes[GameAttribute.Armor_Total] = new GameAttributeValue(actor.gameAttributes[GameAttribute.Armor_Total].Value * 2);
            }

            yield break;
        }
    }*/

   // [PowerImplementationAttribute(0x00017C9B/*Skills.Skills.Barbarian.FuryGenerator.FuriousCharge*/)]
    /*public class BarbarianFuriousCharge : PowerImplementation
    {
        public override IEnumerable<int> Run(PowerParameters pp, PowersManager fx)
        {
            //Add start effect
            //fx.PlayEffectGroupActorToActor(18680, pp.User, pp.User);

            //Move targe
            //fx.MoveActorNormal(pp.User, pp.TargetPosition);
            fx.SpawnEffect(pp.User, 166221, pp.User.Position, -1, 500);


            //Play end effect
            fx.PlayEffectGroupActorToActor(18679, pp.User, pp.User);

            //Calculate monster in path

            //Calculate monster affected by knockback

            yield break;
        }
    }*/
}