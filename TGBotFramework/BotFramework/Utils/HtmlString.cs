using System.Net;
using System.Text;
using Telegram.Bot.Types;

namespace BotFramework.Utils
{
    public class HtmlString
    {
        private readonly StringBuilder sb = new StringBuilder();

        public static HtmlString Empty => new HtmlString();

        public HtmlString Bold(string text) => TagBuilder("b", text);

        public HtmlString Bold(HtmlString inner) => TagBuilder("b", inner);

        public HtmlString Italic(string text) => TagBuilder("i", text);

        public HtmlString Italic(HtmlString inner) => TagBuilder("i", inner);

        public HtmlString Underline(string text) => TagBuilder("u", text);

        public HtmlString Underline(HtmlString inner) => TagBuilder("u", inner);

        public HtmlString Strike(string text) => TagBuilder("s", text);

        public HtmlString Strike(HtmlString inner) => TagBuilder("s", inner);

        public HtmlString Url(string url, string text) => UrlTagBuilder("a", $"href=\"{url}\"", text);

        public HtmlString User(long id, string text) => Url( $"tg://user?id={id}", text);

        public HtmlString Text(string text)
        {
            sb.Append(WebUtility.HtmlEncode(text));
            return this;
        }

        public HtmlString TextBr(string text) => Text(text + "\n\r");

        public HtmlString Br()
        {
            sb.Append("\n\r");
            return this;
        }

        public HtmlString Append(HtmlString @string)
        {
            sb.Append(@string);
            return this;
        }

        public HtmlString UserMention(User user)
        {
            var name = string.Empty;
            var appended = false;

            if(user.FirstName?.Length > 0)
            {
                name = user.FirstName;
                appended = true;
            }

            if(user.LastName?.Length > 0)
            {
                if(appended)
                {
                    name += " ";
                }

                name += user.LastName;
            }

            var fullName = name.Length > 0 ? name : user.Username;
            return User(user.Id, fullName);
        }

        public HtmlString Code(string text) => TagBuilder("code", text);

        public HtmlString Pre(string text) => TagBuilder("pre", text);

        public HtmlString CodeWithStyle(string style, string text)
        {
            var str = WebUtility.HtmlEncode(text);
            sb.Append($"<pre><code class=\"{style}\">");
            sb.Append(str);
            sb.Append("</code></pre>");
            return this;
        }

        private HtmlString TagBuilder(string tag, string text)
        {
            var str = WebUtility.HtmlEncode(text);
            sb.Append($"<{tag}>");
            sb.Append(str);
            sb.Append($"</{tag}>");
            return this;
        }

        private HtmlString UrlTagBuilder(string tag, string tagParams, string text)
        {
            var str = WebUtility.HtmlEncode(text);
            sb.Append($"<{tag} {tagParams}>");
            sb.Append(str);
            sb.Append($"</{tag}>");
            return this;
        }

        private HtmlString TagBuilder(string tag, HtmlString innerSting)
        {
            sb.Append($"<{tag}>");
            sb.Append(innerSting);
            sb.Append($"</{tag}>");
            return this;
        }

        public override string ToString() => sb.ToString();
    }
}
