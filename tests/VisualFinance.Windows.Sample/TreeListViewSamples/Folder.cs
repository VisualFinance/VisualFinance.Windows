using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace VisualFinance.Windows.Sample.TreeListViewSamples
{
    //public class Folder
    //{
    //}

    //public class File
    //{
    //    //Name
    //    public string Name { get; set; }
    //    //Size/Length
    //    public int Length { get; set; }
    //    //CreatedDate
    //    public DateTimeOffset CreatedDate { get; set; }
    //    //CreatedBy
    //    public string CreatedBy { get; set; }
    //    //ModifiedDate
    //    public DateTimeOffset ModifiedDate { get; set; }
    //    //ModifiedBy
    //    public string ModifiedBy { get; set; }
    //    //FileType
    //    public string FileType { get; set; }
    //    //LastAccessedDate
    //    public DateTimeOffset LastAccessedDate { get; set; }
    //    //IsReadonly
    //    public bool IsReadonly { get; set; }
    //    //IsHidden
    //    public bool IsHidden { get; set; }
    //    //IsCompress
    //    public bool IsCompressed { get; set; }
    //    //IsEncrypted
    //    public bool IsEncrypted { get; set; }
    //    //IsSystem
    //    public bool IsSystem { get; set; }
    //    //VCS URL
    //    public Uri VcsUri { get; set; }
    //    //VCS Version

    //}

    public class Folder : FileSystemNode
    {
        private readonly IScheduler _loadScheduler;
        private readonly IScheduler _dispatherScheduler;
        private readonly ObservableCollection<FileSystemNode> _children = new ObservableCollection<FileSystemNode>();
        private readonly string _name;

        public Folder(string path, IScheduler loadScheduler, IScheduler dispatherScheduler)
            : this(path, loadScheduler, dispatherScheduler, null)
        { }

        public Folder(string path, IScheduler loadScheduler, IScheduler dispatherScheduler, Folder parent)
            : base(path, parent)
        {
            _loadScheduler = loadScheduler;
            _dispatherScheduler = dispatherScheduler;
            _name = new DirectoryInfo(path).Name;
            IsLoaded = false;
            _loadScheduler.Schedule(LoadChildren);
        }

        public override string Name
        {
            get { return _name; }
        }
        public override ObservableCollection<FileSystemNode> Children
        {
            get { return _children; }
        }
        public override DateTimeOffset CreatedDate { get; set; }
        public override string CreatedBy { get; set; }
        public override DateTimeOffset ModifiedDate { get; set; }

        private void LoadChildren()
        {
            var dirs = Directory.GetDirectories(FullPath)
                .OrderBy(path => path)
                .Select(path => new Folder(path, _loadScheduler, _dispatherScheduler, this))
                .Cast<FileSystemNode>();
            var files = Directory.GetFiles(FullPath)
                .OrderBy(path => path)
                .Select(path => new FileNode(path, this))
                .Cast<FileSystemNode>();

            _children.WhenItemsPropertyChange(node => node.IsLoaded, _ => IsLoaded = _children.All(node => node.IsLoaded));
            _children.WhenItemsPropertyChange(node => node.Length, _ => Length = _children.Sum(c => c.Length));
            var nodes = dirs.Concat(files);

            _dispatherScheduler.Schedule(() =>
            {
                foreach (var node in nodes)
                {
                    _children.Add(node);
                }
            });
        }
    }
}
