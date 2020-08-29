using System;
using System.Net;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GoldenSentry
{
    class GoldenSentry
    {
        //Variable Assignment Sector
        private static DiscordSocketClient _discord;
        private static Configuration _config;
        private static string externalip = new WebClient().DownloadString("https://api.ipify.org");
        private static string LogTime;
        private static int Verbosity;
        private static bool IsBlacklisted;
        private static string Assist;
        private static string OpMan;
        private static string CommandAndControl;
        private static string DumpedInfo;
        private static int ScanInterval;
        private static SocketUser BlacklistInfo;
        private static SocketUser UserInfo;
        private static IRole RoleToAssign;
        private static HttpStatusCode Error;

        static readonly Dictionary<LogSeverity, ConsoleColor> _severityColors = new Dictionary<LogSeverity, ConsoleColor>
        {
                { LogSeverity.Debug, ConsoleColor.Green },
                { LogSeverity.Verbose, ConsoleColor.Blue },
                { LogSeverity.Info, ConsoleColor.Cyan },
                { LogSeverity.Warning, ConsoleColor.Yellow },
                { LogSeverity.Error, ConsoleColor.Red },
                { LogSeverity.Critical, ConsoleColor.Red }
        };

        static async Task Main(string[] args)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.SetWindowSize(146, 52);

            }

            Console.Title = "GoldenSentry Discord IPS Bot v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var arr = new[]
            {
                    @"",
                    @"",
                    @"                    ___.-------.___               ",
                    @"                _.-' ___.--;--.___ `-._           ",
                    @"             .-' _.-'  /  .+.  \  `-._ `-.        ",
                    @"           .' .-'      |-|-o-|-|      `-. `.      ",
                    @"          (_ <O__      \  `+'  /      __O> _)     ",
                    @"            `--._``-..__`._|_.'__..-''_.--'       ",
                    @"                  ``--._________.--''                ",
                    @"",
                    @"",
            };
            Console.WriteLine("\n\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (string line in arr)
            {
                Console.WriteLine(line);
            }
            if (args.Contains("-h") || args.Contains("--help"))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n");
                Console.WriteLine("GoldenSentry Discord IPS Bot v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + $" Console interface By Naix \n");
                Console.WriteLine("Command line options");
                Console.WriteLine("====================");
                Console.WriteLine("");
                Console.WriteLine("Usage: " + Process.GetCurrentProcess().MainModule.FileName + " [arguments]");
                Console.WriteLine("-h | --help Displays this help menu");
                Console.WriteLine("-v | --verbosity <verbosity level> Change the Sniper bot's verbosity level by defualt the verbosity level is 4");
                Console.WriteLine("");
                Console.WriteLine("Verbosity levels");
                Console.WriteLine("=================");
                Console.WriteLine("");
                Console.WriteLine("[0] Prints only critical messages to console log");
                Console.WriteLine("[1] Prints critical and error messages to console log");
                Console.WriteLine("[2] Prints critical,error and warning messages to console log");
                Console.WriteLine("[3] Prints critical,error, warning and info messages to console log");
                Console.WriteLine("[4] Prints critical,error, warning, info and verbose messages to console log");
                Console.WriteLine("[5] Prints critical,error, warning, info, verbose and debug messages to console log");
                Console.ResetColor();
                Environment.Exit(0);
            }

            else if (args.Contains("-v") || args.Contains("--verbosity"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                if (args.Contains("0"))
                {
                    Console.WriteLine("Setting verbose level to 0");
                    Verbosity = 0;
                }
                else if (args.Contains("1"))
                {
                    Console.WriteLine("Setting verbose level to 1");
                    Verbosity = 1;
                }
                else if (args.Contains("2"))
                {
                    Console.WriteLine("Setting verbose level to 2");
                    Verbosity = 2;
                }
                else if (args.Contains("3"))
                {
                    Console.WriteLine("Setting verbose level to 3");
                    Verbosity = 3;
                }
                else if (args.Contains("4"))
                {
                    Console.WriteLine("Setting verbose level to 4");
                    Verbosity = 4;
                }
                else if (args.Contains("5"))
                {
                    Console.WriteLine("Setting verbose level to 5");
                    Verbosity = 5;
                }
                else
                {
                    Console.WriteLine("Invalid argument or verbosity level specified setting verbosity to 4");
                    Verbosity = 4;
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No Options specified setting verbosity to 3");
                Verbosity = 3;
                Console.ResetColor();
            }
            while (true)
            {
                try
                {
                    await AsyncMonitor();
                }
                catch (Exception e)
                {
                    // On fatal error recovery
                    // Print Error and Restart
                    Log(LogSeverity.Critical, e.ToString());
                    Log(LogSeverity.Warning, "Restarting GoldenSentry Discord IPS Bot in 10 seconds...");

                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }


        }

        static async Task AsyncMonitor()
        {
            // load Config
            await LoadConfigAsync();
            // reinitialize Discord client
            _discord?.Dispose();
            if (Verbosity == 0)
            {
                _discord = new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Critical,
                    MessageCacheSize = 5
                }
            );
            }
            else if (Verbosity == 1)
            {
                _discord = new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Error,
                    MessageCacheSize = 5
                }
            );
            }
            else if (Verbosity == 2)
            {
                _discord = new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Warning,
                    MessageCacheSize = 5
                }
            );
            }
            else if (Verbosity == 3)
            {
                _discord = new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Info,
                    MessageCacheSize = 5
                }
            );
            }
            else if (Verbosity == 4)
            {
                _discord = new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 5
                }
            );
            }

            else if (Verbosity == 5)
            {
                _discord = new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Debug,
                    MessageCacheSize = 5
                }
            );
            }
            else
            {
                _discord = new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Info,
                    MessageCacheSize = 5
                }
            );
            }

            if (string.IsNullOrEmpty(_config.AuthToken))
            {
                Log(LogSeverity.Info, $"GoldenSentry Discord IPS Bot version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " By Naix");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("GoldenSentry requires your Bot token in order to proceed.\n" +
                              "A Bot token is a long piece of text that is synonymous to your Discord password.\n" +
                              "\n" +
                              "What happens when you enter your token ?:\n" +
                              "- GoldenSentry will save this token to the disk UNENCRYPTED.\n" +
                              "- GoldenSentry will authenticate to Discord using this token.\n" +
                              "\n" +
                              "GoldenSentry makes no guarantee regarding your account's privacy nor safety.\n" +
                              "\n" +
                              "Proceed? (y/n) ");
                Console.ResetColor();

                if (!Console.ReadKey().KeyChar.ToString().Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    Environment.Exit(1);
                }

                Console.Write("\n Please enter the Token: ");

                _config.AuthToken = Console.ReadLine();

                Console.Clear();

                await SaveConfigAsync();
            }
            // Even Handeling
            _discord.Log += HandleLogAsync;
            _discord.MessageReceived += HandleMessageAsync;


            // login and wait for ready
            var connectionSource = new TaskCompletionSource<object>();

            _discord.Connected += handleConnect;


            Task handleConnect()
            {
                connectionSource.SetResult(null);
                return Task.CompletedTask;
            }

            try
            {
                await _discord.LoginAsync(TokenType.Bot, _config.AuthToken);
            }
            catch (Exception e)
            {
                Log(LogSeverity.Error, e.ToString());

                _config.AuthToken = null;
                await SaveConfigAsync();

                Log(LogSeverity.Error,
                    "Bot token has been erased due to an error while authenticating to Discord.");

                return;
            }
            await _discord.StartAsync();

            await connectionSource.Task;

            _discord.Connected -= handleConnect;

            Log(LogSeverity.Verbose, $"Golden Sentry authenticated as Bot: " + _discord.CurrentUser.Username + " with ID: " + _discord.CurrentUser.Id);

            if (_config.StealthMode == true)
            {
                await _discord.SetStatusAsync(UserStatus.Invisible);
            }
            else if (_config.StealthMode == false)
            {
                await _discord.SetStatusAsync(UserStatus.Online);
            }
            else
            {
                await _discord.SetStatusAsync(UserStatus.Online);
                _config.StealthMode = false;
                Log(LogSeverity.Error, "Something went wrong with Stealth mode ! Variable is: " + _config.StealthMode);
            }
            //Keep Bot running
            while (true)
            {
                Log(LogSeverity.Info, "Beginning IPS Scan routine");
                await ScanServerMemebers();
                await Task.Delay(Convert.ToInt32(TimeSpan.FromSeconds(_config.ScanInterval).TotalMilliseconds));
            }
        }

        static async Task ScanServerMemebers()
        {
            foreach (SocketGuild Guild in _discord.Guilds)
            {
                foreach (SocketGuildUser User in Guild.Users)
                {
                    
                    IsBlacklisted = _config.Blacklist.Contains(User.Id);
                    if (IsBlacklisted == true)
                    {
                        if (_config.Overwatch == true)
                        {
                            //Kick mode
                            await IPSKickUser(User,Guild);
                        }
                        else if (_config.LongWatch == true)
                        {
                            //Ban mode
                            await IPSBanUser(User,Guild);
                        }
                        else
                        {
                            Log(LogSeverity.Info, $"Detected Blacklisted user with ID: {User.Id} with name: {User.Username} in a monitored server with ID: `{Guild.Id}` Guild name: `{Guild.Name}` however Overwatch or longWatch was not enabled therefore doing nothing !");
                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                Log(LogSeverity.Warning, $"Cannot Log event into specified channels because Golden Sentry is currently in Stealth mode !");
                            }
                            else
                            {
                                foreach (ulong ChanId in _config.LoggingChannels)
                                {
                                    if (_discord.GetChannel(ChanId) is ITextChannel LoggingChannel)
                                    {
                                        await LoggingChannel.SendMessageAsync($"Detected **Blacklisted user** with ID: `{User.Id}` with **name**: `{User.Username}` in a monitored **server with ID**: `{Guild.Id}` Guild name: `{Guild.Name}` however Overwatch or longWatch was not enabled therefore doing nothing !");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Log(LogSeverity.Warning, $"Scanned user {User.Username} not blacklisted in Server proceeding to next user !");
                    }

                }
            }

        }

        static async Task IPSKickUser(SocketGuildUser TargetedUser, SocketGuild TargetGuild)
        {
            await TargetedUser.KickAsync("You have been kicked from the server by Golden Sentry Discord IPS due to being blacklisted by the operators \n Please contact my operators to appeal the soft ban");
            if (_config.StealthMode == true)
            {
                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                Log(LogSeverity.Info, $"Golden Sentry Detected Blacklisted User named: {TargetedUser.Username} with ID: {TargetedUser.Id} in Server named: {TargetGuild.Name} with ID: {TargetGuild.Id} during IPS Scan and has Kicked the User from the Server\n Proceeding with IPS Scan");
            }
            else
            {
                Log(LogSeverity.Info, $"Golden Sentry Detected Blacklisted User named: {TargetedUser.Username} with ID: {TargetedUser.Id} in Server named: {TargetGuild.Name} with ID: {TargetGuild.Id} during IPS Scan and has Kicked the User from the Server\n Proceeding with IPS Scan");
                foreach (ulong Channel in _config.LoggingChannels)
                {
                    if (_discord.GetChannel(Channel) is ITextChannel LoggingChannel)
                    {
                        await LoggingChannel.SendMessageAsync($"Golden Sentry Detected Blacklisted User named: {TargetedUser.Username} with ID: {TargetedUser.Id} in Server named: {TargetGuild.Name} with ID: {TargetGuild.Id} during IPS Scan and has Kicked the User from the Server\n Proceeding with IPS Scan");
                    }
                }
            }

        }

        static async Task IPSBanUser(SocketGuildUser TargetedUser, SocketGuild TargetGuild)
        {
            await TargetedUser.BanAsync(0, "You have been Banned from the server by Golden Sentry Discord IPS due to being blacklisted by the operators. \n Please contact my operators to appeal the ban");
            if (_config.StealthMode == true)
            {
                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                Log(LogSeverity.Info, $"Golden Sentry Detected Blacklisted User named: {TargetedUser.Username} with ID: {TargetedUser.Id} in Server named: {TargetGuild.Name} with ID: {TargetGuild.Id} during IPS Scan and has Banned the User from the Server\n Proceeding with IPS Scan");
            }
            else
            {
                Log(LogSeverity.Info, $"Golden Sentry Detected Blacklisted User named: {TargetedUser.Username} with ID: {TargetedUser.Id} in Server named: {TargetGuild.Name} with ID: {TargetGuild.Id} during IPS Scan and has Banned the User from the Server\n Proceeding with IPS Scan");
                foreach (ulong Channel in _config.LoggingChannels)
                {
                    if (_discord.GetChannel(Channel) is ITextChannel LoggingChannel)
                    {
                        await LoggingChannel.SendMessageAsync($"Golden Sentry Detected Blacklisted User named: {TargetedUser.Username} with ID: {TargetedUser.Id} in Server named: {TargetGuild.Name} with ID: {TargetGuild.Id} during IPS Scan and has Banned the User from the Server\n Proceeding with IPS Scan");
                    }
                }
            }
        }
        
        static async Task HandleMessageAsync(SocketMessage message)
        {
            if (!(message is SocketUserMessage userMessage))
                return;

            var author = message.Author;
            Debug.WriteLine("Author ID is: " + author.Id);

            // author is ourselves or authorized user
            if (author.Id == _discord.CurrentUser.Id || _config.AuthorizedUserIDs.Contains(author.Id) )
            {
                await HandleCommandsAsync(userMessage);
            }
        }

        static async Task HandleCommandsAsync(SocketUserMessage message)
        {
            var content = message.Content;

            if (!content.StartsWith(_config.prefix))
            {
                return;
            }

            content = content.Substring(1);

            var delimitor = content.IndexOf(' ');
            var command = delimitor == -1 ? content : content.Substring(0, delimitor);
            var argument = delimitor == -1 ? null : content.Substring(delimitor).Trim();



            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            switch (command.ToLowerInvariant())
            {
                case "assistance":
                    {
                        Log(LogSeverity.Warning, $"{_config.prefix}assistance command was requested by authorised user {message.Author.Username} executing ! \n\n");

                        Log(LogSeverity.Warning, $"KEY: Command - Description.\n");
                        Log(LogSeverity.Debug, $"[== Assistance & information ==]\n");
                        Log(LogSeverity.Info, $"{_config.prefix}assistance - Shows this help prompt for the GoldenSentry");
                        Log(LogSeverity.Info, $"{_config.prefix}server-ip - Shows the IP of server that GoldenSentry is running on");
                        Log(LogSeverity.Info, $"{_config.prefix}ips-status - Shows the current status of GoldenSentry");
                        Log(LogSeverity.Info, $"{_config.prefix}blacklist - Shows a list of Blacklisted User IDs.\n");

                        Log(LogSeverity.Debug, $"[== Operations Management ==]\n");
                        Log(LogSeverity.Info, $"{_config.prefix}toggle-stealth-mode - Cloaks GoldenSentry for extra stealth (This command will stop the bot\n " + $"from replying to your commands in discord while setting it's status to offline!)\n");
                        Log(LogSeverity.Info, $"{_config.prefix}clear-blacklist - Clears all User IDs from the blacklist.\n");
                        Log(LogSeverity.Info, $"{_config.prefix}add-blacklist <id> - Adds a User ID to blacklist.\n");
                        Log(LogSeverity.Info, $"{_config.prefix}remove-blacklist <id> - Removes a User ID from blacklist.\n");
                        Log(LogSeverity.Info, $"{_config.prefix}toggle-overwatch - Turns On/Off Overwatch (Soft banning) and automatically disables Longwatch.\n");
                        Log(LogSeverity.Info, $"{_config.prefix}toggle-longwatch - Turns On/Off Longwatch (Hard banning) and automatically disables Overwatch.\n");
                        Log(LogSeverity.Info, $"{_config.prefix}set-logging-channel - Sets IPS logging on the channel");
                        Log(LogSeverity.Info, $"{_config.prefix}unset-logging-channel - Unsets IPS logging on the channel");
                        Log(LogSeverity.Info, $"{_config.prefix}search-blacklist <id> - Searches the blacklist to return Blacklisted user information based upon specified ID");
                        Log(LogSeverity.Info, $"{_config.prefix}change-command-prefix <prefix> - Changes Golden Sentry's discord command prefix to a desired prefix");
                        Log(LogSeverity.Info, $"{_config.prefix}set-scaninterval <seconds> - Set Golden Sentry member scan intervals (Default: 30 Seconds, Minimum: 10 Seconds, Max: 120 Seconds)");

                        Log(LogSeverity.Debug, $"[== Command and Control Management ==]\n");
                        Log(LogSeverity.Info, $"{_config.prefix}authorize-user-id <id> - Allow another discord account to operate GoldenSentry.");
                        Log(LogSeverity.Info, $"{_config.prefix}unauthorized-user-id <id> - Removes an authorized user from operating GoldenSentry.");
                        Log(LogSeverity.Info, $"{_config.prefix}search-authorized-users <id> - Search the database for an authorized user via User ID.");
                        Log(LogSeverity.Info, $"{_config.prefix}list-authorized-user-ids - Shows a list of Authorized User IDs.");
                        Log(LogSeverity.Info, $"{_config.prefix}assign-role <Role ID> <User ID> - Assign a role to a specified user id via Role Id.");
                        Log(LogSeverity.Info, $"{_config.prefix}poweroff - Shuts down GoldenSentry\n");
                        var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                        Assist = $"```KEY: Command - Description.\n" + $"[== Assistance & information ==]\n" + $"{_config.prefix}assistance - Shows this help prompt for the GoldenSentry\n" + $"{_config.prefix}server-ip - Shows the IP of server that GoldenSentry is running on\n" + $"{_config.prefix}ips-status - Shows the current status of GoldenSentry\n" + $"{_config.prefix}blacklist - Shows a list of Blacklisted User IDs. ```\n";
                        OpMan = $"```[== Operations Management ==]\n" + $"{_config.prefix}toggle-stealth-mode - Cloaks GoldenSentry for extra stealth (This command will stop the bot from replying to your commands in discord while setting it's status to offline!)\n" + $"{_config.prefix}clear-blacklist - Clears all User IDs from the blacklist.\n" + $"{_config.prefix}add-blacklist <id> - Adds a User ID to blacklist.\n" + $"{_config.prefix}remove-blacklist <id> - Removes a User ID from blacklist.\n" + $"{_config.prefix}toggle-overwatch - Turns On/Off Overwatch (Soft banning) and automatically disables Longwatch.\n" + $"{_config.prefix}toggle-longwatch - Turns On/Off Longwatch (Hard banning) and automatically disables Overwatch.\n" + $"{_config.prefix}set-logging-channel - Sets IPS logging on the channel \n" + $"{_config.prefix}unset-logging-channel - Unsets IPS logging on the channel\n"+ $"{_config.prefix}search-blacklist - Searches the blacklist to return Blacklisted user information \n" + $"{_config.prefix}change-command-prefix <prefix> - Changes Golden Sentry's discord command prefix to a desired prefix \n" + $"{_config.prefix}set-scaninterval <seconds> - Set Golden Sentry member scan intervals (Default: 30 Seconds, Minimum: 10 Seconds, Max: 120 Seconds)``` \n";
                        CommandAndControl = $"```[== Command and Control Management ==]\n" + $"{_config.prefix}authorize-user-id <id> - Allow another discord account to operate GoldenSentry.\n" + $"{_config.prefix}unauthorized-user-id <id> - Removes an authorized user from operating GoldenSentry.\n" + $"{_config.prefix}search-authorized-users <id> - Search the database for an authorized user via User ID\n" + $"{_config.prefix}list-authorized-user-ids - Shows a list of Authorized User IDs.\n" + $"{_config.prefix}assign-role <Role ID> <User ID> - Assign a role to a specified user id via Role Id.\n" + $"{_config.prefix}poweroff - Shuts down GoldenSentry ```\n";
                        if (_config.StealthMode == true)
                        {
                            Log(LogSeverity.Warning, $"Currently in stealth mode !");
                            Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                            Log(LogSeverity.Info, $"Help sent to console above");

                        }
                        else
                        {
                            await chan.SendMessageAsync($"assistance command was requested by authorised user {message.Author.Username} executing !\n");
                            await chan.SendMessageAsync(Assist);
                            await chan.SendMessageAsync(OpMan);
                            await chan.SendMessageAsync(CommandAndControl);
                            
                        }
                        //Log(LogSeverity.Info, $"$");
                    }
                    return;

                case "assign-role":
                    {
                        var UserID = argument.Substring(argument.IndexOf(' ') + 1);
                        argument = argument.Remove(argument.IndexOf(' '));
                        var channel = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                        SocketGuildChannel chnl = message.Channel as SocketGuildChannel;
                        RoleToAssign = chnl.Guild.GetRole(Convert.ToUInt64(argument));
                        Log(LogSeverity.Warning, $"{_config.prefix}assign-role command was requested by authorised user {message.Author.Username} executing !");
                        if (RoleToAssign == null)
                        {
                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the {_config.prefix}toggle-stealth-mode command");
                                Log(LogSeverity.Info, $"The role with the ID: {argument} does not exist within this server");
                            }
                            else
                            {
                                await channel.SendMessageAsync($"assign-role command was requested by authorised user {message.Author.Username} executing !");
                                await channel.SendMessageAsync($"The role with the ID: {argument} does not exist within this server");
                            }
                        }
                        else
                        {
                            try
                            {
                                await chnl.Guild.GetUser(Convert.ToUInt64(UserID)).AddRoleAsync(RoleToAssign);
                                if (_config.StealthMode == true)
                                {
                                    Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                    Log(LogSeverity.Warning, $"De-cloak me using the {_config.prefix}toggle-stealth-mode command");
                                    Log(LogSeverity.Info, $"Assigned the User with the ID {UserID} with the role with the ID {argument}");
                                }
                                else
                                {
                                    await channel.SendMessageAsync($"assign-role command was requested by authorised user {message.Author.Username} executing !");
                                    await channel.SendMessageAsync($"Assigned the User with the ID `{UserID}` with the role with the ID `{argument}`");
                                }
                            }
                            catch (Discord.Net.HttpException e)
                            {
                                Error = e.HttpCode;

                                if (Error.ToString() == "Forbidden")
                                {
                                    if (_config.StealthMode == true)
                                    {
                                        Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                        Log(LogSeverity.Warning, $"De-cloak me using the {_config.prefix}toggle-stealth-mode command");
                                        Log(LogSeverity.Info, $"Golden Sentry cannot assign this role with the ID {argument} to the user with ID `{UserID} due to insufficient privileges");
                                    }
                                    else
                                    {
                                        await channel.SendMessageAsync($"assign-role command was requested by authorised user {message.Author.Username} executing !");
                                        await channel.SendMessageAsync($"Golden Sentry cannot assign this role with the ID `{argument}` to the user with ID `{UserID}` due to insufficient privileges");
                                    }
                                }
                            }
                        }
                    }
                    return;

                case "list-authorized-user-ids":
                    {
                        Log(LogSeverity.Warning, $"{_config.prefix}list-authorized-user-ids command was requested by authorised user {message.Author.Username} executing !");
                        var author = message.Author;
                        var channel = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                        if (author.Id == _discord.CurrentUser.Id)
                        {
                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the {_config.prefix}toggle-stealth-mode command");
                                Log(LogSeverity.Info, $"You may not inspect the authorized user database right now, doing so would elimnate the element of stealth");

                            }

                            else
                            {
                                Log(LogSeverity.Info, $"Sending authorized user database");
                                await channel.SendMessageAsync("Authorized Users Ids: \n");
                                foreach (ulong Id in _config.AuthorizedUserIDs)
                                {
                                    await channel.SendMessageAsync($"```{Id}```");
                                }
                            }


                        }
                        else if (_config.AuthorizedUserIDs.Contains(author.Id))
                        {

                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the {_config.prefix}toggle-stealth-mode command");
                                Log(LogSeverity.Info, $"You may not inspect the authorized user database right now, doing so would elimnate the element of stealth");

                            }

                            else
                            {
                                Log(LogSeverity.Info, $"Sending authorized user database");
                                await channel.SendMessageAsync($"list-authorized-user-ids command was requested by authorised user {message.Author.Username} executing !");
                                await channel.SendMessageAsync("Authorized Users Ids: \n");
                                foreach (ulong Id in _config.AuthorizedUserIDs)
                                {
                                    await channel.SendMessageAsync($"```{Id}```");
                                }
                            }
                        }
                    }
                    return;

                case "search-authorized-users":
                    {
                        var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                        Log(LogSeverity.Warning, $"{_config.prefix}search-authorized-users command was requested by authorised user {message.Author.Username} executing ! \n\n");
                        //Code to Search authorized-users list
                        if (_config.AuthorizedUserIDs.Contains(Convert.ToUInt64(argument)) == true)
                        {
                            if (_config.StealthMode == true)
                            {
                                UserInfo = _discord.GetUser(Convert.ToUInt64(argument));
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                Log(LogSeverity.Info, $"The Specified User ID: {argument} exists within the Authorized Users Database!");
                                Log(LogSeverity.Info, $"Discord Username: {UserInfo.Username}");

                            }
                            else
                            {
                                UserInfo = _discord.GetUser(Convert.ToUInt64(argument));
                                await chan.SendMessageAsync($"search-authorized-users command was requested by authorised user {message.Author.Username} executing !");
                                await chan.SendMessageAsync($"The Specified User ID `{argument}` exists within the Authorized Users Database!");
                                DumpedInfo = $"`Discord Username: {UserInfo.Username}`";
                                await chan.SendMessageAsync(DumpedInfo);
                            }
                        }
                        else
                        {
                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                Log(LogSeverity.Info, $"The Specified User ID: {argument} does not exist within the Authorized Users Database");

                            }
                            else
                            {
                                await chan.SendMessageAsync($"search-authorized-users command was requested by authorised user {message.Author.Username} executing !");
                                await chan.SendMessageAsync($"The Specified User ID: `{argument}` does not exist the within Authorized Users Database");

                            }
                        }
                    }
                    return;
                
                case "search-blacklist":
                    {
                        var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                        Log(LogSeverity.Warning, $"{_config.prefix}search-blacklist command was requested by authorised user {message.Author.Username} executing ! \n\n");
                        //Code to Search Blacklist
                        if (_config.Blacklist.Contains(Convert.ToUInt64(argument)) == true)
                        {
                            if (_config.StealthMode == true)
                            {
                                BlacklistInfo = _discord.GetUser(Convert.ToUInt64(argument));
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                Log(LogSeverity.Info, $"The Specified User ID: {argument} exists within the blacklist dumping known information !");
                                Log(LogSeverity.Info, $"Discord Username: {BlacklistInfo.Username}");
                                Log(LogSeverity.Info, $"Discord ID: {BlacklistInfo.Id}");
                                Log(LogSeverity.Info, $"Discord Avatar ID: {BlacklistInfo.AvatarId}");
                                Log(LogSeverity.Info, $"Discord Status: {BlacklistInfo.Status}");
                                Log(LogSeverity.Info, $"Active Discord Clients: {BlacklistInfo.ActiveClients.Count}");

                            }
                            else
                            {
                                BlacklistInfo = _discord.GetUser(Convert.ToUInt64(argument));
                                await chan.SendMessageAsync($"Search-blacklist command was requested by authorised user {message.Author.Username} executing !");
                                await chan.SendMessageAsync($"The Specified User ID `{argument}` exists within the blacklist dumping known information !");
                                DumpedInfo = $"```Discord Username: {BlacklistInfo.Username}\n" + $"Discord ID: {BlacklistInfo.Id} \n" + $"Discord Avatar ID: {BlacklistInfo.AvatarId}\n" + $"Discord Status: {BlacklistInfo.Status} \n" + $"Active Discord Clients: {BlacklistInfo.ActiveClients.Count}``` \n";
                                await chan.SendMessageAsync(DumpedInfo);
                            }
                        }
                        else
                        {
                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                Log(LogSeverity.Info, $"The Specified User ID: {argument} does not exist within the Blacklist Database");

                            }
                            else
                            {
                                await chan.SendMessageAsync($"Search-blacklist command was requested by authorised user {message.Author.Username} executing !");
                                await chan.SendMessageAsync($"The Specified User ID: `{argument}` does not exist within the Blacklist Database");

                            }
                        }
                    }
                    return;

                case "change-command-prefix":
                    {
                        Log(LogSeverity.Warning, $"change-command-prefix command was requested by authorised user {message.Author.Username} executing !");
                        var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                        if (_config.StealthMode == true)
                        {
                            if (argument.Length == 1)
                            {
                                _config.prefix = Convert.ToChar(argument);
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $stealth-mode command");
                                Log(LogSeverity.Info, $"Setting Command Prefix to: " + argument);
                            }
                            else
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $stealth-mode command");
                                Log(LogSeverity.Error, $"The specified Command prefix is not 1 character long !");
                            }
                        }
                        else
                        {
                            if (argument.Length == 1)
                            {
                                _config.prefix = Convert.ToChar(argument);
                                Log(LogSeverity.Info, $"Setting Command Prefix to: " + argument);
                                await chan.SendMessageAsync("Acknowledged Operator: " + message.Author.Username + " !");
                                await chan.SendMessageAsync("Setting Command Prefix to: " + argument);

                            }
                            else
                            {
                                Log(LogSeverity.Error, $"The specified Command prefix is not 1 character long !");
                                await chan.SendMessageAsync("Acknowledged Operator: " + message.Author.Username + " !");
                                await chan.SendMessageAsync("The specified Command prefix is not 1 character long !");

                            }
                        }
                        await SaveConfigAsync();
                    }
                    return;

                case "toggle-longwatch":
                    {
                        var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                        Log(LogSeverity.Warning, $"{_config.prefix}toggle-longwatch command was requested by authorised user {message.Author.Username} executing !");
                        if (_config.Overwatch == true)
                        {
                            _config.Overwatch = false;
                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                Log(LogSeverity.Warning, $"Overwatch is enabled, Disabling !");


                            }
                            else
                            {
                                Log(LogSeverity.Warning, $"Overwatch is enabled, Disabling !");
                                await chan.SendMessageAsync($"**WARNING !** Overwatch is **enabled**, Disabling !");
                            }

                        }
                        if (_config.LongWatch == true)
                        {
                            // Disable Longwatch
                            _config.LongWatch = false;
                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                Log(LogSeverity.Warning, $"Authorised User {message.Author.Username} Disabled LongWatch on Golden Sentry!");


                            }
                            else
                            {
                                Log(LogSeverity.Warning, $"Authorised User {message.Author.Username} Disabled LongWatch on Golden Sentry!");
                                await chan.SendMessageAsync($"Authorised User `{message.Author.Username}` Disabled LongWatch on Golden Sentry!");
                            }
                        }
                        else if (_config.LongWatch == false)
                        {
                            // Enable Longwatch
                            _config.LongWatch = true;
                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                Log(LogSeverity.Warning, $"Authorised User {message.Author.Username} Enabled LongWatch on Golden Sentry!");


                            }
                            else
                            {
                                Log(LogSeverity.Warning, $"Authorised User `{message.Author.Username}` Enabled LongWatch on Golden Sentry!");
                                await chan.SendMessageAsync($"Authorised User `{message.Author.Username}` Enabled LongWatch on Golden Sentry!");
                            }
                        }
                        await SaveConfigAsync();
                    }
                    return;

                case "toggle-overwatch":
                    {
                        var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                        Log(LogSeverity.Warning, $"{_config.prefix}toggle-overwatch command was requested by authorised user {message.Author.Username} executing !");
                        if (_config.LongWatch == true)
                        {
                            _config.LongWatch = false;
                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                Log(LogSeverity.Warning, $"Long watch is enabled, Disabling !");
                                

                            }
                            else
                            {
                                Log(LogSeverity.Warning, $"Long watch is enabled, Disabling !");
                                await chan.SendMessageAsync($"**WARNING !** Long watch is **enabled**, Disabling !");
                            }

                        }
                        if (_config.Overwatch == true)
                        {
                            // Disable Overwatch
                            _config.Overwatch = false;
                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                Log(LogSeverity.Warning, $"Authorised User {message.Author.Username} Disabled Overwatch on Golden Sentry!");


                            }
                            else
                            {
                                Log(LogSeverity.Warning, $"Authorised User {message.Author.Username} Disabled Overwatch on Golden Sentry!");
                                await chan.SendMessageAsync($"Authorised User `{message.Author.Username}` Disabled Overwatch on Golden Sentry!");
                            }
                        }
                        else if (_config.Overwatch == false)
                        {
                            // Enable Overwatch
                            _config.Overwatch = true;
                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                Log(LogSeverity.Warning, $"Authorised User {message.Author.Username} Enabled Overwatch on Golden Sentry!");


                            }
                            else
                            {
                                Log(LogSeverity.Warning, $"Authorised User `{message.Author.Username}` Enabled Overwatch on Golden Sentry!");
                                await chan.SendMessageAsync($"Authorised User `{message.Author.Username}` Enabled Overwatch on Golden Sentry!");
                            }
                        }
                        await SaveConfigAsync();
                    }
                    return;

                case "set-scaninterval":
                    {
                        Log(LogSeverity.Warning, $"{_config.prefix}set-scaninterval command was requested by authorised user {message.Author.Username} executing !");
                        if (int.TryParse(argument, out ScanInterval))
                        {
                            var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                            if (ScanInterval >= 10 && ScanInterval <= 120)
                            {
                                _config.ScanInterval = ScanInterval;
                                if (_config.StealthMode == true)
                                {
                                    Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                    Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                    Log(LogSeverity.Info, $"Setting User IPS Scan interval to {ScanInterval}");
                                }
                                else
                                {
                                    await chan.SendMessageAsync($"Set-scaninterval command was requested by authorised user {message.Author.Username} executing !");
                                    await chan.SendMessageAsync($"Setting User IPS Scan interval to {ScanInterval}");
                                }
                            }
                            else if (ScanInterval < 10)
                            {
                                if (_config.StealthMode == true)
                                {
                                    Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                    Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                    Log(LogSeverity.Error, $"The Scan Interval specified is too low, Minimum Scan interval is 10. Specified Scan interval: {ScanInterval}");
                                }
                                else
                                {
                                    Log(LogSeverity.Error, $"The Scan Interval specified is too low Minimum Scan interval is 10. Specified Scan interval: {ScanInterval}");
                                    await chan.SendMessageAsync($"Set-scaninterval command was requested by authorised user {message.Author.Username} executing !");
                                    await chan.SendMessageAsync($"The Scan Interval specified is too low, Minimum Scan interval is 10. Specified Scan interval: {ScanInterval}");
                                }
                            }
                            else if (ScanInterval > 120)
                            {
                                if (_config.StealthMode == true)
                                {
                                    Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                    Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                    Log(LogSeverity.Error, $"The Scan Interval specified is too High, Maxiumum Scan interval is 120. Specified Scan interval: {ScanInterval}");
                                }
                                else
                                {
                                    Log(LogSeverity.Error, $"The Scan Interval specified is too High Maxiumum Scan interval is 120. Specified Scan interval: {ScanInterval}");
                                    await chan.SendMessageAsync($"Set-scaninterval command was requested by authorised user {message.Author.Username} executing !");
                                    await chan.SendMessageAsync($"The Scan Interval specified is too High, Maxiumum Scan interval is 120. Specified Scan interval: {ScanInterval}");
                                }
                            }

                        }
                        else
                        {
                            var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                Log(LogSeverity.Error, $"Invalid interger in argument: {argument}");
                            }
                            else
                            {
                                Log(LogSeverity.Error, $"Invalid interger in argument: {argument}");
                                await chan.SendMessageAsync($"Set-scaninterval command was requested by authorised user {message.Author.Username} executing !");
                                await chan.SendMessageAsync($"Invalid interger in argument: {argument}");
                            }
                        }
                        await SaveConfigAsync();
                    }
                    return;
                
                case "add-blacklist":
                    {
                        if (_config.Blacklist.Add(Convert.ToUInt64(argument)))
                        {
                            Log(LogSeverity.Warning, $"{_config.prefix}add-blacklist command was requested by authorised user {message.Author.Username} executing !");
                            var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                Log(LogSeverity.Info, $"Blacklisted user with ID:'{argument}' from all servers present.");
                            }
                            else
                            {
                                Log(LogSeverity.Info, $"Blacklisted user with ID:'{argument}' from all servers present.");
                                await chan.SendMessageAsync($"Added user: **{_discord.GetUser(Convert.ToUInt64(argument)).Username}** to the Blacklist");
                            }
                            await SaveConfigAsync();
                        }
                    }
                    return;

                case "remove-blacklist":
                    {
                        if (_config.Blacklist.Remove(Convert.ToUInt64(argument)))
                        {
                            Log(LogSeverity.Warning, $"{_config.prefix}remove-blacklist command was requested by authorised user {message.Author.Username} executing !");
                            var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;

                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the $toggle-stealth-mode command");
                                Log(LogSeverity.Info, $"Whitelisted user with ID:'{argument}' from all servers present.");
                            }
                            else
                            {
                                Log(LogSeverity.Info, $"Whitelisted user with ID:'{argument}' from all servers present.");
                                await chan.SendMessageAsync($"Removed user: **{_discord.GetUser(Convert.ToUInt64(argument)).Username}** from the Blacklist");
                            }
                            await SaveConfigAsync();
                        }
                    }
                    return;

                case "toggle-stealth-mode":
                    {
                        Log(LogSeverity.Warning, $"{_config.prefix}stealth-mode command was requested by authorised user {message.Author.Username} executing !");
                        var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                        if (_config.StealthMode == true)
                        {
                            await _discord.SetStatusAsync(UserStatus.Online);
                            _config.StealthMode = false;
                            Log(LogSeverity.Info, $"Invisibility cloak disengaged !");
                            await chan.SendMessageAsync("Acknowledged Operator: " + message.Author.Username + " !");
                            await chan.SendMessageAsync("Disabling stealth operations mode !");
                        }
                        else if (_config.StealthMode == false)
                        {
                            await _discord.SetStatusAsync(UserStatus.Invisible);
                            _config.StealthMode = true;
                            Log(LogSeverity.Info, $"Invisibility cloak engaged !");
                            await chan.SendMessageAsync("Acknowledged Operator: " + message.Author.Username + " !");
                            await chan.SendMessageAsync("Enabling stealth operations mode !");
                        }
                        else
                        {
                            await _discord.SetStatusAsync(UserStatus.Online);
                            Log(LogSeverity.Error, $"Something went wrong with stealth  mode variable :" + _config.StealthMode + " Resetting Variable now !");
                            _config.StealthMode = false;
                        }
                        await SaveConfigAsync();
                    }
                    return;

                case "ips-status":
                    {
                        Log(LogSeverity.Warning, $"{_config.prefix}ips-status command was requested by authorised user {message.Author.Username} executing !");
                        var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                        if (_config.StealthMode == true)
                        {
                            Log(LogSeverity.Warning, $"Currently in Stealth mode !");
                            Log(LogSeverity.Warning, $"De-cloak me using the {_config.prefix}toggle-stealth-mode command");
                            Log(LogSeverity.Info, $"Printing the status would give away my position!");
                        }
                        else
                        {
                            await chan.SendMessageAsync($"Ips-status command was requested by authorised user {message.Author.Username} executing !");
                            Log(LogSeverity.Info, $"Printing Golden Sentry Status");
                            await chan.SendMessageAsync($"**Golden Sentry Discord Intrustion Prevention System (IPS) Bot By Naix**");
                            await chan.SendMessageAsync($"**Version:** {Assembly.GetExecutingAssembly().GetName().Version.ToString()}");
                            await chan.SendMessageAsync($"**Golden Sentry Status Screen** \n" 
                                + "**Stealth mode**: OFF !");
                            if (_config.Overwatch == true)
                            {
                                await chan.SendMessageAsync($"Overwatch Status: **ONLINE !**");
                            }
                            else if (_config.Overwatch == false)
                            {
                                await chan.SendMessageAsync($"Overwatch Status: **OFFLINE !**");
                            }
                            if (_config.LongWatch == true)
                            {
                                await chan.SendMessageAsync($"LongWatch Status: **ONLINE !**");
                            }
                            else if (_config.LongWatch == false)
                            {
                                await chan.SendMessageAsync($"LongWatch Status: **OFFLINE !**");
                            }
                            SocketGuildChannel chnl = message.Channel as SocketGuildChannel;
                            await chan.SendMessageAsync($"Current User count in Server: `{chnl.Guild.Users.Count}` ");
                        }
                    }
                    return;

                case "set-logging-channel":
                    Log(LogSeverity.Warning, $"{_config.prefix}set-sniper-channel command was requested by authorised user {message.Author.Username} executing !");
                    if (_config.LoggingChannels.Add(message.Channel.Id))
                    {
                        var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;

                        if (_config.StealthMode == true)
                        {
                            Log(LogSeverity.Warning, $"Currently in stealth mode !");
                            Log(LogSeverity.Warning, $"De-cloak me using the {_config.prefix}toggle-stealth-mode command");
                            Log(LogSeverity.Info, $"Setting up IPS logging on channel '{message.Channel.Id}'.");
                        }
                        else
                        {
                            Log(LogSeverity.Info, $"Setting up IPS logging on channel '{message.Channel.Id}'.");
                            await chan.SendMessageAsync("Setting up IPS logging on this channel.");
                        }
                        await SaveConfigAsync();
                    }
                    return;

                case "unset-logging-channel":
                    {
                        Log(LogSeverity.Warning, $"{_config.prefix}unset-sniper-channel command was requested by authorised user {message.Author.Username} executing !");
                        if (_config.LoggingChannels.Remove(message.Channel.Id))
                        {
                            var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;

                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the {_config.prefix}toggle-stealth-mode command");
                                Log(LogSeverity.Info, $"Removed IPS logging on channel '{message.Channel.Id}'.");
                            }
                            else
                            {
                                Log(LogSeverity.Info, $"Removed IPS logging on channel '{message.Channel.Id}'.");
                                await chan.SendMessageAsync("Removing IPS logging on this channel.");
                            }
                            await SaveConfigAsync();
                        }
                        if (_config.StealthMode == true)
                        {
                            await message.DeleteAsync();
                        }
                        else
                        {

                        }
                    }
                    return;

                case "blacklist":
                    {
                        Log(LogSeverity.Warning, $"{_config.prefix}blacklist command was requested by authorised user {message.Author.Username} executing !");
                        var author = message.Author;
                        var channel = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                        if (author.Id == _discord.CurrentUser.Id)
                        {
                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the {_config.prefix}toggle-stealth-mode command");
                                Log(LogSeverity.Info, $"You may not inspect the blacklist right now, doing so would elimnate the element of stealth");

                            }

                            else
                            {
                                Log(LogSeverity.Info, $"Sending blacklist");
                                await channel.SendMessageAsync("Blacklisted Users Ids: \n");
                                foreach (ulong Id in _config.Blacklist)
                                {
                                    await channel.SendMessageAsync($"```{Id}```");
                                }
                            }


                        }
                        else if (_config.AuthorizedUserIDs.Contains(author.Id))
                        {

                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the {_config.prefix}toggle-stealth-mode command");
                                Log(LogSeverity.Info, $"You may not inspect the blacklist right now, doing so would elimnate the element of stealth");

                            }

                            else
                            {
                                Log(LogSeverity.Info, $"Sending Blacklist");
                                await channel.SendMessageAsync($"Blacklist command was requested by authorised user {message.Author.Username} executing !");
                                await channel.SendMessageAsync("Blacklisted Users Ids: \n");
                                foreach (ulong Id in _config.Blacklist)
                                {
                                    await channel.SendMessageAsync($"```{Id}```");
                                }
                            }
                        }
                    }
                    return;
                case "poweroff":
                    {
                        Log(LogSeverity.Warning, $"{_config.prefix}poweroff command was requested by authorised user {message.Author.Username} executing !");
                        var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                        if (_config.StealthMode == true)
                        {
                            Log(LogSeverity.Warning, $"Currently in Stealth mode ! ");
                            Log(LogSeverity.Warning, $"Shutting down Golden Sentry !");
                        }
                        else
                        {
                            await chan.SendMessageAsync("Acknowledged Commander: " + message.Author.Username + " !");
                            await chan.SendMessageAsync("Discord Intrustion prevntion: **OFFLINE !**");
                            await chan.SendMessageAsync($"Thank you for using Golden Sentry Discord Intrusion Prevention System v{Assembly.GetExecutingAssembly().GetName().Version.ToString()} by Naix");
                            await chan.SendMessageAsync("Shutting down Golden Sentry Discord IPS !");
                            Log(LogSeverity.Warning, $"Shutting down Golden Sentry !");
                        }
                        await SaveConfigAsync();
                        Environment.Exit(0);

                    }
                    return;


                case "server-ip":
                    {
                        Log(LogSeverity.Warning, $"server-ip command was requested by authorised user {message.Author.Username} executing !");
                        externalip = new WebClient().DownloadString("https://api.ipify.org");
                        var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;

                        if (_config.StealthMode == true)
                        {
                            Log(LogSeverity.Warning, $"Currently in stealth mode !");
                            Log(LogSeverity.Warning, $"De-cloak me using the {_config.prefix}toggle-stealth-mode command");
                            Log(LogSeverity.Info, $"Server is Running on IP : '{externalip}' ");
                        }
                        else
                        {
                            await chan.SendMessageAsync($"Server-ip command was requested by authorised user {message.Author.Username} executing !");
                            await chan.SendMessageAsync($"Server-IP sent to server log console for privacy reasons");
                            Log(LogSeverity.Info, $"Server is Running on IP : '{externalip}' ");
                        }
                    }
                    return;

                case "clear-blacklist":
                    {
                        Log(LogSeverity.Warning, $" [DANGEROUS CMD] {_config.prefix}clear-blacklist command was requested by authorised user {message.Author.Username} executing !");
                        _config.Blacklist.Clear();
                        var channel = _discord.GetChannel(message.Channel.Id) as IMessageChannel;

                        if (_config.StealthMode == true)
                        {
                            Log(LogSeverity.Warning, $"Currently in stealth mode !");
                            Log(LogSeverity.Warning, $"De-cloak me using the {_config.prefix}toggle-stealth-mode command");
                            Log(LogSeverity.Info, "Cleared all User IDs from blacklist.");
                        }
                        else
                        {
                            await channel.SendMessageAsync($"clear-blacklist Command requested by authorised {message.Author.Username} executing !");
                            Log(LogSeverity.Warning, $"Cleared all characters and animes Watchlist.");
                            await channel.SendMessageAsync("Cleared all User IDs from blacklist");
                        }
                        await SaveConfigAsync();
                    }
                    return;

                case "authorize-user-id":
                    {
                        if (_config.AuthorizedUserIDs.Add(Convert.ToUInt64(argument)))
                        {
                            Log(LogSeverity.Warning, $"{_config.prefix}authorize-user-id command was requested by authorised user {message.Author.Username} executing !");
                            var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;
                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the {_config.prefix}toggle-stealth-mode command");
                                Log(LogSeverity.Info, $"Authorized user ID:'{argument}' to Golden Sentry.");
                            }
                            else
                            {
                                Log(LogSeverity.Info, $"Authorized user ID:'{argument}' to operate Golden Sentry.");
                                await chan.SendMessageAsync("Authorized Operator to Operate Golden Sentry !");
                            }
                            await SaveConfigAsync();
                        }
                    }
                    return;

                case "unauthorize-user-id":
                    {
                        if (_config.AuthorizedUserIDs.Remove(Convert.ToUInt64(argument)))
                        {
                            Log(LogSeverity.Warning, $"{_config.prefix}unauthorize-user-id command was requested by authorised user {message.Author.Username} executing !");
                            var chan = _discord.GetChannel(message.Channel.Id) as IMessageChannel;

                            if (_config.StealthMode == true)
                            {
                                Log(LogSeverity.Warning, $"Currently in stealth mode !");
                                Log(LogSeverity.Warning, $"De-cloak me using the {_config.prefix}toggle-stealth-mode command");
                                Log(LogSeverity.Info, $"Removed Authorized user ID: '{argument}' from operating Golden Sentry.");
                            }
                            else
                            {
                                Log(LogSeverity.Info, $"Removed Authorized user ID: '{argument}' from operating Golden Sentry.");
                                await chan.SendMessageAsync("Remove Authorized Operator from Operating Golden Sentry !");
                            }
                            await SaveConfigAsync();
                        }
                    }
                    return;
            }

            if (_config.StealthMode == true)
            {
                await message.DeleteAsync();
            }
            else
            {

            }
            await SaveConfigAsync();

        }
        static async Task LoadConfigAsync()
        {
            try
            {
                Log(LogSeverity.Verbose, $"Trying to Load Configuration file");
                _config = JsonConvert.DeserializeObject<Configuration>(await File.ReadAllTextAsync("GoldenSentry config.json"));
                Log(LogSeverity.Debug, $"Configuration Loaded !");
            }
            catch (FileNotFoundException)
            {
                Log(LogSeverity.Error, $"Configuration file not found, creating the file");
                _config = new Configuration();
            }
        }
        static void Log(LogSeverity severity, string message)
        {
            lock (_logLock)
            {
                var oldColor = Console.ForegroundColor;
                LogTime = Convert.ToString(DateTime.Now);

                if (_severityColors.TryGetValue(severity, out var newColor))
                    Console.ForegroundColor = newColor;

                Console.WriteLine($"{$"[{severity}] [{LogTime}] ".PadRight(10, ' ')} {message.Trim()}");

                Console.ForegroundColor = oldColor;
            }
        }

        static readonly object _logLock = new object();
        static Task SaveConfigAsync() => File.WriteAllTextAsync("GoldenSentry config.json", JValue.Parse(JsonConvert.SerializeObject(_config)).ToString());

        static void LogTargeting(LogSeverity severity, string message)
        {
            lock (_logLock)
            {
                var oldColor = Console.ForegroundColor;
                LogTime = Convert.ToString(DateTime.Now);
                var newColor = ConsoleColor.Magenta;
                Console.ForegroundColor = newColor;
                Console.WriteLine($"{$"[Targeting!] [{LogTime}] ".PadRight(10, ' ')} {message.Trim()}");
                Console.ForegroundColor = oldColor;
            }
        }

        static Task HandleLogAsync(LogMessage m)
        {
            var message = m.Exception == null
                ? m.Message
                : $"{m.Message}: {m.Exception}";

            Log(m.Severity, message);

            return Task.CompletedTask;
        }

    }
}
