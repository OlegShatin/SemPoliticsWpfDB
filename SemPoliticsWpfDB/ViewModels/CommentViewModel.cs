using Politics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace SemPoliticsWpfDB.ViewModels
{
    public class CommentViewModel : TreeViewItem
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Comment Comment { get; set; }
        private PoliticsDBContext _context;

        public CommentViewModel(Comment comment, PoliticsDBContext context)
        {
            this.Comment = comment;
            this._context = context;
        }
        public string Text
        {
            get { return Comment.Text; }
            set
            {
                Comment.Text = value;
                OnPropertyChanged("Text");
            }
        }
        private ObservableCollection<CommentViewModel> _items;
        public virtual new ObservableCollection<CommentViewModel> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ObservableCollection<CommentViewModel>();
                    foreach (var comment in Comment.ChildrenComments)
                    {
                        _items.Add(new CommentViewModel(comment, _context));
                    }

                }
                return _items;
            }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

    }
}