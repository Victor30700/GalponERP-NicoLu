using GalponERP.Application.Common.Extensions;

namespace GalponERP.Application.Common;

public static class EntityResolver
{
    public static (T? Entity, string? SuggestionMessage) Resolve<T>(
        IEnumerable<T> items, 
        string? searchName, 
        Func<T, string> nameSelector, 
        string entityTypeDisplayName) where T : class
    {
        var itemList = items.ToList();

        // 1. Inferencia: Si hay uno solo, seleccionarlo automáticamente (Regla 7)
        if (itemList.Count == 1)
        {
            return (itemList.First(), null);
        }

        // 2. Si no hay nombre para buscar, pedir aclaración
        if (string.IsNullOrWhiteSpace(searchName))
        {
            var options = string.Join("', '", itemList.Select(nameSelector));
            return (null, $"Hay múltiples {entityTypeDisplayName} disponibles: ['{options}']. ¿A cuál de ellos te refieres?");
        }

        // 3. Búsqueda exacta o parcial simple
        var match = itemList.FirstOrDefault(i => nameSelector(i).Equals(searchName, StringComparison.OrdinalIgnoreCase));
        if (match != null) return (match, null);

        // Búsqueda parcial: "Galpon 1" -> "Galpon 1"
        var partialMatches = itemList.Where(i => nameSelector(i).Contains(searchName, StringComparison.OrdinalIgnoreCase)).ToList();
        if (partialMatches.Count == 1) return (partialMatches.First(), null);
        if (partialMatches.Count > 1)
        {
            var options = string.Join("', '", partialMatches.Select(nameSelector));
            return (null, $"Encontré varios {entityTypeDisplayName} que coinciden con '{searchName}': ['{options}']. ¿A cuál te refieres exactamente?");
        }

        // 4. Búsqueda Difusa (Regla 8)
        var fuzzyMatches = itemList
            .Select(i => new { Item = i, Similarity = nameSelector(i).Similarity(searchName) })
            .Where(x => x.Similarity >= 0.4) // Bajamos un poco el umbral para capturar más errores tipográficos
            .OrderByDescending(x => x.Similarity)
            .ToList();

        if (fuzzyMatches.Any())
        {
            if (fuzzyMatches.First().Similarity >= 0.85) // Si es muy parecido, asumimos que es ese
            {
                return (fuzzyMatches.First().Item, null);
            }

            // Si hay varios candidatos cercanos, preguntamos
            var topMatches = fuzzyMatches.Take(3).ToList();
            var suggestions = string.Join("', '", topMatches.Select(x => nameSelector(x.Item)));
            return (null, $"No encontré un {entityTypeDisplayName} llamado exactamente '{searchName}'. ¿Te refieres a ['{suggestions}']?");
        }

        // 5. Fallback total
        var allOptions = string.Join("', '", itemList.Select(nameSelector));
        return (null, $"No logré encontrar ningún {entityTypeDisplayName} que se parezca a '{searchName}'. Las opciones disponibles son: ['{allOptions}']. ¿Cuál de estas deseas usar?");
    }
}
