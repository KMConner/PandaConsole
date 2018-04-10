using System;
using System.Text;

namespace PandaConsole.Shell
{
    /// <summary>
    /// This class provides item selection feature in the console.
    /// </summary>
    public class ShellSelection
    {
        /// <summary>
        /// The items to select.
        /// </summary>
        private readonly string[] items;

        /// <summary>
        /// The range shown in the cosole.
        /// </summary>
        (int fromIndex, int toIndex) viewRange;

        /// <summary>
        /// The previous forecolor of the console.
        /// </summary>
        static readonly ConsoleColor previousForecolor = Console.ForegroundColor;

        /// <summary>
        /// The color of the previous background of the console.
        /// </summary>
        static readonly ConsoleColor previousBackgroundColor = Console.BackgroundColor;

        /// <summary>
        /// The index of the selected item.
        /// </summary>
        int selectedIndex;

        /// <summary>
        /// The indices to when displaying items.
        /// </summary>
        public static readonly char[] Indices =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
            'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
            'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
            'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
            'U', 'V', 'W', 'X', 'Y', 'Z',
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ShellSelection.ShellSelection"/> class.
        /// </summary>
        /// <param name="items">Items.</param>
        /// <remarks>
        /// <paramref name="items"/> has some restrictions.
        /// <list type="bullet">
        /// <item>The length must be 62 or less.</item>
        /// <item>New line characters will be removed.</item>
        /// </list>
        /// </remarks>
        public ShellSelection(string[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = items[i].Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
            }
            this.items = items;
            viewRange = (0, Math.Min(Console.BufferHeight, items.Length - 1));
        }

        /// <summary>
        /// Show the selection prompt on the console.
        /// </summary>
        /// <returns>The selected index.</returns>
        public int DoSelection()
        {
            Console.Clear();
            Console.CursorVisible = false;
            DecideViewRange();
            Print();

            ConsoleKeyInfo key;
            int tempIndex;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.UpArrow)
                {
                    selectedIndex = Math.Max(0, selectedIndex - 1);
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    selectedIndex = Math.Min(items.Length - 1, selectedIndex + 1);
                }
                else if ((tempIndex = Array.IndexOf(Indices, key.KeyChar)) >= 0 && tempIndex < items.Length)
                {
                    selectedIndex = tempIndex;
                    break;
                }
                else
                {
                    continue;
                }
                DecideViewRange();
                Print();
            }
            Console.CursorVisible = true;
            Console.CursorTop = Math.Min(viewRange.toIndex, items.Length);
            Console.WriteLine();
            return selectedIndex;
        }

        /// <summary>
        /// Print items on the console.
        /// </summary>
        void Print()
        {
            Console.CursorTop = 0;
            Console.CursorLeft = 0;

            for (int i = viewRange.fromIndex; i < viewRange.toIndex; i++)
            {
                Console.CursorTop = i - viewRange.fromIndex;
                Console.CursorLeft = 0;
                string showStr = TrimToWidth(items[i], Console.BufferWidth - 6);
                if (selectedIndex == i)
                {
                    Console.ForegroundColor = previousBackgroundColor;
                    Console.BackgroundColor = previousForecolor;
                    Console.Write($"[{Indices[i]}] {showStr}");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write($"[{Indices[i]}] {showStr}");
                }
                Console.Write(new string(' ', Console.BufferWidth - Console.CursorLeft - GetCharWidth(showStr[showStr.Length - 1])));
            }
        }

        /// <summary>
        /// Decides the view range.
        /// </summary>
        void DecideViewRange()
        {
            // Adjust range width
            viewRange.toIndex = Math.Min(viewRange.fromIndex + Console.BufferHeight - 1, items.Length);

            // Show upper items when selected item is above the console buffer.
            if (selectedIndex < viewRange.fromIndex)
            {
                viewRange.fromIndex = selectedIndex;
                viewRange.toIndex = Math.Min(viewRange.fromIndex + Console.BufferHeight - 1, items.Length);
            }
            // Show lower items when selected item is below the console buffer.
            else if (viewRange.toIndex <= selectedIndex)
            {
                viewRange.toIndex = selectedIndex + 1;
                viewRange.fromIndex = Math.Max(0, viewRange.toIndex - Console.BufferHeight + 1);
            }
        }

        /// <summary>
        /// Trims the string to specified width, append "…" if specified string is longer.
        /// </summary>
        /// <returns>The width.</returns>
        /// <param name="str">String.</param>
        /// <param name="length">Trimed string.</param>
        public string TrimToWidth(string str, int length)
        {
            var builder = new StringBuilder();
            int width = 0;
            foreach (char c in str)
            {
                width += GetCharWidth(c);
                if (width >= length)
                {
                    builder.Append('…');
                    break;
                }
                builder.Append(c);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Gets the width of the specified character.
        /// </summary>
        /// <remarks>
        /// Returns 1 when the specified character is ascii characters or half-width kana.
        /// Returns 2 otherwise.
        /// </remarks>
        /// <returns>The character width.</returns>
        /// <param name="c">The specified character.</param>
        int GetCharWidth(char c)
        {
            #region Inner methods

            bool IsAscii(char ch)
            {
                return ch < 0xF0;
            }

            bool IsHalfwidthKana(char ch)
            {
                return ch >= 0xFF66 && ch <= 0xFFDC;
            }

            #endregion

            if (IsAscii(c)
                || IsHalfwidthKana(c))
            {
                return 1;
            }

            return 2;
        }
    }
}
