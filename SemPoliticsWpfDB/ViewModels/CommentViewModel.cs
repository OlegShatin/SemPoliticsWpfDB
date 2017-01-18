using Politics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Linq;
using System.Windows;
using SemPoliticsWpfDB.Commands;
using System.Windows.Input;

namespace SemPoliticsWpfDB.ViewModels
{
    public class CommentViewModel : BaseViewModel
    {
        
        public Comment Comment { get; set; }
        private PoliticsDBContext _context;
        private ObservableCollection<CommentViewModel> _brothers;

        
        public CommentViewModel(Comment comment, ObservableCollection<CommentViewModel> brothers, PoliticsDBContext context)
        {
            this.Comment = comment;
            this._context = context;
            this._brothers = brothers;
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

        public string Author
        {
            get { return Comment.Author.Name + " " + Comment.Author.Surname + ":"; }
            private set
            {
                
            }
        }
        public string Rating
        {
            get { return Comment.Rating.ToString(); }
            set
            {
                int resultValue = 0;
                if (int.TryParse(value, out resultValue))
                {                   
                    Comment.Rating = resultValue; 
                }
                else
                {
                    if (value.Equals(""))
                    {
                        Comment.Rating = 0;
                    }
                    else
                    {
                        MessageBox.Show("Только целые числа",
                        "Предупреждение", MessageBoxButton.OK);
                    }

                }
                OnPropertyChanged("Rating");
            }
        }

        private ObservableCollection<CommentViewModel> _children;
        public virtual ObservableCollection<CommentViewModel> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new ObservableCollection<CommentViewModel>();                    
                    foreach (var comment in Comment.ChildrenComments.OrderByDescending(x => x.Rating))
                    {
                        _children.Add(new CommentViewModel(comment, _children, _context));
                    }
                    

                }
                return _children;
            }
            set
            {
                _children = value;
                OnPropertyChanged("Children");
            }
        }

        private DelegateCommand _removeCommentCommand;
        public ICommand RemoveComment => _removeCommentCommand ?? (_removeCommentCommand = new DelegateCommand(RemoveCommentMethod));
        private void RemoveCommentMethod()
        {
            _brothers.Remove(this);
            _context.Comments.Local.Remove(Comment);
        }

    }
}