using System;
using System.Collections.Generic;
using System.Linq;
using Common.Shell;
using PandaLib.Panda;
using HtmlAgilityPack;
using AnnouncementNotifier.Models;
using System.Text;
using AnnouncementNotifier.Data;
using Microsoft.EntityFrameworkCore;

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
