using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Simulacra.IO.Watching;

namespace Calame
{
    public abstract class CalamePersistedDocument : PersistedDocument
    {
        protected readonly IEventAggregator EventAggregator;
        protected readonly FileFolderWatcher FileWatcher;
        private FileChangeType? _externalFileChange;

        protected abstract Task NewDocument();
        protected abstract Task LoadDocument(string filePath);
        protected abstract Task SaveDocument(string filePath);
        protected abstract Task InitializeDocument();

        public CalamePersistedDocument(IEventAggregator eventAggregator, FileFolderWatcher fileWatcher)
        {
            EventAggregator = eventAggregator;
            EventAggregator.SubscribeOnUI(this);

            FileWatcher = fileWatcher;
        }

        protected override sealed async Task DoNew()
        {
            await NewDocument();
            await InitializeDocument();
        }

        protected override sealed async Task DoLoad(string filePath)
        {
            await LoadDocument(filePath);
            FileWatcher.Subscribe(filePath, OnFileChanged);

            await InitializeDocument();
        }

        protected override sealed async Task DoSave(string filePath)
        {
            if (_externalFileChange == FileChangeType.Edited || _externalFileChange == FileChangeType.Created)
            {
                const string caption = "External changes detected";
                string message = $"{FileName} has been edited by an external process." + Environment.NewLine + "Are you sure you want to replace it ?";

                MessageBoxResult messageBoxResult = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (messageBoxResult != MessageBoxResult.Yes)
                {
                    // IsDirty is set to false after DoSave so we hack our way in
                    Task.Run(async () =>
                        {
                            await Task.Delay(10);
                            Application.Current.Dispatcher.Invoke(() => IsDirty = true);
                        });
                    return;
                }
            }

            if (!IsNew)
                FileWatcher.Unsubscribe(OnFileChanged);

            await SaveDocument(filePath);

            _externalFileChange = null;
            FileWatcher.Subscribe(filePath, OnFileChanged);
        }

        private void OnFileChanged(object sender, FileChangedEventArgs e)
        {
            IsDirty = true;
            _externalFileChange = e.ChangeType;
        }

        public override async Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            if (!IsDirty)
                return await base.CanCloseAsync(cancellationToken);

            var messageBuilder = new StringBuilder();
            if (_externalFileChange != null)
            {
                messageBuilder.AppendLine($"{FileName} has been {(_externalFileChange == FileChangeType.Deleted ? "deleted" : "edited")} by an external process.");
                messageBuilder.AppendLine();
            }
            messageBuilder.AppendLine("You will lose your changes if you don't save them !");
            messageBuilder.Append("Do you want to save before closing ?");

            const string caption = "Unsaved changes";
            string message = messageBuilder.ToString();

            MessageBoxResult messageBoxResult = MessageBox.Show(message, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);
            switch (messageBoxResult)
            {
                case MessageBoxResult.Yes:
                    _externalFileChange = null;
                    await Save(FilePath);
                    break;
                case MessageBoxResult.No:
                    IsDirty = false;
                    break;
                case MessageBoxResult.Cancel:
                    return false;
                default:
                    throw new NotSupportedException();
            }

            return await base.CanCloseAsync(cancellationToken);
        }
    }
}