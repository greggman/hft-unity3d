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
    *   remove startServer?
    *   for missing .html file serve missing.html which pings for restart
        *   needs to check it will be redirected to a different place than it already is?
    *   check controllers work on windows
    *   check controller path with / and \
    *   send URL not raw IP because IP6
    *   Check if port busy
        *   inc port
    *   move HFTDialog out of HFTDnsRunner
    *   remove showInList
    *   remove disonnectPlayersIfGameDisonnects
    *   make instructions work (do I need this?) Maybe this should move to game out of HFT
        *   have default instructions
        *   settable from cmdline/envar
*   Log to console
*   move the disconnect handing out of commonUI
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
*   Move JS to separate project? Build it into 1 file
*   Put all samples in same package? (can't because one folder? Fix that shit!)
*   try to support Android captive portal
    *   monitor requests

DONE
====

*   Remove file uploader
*   refactor for standlone
    *   change color to public field
    *   remove enter-name.html
    *   move controller into gamepad subfolder?
    *   move name stuff to game
    *   remove Application.Quit
    *   remove "main menu" from menu
    *   remove GameSystem
    *   Remove package.json
    *   Remove version stuff
        *   JS
        *   C#
    *   don't respond to ping until game started
    *   remove gameId, not needed as only one server
    *   remove allowMultipleGames, not needed as only one server
    *   getting rid of __hft__ and/or make reconnect logic different
    *   Make PlayerSpawner and PlayerConnector use GameServer.Options directly
    *   remove cwd
    *   merge css
    *   add controller folder name to pick controller
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

