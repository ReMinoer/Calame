using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Calame.Icons;
using Caliburn.Micro;
using Gemini.Framework;
using Microsoft.Extensions.Logging;
using Simulacra.IO.Watching;

namespace Calame
{
    public abstract class CalamePersistedDocumentBase : CalameDocumentBase, IPersistedDocument, IHandle<IDirtyMessage>
    {
        protected readonly PathWatcher FileWatcher;

        private bool _isNew;
        public bool IsNew
        {
            get => _isNew;
            private set => Set(ref _isNew, value);
        }

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            private set
            {
                Set(ref _filePath, value);
                FileName = Path.GetFileName(_filePath);
            }
        }

        private string _fileName;
        public string FileName
        {
            get => _fileName;
            private set
            {
                if (Set(ref _fileName, value))
                    UpdateDisplayName();
            }
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get => _isDirty;
            protected set
            {
                if (Set(ref _isDirty, value))
                    UpdateDisplayName();
            }
        }

        private FileChangeType? _externalFileChange;
        private FileChangeType? ExternalFileChange
        {
            get => _externalFileChange;
            set
            {
                if (Set(ref _externalFileChange, value))
                    UpdateDisplayName();
            }
        }

        public CalamePersistedDocumentBase(IEventAggregator eventAggregator, ILoggerProvider loggerProvider, PathWatcher fileWatcher, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(eventAggregator, loggerProvider, iconProvider, iconDescriptorManager)
        {
            FileWatcher = fileWatcher;
            FileWatcher.Logger = Logger;
        }

        protected abstract Task NewDocumentAsync();
        protected abstract Task LoadDocumentAsync(string filePath);
        protected abstract Task SaveDocumentAsync(string filePath);

        public async Task New(string fileName)
        {
            FileName = fileName;
            IsNew = true;
            IsDirty = false;

            await NewDocumentAsync();
        }

        public async Task Load(string filePath)
        {
            FilePath = filePath;
            IsNew = false;
            IsDirty = false;

            await LoadDocumentAsync(filePath);
            StartWatchingFilePath(filePath);
        }

        public async Task Save(string filePath)
        {
            switch (_externalFileChange)
            {
                case FileChangeType.Edited:
                case FileChangeType.Created:
                {
                    if (UserDoNotWantToReplaceExternalChanges())
                        return;
                    break;
                }
            }

            StopWatchingFilePath(filePath);
            await SaveDocumentAsync(filePath);
            StartWatchingFilePath(filePath);

            FilePath = filePath;
            IsNew = false;
            IsDirty = false;
        }

        public override async Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            if (IsDirty)
            {
                switch (UserWantToSaveUnsavedChanged())
                {
                    case true:
                        _externalFileChange = null;
                        await Save(FilePath);
                        break;
                    case false:
                        IsDirty = false;
                        break;
                    case null:
                        return false;
                }
            }

            StopWatchingFilePath(FilePath);
            await DisposeDocumentAsync();
            return true;
        }

        private void StartWatchingFilePath(string filePath)
        {
            FileWatcher.WatchFile(filePath, OnFileChanged);
        }

        private void StopWatchingFilePath(string filePath)
        {
            if (!IsNew)
                FileWatcher.Unwatch(filePath, OnFileChanged);

            ExternalFileChange = null;
        }

        private void OnFileChanged(object sender, FileChangedEventArgs e)
        {
            IsDirty = true;
            ExternalFileChange = e.ChangeType;
        }

        private void UpdateDisplayName()
        {
            string displayName = FileName;

            if (IsDirty)
            {
                displayName += "*";
                switch (_externalFileChange)
                {
                    case FileChangeType.Created:
                    case FileChangeType.Edited:
                        displayName += " (external changes)";
                        break;
                    case FileChangeType.Deleted:
                        displayName += " (deleted)";
                        break;
                }
            }

            DisplayName = displayName;
        }

        private bool UserDoNotWantToReplaceExternalChanges()
        {
            var messageBuilder = new StringBuilder();

            messageBuilder.AppendLine($"{FileName} has been edited by an external process.");
            messageBuilder.Append("Are you sure you want to replace it ?");

            MessageBoxResult result = MessageBox.Show(messageBuilder.ToString(), "External changes detected", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result != MessageBoxResult.Yes;
        }

        private bool? UserWantToSaveUnsavedChanged()
        {
            var messageBuilder = new StringBuilder();

            if (ExternalFileChange != null)
            {
                messageBuilder.AppendLine($"{FileName} has been {(_externalFileChange == FileChangeType.Deleted ? "deleted" : "edited")} by an external process.");
                messageBuilder.AppendLine();
            }

            messageBuilder.AppendLine("You will lose your changes if you don't save them !");
            messageBuilder.Append("Do you want to save before closing ?");

            MessageBoxResult result = MessageBox.Show(messageBuilder.ToString(), "Unsaved changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);
            switch (result)
            {
                case MessageBoxResult.Yes: return true;
                case MessageBoxResult.No: return false;
                case MessageBoxResult.Cancel: return null;
                default: throw new NotSupportedException();
            }
        }

        Task IHandle<IDirtyMessage>.HandleAsync(IDirtyMessage message, CancellationToken cancellationToken)
        {
            if (message.DocumentContext == this)
                IsDirty = true;

            return Task.CompletedTask;
        }
    }
}