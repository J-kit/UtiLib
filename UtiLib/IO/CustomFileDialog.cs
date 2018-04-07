using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using UtiLib.IO.Dialog;

namespace UtiLib.IO
{
    [Obsolete]
    public static class FileDialogHelper
    {
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