using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
                // This is a kind of hack I'm not sure how to deal with at the moment.
                // The issue is if you call PlayerSpawner/PlayerConnector.Close it will
                // first call GameServer.Close. That disconnects the game. Because the HFTWebServer
                // and the GameServer are connected over websockets it will asynchronously disconnect
                // the game. (GameServer closes socket, HFTGame notices disconnection, removes itself
                // from HFTGameGroup, it's the last game so it removes itself from HFTGameManager.

                // Also HFTManager.Close is called. This shuts down the HFTWebServer which calls
                // HFTGameManager.Stop which deletes all the games groups. Sockets messges happen
                // in different threads which means it's possible for this HFTManager.Close to
                // get a list of games to close and in some other thread for that game to be
                // removed from under it?

                // Or maybe I don't understand the issue fully - nor the solution. In anycase
                // I don't want ot print error messages if we're closing down
                if (!closing_)
                {
                    log_.Error("no game group '" + gameId + "' to remove");
                }
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
            gameGroup.AssignClient(client, data);
        }

        public bool HaveGame()
        {
            return gameCount_ > 0;
        }

        public void Close()
        {
            closing_ = true;
            log_.Info("Close");
            foreach (var group in gameGroups_.Values.ToArray())
            {
                group.Close();
            }
            closing_ = false;
        }

        bool closing_ = false;
        int connectCount_ = 0;
        int gameCount_ = 0;
        HFTLog log_;
        HFTThreadSafeDictionary<string, HFTGameGroup> gameGroups_ = new HFTThreadSafeDictionary<string, HFTGameGroup>();
        static HFTGameManager s_instance;
    }

}
