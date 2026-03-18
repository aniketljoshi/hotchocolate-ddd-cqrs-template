using System.Threading;

namespace HotChocolateDddCqrsTemplate.Infrastructure.Observability;

public sealed class CatalogQueryCounter
{
    private int _categoryQueryCount;

    public int CategoryQueryCount => Volatile.Read(ref _categoryQueryCount);

    public void Reset()
    {
        Interlocked.Exchange(ref _categoryQueryCount, 0);
    }

    public void Record(string commandText)
    {
        if (commandText.Contains("from \"categories\"", StringComparison.OrdinalIgnoreCase) ||
            commandText.Contains("from categories", StringComparison.OrdinalIgnoreCase))
        {
            Interlocked.Increment(ref _categoryQueryCount);
        }
    }
}
