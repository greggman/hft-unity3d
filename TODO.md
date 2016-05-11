TODO
====

*   make starup emit status messages
    *   make object that displays status
        *   checking for server
        *   getting domain
        *   getting certificate
        *   done
        *   disconnected
*   test android
*   test ios
*   don't ping hft.net if installation mode?
*   Add Start/Stop for server
*   Turn on log with showMessages?
*   Turn on log from command line/env
*   refactor for standalone
    *   don't respond to ping until game started
    *   send URL not raw IP because IP6
    *   remove gameId, not needed as only one server
    *   remove allowMultipleGames, not needed as only one server
    *   move name stuff to game
    *   getting rid of __hft__ and/or make reconnect logic different
    *   Check if port busy
        *   inc port
    *   remove Application.Quit
    *   remove GameSystem
    *   move HFTDialog out of HFTDnsRunner
    *   Make PlayerSpawner and PlayerConnector use GameServer.Options directly
    *   remove cwd
    *   remove showInList
    *   remove disonnectPlayersIfGameDisonnects
    *   make instructions work (do I need this?) Maybe this should move to game out of HFT
        *   have default instructions
        *   settable from cmdline/envar
*   Log to console
*   make hft.net keep trying
*   test switching games
*   ../
*   check multi-game
*   make GameServer stop Server?
*   Check lots of clients
*   Add DNS server
    *   Make it return 1 where it's currently returning 0
*   Add captive portal support
*   Make DNS server not on mobile?
*   Remove file uploader
*   Move JS to separate project? Build it into 1 file
*   Remove package.json
*   Remove version stuff
    *   JS
    *   C#
*   Put all samples in same package? (can't because one folder? Fix that shit!)
*   try to support Android captive portal
    *   monitor requests

DONE
====

*   upload controller files if other server? (WontFix)
*   make sure server has started before starting game
*   fix gear icon
*   sound
*   make ask for name work
*   add option to ask for name
*   Add option for no server
*   showMessages
*   add port option?
*   allow setting HFTSite
*   server both / and /games/id/
*   make reconnect work
*   fix close
*   respond to exists ping
*   make index.html redirect to /games/id/controller.html
*   make HFTSite work

