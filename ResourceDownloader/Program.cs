using Common.Shell;
using PandaConsole.Shell;
using PandaLib.Panda;
using System;
using System.IO;
using System.Linq;

namespace PandaConsole
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // Authencation
            Console.Write("Enter ECS-ID:");
            string ecsId = Console.ReadLine();
            string password = ShellMethods.EnterPassword("EnterPassword:");
            var user = new SakaiUser(ecsId, password);
            try
            {
                user.LogIn();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to sign in:\r\n" + ex.Message);
                return;
            }

            // Get site list
            SakaiSiteCollection collection = user.GetSites();
            ShellSelection seleciton = new ShellSelection(collection.Items.Select(i => i.Title).ToArray());
            int index = seleciton.DoSelection();
            Console.WriteLine(collection.Items[index].Title + " was selected.");

            // Get Resource List
            SakaiResourceCollection resources = user.GetResources(collection.Items[index]);

            foreach (var item in resources.Items)
            {
                string dirName = string.Empty;
                if (item.Container?.Contains(collection.Items[index].Id) == true)
                {
                    dirName = item.Container.Substring(item.Container.IndexOf(collection.Items[index].Id) + collection.Items[index].Id.Length + 1);
                }

                var title = collection.Items[index].Title;
                foreach (var invalidChar in Path.GetInvalidFileNameChars())
                {
                    title = title.Replace(invalidChar, '_');
                }
                foreach (var invalidChar in Path.GetInvalidPathChars())
                {
                    dirName = dirName.Replace(invalidChar, '_');
                }

                string savePath = $"./PandaResource/{title}/{dirName}{Path.GetFileNameWithoutExtension(item.Title)}{Path.GetExtension(item.Url)}";
                try
                {
                    user.DownloadResource(item, savePath);
                    Console.WriteLine($"The resource {item.Title} was saved to {savePath}.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
