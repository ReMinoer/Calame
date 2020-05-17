using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework;

namespace Calame
{
    public abstract class CalameDocumentBase : Document
    {
        protected readonly IEventAggregator EventAggregator;

        public CalameDocumentBase(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            EventAggregator.SubscribeOnUI(this);
        }

        protected abstract Task DisposeDocumentAsync();

        public override async Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            if (!await base.CanCloseAsync(cancellationToken))
                return false;

            await DisposeDocumentAsync();
            return true;
        }
    }
}