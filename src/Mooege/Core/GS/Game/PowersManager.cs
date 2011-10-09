using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Net.GS.Message.Fields;
using Mooege.Core.GS.Actors;
using Mooege.Net.GS;
using Mooege.Net.GS.Message.Definitions.Combat;
using Mooege.Core.GS.Skills;
using Mooege.Net.GS.Message.Definitions.ACD;
using Mooege.Net.GS.Message.Definitions.Misc;
using Mooege.Common.Helpers;
using Mooege.Net.GS.Message.Definitions.Animation;
using Mooege.Net.GS.Message;
using Mooege.Net.GS.Message.Definitions.Effect;
using Mooege.Net.GS.Message.Definitions.Attribute;
using Mooege.Net.GS.Message.Definitions.Player;
using Mooege.Common;
using Mooege.Core.Common.Toons;
using Mooege.Core.MooNet.Games;
using Mooege.Core.GS.Map;
using Mooege.Core.GS.Powers;


//For testing only
//using Mooege.Core.GS.NPC;


namespace Mooege.Core.GS.Game
{
    /*public class ClientEffect : Actor
    {
        public override ActorType ActorType { get { return ActorType.ClientEffect; } }
        public DateTime timeout;
    }*/

    public class PowersManager
    {
        public Logger Logger = LogManager.CreateLogger();
        public Game Game { get; private set; }
        public List<Actor> mobList = new List<Actor>();

        public Random _rand = new Random();

        public PowersManager(Game game)
        {
            this.Game = game;
        }

        // list of all waiting to execute powers
        class WaitingPower
        {
            public IEnumerator<int> PowerEnumerator;
            public DateTime Timeout;
            public Actor User;
        }
        List<WaitingPower> _waitingPowers = new List<WaitingPower>();

        // tracking information for currently channel-casting actors
        /*class ChanneledCast
        {
            public DateTime CastDelay;
            public IList<ClientEffect> Effects;
            public int CastDelayAmount;
        }*/
        //private Dictionary<Actor, ChanneledCast> _channelingActors = new Dictionary<Actor, ChanneledCast>();

        // list of all waiting to execute powers
        /*class WaitingPower
        {
            public IEnumerator<int> PowerEnumerator;
            public DateTime Timeout;
            public Actor User;
        }

        List<WaitingPower> _waitingPowers = new List<WaitingPower>();*/

        // supplies Powers with all available targets.
        /*private IEnumerable<Actor> Targets
        {
            get
            {
                return PowersMobTester.AllMobs;
            }
        }*/



        //public void UsePower(Actor user, int powerId, int targetId = -1, Vector3D targetPos = null, TargetMessage message = null)
        public void Manage(Mooege.Core.GS.Player.Player player, TargetMessage message)
        {
            //Extract message information
            int snoPower = message.PowerSNO;
            uint targetID = message.TargetID;
            WorldPlace cursor = message.Field2;

            if (snoPower == Skills.Skills.Barbarian.FuryGenerators.LeapAttack) // HACK: intercepted to use for spawning test mobs
            {
                Console.Write("Player " + player.DynamicID + " want to spawn a mob \r\n");

                //Spawn moonclan
                Monster monster = new Monster(player.World, 4282, new Vector3D(player.Position.X + 5f, player.Position.Y + 5f, player.Position.Z));
                monster.Reveal(player);
            }
            else
            {
                Actor target = player.World.GetActor(message.TargetID);

                // find and run a power implementation
                var implementation = PowerImplementation.ImplementationForId(snoPower);

                Console.Write("Implementation found : " + implementation + "\r\n" );

                if (implementation != null)
                {
                    // process channeled skill params
                    bool userIsChanneling = false;
                    bool throttledCast = false;
                    /*if (_channelingActors.ContainsKey(user))
                    {
                        userIsChanneling = true;
                        if (DateTime.Now > _channelingActors[user].CastDelay)
                        {
                            _channelingActors[user].CastDelay = DateTime.Now.AddMilliseconds(_channelingActors[user].CastDelayAmount);
                        }
                        else
                        {
                            throttledCast = true;
                        }
                    }*/

                    IEnumerable<int> powerExe = implementation.Run(new PowerParameters
                    {
                        User = player,
                        Target = target,
                        TargetPosition = cursor,
                        Message = message,
                        UserIsChanneling = userIsChanneling,
                        ThrottledCast = throttledCast,
                    },
                    this);

                    var powerEnum = powerExe.GetEnumerator();
                    // actual power will first run here, if it yielded a value process it in the waiting list
                    if (powerEnum.MoveNext())
                    {
                        AddWaitingPower(_waitingPowers, powerEnum, player);
                    }
                }
            }
        }

        public void Tick()
        {
            UpdateWaitingPowers();
            //CleanUpEffects();
            //CleanUpAttribute();
            //CleanUpInternalAttribute();
            //_mobtester.Tick();
            //Console.Write("tick");
        }

        private void AddWaitingPower(IList<WaitingPower> list, IEnumerator<int> powerEnum, Actor user)
        {
            WaitingPower wait = new WaitingPower();
            wait.PowerEnumerator = powerEnum;
            wait.User = user;
            if (powerEnum.Current == 0)
                wait.Timeout = DateTime.MinValue;
            else
                wait.Timeout = DateTime.Now.AddMilliseconds(powerEnum.Current);

            list.Add(wait);
        }

        private void UpdateWaitingPowers()
        {
            List<WaitingPower> newWaitList = new List<WaitingPower>();
            foreach (WaitingPower wait in _waitingPowers)
            {
                if (DateTime.Now > wait.Timeout)
                {
                    if (wait.PowerEnumerator.MoveNext())
                    {
                        // re-add with new timeout
                        AddWaitingPower(newWaitList, wait.PowerEnumerator, wait.User);
                    }
                }
                else
                {
                    // re-add with same timeout
                    newWaitList.Add(wait);
                }
            }
            _waitingPowers = newWaitList;
        }

        public bool WillHitMeleeTarget(Actor user, Actor target)
        {
            if (target == null) return false;
            return (Math.Abs(user.Position.X - target.Position.X) < 12f &&
                    Math.Abs(user.Position.Y - target.Position.Y) < 12f);
        }

        public void DoKnockback(Actor user, Actor target, float amount)
        {
            if (target == null) return;

            // TODO: figure out how to implement with amount
            Vector3D move = new Vector3D();
            move.Z = target.Position.Z;
            move.X = target.Position.X + (target.Position.X - user.Position.X);
            move.Y = target.Position.Y + (target.Position.Y - user.Position.Y);
            MoveActorNormal(user, target, move, getRadian(target.Position, user.Position));
        }

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

        public void flushAll(Actor actor)
        {
            foreach (Mooege.Core.GS.Player.Player player in actor.World.GetPlayersInRange(actor.Position, 150f))
            {
                player.InGameClient.FlushOutgoingBuffer();
            }
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
                    Field4 = 1.0f,
                });

                SendDWordTick(player.InGameClient);
            }

            //Update actor position
            actor.Position = pos;
        }

        //Temp dmg visual 
        public void DoDamage(Actor target, float amount, DamageType type)
        {
            foreach (Mooege.Core.GS.Player.Player player in target.World.GetPlayersInRange(target.Position, 150f))
            {
                player.InGameClient.SendMessage(new FloatingNumberMessage()
                {
                    Id = 0xd0,
                    ActorID = target.DynamicID,
                    Number = amount,
                    Field2 = (int)type,
                });

                SendDWordTick(player.InGameClient);
            }
        }

        public IList<Actor> FindActorsInRadius(Actor refActor, float radius, int maxCount = -1)
        {
            List<Actor> actors = new List<Actor>();
            foreach (Actor actor in refActor.World.GetActorsInRange(refActor.Position, radius))
            {
                if (actors.Count == maxCount)
                    break;

                actors.Add(actor);
            }
            return actors;
        }

        //Get actor that are in a "cone shaped" zone in front of the refActor
        public IList<Actor> FindActorsInFront(Actor refActor, Vector3D targetPos, float degOpening, float maxDistance, int maxCount = -1)
        {
            List<Actor> actors = new List<Actor>();

            //Restrict calculation to actor that are in range radiux
            List<Actor> targets = refActor.World.GetActorsInRange(refActor.Position, maxDistance);

            float deg_ref = getDeg(refActor.Position, targetPos);

            foreach (Actor actor in targets)
            {
                if (actors.Count == maxCount)
                    break;

                //Calculate deg
                float deg_target = getDeg(refActor.Position, actor.Position);

                float deg_upper_limit = (deg_ref + degOpening / 2);
                float deg_lower_limit = (deg_ref - degOpening / 2);

                //Correction need to be made when 0 deg is in the cone
                if (deg_upper_limit > 360f || deg_lower_limit < 0)
                {
                    deg_upper_limit += 360f;
                    deg_lower_limit += 360f;
                    deg_target += 360f;
                }

                if ((deg_upper_limit > deg_target) && (deg_lower_limit < deg_target))
                {
                    actors.Add(actor);
                }

            }
            return actors;
        }

        float getDeg(Vector3D startingPoint, Vector3D endingPoint)
        {
            float radians = getRadian(startingPoint, endingPoint); // get vector in radians
            float degrees = radians * (180f / 3.1416f); // convert to degrees
            degrees = (degrees > 0f ? degrees : (360f + degrees)); // correct discontinuity
            return degrees;
        }

        float getRadian(Vector3D startingPoint, Vector3D endingPoint)
        {
            return (float)Math.Atan2(startingPoint.Y - endingPoint.Y, startingPoint.X - endingPoint.X);
        }

        public void SendDWordTick(GameClient client)
        {
            client.PacketId += 10 * 2;
            client.SendMessage(new DWordDataMessage()
            {
                Id = 0x89,
                Field0 = client.PacketId,
            });
        }

            /*

            if (powerId == Skills.Skills.Monk.SpiritSpenders.BlindingFlash) // HACK: intercepted to use for spawning test mobs
            {
                _mobtester.SpawnMob(user);
            }
            else
            {
                // find and run a power implementation
                var implementation = PowerImplementation.ImplementationForId(powerId);
                if (implementation != null)
                {
                    // process channeled skill params
                    bool userIsChanneling = false;
                    bool throttledCast = false;
                    if (_channelingActors.ContainsKey(user))
                    {
                        userIsChanneling = true;
                        if (DateTime.Now > _channelingActors[user].CastDelay)
                        {
                            _channelingActors[user].CastDelay = DateTime.Now.AddMilliseconds(_channelingActors[user].CastDelayAmount);
                        }
                        else
                        {
                            throttledCast = true;
                        }
                    }

                    IEnumerable<int> powerExe = implementation.Run(new PowerParameters
                    {
                        User = user,
                        Target = target,
                        TargetPosition = targetPos,
                        Message = message,
                        UserIsChanneling = userIsChanneling,
                        ThrottledCast = throttledCast,
                    },
                    this);

                    var powerEnum = powerExe.GetEnumerator();
                    // actual power will first run here, if it yielded a value process it in the waiting list
                    if (powerEnum.MoveNext())
                    {
                        AddWaitingPower(_waitingPowers, powerEnum, user);
                    }
                    // send tick after executing power
                    SendDWordTickFor(user);
                }
            }
        }*/        
    }
}
