using System;
using System.Linq;
using System.IO;
using PandaConsole.Shell;
using PandaConsole.Sakai;

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
            user.LogIn();

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
                    var temp = item.Container.Substring(item.Container.IndexOf(collection.Items[index].Id) + collection.Items[index].Id.Length + 1);
                    dirName = temp.Substring(0, temp.IndexOf("/") + 1);
                }

                var title = collection.Items[index].Title;
                foreach (var invalidChar in Path.GetInvalidFileNameChars())
                {
                    title = title.Replace(invalidChar, '_');
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
