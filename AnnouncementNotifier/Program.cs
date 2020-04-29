using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AnnouncementNotifier.Data;
using AnnouncementNotifier.Models;
using Common.Shell;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using PandaLib.Panda;
using Slack.Webhooks;

namespace AnnouncementNotifier
{
    class Program
    {
        /// <summary>
        /// Regular expression for URL.
        /// </summary>
        private static readonly Regex urlRegex = new Regex(
            @"((https?|ftp|file)\://|www.)[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Parse HTML and convert into plain text.
        /// </summary>
        /// <param name="html">HTML to convert from.</param>
        /// <returns>Plain text.</returns>
        private static string Html2Text(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
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
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Encapsulate urls in <paramref name="txt"/> with &lt; and  &gt;.
        /// </summary>
        /// <param name="txt">Text with urls.</param>
        /// <returns></returns>
        private static string EncapsulateUrl(string txt)
        {
            StringBuilder txtBuilder = new StringBuilder(txt);
            foreach (var match in urlRegex.Matches(txt).Reverse())
            {
                txtBuilder.Insert(match.Index + match.Length, '>');
                txtBuilder.Insert(match.Index, '<');
            }
            return txtBuilder.ToString();
        }

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

                    string body = Html2Text(announce.Body);

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
                        Text = announce.Title + "\n" + EncapsulateUrl(body),
                    };
                    client.Post(message);

                    changed = true;
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
