public static class TextHelper
{
    public static string Shorten(string? text, int max = 80) =>
        string.IsNullOrWhiteSpace(text)
            ? string.Empty
            : text.Length <= max
                ? text
                : $"{text[..max]}...";

    public static string GetCategoryEmoji(string? category) =>
           category?.ToLower() switch
           {
               { } c when c.Contains("plumb") => "🚿",
               { } c when c.Contains("electr") => "⚡",
               { } c when c.Contains("ac") => "❄️",
               { } c when c.Contains("paint") => "🎨",
               { } c when c.Contains("clean") => "🌿",
               { } c when c.Contains("lock") => "🔒",
               { } c when c.Contains("renov") => "🏠",
               _ => "🔧"
           };

    public static string FormatRate(decimal? hourlyRate, decimal? callOutFee)
    {
        if (hourlyRate is null && callOutFee is null)
            return "Harga nego";

        var parts = new List<string>();
        if (hourlyRate is not null)
            parts.Add($"Rp{hourlyRate:N0}/jam");
        if (callOutFee is not null)
            parts.Add($"+Rp{callOutFee:N0} kunjungan");

        return string.Join(" ", parts);
    }

    public static string? GetWhatsAppUrl(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return null;
        // convert 08xx → 628xx
        var normalized = phone.TrimStart().StartsWith("0")
            ? "62" + phone.Substring(1)
            : phone;
        return $"https://wa.me/{normalized}";
    }
}
