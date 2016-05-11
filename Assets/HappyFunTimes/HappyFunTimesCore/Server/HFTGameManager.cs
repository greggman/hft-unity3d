using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyFunTimes
{
    public class HFTGameManager
    {
        static public HFTGameManager GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new HFTGameManager();
            }

            return s_instance;
        }

        HFTGameManager()
        {
            log_ = new HFTLog("HFTGameManager");
        }

        public HFTGameGroup GetGameGroup(string gameId, bool makeGroup = false)
        {
            HFTGameGroup gameGroup = null;
            if (String.IsNullOrEmpty(gameId))
            {
                log_.Error("no game id");
            }
            else
            {
                if (!gameGroups_.TryGetValue(gameId, out gameGroup))
                {
                    if (makeGroup)
                    {
                        gameGroup = new HFTGameGroup(gameId, this);
                        gameGroups_[gameId] = gameGroup;
                        log_.Info("added game group: " + gameId + ", num game groups = " + gameGroups_.Count);
                    }
                }
            }
            return gameGroup;
        }

        public void RemoveGameGroup(string gameId)
        {
            if (!gameGroups_.Remove(gameId))
            {
                log_.Error("no game group '" + gameId + "' to remove");
                return;
            }
            else
            {
                --gameCount_;
                log_.Info("removed game group: " + gameId + ", num game groups = " + gameGroups_.Count);
            }
        }

        public void AddPlayer(HFTSocket client)
        {
            new HFTPlayer(client, this, (++connectCount_).ToString());
        }

        public HFTGame AddPlayerToGame(HFTPlayer player, string gameId, object data)
        {
            HFTGameGroup gameGroup = GetGameGroup(gameId);
            if (gameGroup == null)
            {
                return null;
            }
            return gameGroup.AddPlayer(player, data);
        }

        public void AssignAsClientForGame(HFTRuntimeOptions data, HFTSocket client)
        {
            string gameId = String.IsNullOrEmpty(data.gameId) ? "HFTUnity" : data.gameId;
            HFTGameGroup gameGroup = GetGameGroup(gameId, true);
            if (!gameGroup.HasClient())
            {
                ++gameCount_;
            }
            HFTGame game = gameGroup.AssignClient(client, data);
            //FIX!
            //if (options_.instructions)
            //{
            //    game.SendInstructions("//FIX", true/* position */);
            //}

        }

        public bool HaveGame()
        {
            return gameCount_ > 0;
        }

        int connectCount_ = 0;
        int gameCount_ = 0;
        HFTLog log_;
        Dictionary<string, HFTGameGroup> gameGroups_ = new Dictionary<string, HFTGameGroup>();
        static HFTGameManager s_instance;
    }

}
