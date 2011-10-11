using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;
using Mooege.Common;

namespace Mooege.Core.GS.Actors
{
    public static class ActorAnimations
    {
        public class AnimSet
        {
            public int ActorSNO;
            public int NumAnimations;
            public List<Animation> Animations = new List<Animation>();
            public Animation GetAniByID(int id)
            {
                return Animations.Single(s => s.AniSNO == id);
            }
            private static readonly Logger Logger = LogManager.CreateLogger();
            public int Idle { get {
                if (Animations.Exists(ani => ani.AniTagID == 69968 && ani.AniSNO != -1))
                {
                    return Animations.Single(ani => ani.AniTagID == 69968 && ani.AniSNO != -1).AniTagID; 
                }
                if (Animations.Exists(ani => ani.AniTagID == 69632 && ani.AniSNO != -1))
                {
                    return Animations.Single(ani => ani.AniTagID == 69632 && ani.AniSNO != -1).AniTagID; 
                    
                }
                Logger.Trace("No Idle found for actor: " + this.ActorSNO + " Sending Zombies Idle");
                return 0x11150;
                //Logger.Trace("using string matched ani: " + Animations.Single(ani => ani.name.Contains("idle") == true).AniTagID);
                //return Animations.Single(ani => ani.name.Contains("idle") == true).AniTagID;
               
                
            } }
            public int Hit
            {
                get
                {
                    if (Animations.Exists(ani => ani.AniTagID == 69664 && ani.AniSNO != -1))
                    {
                        return Animations.Single(ani => ani.AniTagID == 69664 && ani.AniSNO != -1).AniSNO;
                    }
                    /*if (Animations.Exists(ani => ani.AniTagID == 69632 && ani.AniSNO != -1))
                    {
                        return Animations.Single(ani => ani.AniTagID == 69632 && ani.AniSNO != -1).AniTagID;

                    }*/
                    Logger.Trace("No Hit found for actor: " + this.ActorSNO + " Sending Zombies Hit");
                    return 69664;
                    //Logger.Trace("using string matched ani: " + Animations.Single(ani => ani.name.Contains("idle") == true).AniTagID);
                    //return Animations.Single(ani => ani.name.Contains("idle") == true).AniTagID;


                }
            }
            public int Death
            {
                get
                {
                    if (Animations.Exists(ani => ani.AniTagID == 69648 && ani.AniSNO != -1))
                    {
                        return Animations.Single(ani => ani.AniTagID == 69664 && ani.AniSNO != -1).AniSNO;
                    }
                    /*if (Animations.Exists(ani => ani.AniTagID == 69632 && ani.AniSNO != -1))
                    {
                        return Animations.Single(ani => ani.AniTagID == 69632 && ani.AniSNO != -1).AniTagID;

                    }*/
                    Logger.Trace("No Death found for actor: " + this.ActorSNO + " Sending Zombies Death");
                    return 11479;
                    //Logger.Trace("using string matched ani: " + Animations.Single(ani => ani.name.Contains("idle") == true).AniTagID);
                    //return Animations.Single(ani => ani.name.Contains("idle") == true).AniTagID;


                }
            }

            //public int Death { get { return Animations.Single(ani => ani.AniTagID == 69648 && ani.AniSNO != -1).AniTagID; } }
            public int Walk { get { return Animations.Single(ani => ani.AniTagID == 69728 && ani.AniSNO != -1).AniTagID; } }
            
        }
        public class Animation
        {
            public string name;
            public int AniSNO;
            public int AniTagID;
        }


        public static AnimSet GetAnimSetByID(int ActorID)
        {
            SQLiteConnection Connection = new SQLiteConnection("Data Source=Assets/animation.db");
            Connection.Open();
            SQLiteCommand cmd = new SQLiteCommand(Connection);
            cmd.CommandText = "SELECT * from anim WHERE actorsno=" + ActorID;
            var reader = cmd.ExecuteReader();
            if (!reader.HasRows) return null;
            reader.Read();
            int start = reader.GetInt32(1); int end = reader.GetInt32(2);
            reader.Close();
            cmd.CommandText = "SELECT * from Animations WHERE animindex >" + start + " AND animindex <" + end;
            reader = cmd.ExecuteReader();
            AnimSet anim = new AnimSet();
            anim.ActorSNO = ActorID;
            while (reader.Read())
            {
                Animation ani = new Animation();
                ani.AniSNO = reader.GetInt32(1);
                ani.name = reader.GetString(2);
                ani.AniTagID = reader.GetInt32(3);
                anim.Animations.Add(ani);
            }
            Connection.Close();
            return anim;
        }

    }
}
