using AnnouncementNotifier.Data;
using AnnouncementNotifier.Models;
using Common.Shell;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using PandaLib.Panda;
using Slack.Webhooks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AnnouncementNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new NotifierContext())
            {
                context.Database.Migrate();
            }
            Console.WriteLine("Enter your ECS ID: ");
            string ecsId = Console.ReadLine();
            Console.WriteLine("Enter your password: ");
            string pass = ShellMethods.EnterPassword();

            var user = new SakaiUser(ecsId, pass);
            user.LogIn();
            SakaiSiteCollection sites = user.GetSites();
            var anns = new List<Announcement>();
            foreach (var site in sites.Items)
            {
                var ann = user.GetAnnouncement(site);
                anns.AddRange(ann);
                Console.WriteLine(ann.Length);
            }

            string pattern = @"((https?|ftp|file)\://|www.)[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var client = new SlackClient(ConfigurationManager.AppSettings["SlackWebhookUrl"]);

            using (var context = new NotifierContext())
            {
                bool changed = false;
                foreach (var announce in anns.OrderByDescending(a => a.CreatedAt))
                {
                    if (context.NotifyHistory.AsNoTracking().Any(h => h.AnnouncementId == announce.Id))
                    {
                        continue;
                    }

                    Console.WriteLine($"{announce.Title} -- {announce.SiteTitle}");
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(announce.Body);
                    var root = doc.DocumentNode;
                    var sb = new StringBuilder();
                    foreach (var node in root.DescendantsAndSelf())
                    {
                        if (!node.HasChildNodes)
                        {
                            string text = node.InnerText;
                            if (!string.IsNullOrEmpty(text))
                                sb.AppendLine(text.Trim());
                        }
                    }
                    Console.WriteLine(sb.ToString().Trim());
                    Console.WriteLine(new string('#', 20));
                    context.NotifyHistory.Add(new NotifyHistory
                    {
                        AnnouncementId = announce.Id,
                    });

                    string msgText = sb.ToString().Trim();
                    StringBuilder txtBuilder = new StringBuilder(msgText);
                    foreach (var match in reg.Matches(msgText).Reverse())
                    {
                        txtBuilder.Insert(match.Index + match.Length, '>');
                        txtBuilder.Insert(match.Index, '<');
                    }

                    var message = new SlackMessage
                    {
                        IconEmoji = ":panda_face:",
                        Username = announce.SiteTitle,
                        Text = announce.Title + "\n" + txtBuilder.ToString(),
                    };

                    client.Post(message);

                    changed = true;
                }
                if (changed)
                {
                    context.SaveChanges();
                }
            }

            Console.WriteLine("Hello World!");
        }
    }
}
