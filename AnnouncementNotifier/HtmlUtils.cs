using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace AnnouncementNotifier
{
    class HtmlUtils
    {
        /// <summary>
        /// Regular expression for URL.
        /// </summary>
        private static readonly Regex urlRegex = new Regex(
            @"((https?|ftp|file)\://|www.)[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string HtmlToPlainText(string html)
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
            return HttpUtility.HtmlDecode(sb.ToString()).Trim();
        }

        /// <summary>
        /// Encapsulate urls in <paramref name="txt"/> with &lt; and  &gt;.
        /// </summary>
        /// <param name="txt">Text with urls.</param>
        /// <returns></returns>
        public static string EncapsulateUrl(string txt)
        {
            StringBuilder txtBuilder = new StringBuilder(txt);
            txtBuilder.Replace("<", "&lt;");
            txtBuilder.Replace(">", "&gt;");
            txtBuilder.Replace("&", "&amp;");
            foreach (var match in urlRegex.Matches(txt).Reverse())
            {
                txtBuilder.Insert(match.Index + match.Length, '>');
                txtBuilder.Insert(match.Index, '<');
            }
            return txtBuilder.ToString();
        }
    }
}