using System;
using System.Collections.Generic;
using System.Linq;

namespace UtiLib.IO.Dialog
{
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
}