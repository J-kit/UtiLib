using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtiLib.Extensions;

namespace UtiLib.IO
{
    public struct FileDialogFilterItem
    {
        public string Description { get; set; }
        public string Extension { get; set; }

        public static FileDialogFilterItem Create(string extension, string description)
        {
            return new FileDialogFilterItem { Extension = extension, Description = description };
        }
    }

    public class FileDialogFilterArgs
    {
        public static FileDialogFilterArgs DefaultFilter
        {
            get
            {
                var res = new FileDialogFilterArgs();
                res.AddFilter("*.*", "All Files");
                return res;
            }
        }

        private readonly List<FileDialogFilterItem> _internalFilters;

        /// <summary>
        /// Defines wether extension will be added to the description automatically or not.
        /// </summary>
        public bool AddExtension { get; set; } = true;

        public FileDialogFilterArgs()
        {
            _internalFilters = new List<FileDialogFilterItem>();
        }

        public void AddFilter(string extension, string description)
        {
            _internalFilters.Add(FileDialogFilterItem.Create(extension, description));
        }

        public void AddFilters(IEnumerable<FileDialogFilterItem> filters)
        {
            _internalFilters.AddRange(filters);
        }

        public bool TryRemoveFilter(string extension)
        {
            return TryRemoveFilter(x => x.Extension == extension);
        }

        public bool TryRemoveFilter(Predicate<FileDialogFilterItem> predicate)
        {
            var flag = false;
            for (var i = _internalFilters.Count - 1; i >= 0; i--)
            {
                if (predicate(_internalFilters[i]))
                {
                    _internalFilters.RemoveAt(i);
                    flag = true;
                }
            }

            return flag;
        }

        public override string ToString()
        {
            return string.Join("|", _internalFilters.Select(m => $"{m.Description}{AutoExt(m.Extension)}|{m.Extension}"));
        }

        private string AutoExt(string extensionString)
            => AddExtension ? $" ({extensionString})" : string.Empty;
    }

    public static class FileDialogHelper
    {
        public enum DialogType
        {
            Default,
            Open,
            Save
        }

        public static Tuple<bool, string> GetDialog(DialogType type, string fileName, FileDialogFilterArgs filter = null)
        {
            filter = filter ?? FileDialogFilterArgs.DefaultFilter;

            FileDialog fd;
            switch (type)
            {
                case DialogType.Open:
                    fd = new OpenFileDialog
                    {
                        Title = $"Open a File ({fileName})",
                        Filter = filter.ToString()
                    };

                    break;

                case DialogType.Save:
                    fd = new SaveFileDialog
                    {
                        Title = $"Save a File ({fileName})",
                        Filter = filter.ToString()
                    };

                    break;

                case DialogType.Default:
                default:
                    return Tuple.Create(false, string.Empty);
            }

            fd.FileName = fileName;

            if (fd.ShowDialog() == DialogResult.OK && fd.FileName != string.Empty)
            {
                return Tuple.Create(true, fd.FileName);
            }

            return Tuple.Create(false, string.Empty);
        }
    }
}