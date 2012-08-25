using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;

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

    public class FolderNode : FileSystemNode
    {
        private readonly IScheduler _loadScheduler;
        private readonly IScheduler _dispatherScheduler;
        private readonly ObservableCollection<FileSystemNode> _children = new ObservableCollection<FileSystemNode>();
        private readonly string _name;
        private bool _hasAccessProblems;
        private int _childCount;
        private int _descendantCount;

        public FolderNode(string path, IScheduler loadScheduler, IScheduler dispatherScheduler)
            : this(path, loadScheduler, dispatherScheduler, null)
        { }

        public FolderNode(string path, IScheduler loadScheduler, IScheduler dispatherScheduler, FolderNode parent)
            : base(path, parent)
        {
            _loadScheduler = loadScheduler;
            _dispatherScheduler = dispatherScheduler;

            IsLoaded = false;

            var directoryInfo = new DirectoryInfo(path);
            _name = directoryInfo.Name;
            CreatedDate = directoryInfo.CreationTimeUtc;


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

        public bool HasAccessProblems
        {
            get { return _hasAccessProblems; }
            set
            {
                if (_hasAccessProblems != value)
                {
                    _hasAccessProblems = value;
                    OnPropertyChanged("HasAccessProblems");
                }
            }
        }

        

        private void LoadChildren()
        {
            try
            {
                var dirs = Directory.GetDirectories(FullPath)
                    .OrderBy(path => path)
                    .Select(path => new FolderNode(path, _loadScheduler, _dispatherScheduler, this))
                    .Cast<FileSystemNode>();
                var files = Directory.GetFiles(FullPath)
                    .OrderBy(path => path)
                    .Select(path => new FileNode(path, this))
                    .Cast<FileSystemNode>();
                
                _children.WhenItemsPropertyChange(node => node.IsLoaded, _ => IsLoaded = _children.All(node => node.IsLoaded));
                _children.WhenItemsPropertyChange(node => node.Length, _ => Length = _children.Sum(c => c.Length));
                var nodes = dirs.Concat(files);

                Children.WhenItemsPropertyChange(node => node.DescendantCount,
                                                 _ => DescendantCount = Children.Sum(c => c.DescendantCount + 1));
                _dispatherScheduler.Schedule(() =>
                                                 {
                                                     foreach (var node in nodes)
                                                     {
                                                         _children.Add(node);
                                                     }
                                                 });
            }
            catch (UnauthorizedAccessException uae)
            {
                HasAccessProblems = true;
            }
        }
    }
}
