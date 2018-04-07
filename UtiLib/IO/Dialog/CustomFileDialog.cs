using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UtiLib.IO.Dialog
{
    public class CustomFileDialog
    {
        private readonly DialogType _type;
        private readonly FileDialogFilterArgs _filter;
        private readonly string _fileName;

        public bool Success { get; private set; }
        public FileInfo SelectedFileInfo { get; private set; }

        private bool _shown = false;

        public CustomFileDialog(DialogType type, string fileName, FileDialogFilterArgs filter = null)
        {
            _type = type;
            _fileName = fileName;
            _filter = filter ?? FileDialogFilterArgs.DefaultFilter;
        }

        public CustomFileDialog Show()
        {
            FileDialog fd = null;
            _shown = true;
            switch (_type)
            {
                case DialogType.Open:
                    fd = new OpenFileDialog
                    {
                        Title = $"Open a File ({_fileName})",
                        Filter = _filter.ToString()
                    };

                    break;

                case DialogType.Save:
                    fd = new SaveFileDialog
                    {
                        Title = $"Save a File ({_fileName})",
                        Filter = _filter.ToString()
                    };

                    break;

                case DialogType.Default:
                default:
                    break;
            }

            if (fd == null)
                return default;

            fd.FileName = _fileName;

            if (fd.ShowDialog() == DialogResult.OK && fd.FileName != string.Empty)
            {
                Success = true;
                SelectedFileInfo = fd.FileName.ToFileInfo();
            }
            return this;
        }

        public void OnSuccess(Action<FileInfo> action)
        {
            if (!_shown)
                Show();

            if (Success)
            {
                action(SelectedFileInfo);
            }
        }
    }
}