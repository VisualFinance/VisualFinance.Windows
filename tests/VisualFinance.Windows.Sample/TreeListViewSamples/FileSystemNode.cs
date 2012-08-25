using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace VisualFinance.Windows.Sample.TreeListViewSamples
{
    public abstract class FileSystemNode : INotifyPropertyChanged
    {
        private readonly string _fullPath;
        private readonly FolderNode _parent;
        private readonly int _depth;
        private int _descendantCount;
        private bool _isLoaded;
        private long _length;

        protected FileSystemNode(string fullPath, FolderNode parent)
        {
            _fullPath = fullPath;
            _parent = parent;
            _depth = parent == null ? 0 : parent.Depth + 1;
        }

        public string FullPath
        {
            get { return _fullPath; }
        }
        public abstract string Name { get; }
        public abstract ObservableCollection<FileSystemNode> Children { get; }

        public long Length
        {
            get { return _length; }
            protected set
            {
                if (_length != value)
                {
                    _length = value;
                    OnPropertyChanged("Length");
                }
            }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            protected set
            {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                    OnPropertyChanged("IsLoaded");
                }
            }
        }


        public int Depth
        {
            get { return _depth; }
        }

        public int DescendantCount
        {
            get { return _descendantCount; }
            set
            {
                _descendantCount = value;
                OnPropertyChanged("DescendantCount");
            }
        }


        public DateTimeOffset CreatedDate { get; protected set; }
        public string CreatedBy { get; protected set; }
        public DateTimeOffset ModifiedDate { get; protected set; }

        public FolderNode Parent
        {
            get { return _parent; }
        }

        //public string ModifiedBy { get; set; }
        ////    //FileType
        ////    public string FileType { get; set; }
        //public DateTimeOffset LastAccessedDate { get; set; }
        //public bool IsReadonly { get; set; }
        //public bool IsHidden { get; set; }
        //public bool IsCompressed { get; set; }
        //public bool IsEncrypted { get; set; }
        //public bool IsSystem { get; set; }
        //public Uri VcsUri { get; set; }
        ////VCS Version

        
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}