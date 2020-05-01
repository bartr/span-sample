using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Unicode;

namespace span
{
    class Program
    {
        const string kv = "bluebelltest-kv";
        const string auth = "CLI";
        const string kvField = "keyvaultName";
        const string authField = "authType";

        static void Main(string[] args)
        {
            int length = 1;

            Stopwatch sw = new Stopwatch();

            string s = string.Empty;

            // run a baseline using string concat
            sw.Start();

            s = "[";

            for (int i = 0; i < length; i++)
            {
                s += '"';
                s += kvField;
                s += "\":\"";
                s += kv;
                s += "\",\"";
                s += authField;
                s += "\":\"";
                s += auth;
                s += "\",";
            }
            s = s[0..^1] + ']';

            sw.Stop();

            Console.WriteLine($"{s.Length}  {sw.ElapsedMilliseconds}");

            if (s.Length < 80)
            {
                Console.WriteLine(s);
            }

            Magic(length);
        }

        // use Span<T> for memory copy
        // it's REALLY fast
        static void Magic(int iterations)
        {
            Console.WriteLine("\nMagic");

            Stopwatch sw = new Stopwatch();

            sw.Start();

            char[] chars = new char[iterations * 64];

            ReadOnlySpan<char> span = new ReadOnlySpan<char>(chars);

            chars[0] = '[';

            int pos = 1;

            for (int i = 0; i < iterations; i++)
            {
                ReadOnlySpan<char> kvSpan = kv;
                ReadOnlySpan<char> authSpan = auth;

                ReadOnlySpan<char> kvfSpan = kvField;
                ReadOnlySpan<char> authfSpan = authField;

                chars[pos++] = '"';

                for (int j = 0; j < kvfSpan.Length; j++) chars[pos++] = kvfSpan[j];
                chars[pos++] = '"';
                chars[pos++] = ':';

                chars[pos++] = '"';

                for (int j = 0; j < kvSpan.Length; j++) chars[pos++] = kvSpan[j];

                chars[pos++] = '"';
                chars[pos++] = ',';

                chars[pos++] = '"';
                for (int j = 0; j < authfSpan.Length; j++) chars[pos++] = authfSpan[j];
                chars[pos++] = '"';
                chars[pos++] = ':';

                chars[pos++] = '"';

                for (int j = 0; j < authSpan.Length; j++) chars[pos++] = authSpan[j];

                chars[pos++] = '"';
                chars[pos++] = ',';
            }

            chars[--pos] = ']';

            string s = span.Slice(0, pos + 1).ToString();


            sw.Stop();

            Console.WriteLine($"{s.Length}  {sw.ElapsedMilliseconds}");

            if (s.Length < 80)
            {
                Console.WriteLine(s);
            }
        }
    }
}
