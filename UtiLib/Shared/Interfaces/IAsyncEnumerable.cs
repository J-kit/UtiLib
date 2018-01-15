using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtiLib.Shared.Interfaces
{
    public interface IAsyncEnumerator<out T> : IDisposable
    {
        //
        // Zusammenfassung:
        //     Gets the element in the collection at the current position of the enumerator.
        T Current { get; }

        //
        // Zusammenfassung:
        //     Move to next item
        //
        // Rückgabewerte:
        //     Completion task
        Task<bool> MoveNextAsync();
    }

    public interface IAsyncEnumerable<out T>
    {
        //
        // Zusammenfassung:
        //     Returns an enumerator that supports asynchronous iterates through a collection.
        //
        // Rückgabewerte:
        //     Async Enumerator instance
        IAsyncEnumerator<T> GetEnumerator();
    }
}