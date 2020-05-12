using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AnnouncementNotifier.Data;
using AnnouncementNotifier.Models;
using Common.Shell;
using Microsoft.EntityFrameworkCore;
using PandaLib.Panda;
using Slack.Webhooks;

namespace AnnouncementNotifier
{
    class Program
    {
        private static SakaiUser Signin()
        {
            string ecsId = ConfigurationManager.AppSettings["EcsId"];
            if (!string.IsNullOrWhiteSpace(ecsId))
            {
                Console.WriteLine("Use ECS ID from configuration.");
            }
            else
            {
                Console.Write("Enter your ECS ID: ");
                ecsId = Console.ReadLine();
            }
            string pass = ConfigurationManager.AppSettings["EcsPass"];

            if (!string.IsNullOrWhiteSpace(pass))
            {
                Console.WriteLine("Use password from configuration.");
            }
            else
            {
                pass = ShellMethods.EnterPassword("Enter your password: ");
            }

            var user = new SakaiUser(ecsId, pass);
            user.LogIn();
            return user;
        }

        static int Main(string[] args)
        {
            bool silent = args.Length == 1 && args[0] == "--silent";
            if (silent)
            {
                Console.WriteLine("Silent mode");
            }
            using (var context = new NotifierContext())
            {
                context.Database.Migrate();
            }

            SakaiUser user;
            try
            {
                user = Signin();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error occuered while sign in: " + ex.Message);
                return -1;
            }

            SakaiSiteCollection sites = user.GetSites();
            var anns = new List<Announcement>();
            foreach (var site in sites.Items)
            {
                var ann = user.GetAnnouncement(site);
                anns.AddRange(ann);
            }

            var messages = new List<SlackMessage>();

            using (var context = new NotifierContext())
            {
                bool changed = false;
                foreach (var announce in anns.OrderByDescending(a => a.CreatedAt))
                {
                    if (context.NotifyHistory.AsNoTracking().Any(h => h.AnnouncementId == announce.Id))
                    {
                        continue;
                    }

                    string body = HtmlUtils.HtmlToPlainText(announce.Body);

                    // Print to console
                    Console.WriteLine($"{announce.Title} -- {announce.SiteTitle}");
                    Console.WriteLine(body);
                    Console.WriteLine(new string('#', 20));
                    context.NotifyHistory.Add(new NotifyHistory
                    {
                        AnnouncementId = announce.Id,
                    });

                    // Send notification to Slack
                    var message = new SlackMessage
                    {
                        IconEmoji = ":panda_face:",
                        Username = announce.SiteTitle,
                        Text = announce.Title + "\n" + HtmlUtils.EncapsulateUrl(body),
                    };
                    messages.Add(message);

                    changed = true;
                }

                if (!silent)
                {
                    var client = new SlackClient(ConfigurationManager.AppSettings["SlackWebhookUrl"]);
                    foreach (var msg in messages)
                    {
                        client.Post(msg);
                    }
                }

                if (changed)
                {
                    context.SaveChanges();
                }
            }
            return 0;
        }
    }
}
