using System.Text;

namespace VRPN2
{
    public class Utility {
        public static string RemoveChars(string s, char[] characters)
        {
            StringBuilder buf = new StringBuilder(s);
            foreach (char c in characters)
            {
                buf.Replace(c.ToString(), "");
            }
            return buf.ToString();
        }
    }
}
