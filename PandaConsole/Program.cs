using System;
using System.Linq;
using System.IO;

using PandaConsole.Shell;

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
            ShellSelection selection2 = new ShellSelection(resources.Items.Select(i => i.Title).ToArray());
            int resourceIndex = selection2.DoSelection();
            Console.WriteLine(resources.Items[resourceIndex].Title + " was selected.");

            var selectedResource = resources.Items[resourceIndex];

            // Decide path to save resource
            string savePath = $"./temp/{collection.Items[index].Id}/{Path.GetFileNameWithoutExtension(selectedResource.Title)}-{selectedResource.ModifiedDate}{Path.GetExtension(selectedResource.Url)}";

            // Download resource
            user.DownloadResource(selectedResource, savePath);
            Console.WriteLine($"The resource {selectedResource.Title} was saved to {savePath}.");
            Utilities.OpenFileDefaultWithApp(savePath);
        }
    }
}
