using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Politics;

namespace SemPoliticsWpfDB.ViewModels
{
    class CandidateViewModel : BaseViewModel
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
            set { Candidate.ImageSrc = value; }
        }
        public string Name
        {
            get { return Candidate.Name; }
            set { Candidate.Name = value; }
        }


        public bool Equals(CandidateViewModel other)
        {
            return Candidate.Equals(other.Candidate);
        }
    }
}
