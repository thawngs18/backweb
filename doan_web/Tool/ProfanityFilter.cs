using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ProfanityFilter
{
    private readonly HashSet<string> _badWords;

    public ProfanityFilter(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        _badWords = File.ReadAllLines(filePath)
                        .Where(line => !string.IsNullOrWhiteSpace(line)) // Loại bỏ dòng trống
                        .Select(word => word.Trim().ToLower()) // Loại bỏ khoảng trắng và chuyển về chữ thường
                        .ToHashSet();
    }

    public bool ContainsProfanity(string content)
    {
        var words = content.Split(new[] { ' ', '.', ',', '!', '?', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
        return words.Any(word => _badWords.Contains(word.ToLower()));
    }
}
