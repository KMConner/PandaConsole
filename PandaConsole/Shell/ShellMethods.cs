using System;
using System.Text;
namespace PandaConsole.Shell
{
    /// <summary>
    /// This class provides features on the console.
    /// </summary>
    static class ShellMethods
    {
        /// <summary>
        /// Enter password without displaying characters.
        /// </summary>
        /// <returns>The user's password.</returns>
        /// <param name="prompt">Text prompt to make user to enter their password.</param>
        public static string EnterPassword(string prompt = null)
        {
            if (!string.IsNullOrWhiteSpace(prompt))
            {
                Console.Write(prompt);
            }
            var builder = new StringBuilder();
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace && builder.Length > 0)
                {
                    builder.Remove(builder.Length - 1, 1);
                }
                else
                {
                    builder.Append(key.KeyChar);
                }
            }
            Console.WriteLine();
            return builder.ToString();
        }
    }
}
