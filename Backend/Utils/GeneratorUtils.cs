using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Backend.Utils
{
    public static class GeneratorUtils
    {
        public static string GenerateNumber(int minDecimals, int maxDecimals = 0)
        {
            var rand = new Random();
            if (maxDecimals == 0)
                return rand.Next((int)(Math.Pow(10.0f, minDecimals - 1)), (int)Math.Pow(10.0f, minDecimals)).ToString();
            else
                return rand.Next((int)(Math.Pow(10.0f, minDecimals - 1)), (int)Math.Pow(10.0f, maxDecimals)).ToString();
        }

        public static string GeneratePassword()
        {
            return GenerateNumber(6) + "#Q";
        }

        public static string GenerateAttribute(string attribute, int idx = -1)
        {
            var attr = new List<string>();
            string path = "./Resources/" + attribute + ".txt";
            try
            {
                using (var reader = new StreamReader(path))
                {
                    while (reader.Peek() >= 0)
                        attr.Add(reader.ReadLine());
                    var rand = new Random();
                    if (idx == -1)
                        return attr[rand.Next(0, attr.Count - 1)];
                    else
                        return attr[idx];
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static string GenerateLoremIpsum(int minWords, int maxWords,
        int minSentences, int maxSentences)
        {
            var words = new[]{"a", "ac", "accumsan", "ad", "adipiscing", "aenean", "aliquam", "aliquet",
            "amet", "ante", "aptent", "arcu", "at", "auctor", "augue", "bibendum", "blandit", "class",
            "commodo", "condimentum", "congue", "consectetuer", "consequat", "conubia", "convallis",
            "cras", "cubilia", "cum", "curabitur", "curae;", "cursus", "dapibus", "diam", "dictum",
            "dignissim", "dis", "dolor", "donec", "dui", "duis", "egestas", "eget", "eleifend",
            "elementum", "elit", "enim", "erat", "eros", "est", "et", "etiam", "eu", "euismod",
            "facilisi", "facilisis", "fames", "faucibus", "felis", "fermentum", "feugiat", "fringilla",
            "fusce", "gravida", "habitant", "hendrerit", "hymenaeos", "iaculis", "id", "imperdiet", "in",
            "inceptos", "integer", "interdum", "ipsum", "justo", "lacinia", "lacus", "laoreet", "lectus",
            "leo", "libero", "ligula", "litora", "lobortis", "lorem", "luctus", "maecenas", "magna",
            "magnis", "malesuada", "massa", "mattis", "mauris", "metus", "mi", "molestie", "mollis",
            "montes", "morbi", "mus", "nam", "nascetur", "natoque", "nec", "neque", "netus", "nibh",
            "nisi", "nisl", "non", "nonummy", "nostra", "nulla", "nullam", "nunc", "odio", "orci",
            "ornare", "parturient", "pede", "pellentesque", "penatibus", "per", "pharetra", "phasellus",
            "placerat", "porta", "porttitor", "posuere", "praesent", "pretium", "primis", "proin",
            "pulvinar", "purus", "quam", "quis", "quisque", "rhoncus", "ridiculus", "risus", "rutrum",
            "sagittis", "sapien", "scelerisque", "sed", "sem", "semper", "senectus", "sit", "sociis",
            "sociosqu", "sodales", "sollicitudin", "suscipit", "suspendisse", "taciti", "tellus",
            "tempor", "tempus", "tincidunt", "torquent", "tortor", "tristique", "turpis", "ullamcorper",
            "ultrices", "ultricies", "urna", "ut", "varius", "vehicula", "vel", "velit", "venenatis",
            "vestibulum", "vitae", "vivamus", "viverra", "volutpat", "vulputate"
            };

            var rand = new Random();
            int numSentences = rand.Next(maxSentences - minSentences)
                + minSentences + 1;
            int numWords = rand.Next(maxWords - minWords) + minWords + 1;

            StringBuilder result = new StringBuilder();
            for (int s = 0; s < numSentences; s++)
            {
                for (int w = 0; w < numWords; w++)
                {
                    if (w > 0) { result.Append(" "); }
                    result.Append(words[rand.Next(words.Length)]);
                }
                result.Append(". ");
            }
            return result.ToString();
        }
    }
}