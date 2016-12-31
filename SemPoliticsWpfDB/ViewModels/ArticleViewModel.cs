using Politics;
using SemPoliticsWpfDB.Commands;
using SemPoliticsWpfDB.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SemPoliticsWpfDB.ViewModels
{
    public class ArticleViewModel : BaseViewModel
    {
        private PoliticsDBContext _context;
        private Root _root;
        public Article Article { get; set; }

        public ArticleViewModel(Article article, PoliticsDBContext context, Root root)
        {
            Article = article;
            _context = context;
            _root = root;
        }
        public string Headline
        {
            get { return Article.Headline; }
            set
            {
                Article.Headline = value;
                OnPropertyChanged("Headline");
            }
        }
        public string Content
        {
            get { return Article.Content; }
            set
            {
                Article.Content = value;
                OnPropertyChanged("Content");
            }
        }
        public DateTime Date
        {
            get { return Article.SendingTime.Value.DateTime; }
            set
            {
                Article.SendingTime = new DateTimeOffset(value, new TimeSpan(3, 0, 0)).AddHours(8);
                OnPropertyChanged("Date");
            }
        }

        //show comments list
        private DelegateCommand _showCommentsCommand;
        public ICommand ShowComments => _showCommentsCommand ?? (_showCommentsCommand = new DelegateCommand(ShowCommentsMethod));
        private void ShowCommentsMethod()
        {
            ArticlesCommentsWindow w = new ArticlesCommentsWindow();
            w.DataContext = this;
            w.Show();
        }
        private ObservableCollection<CommentViewModel> _commentsVMList;
        public virtual ObservableCollection<CommentViewModel> CommentsVMList
        {
            get
            {
                if (_commentsVMList == null)
                {
                    _commentsVMList = new ObservableCollection<CommentViewModel>();
                    foreach (var comment in Article?.Comments.Where(x => x.ParentCommentId == null))
                    {
                        _commentsVMList.Add(new CommentViewModel(comment, _context));
                    }
                }                
                return _commentsVMList;
            }
            set
            {
                _commentsVMList = value;
                OnPropertyChanged("CommentsVMList");
            }
        }

    }
}
