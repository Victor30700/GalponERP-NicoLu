namespace GalponERP.Application.Common.Extensions;

public static class StringExtensions
{
    public static int LevenshteinDistance(this string s, string t)
    {
        if (string.IsNullOrEmpty(s)) return string.IsNullOrEmpty(t) ? 0 : t.Length;
        if (string.IsNullOrEmpty(t)) return s.Length;

        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; d[i, 0] = i++) ;
        for (int j = 0; j <= m; d[0, j] = j++) ;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }
        return d[n, m];
    }

    public static double Similarity(this string s, string t)
    {
        if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(t)) return 0;
        int distance = s.LevenshteinDistance(t);
        return 1.0 - ((double)distance / Math.Max(s.Length, t.Length));
    }

    public static bool IsFuzzyMatch(this string s, string t, double threshold = 0.6)
    {
        if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(t)) return false;
        
        // 1. Coincidencia exacta o contiene (case-insensitive)
        if (s.Contains(t, StringComparison.OrdinalIgnoreCase) || t.Contains(s, StringComparison.OrdinalIgnoreCase))
            return true;

        // 2. Similaridad Levenshtein
        return s.Similarity(t) >= threshold;
    }
}
