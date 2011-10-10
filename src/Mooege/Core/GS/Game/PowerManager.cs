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

namespace Mooege.Core.GS.Game
{
    public class ClientEffect
    {
        //public override ActorType ActorType { get { return ActorType.Effect; } }
        public DateTime timeout;
    }

    public class PowerManager
    {
        public Logger Logger = LogManager.CreateLogger();
        public Game Game { get; private set; }
        
        public Random _rand = new Random();
        public PowerEffects fx = new PowerEffects();
        public float meleeRange = 12f;

        public PowerManager(Game game)
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

        // list of all waiting to update attribute
        class WaitingAttribute
        {
            //public IEnumerator<int> PowerEnumerator;
            //public DateTime Timeout;
            //public Actor User;
        }
        List<WaitingAttribute> _waitingAttribute = new List<WaitingAttribute>();

        // tracking information for currently channel-casting actors
        class ChanneledCast
        {
            public DateTime CastDelay;
            public IList<ClientEffect> Effects;
            public int CastDelayAmount;
        }
        private Dictionary<Actor, ChanneledCast> _channelingActors = new Dictionary<Actor, ChanneledCast>();

        List<WaitingPower> _waitingPowers = new List<WaitingPower>();

        public void Manage(Mooege.Core.GS.Player.Player player, GameMessage gameMessage)
        {
            //Extract message information
            int snoPower;
            uint targetID;
            WorldPlace cursor;
            Actor target;
            int swingSide;
            
            //Target message
            if (gameMessage.Id == 80)
            {
                TargetMessage message = (TargetMessage)gameMessage;
                snoPower = message.PowerSNO;
                targetID = message.TargetID;
                cursor = message.Field2;
                target = player.World.GetActor(message.TargetID);
                swingSide = message.Field6 == null ? 0 : message.Field6.Field0;
            }
            //SecondaryAnimationPowerMessage
            else
            {
                SecondaryAnimationPowerMessage message = (SecondaryAnimationPowerMessage)gameMessage;
                snoPower = message.PowerSNO;
                targetID = 0;
                cursor = new WorldPlace();
                target = player.World.GetActor(0);
                swingSide = 0;
            }

            if (snoPower == Skills.Skills.Barbarian.FuryGenerators.LeapAttack) // HACK: intercepted to use for spawning test mobs
            {
                //Spawn moonclan
                Monster monster = new Monster(player.World, 4282, new Vector3D(player.Position.X + 5f, player.Position.Y + 5f, player.Position.Z));
                monster.Reveal(player);
            }
            else
            {
                // find and run a power implementation
                var implementation = PowerImplementation.ImplementationForId(snoPower);

                if (implementation != null)
                {
                    // process channeled skill params
                    bool userIsChanneling = false;
                    bool throttledCast = false;
                    if (_channelingActors.ContainsKey(player))
                    {
                        userIsChanneling = true;
                        if (DateTime.Now > _channelingActors[player].CastDelay)
                        {
                            _channelingActors[player].CastDelay = DateTime.Now.AddMilliseconds(_channelingActors[player].CastDelayAmount);
                        }
                        else
                        {
                            throttledCast = true;
                        }
                    }

                    IEnumerable<int> powerExe = implementation.Run(new PowerParameters
                    {
                        User = player,
                        Target = target,
                        TargetPosition = cursor,                        
                        UserIsChanneling = userIsChanneling,
                        ThrottledCast = throttledCast,
                        SwingSide = swingSide
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
        
        public void flushAll(Actor actor)
        {
            foreach (Mooege.Core.GS.Player.Player player in actor.World.GetPlayersInRange(actor.Position, 150f))
            {
                player.InGameClient.FlushOutgoingBuffer();
            }
        }
                
        //Temp dmg visual should not be there
        public void DoDamage(Actor target, float amount, FloatingNumberMessage.FloatType type)
        {
            foreach (Mooege.Core.GS.Player.Player player in target.World.GetPlayersInRange(target.Position, 150f))
            {
                player.InGameClient.SendMessage(new FloatingNumberMessage()
                {
                    Id = 0xd0,
                    ActorID = target.DynamicID,
                    Number = amount,
                    Type = type,
                });

                SendDWordTick(player.InGameClient);
            }
        }

        public void generateRessource(Mooege.Core.GS.Player.Player player, float amount)
        {
            if(player.Attributes[GameAttribute.Resource_Cur, player.ResourceID] < player.Attributes[GameAttribute.Resource_Max, player.ResourceID])
            {
                if ((amount + player.Attributes[GameAttribute.Resource_Cur, player.ResourceID]) > player.Attributes[GameAttribute.Resource_Max, player.ResourceID])
                {
                    amount = player.Attributes[GameAttribute.Resource_Max, player.ResourceID] - player.Attributes[GameAttribute.Resource_Cur, player.ResourceID];
                }

                player.setAttribute(GameAttribute.Resource_Cur, new GameAttributeValue(amount + player.Attributes[GameAttribute.Resource_Cur, player.ResourceID]), player.ResourceID);
            }
            SendDWordTick(player.InGameClient);
        }

        public void userRessource(Mooege.Core.GS.Player.Player player, float amount)
        {
            if (player.Attributes[GameAttribute.Resource_Cur, player.ResourceID] < player.Attributes[GameAttribute.Resource_Max, player.ResourceID])
            {
                if (player.Attributes[GameAttribute.Resource_Cur, player.ResourceID] - amount < 0)
                {
                    amount = player.Attributes[GameAttribute.Resource_Cur, player.ResourceID];
                }
                player.setAttribute(GameAttribute.Resource_Cur, new GameAttributeValue(player.Attributes[GameAttribute.Resource_Cur, player.ResourceID] - amount), player.ResourceID);
            }
            SendDWordTick(player.InGameClient);
        }

        public IList<Actor> FindActorsInFront(Actor refActor, Vector3D targetPos, float degOpening, float maxDistance, int maxCount = -1)
        {
            List<Actor> actors = new List<Actor>();

            //Restrict calculation to actor that are in range radiux
            List<Actor> targets = refActor.World.GetActorsInRange(refActor.Position, maxDistance);

            float deg_ref = this.fx.getDeg(refActor.Position, targetPos);

            foreach (Actor actor in targets)
            {
                if (actors.Count == maxCount)
                    break;

                //Calculate deg
                float deg_target = this.fx.getDeg(refActor.Position, actor.Position);

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
        
        public void SendDWordTick(GameClient client)
        {
            client.PacketId += 10 * 2;
            client.SendMessage(new DWordDataMessage()
            {
                Id = 0x89,
                Field0 = client.PacketId,
            });
        }

        public void AddTemporaryAttribute()
        {
        }
             
    }
}
