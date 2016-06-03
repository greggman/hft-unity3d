using DeJson;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace HappyFunTimes
{
    public class HFTSystemGame
    {
        // This is all kind of a hack.
        // the original HFT used a separate server running separate from the game.
        // That meant if the game quits the server is still running.
        // A controlller was talking to this server waiting for game to start
        // Rahter than re-write the code I'm just implementing that system except
        // unike the old system since the server and the game are the same thing
        // in this case then when the game quits the server quits.
        // The controller will keep trying to contact the server.
        // Once it does (the game is restarted) then it just sends itself as
        // the running game,
        public HFTSystemGame(HFTGameOptions options)
        {
            options_ = options;

            gameObject_ = new GameObject("HFTSystemGame");
            gameObject_.hideFlags = HideFlags.HideInHierarchy;
            gameObject_.hideFlags = HideFlags.HideInInspector;

            GameServer.Options sysOptions = new GameServer.Options();
            sysOptions.gameId = "__hft__";
            server_ = new GameServer(sysOptions, gameObject_);
            server_.OnPlayerConnect += StartNewPlayer;
            server_.Init();
        }

        void StartPlayer(NetPlayer netPlayer, object data)
        {
            new HFTSystemPlayer(netPlayer, options_);
        }

        void StartNewPlayer(object sender, PlayerConnectMessageArgs e)
        {
            StartPlayer(e.netPlayer, e.data);
        }

        public void Stop()
        {
            //FIX
            server_.Close();
            GameObject.Destroy(gameObject_);
        }

        GameObject gameObject_;
        GameServer server_;
        HFTGameOptions options_;

        class HFTSystemPlayer
        {
            public HFTSystemPlayer(NetPlayer netPlayer, HFTGameOptions options)
            {
                m_netPlayer = netPlayer;
                m_options = options;

                m_netPlayer.RegisterCmdHandler<object>("getRunningGames", GetRunningGames);
            }

            void GetRunningGames(object data)
            {
                // the controller sends this to the server to tell it "hey! send me messages about the running games
                RunningGame[] games = new RunningGame[1];
                RunningGame game = new RunningGame(m_options.getGameId(), m_options.controllerUrl);
                games[0] = game;
                m_netPlayer.SendCmd("runningGames", games);
            }

            class RunningGame
            {
                public RunningGame(string _gameId, string _controllerUrl)
                {
                    gameId = _gameId;
                    controllerUrl = _controllerUrl;
                }
                public string gameId;
                public string controllerUrl;
            }

            HFTGameOptions m_options;
            NetPlayer m_netPlayer;
        }
    }


}  // namespace HappyFunTimes

