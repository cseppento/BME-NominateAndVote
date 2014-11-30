﻿using System;
using System.Collections.Generic;

namespace NominateAndVote.DataModel.Model
{
    public enum PollState { Nomination, Voting, Closed }

    public class Poll : BasePocoWithId<Guid, Poll>
    {
        public string Text { get; set; }

        public PollState State { get; set; }

        public DateTime PublicationDate { get; set; }

        public DateTime NominationDeadline { get; set; }

        public DateTime VotingStartDate { get; set; }

        public DateTime VotingDeadline { get; set; }

        public DateTime AnnouncementDate { get; set; }

        public List<Nomination> Nominations { get; private set; }

        public Poll()
        {
            Nominations = new List<Nomination>();
        }

        public override bool Equals(Poll other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return IdEquals(other) && Text == other.Text && State == other.State
                && PublicationDate == other.PublicationDate && NominationDeadline == other.NominationDeadline
                && VotingStartDate == other.VotingStartDate && VotingDeadline == other.VotingDeadline
                && AnnouncementDate == other.AnnouncementDate;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)State;
                hashCode = (hashCode * 397) ^ PublicationDate.GetHashCode();
                hashCode = (hashCode * 397) ^ NominationDeadline.GetHashCode();
                hashCode = (hashCode * 397) ^ VotingStartDate.GetHashCode();
                hashCode = (hashCode * 397) ^ VotingDeadline.GetHashCode();
                hashCode = (hashCode * 397) ^ AnnouncementDate.GetHashCode();
                return hashCode;
            }
        }
    }
}