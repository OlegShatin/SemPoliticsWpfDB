using Politics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemPoliticsWpfDB.ViewModels
{
    public class CandidateOnElectionViewModel : CandidateViewModel
    {
        public CandidateOnElectionViewModel(CandidateOnElection candidateItem, PoliticsDBContext dbcontext) : base(candidateItem.Candidate, dbcontext)
        {
            Candidate = candidateItem.Candidate;
            CandidateOnElection = candidateItem;
        }
        public CandidateOnElection CandidateOnElection { get; set; }
        public int Votes
        {
            get { return CandidateOnElection.VotesForCandidate; }
            set
            {
                CandidateOnElection.VotesForCandidate = value;
                OnPropertyChanged("Votes");
            }
        }
    }
}
