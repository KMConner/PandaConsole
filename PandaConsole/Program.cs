using System;
using System.Linq;
using System.Collections.Generic;

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

        }
    }
}
