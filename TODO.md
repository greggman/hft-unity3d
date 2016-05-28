TODO
====

*   make starup emit status messages
    *   make object that displays status
        *   checking for server
        *   pinging hftsite
        *   getting domain
        *   getting certificate
        *   ready
        *   disconnected
*   test android
*   test ios
*   Add Start/Stop for server
*   refactor for standalone
    *   check all refs to Deserialize, try/catch
    *   make web laucher launch a launcher or not quit so user doesn't have to enter stuff multiple times?
    *   fix readme at build
    *   fix 5.6.7.8 and http://192.168.2.9 hard coded shit
    *   check controllers work on windows
    *   check controller path with / and \
    *   Check if port busy
        *   inc port
    *   remove showInList
    *   remove disonnectPlayersIfGameDisonnects
    *   add dnsAddress and --hft-dns-address ?
    *   check DNS address?
*   link all samples to online docs
*   make hft.net keep trying
*   ../
*   make GameServer stop Server?
*   Check lots of clients
*   Add DNS server
    *   Make it return 1 where it's currently returning 0
*   Add captive portal support
*   Make DNS server not on mobile?
*   try to support Android captive portal
    *   monitor requests

DONE
====

*   Put all samples in same package? (can't because one folder? Fix that shit!)
    *   Objects in scene
*   make instructions work (do I need this?) Maybe this should move to game out of HFT
    (no! Instructions should be up to the dev. Maybe an example would be good? Just
    *   have default instructions
    *   settable from cmdline/envar
*   move the disconnect handing out of commonUI
*   don't ping hft.net if installation mode?
*   Turn on log from command line/env
*   Turn on log with showMessages?
*   Move JS to separate project? Build it into 1 file
*   check multi-game
*   test switching games
*   Log to console (couldn't get this to work)
*   Remove file uploader
*   refactor for standlone
    *   change color to public field
    *   make separate program for webserver?
    *   check arguments so we can print errors to console? Except we can't print errors :(
    *   remove refs to GetEnvironmment
    *   make game-login/captive-portal buttons enter button, not just A
    *   check all args
    *   move HFTDialog out of HFTDnsRunner or at least detach them.
    *   send URL not raw IP because IP6
    *   don't print Checking for HFT unless try > 1
    *   don't print connected to HFT unless debug
    *   remove disconnect from HFTGamepad and add to BirdScript and others
    *   move debug, showMessages to globals with EditorWindow
    *   add debug server flag, different from show messages
    *   inform both ipv6 and ipv4
        *   only report errors if tries > 2 and !success
    *   remove startServer?
    *   for missing .html file serve missing.html which pings for restart
        *   needs to check it will be redirected to a different place than it already is?
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

