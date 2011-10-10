using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Core.GS.Actors;
using Mooege.Net.GS.Message.Fields;
using Mooege.Net.GS.Message.Definitions.ACD;
using Mooege.Net.GS.Message.Definitions.Effect;
using Mooege.Net.GS;
using Mooege.Net.GS.Message;
using Mooege.Net.GS.Message.Definitions.Misc;
using Mooege.Net.GS.Message.Definitions.Animation;

namespace Mooege.Core.GS.Powers
{
    public class PowerEffects
    {
        #region Effect
        public void PlayEffectGroupActorToActor(int effectId, Actor from, Actor target)
        {
            if (target == null) return;

            foreach (Mooege.Core.GS.Player.Player player in from.World.GetPlayersInRange(from.Position, 150f))
            {
                player.InGameClient.SendMessage(new EffectGroupACDToACDMessage()
                {
                    Id = 0xaa,
                    effectSNO = effectId,
                    fromActorID = from.DynamicID,
                    toActorID = target.DynamicID
                });

                SendDWordTick(player.InGameClient);
            }
        }

        public void PlayAnimation(Actor actor, int animationId)
        {
            if (actor == null) return;

            foreach (Mooege.Core.GS.Player.Player player in actor.World.GetPlayersInRange(actor.Position, 150f))
            {
                //Stop actor current animation
                player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.ANNDataMessage13)
                {
                    ActorID = actor.DynamicID
                });

                player.InGameClient.SendMessage(new PlayAnimationMessage()
                {
                    ActorID = actor.DynamicID,
                    Field1 = 0xb,
                    Field2 = 0,
                    tAnim = new PlayAnimationMessageSpec[1]
                    {
                        new PlayAnimationMessageSpec()
                        {
                            Field0 = 0x2,
                            Field1 = animationId,
                            Field2 = 0x0,
                            Field3 = 0.6f
                        }
                    }
                });
            } 
        }

        public void PlayHitEffect(HitEffect id, Actor target, Actor from)
        {
            if (target == null) return;

            foreach (Mooege.Core.GS.Player.Player player in target.World.GetPlayersInRange(target.Position, 150f))
            {
                player.InGameClient.SendMessage(new PlayHitEffectMessage()
                {
                    Id = 0x7b,
                    ActorID = target.DynamicID,
                    HitDealer = from.DynamicID,
                    Field2 = (int)id,
                    Field3 = false
                });

                SendDWordTick(player.InGameClient);
            }
        }
        #endregion

        #region ACD

        //Need to be fix, with the new monster actor, moving monster warp them instead of a smoot translation
        public void MoveActorNormal(Actor actorRef, Actor actor, Vector3D pos, float angle = 0f)
        {
            if (actor == null) return;

            foreach (Mooege.Core.GS.Player.Player player in actor.World.GetPlayersInRange(actor.Position, 150f))
            {
                player.InGameClient.SendMessage(new ACDTranslateNormalMessage()
                {
                    Id = 0x6e,
                    ActorID = actor.DynamicID,
                    Position = pos,
                    Angle = angle, // TODO: convert quaternion rotation for this?
                    Field3 = false,
                    Field4 = 2000f,
                });

                SendDWordTick(player.InGameClient);
            }

            //Update actor position
            actor.Position = pos;
        }
        

        public void FaceTarget(Actor target, Actor actor)
        {
            if (target == null) return;

            foreach (Mooege.Core.GS.Player.Player player in actor.World.GetPlayersInRange(actor.Position, 150f))
            {
                player.InGameClient.SendMessage(new ACDTranslateFacingMessage(Opcodes.ACDTranslateFacingMessage1)
                {
                    ActorID = actor.DynamicID,
                    Angle = getRadian(target.Position, actor.Position),
                    Field2 = false
                });

                SendDWordTick(player.InGameClient);
            }
        }
        
        public void DoKnockback(Actor user, Actor target, float amount)
        {
            if (target == null) return;

            // TODO: figure out how to implement with amount
            Vector3D move = new Vector3D();
            move.Z = target.Position.Z;
            move.X = target.Position.X + (target.Position.X - user.Position.X) * amount;
            move.Y = target.Position.Y + (target.Position.Y - user.Position.Y) * amount;
            MoveActorNormal(user, target, move, getRadian(target.Position, user.Position));
        }
        #endregion

        #region Calcul
        public float getDeg(Vector3D startingPoint, Vector3D endingPoint)
        {
            float radians = getRadian(startingPoint, endingPoint); // get vector in radians
            float degrees = radians * (180f / 3.1416f); // convert to degrees
            degrees = (degrees > 0f ? degrees : (360f + degrees)); // correct discontinuity
            return degrees;
        }

        public float getRadian(Vector3D startingPoint, Vector3D endingPoint)
        {
            return (float)Math.Atan2(startingPoint.Y - endingPoint.Y, startingPoint.X - endingPoint.X);
        }

        public float getDistance(Vector3D startingPoint, Vector3D endingPoint)
        {
            return (float)Math.Sqrt(Math.Pow(startingPoint.X - endingPoint.X, 2) + Math.Pow(startingPoint.Y - endingPoint.Y, 2));
        }
        #endregion

        public void SendDWordTick(GameClient client)
        {
            client.PacketId += 10 * 2;
            client.SendMessage(new DWordDataMessage()
            {
                Id = 0x89,
                Field0 = client.PacketId,
            });
        }
    }
}
