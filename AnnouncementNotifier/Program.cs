using System;
using System.Collections.Generic;
using System.Linq;
using Common.Shell;
using PandaLib.Panda;
using HtmlAgilityPack;
using System.Text;


namespace AnnouncementNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
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

            foreach (var announce in anns.OrderByDescending(a => a.CreatedAt))
            {
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
                Console.WriteLine(sb.ToString());
            }

            Console.WriteLine("Hello World!");
        }
    }
}
