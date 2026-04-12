using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stardrop.Utilities
{
    internal static class StoragePicker
    {
        public static async Task<string[]?> OpenFilePathsAsync(TopLevel owner, string title, bool allowMultiple, IEnumerable<FilePickerFileType> fileTypes, string? startLocation = null)
        {
            var options = new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = allowMultiple,
                FileTypeFilter = fileTypes.ToArray()
            };

            var suggestedStartLocation = await GetSuggestedStartLocationAsync(owner, startLocation);
            if (suggestedStartLocation is not null)
            {
                options.SuggestedStartLocation = suggestedStartLocation;
            }

            var files = await owner.StorageProvider.OpenFilePickerAsync(options);
            var paths = files
                .Select(file => file.TryGetLocalPath())
                .Where(path => String.IsNullOrWhiteSpace(path) is false)
                .Cast<string>()
                .ToArray();

            return paths.Length > 0 ? paths : null;
        }

        public static async Task<string?> OpenFolderPathAsync(TopLevel owner, string title, string? startLocation = null)
        {
            var options = new FolderPickerOpenOptions
            {
                Title = title,
                AllowMultiple = false
            };

            var suggestedStartLocation = await GetSuggestedStartLocationAsync(owner, startLocation);
            if (suggestedStartLocation is not null)
            {
                options.SuggestedStartLocation = suggestedStartLocation;
            }

            return (await owner.StorageProvider.OpenFolderPickerAsync(options))
                .Select(folder => folder.TryGetLocalPath())
                .FirstOrDefault(path => String.IsNullOrWhiteSpace(path) is false);
        }

        private static async Task<IStorageFolder?> GetSuggestedStartLocationAsync(TopLevel owner, string? startLocation)
        {
            if (String.IsNullOrWhiteSpace(startLocation))
            {
                return null;
            }

            return await owner.StorageProvider.TryGetFolderFromPathAsync(startLocation);
        }
    }
}
