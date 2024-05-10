using System.Text.RegularExpressions;

class Program
{
    const int TopWordsCount = 20;
    const int TopNeighborWordsCount = 5;
    const string TextFileUrl = "https://www.gutenberg.org/files/4300/4300-0.txt";

    static async Task Main()
    {
        // Get the text file
        var text = await GetTextAsync(TextFileUrl);

        // Get all words from the text
        var words = GetWords(text);

        // Get the top words
        var topWords = GetTopWords(words, TopWordsCount);

        Console.WriteLine($"Top {TopWordsCount} Words from {TextFileUrl}");
        Console.WriteLine();

        var i = 0;
        foreach (var pair in topWords)
        {
            // Display the top word and its count
            Console.Write($"{++i}) {pair.Key} ({pair.Value}): ");

            // Get top neighboring words
            var neighborWords = GetTopNeighborWords(pair.Key, words, TopNeighborWordsCount);

            // Display the top neighboring words and their counts
            DisplayNeighborWords(neighborWords);
        }
    }

    static async Task<string> GetTextAsync(string url)
    {
        using (var client = new HttpClient())
        {
            return await client.GetStringAsync(url);
        }
    }

    static string[] GetWords(string text)
    {
        return Regex.Matches(text, @"\b(?:[a-z]{2,}|[ai])\b", RegexOptions.IgnoreCase)
                    .Select(Match => Match.Value.ToLower())
                    .ToArray();
    }

    static Dictionary<string, int> GetTopWords(string[] words, int count)
    {
        var wordsWithCounts = new Dictionary<string, int>();
        foreach (string word in words)
        {
            if (wordsWithCounts.ContainsKey(word))
                wordsWithCounts[word]++;
            else
                wordsWithCounts[word] = 1;
        }

        return wordsWithCounts.OrderByDescending(pair => pair.Value)
                              .Take(count)
                              .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    static Dictionary<string, int> GetTopNeighborWords(string word, string[] words, int count)
    {
        var neighborWords = new Dictionary<string, int>();

        for (var i = 0; i < words.Length; i++)
        {
            if (words[i] == word)
            {
                // Get left neighbor word
                if (i > 0)
                    AddNeighborWord(words[i - 1], neighborWords);

                // Get rigth neighbor word
                if (i < words.Length - 1)
                    AddNeighborWord(words[i + 1], neighborWords);
            }
        }

        return neighborWords.OrderByDescending(pair => pair.Value)
                            .Take(count)
                            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    static void AddNeighborWord(string word, Dictionary<string, int> neighborWords)
    {
        if (neighborWords.ContainsKey(word))
            neighborWords[word]++;
        else
            neighborWords[word] = 1;
    }

    static void DisplayNeighborWords(Dictionary<string, int> neighborWords)
    {
        Console.Write("  ");
        foreach (var pair in neighborWords)
        {
            Console.Write($"{pair.Key} ({pair.Value})  ");
        }
        Console.WriteLine();
    }
}