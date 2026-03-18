using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace HotChocolateDddCqrsTemplate.Infrastructure.Observability;

public sealed class CatalogQueryCounterInterceptor(CatalogQueryCounter counter) : DbCommandInterceptor
{
    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        counter.Record(command.CommandText);
        return base.ReaderExecuted(command, eventData, result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        counter.Record(command.CommandText);
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }
}
