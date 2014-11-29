﻿using NominateAndVote.DataModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NominateAndVote.DataModel
{
    public class DataModelManager : IDataManager
    {
        public IDataModel DataModel { get; private set; }

        public DataModelManager(IDataModel dataModel)
        {
            if (dataModel == null)
            {
                throw new ArgumentNullException("dataModel", "The data model must not be null");
            }

            DataModel = dataModel;
        }

        public bool IsAdmin(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user", "The user must not be null");
            }

            return (from a in DataModel.Administrators
                    where a.UserID.Equals(user.ID)
                    select a).Count() > 0;
        }

        public List<News> QueryNews()
        {
            var q = from n in DataModel.News
                    orderby n.PublicationDate descending
                    select n;

            return q.ToList();
        }

        public News QueryNews(Guid id)
        {
            var q = from n in DataModel.News
                    where n.ID.Equals(id)
                    select n;

            return q.SingleOrDefault();
        }

        public void SaveNews(News news)
        {
            if (news == null)
            {
                throw new ArgumentNullException("news", "The news must not be null");
            }

            if (news.ID.Equals(Guid.Empty))
            {
                // now ID, new object
                news.ID = Guid.NewGuid();
            }
            else
            {
                // maybe a new, maybe an old object
                var q = from n in DataModel.News
                        where n.ID.Equals(news.ID)
                        select n;

                News oldNews = q.SingleOrDefault();

                if (oldNews != null)
                {
                    // remove old
                    DataModel.News.Remove(oldNews);
                }
            }

            // add new
            DataModel.News.Add(news);
        }

        public void DeleteNews(Guid id)
        {
            var q = from n in DataModel.News
                    where n.ID.Equals(id)
                    select n;

            News news = q.SingleOrDefault();

            if (news != null)
            {
                DataModel.News.Remove(news);
            }
        }

        public List<Nomination> QueryNominations(Poll poll)
        {
            if (poll == null)
            {
                throw new ArgumentNullException("poll", "The poll must not be null");
            }

            var q = from n in poll.Nominations
                    orderby n.Subject.Title, n.Subject.Year
                    select n;

            return q.ToList();
        }

        public List<Nomination> QueryNominations(Poll poll, User user)
        {
            if (poll == null)
            {
                throw new ArgumentNullException("poll", "The poll must not be null");
            }
            else if (user == null)
            {
                throw new ArgumentNullException("user", "The user must not be null");
            }

            var q = from n in poll.Nominations
                    where n.User != null && n.User.ID.Equals(user.ID)
                    orderby n.Subject.Title, n.Subject.Year
                    select n;

            return q.ToList();
        }

        public void SaveNomination(Nomination nomination)
        {
            if (nomination == null)
            {
                throw new ArgumentNullException("nomination", "The nomination must not be null");
            }

            if (nomination.ID.Equals(Guid.Empty))
            {
                // now ID, new object
                nomination.ID = Guid.NewGuid();
            }
            else
            {
                // maybe a new, maybe an old object
                var q = from n in DataModel.Nominations
                        where n.ID.Equals(nomination.ID)
                        select n;

                Nomination oldNomination = q.SingleOrDefault();

                if (oldNomination != null)
                {
                    // remove old
                    DataModel.Nominations.Remove(oldNomination);
                }
            }

            // add new
            DataModel.Nominations.Add(nomination);
        }

        public void DeleteNomination(Guid id)
        {
            var q = from n in DataModel.Nominations
                    where n.ID.Equals(id)
                    select n;

            Nomination nomination = q.SingleOrDefault();

            if (nomination != null)
            {
                DataModel.Nominations.Remove(nomination);
            }
        }

        public List<Poll> QueryPolls()
        {
            var q = from p in DataModel.Polls
                    orderby p.AnnouncementDate descending
                    select p;

            return q.ToList();
        }

        public Poll QueryPoll(Guid id)
        {
            var q = from p in DataModel.Polls
                    where p.ID.Equals(id)
                    select p;

            return q.SingleOrDefault();
        }

        public List<Poll> QueryPolls(PollState state)
        {
            var q = from p in DataModel.Polls
                    where p.State.Equals(state)
                    orderby p.AnnouncementDate descending
                    select p;

            return q.ToList();
        }

        public PollSubject QueryPollSubject(long id)
        {
            var q = from ps in DataModel.PollSubjects
                    where ps.ID == id
                    select ps;

            return q.SingleOrDefault();
        }

        public List<PollSubject> SearchPollSubjects(string term)
        {
            if (term == null)
            {
                throw new ArgumentNullException("term", "The search term must not be null");
            }

            var q = from ps in DataModel.PollSubjects
                    where ps.Title.StartsWith(term)
                    select ps;

            return q.ToList();
        }

        public void SavePollSubject(PollSubject pollSubject)
        {
            if (pollSubject == null)
            {
                throw new ArgumentNullException("pollSubject", "The poll subject must not be null");
            }

            // maybe a new, maybe an old object
            var q = from ps in DataModel.PollSubjects
                    where ps.ID == pollSubject.ID
                    select ps;

            PollSubject oldPollSubject = q.SingleOrDefault();

            if (oldPollSubject != null)
            {
                // remove old
                DataModel.PollSubjects.Remove(oldPollSubject);
            }

            // add new
            DataModel.PollSubjects.Add(pollSubject);
        }

        public void SavePollSubjectsBatch(List<PollSubject> pollSubjects)
        {
            // it makes sense during the long import into the cloud
            foreach (var ps in pollSubjects)
            {
                SavePollSubject(ps);
            }
        }

        public List<User> QueryBannedUsers()
        {
            var q = from u in DataModel.Users
                    where u.IsBanned
                    select u;

            return q.ToList();
        }

        public User QueryUser(Guid id)
        {
            var q = from u in DataModel.Users
                    where u.ID.Equals(id)
                    select u;

            return q.SingleOrDefault();
        }

        public List<User> SearchUsers(string term)
        {
            if (term == null)
            {
                throw new ArgumentNullException("term", "The search term must not be null");
            }

            var q = from u in DataModel.Users
                    where u.Name.StartsWith(term)
                    select u;

            return q.ToList();
        }

        public void SaveUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user", "The user must not be null");
            }

            if (user.ID.Equals(Guid.Empty))
            {
                // now ID, new object
                user.ID = Guid.NewGuid();
            }
            else
            {
                // maybe a new, maybe an old object
                var q = from n in DataModel.Users
                        where n.ID.Equals(user.ID)
                        select n;

                User oldUser = q.SingleOrDefault();

                if (oldUser != null)
                {
                    // remove old
                    DataModel.Users.Remove(oldUser);
                }
            }

            // add new
            DataModel.Users.Add(user);
        }

        public Vote QueryVote(Poll poll, User user)
        {
            if (poll == null)
            {
                throw new ArgumentNullException("poll", "The poll must not be null");
            }
            else if (user == null)
            {
                throw new ArgumentNullException("user", "The user must not be null");
            }

            return (from v in DataModel.Votes
                    where v.Nomination.Poll.ID.Equals(poll.ID) && v.User.ID.Equals(user.ID)
                    select v).SingleOrDefault();
        }

        public void SaveVote(Vote vote)
        {
            if (vote == null)
            {
                throw new ArgumentNullException("vote", "The vote must not be null");
            }

            // maybe a new, maybe an old object
            var q = from v in DataModel.Votes
                    where v.Nomination.ID.Equals(vote.Nomination.ID) && v.User.ID.Equals(vote.User.ID)
                    select v;

            Vote oldVote = q.SingleOrDefault();

            if (oldVote != null)
            {
                // remove old
                DataModel.Votes.Remove(oldVote);
            }

            // add new
            DataModel.Votes.Add(vote);
        }
    }
}