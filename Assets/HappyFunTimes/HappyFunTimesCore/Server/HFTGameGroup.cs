using System.Collections.Generic;
using System;
using System.Text;

namespace HappyFunTimes
{
    public class HFTGameGroup
    {

        /**
         * Represents a group of games.
         *
         * Normally a group only has 1 game but
         * for situations where we need more than
         * 1 game ...
         *
         */
        public HFTGameGroup(string gameId, HFTGameManager relayServer)
        {
            log_ = new HFTLog("HFTGameGroup[" + gameId + "]");
            gameId_ = gameId;
            //this.runtimeInfo = gameDB.getGameById(gameId);
            relayServer_ = relayServer;
        }

        HFTGame GetGameById(string id)
        {
            HFTGame game = null;
            games_.TryGetValue(id, out game);
            return game;
        }

        HFTGame GetAnyGame()
        {
            return games_.GetAnyValue();
        }

        public void RemoveGame(HFTGame game)
        {
            List<string> itemsToRemove = new List<string>();

            foreach (var pair in games_.GetAll())
            {
                if (pair.Value == game)
                {
                    itemsToRemove.Add(pair.Key);
                }
            }

            foreach (var item in itemsToRemove)
            {
                games_.Remove(item);
            }

            if (game == masterGame_)
            {
                // Pick a new master
                masterGame_ = GetAnyGame();
            }

            foreach (var otherGame in games_.Values)
            {
                otherGame.SendGameDisconnect(game);
            }

            log_.Info("remove game: num games = " + games_.Count);

            if (games_.Count == 0)
            {
                relayServer_.RemoveGameGroup(gameId_);
            }
        }

        public HFTGame AddPlayer(HFTPlayer player, object data)
        {
            if (masterGame_ != null)
            {
                return AddPlayerToGame(player, masterGame_.id, data);
            }
            log_.Error("no games to add player to");
            return null;
        }

        /**
         * Adds a player to a specific game.
         * @param {Player} player to add
         * @param {string} id
         * @param {Object?} data add to send in connect msg
         */
        public HFTGame AddPlayerToGame(HFTPlayer player, string id, object data)
        {
            HFTGame game = GetGameById(id);
            if (game != null)
            {
                game.AddPlayer(player, data);
                return game;
            }
            log_.Error("no game with id '" + id + "'");
            return null;
        }

        /**
         *
         * @param {!Client} client Websocket client that's connected to
         *        the game.
         * @param {!RelayServer} relayserver relayserver the game is
         *        connected to.
         * @param {Game~GameOptions} data Data sent from the game which
         *        includes
         */
        public HFTGame AssignClient(HFTSocket client, HFTRuntimeOptions data)
        {
            // If there are no games make one
            // If multiple games are allowed make one
            // If multiple games are not allowed re-assign
            string newGameId = !String.IsNullOrEmpty(data.id) ? data.id : ("_hft_" + nextGameId_++);
            HFTGame game = new HFTGame(newGameId, this, data);
            // Add it to 'games' immediately because if we remove the old game games would go to 0
            // for a moment and that would trigger this GameGroup getting removed because there'd be no games
            if (masterGame_ == null)
            {
                masterGame_ = game;
            }
            HFTGame oldGame = null;
            // See if there's an old game with the same id then replace it
            games_.TryGetValue(newGameId, out oldGame);
            games_[newGameId] = game;

            if (oldGame != null)
            {
                log_.Info("tell old game to quit");
                oldGame.SendQuit();
                oldGame.Close();
            }

            log_.Info("add game: num games = " + games_.Count);
            game.AssignClient(client, data);

            if (data.master)
            {
                masterGame_ = game;
            }

            return game;
        }

        //GameGroup.prototype.addFiles = function(files) {
        //  this.relayServer.addFilesForGame(this.gameId, files);
        //};

        public bool HasClient()
        {
            return masterGame_ != null && masterGame_.HasClient();
        }

        bool ShowInList()
        {
            return masterGame_ != null && masterGame_.HasClient() && masterGame_.ShowInList();
        }

        int GetNumPlayers()
        {
            int numPlayers = 0;
            foreach (var game in games_.Values)
            {
                numPlayers += game.GetNumPlayers();
            }
            return numPlayers;
        }

        void SendQuit()
        {
            foreach (var game in games_.Values)
            {
                game.SendQuit();
            }
        }

        void DisconnectGames()
        {
            foreach (var game in games_.Values)
            {
                game.Close();
            }
        }

        public void Close()
        {
            DisconnectGames();
        }

        public void SendMessageToGame(string senderId, string receiverId, object data)
        {
            // this is lame! should change to ids like player.
            HFTGame game = GetGameById(receiverId);
            if (game != null)
            {
                game.Send(null, new HFTRelayToGameMessage("upgame", senderId, data));
            }
            else
            {
                log_.Warn("no game for id: " + receiverId);
            }
        }

        public void BroadcastMessageToGames(string senderId, string receiverId, object data)
        {
            foreach (var game in games_.Values)
            {
                game.Send(null, new HFTRelayToGameMessage("upgame", senderId, data));
            }
        }

        HFTLog log_;
        string gameId_;
        HFTGame masterGame_;
        HFTGameManager relayServer_;
        HFTThreadSafeDictionary<string, HFTGame> games_ = new HFTThreadSafeDictionary<string, HFTGame>();
        int nextGameId_ = 0;
    }

}  // namespace HappyFunTimes

