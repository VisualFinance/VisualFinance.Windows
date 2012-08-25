using System.Collections.ObjectModel;
using System.IO;

namespace VisualFinance.Windows.Sample.TreeListViewSamples
{
    public class FileNode : FileSystemNode
    {
        private readonly string _name;

        public FileNode(string path, FolderNode parent)
            : base(path, parent)
        {
            _name = Path.GetFileName(path);
            var info = new FileInfo(path);
            Length = info.Length;
            CreatedDate = info.CreationTimeUtc;

            IsLoaded = true;
        }

        public override string Name
        {
            get { return _name; }
        }

        public override ObservableCollection<FileSystemNode> Children
        {
            get { return new ObservableCollection<FileSystemNode>(); }
        }
    }
}