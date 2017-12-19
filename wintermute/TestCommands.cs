using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using RiotSharp;

namespace testcommands
{
    class TestCommands
    {
        [Command("ping")] // let's define this method as a command
        [Description("Example ping command")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("pong")] // alternative names for the command
        public async Task Ping(CommandContext ctx) // this command takes no arguments
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            // let's make the message a bit more colourful
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            // respond with ping
            await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");
        }

        [Command("last")] 
        [Description("Stats of last LoL game")] 
        [Aliases("lastgame")] // alternative names for the command
        public async Task Last(CommandContext ctx, String user) 
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            // Validate API key
            var api = RiotApi.GetInstance("//API goes here");

            try
            {
                // Get user's Riot information and 10 recent games
                var summoner = api.GetSummoner(Region.na, user);
                var games = summoner.GetRecentGames();

                try
                {
                    var match = api.GetMatch(Region.na, (long)games[0].GameId);
                }
                catch(RiotSharpException ex) {
                    
                    Console.WriteLine(ex); }


                //match.Participants[0].Stats < - How to access stats

                int totalKills = 1;
                
                foreach (RiotSharp.MatchEndpoint.Participant part in match.Participants) {
                    if (part.TeamId == games[0].TeamId)
                    {
                        totalKills += (int)part.Stats.Kills;
                    }
                }
                
                // Calculate KDR
                var dmg = games[0].Statistics.TotalDamageDealt.ToString();

                float KDR = (float)games[0].Statistics.ChampionsKilled / (float)games[0].Statistics.NumDeaths;
                string KD = "```" + "\n" + games[0].Statistics.ChampionsKilled + " / "
                            + games[0].Statistics.NumDeaths + " / "
                            + games[0].Statistics.Assists + " - "
                            + Math.Round(KDR,2) + " KDR ```";

                // Preparing the embed

                var embed = new DiscordEmbed
                {
                    Title = user +"'s Last Game Stats" + " (" + games[0].GameSubType + ")" ,
                    Description = KD,
                    Footer = new DiscordEmbedFooter
                    {
                        Text = "Copyright © 2017 by A. Webster - all rights reserved."
                    },
                    Url = "https://na.op.gg/summoner/userName=" + user,

                };

                // Constructing and populating embed fields
                embed.Fields = new List<DiscordEmbedField>();
                var dmgfield = new DiscordEmbedField();
                dmgfield.Name = "Damage / Kill Participation";
                dmgfield.Value = "`" + games[0].Statistics.TotalDamageDealtToChampions.ToString("#,##0") + " damage - " 
                                + (games[0].Statistics.ChampionsKilled / totalKills) + "% Kill Participation"
                                + "`";

                var towerfield = new DiscordEmbedField();
                towerfield.Name = "Towers Destroyed";
                towerfield.Value = "`" + games[0].Statistics.TurretsKilled + " - " + Math.Round(((float)games[0].Statistics.TurretsKilled / (float)11),2) + "% of enemy towers" +"`";

                var genStatsfield = new DiscordEmbedField();
                genStatsfield.Name = "But did you win?";

                string winStatus;
                if (games[0].Statistics.Win)
                {
                    winStatus = "`Yes! Game won at ";
                }
                else
                    winStatus = "`No. Game lost at ";

                genStatsfield.Value = winStatus + Math.Round(games[0].Statistics.TimePlayed.TotalMinutes) + " minutes.`";

                

                // Add fields to the embed
                embed.Fields.Add(dmgfield);
                embed.Fields.Add(towerfield);
                embed.Fields.Add(genStatsfield);
                

                // Send the embed to Discord
                await ctx.RespondAsync("", embed: embed);
                

            }
            catch (RiotSharpException ex)
            {
                ex.ToString();
            }
           

        }

        [Command("greet"), Description("Says hi to specified user."), Aliases("sayhi", "say_hi")]
        public async Task Greet(CommandContext ctx, [Description("The user to say hi to.")] DiscordMember member) // this command takes a member as an argument; you can pass one by username, nickname, id, or mention
        {
            // note the [Description] attribute on the argument.
            // this will appear when people invoke help for the
            // command.


            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            // let's make the message a bit more colourful
            var emoji = DiscordEmoji.FromName(ctx.Client, ":wave:");

            // and finally, let's respond and greet the user.
            await ctx.RespondAsync($"{emoji} Hello, {member.Mention}!");
        }
    }
}
