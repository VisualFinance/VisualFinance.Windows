using System;
using System.Collections.ObjectModel;
using System.IO;

namespace VisualFinance.Windows.Sample.TreeListViewSamples
{
    public class FileNode : FileSystemNode
    {
        private readonly string _name;

        public FileNode(string path, Folder parent)
            : base(path, parent)
        {
            _name = Path.GetFileName(path);
            //Load lazily or eagerly? It is only a demo eh?
            var info = new FileInfo(path);
            Length = info.Length;

            IsLoaded = true;
        }

        public override string Name
        {
            get { return _name; }
        }


        public override DateTimeOffset CreatedDate { get; set; }
        public override string CreatedBy { get; set; }
        public override DateTimeOffset ModifiedDate { get; set; }

        public override ObservableCollection<FileSystemNode> Children
        {
            get { return new ObservableCollection<FileSystemNode>(); }
        }
    }
}