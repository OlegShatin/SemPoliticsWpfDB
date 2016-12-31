using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Politics;

namespace SemPoliticsWpfDB.ViewModels
{
    public class CandidateViewModel : BaseViewModel,IEquatable<CandidateViewModel>
    {
        public Candidate Candidate { get; set; }
        private PoliticsDBContext _dbcontext;

        public CandidateViewModel(Candidate candidate, PoliticsDBContext dbcontext)
        {
            this.Candidate = candidate;
            this._dbcontext = dbcontext;            
        }

        public string ImageSrc
        {
            get { return Candidate.ImageSrc; }
            set
            {
                Candidate.ImageSrc = value;
                OnPropertyChanged("ImageSrc");
            }
        }
        public string Name
        {
            get { return Candidate.Name; }
            set
            {
                Candidate.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public string ElectionProgram
        {
            get { return Candidate.ElectionProgram; }
            set
            {
                Candidate.ElectionProgram = value;
                OnPropertyChanged("ElectionProgram");
            }
        }

        public string Achievements
        {
            get { return Candidate.Achievements; }
            set
            {
                Candidate.Achievements = value;
                OnPropertyChanged("Achievements");
            }
        }



        public bool Equals(CandidateViewModel other)
        {
            return Candidate.Equals(other.Candidate);
        }
    }
    
}
