﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NominateAndVote.DataModel.Poco;
using System;
using System.Collections.Generic;

namespace NominateAndVote.DataModel.Tests
{
    public abstract class DataManagerTests
    {
        private IDataManager _dataManager;

        public virtual void DoInitialize()
        {
            _dataManager = _createDataManager(new SampleDataModel());
        }

        protected abstract IDataManager _createDataManager(IDataModel dataModel);

        public virtual void IsAdmin()
        {
            // Act & Assert
            Assert.AreEqual(false, _dataManager.IsAdmin(new User { Id = 0 }));
            Assert.AreEqual(false, _dataManager.IsAdmin(new User { Id = 1 }));
            Assert.AreEqual(false, _dataManager.IsAdmin(new User { Id = 2 }));
            Assert.AreEqual(false, _dataManager.IsAdmin(new User { Id = 3 }));
            Assert.AreEqual(true, _dataManager.IsAdmin(new User { Id = 4 }));
            Assert.AreEqual(false, _dataManager.IsAdmin(new User { Id = 5 }));
        }

        public virtual void QueryNews()
        {
            // Act
            var list = _dataManager.QueryNews();
            var one = _dataManager.QueryNews(list[1].Id);
            var none = _dataManager.QueryNews(Guid.NewGuid());

            // Assert
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Second", list[0].Title);
            Assert.AreEqual("First", list[1].Title);

            Assert.AreEqual("First", one.Title);

            Assert.AreEqual(null, none);
        }

        public virtual void SaveNews()
        {
            // Arrange
            var list = _dataManager.QueryNews();

            // Act
            // create
            var news = new News { Id = Guid.Empty, PublicationDate = DateTime.Now, Title = "Third", Text = "x" };
            _dataManager.SaveNews(news);

            // update
            list[0].Title = "Second2";
            _dataManager.SaveNews(list[0]);

            // Assert
            list = _dataManager.QueryNews();
            Assert.AreEqual(3, list.Count);
            Assert.AreNotEqual(Guid.Empty, list[0].Id); // new id should have been assigned
            Assert.AreEqual("Third", list[0].Title);
            Assert.AreEqual("Second2", list[1].Title);
            Assert.AreEqual("First", list[2].Title);
        }

        public virtual void DeleteNews()
        {
            // Act
            var list = _dataManager.QueryNews();

            // exists, should delete
            _dataManager.DeleteNews(list[1]);

            // not exists, should not cause exception
            _dataManager.DeleteNews(new News { Id = Guid.NewGuid() });

            // Assert
            list = _dataManager.QueryNews();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Second", list[0].Title);
        }

        public virtual void QueryNominationsWithPoll()
        {
            // Act
            var poll = _dataManager.QueryPolls()[0];
            var list = _dataManager.QueryNominations(poll);

            // Assert
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Mert en azt mondtam", list[0].Text);
        }

        public virtual void QueryNominationsWithUser()
        {
            // Act
            var user = _dataManager.QueryUser(1);
            var list = _dataManager.QueryNominations(user);

            // Assert
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual("Mert en azt mondtam", list[0].Text);
            Assert.AreEqual("A kedvencem", list[1].Text);
            Assert.AreEqual("Jok a szineszek", list[2].Text);
            Assert.AreEqual("Valami", list[3].Text);
        }

        public virtual void QueryNominationsWithWrongUser()
        {
            // Act
            var user = new User { Id = 40, Name = "V", IsBanned = false };
            _dataManager.QueryNominations(user);

            // Assert
            // expected exception
        }

        public virtual void QueryNominations()
        {
            // Act
            var poll = _dataManager.QueryPolls()[0];
            var user = _dataManager.QueryUser(1);
            var list = _dataManager.QueryNominations(poll, user);

            // Assert
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Mert en azt mondtam", list[0].Text);
        }

        public virtual void SaveNomination()
        {
            // Arrange
            var poll = _dataManager.QueryPolls()[0];
            var subject = _dataManager.QueryPollSubject(1);
            var user = _dataManager.QueryUser(1);
            var list = _dataManager.QueryNominations(poll);

            // Act
            // create
            var nom = new Nomination { Id = Guid.Empty, Poll = poll, Text = "Second", Subject = subject, User = user };
            _dataManager.SaveNomination(nom);

            // update
            list[0].Text = "Proba";
            _dataManager.SaveNomination(list[0]);

            // Assert
            list = _dataManager.QueryNominations(poll);
            Assert.AreEqual(2, list.Count);
            Assert.AreNotEqual(Guid.Empty, list[1].Id); // new id should have been assigned
            Assert.AreEqual("Proba", list[0].Text);
            Assert.AreEqual("Second", list[1].Text);
        }

        public virtual void DeleteNomination()
        {
            // Act
            var poll = _dataManager.QueryPolls()[1];
            var list = _dataManager.QueryNominations(poll);

            // exists, should delete
            _dataManager.DeleteNomination(list[0]);

            // not exists, should not cause exception
            _dataManager.DeleteNomination(new Nomination { Id = Guid.NewGuid(), Poll = list[0].Poll, User = list[0].User, Subject = list[0].Subject });

            // Assert
            list = _dataManager.QueryNominations(poll);
            Assert.AreEqual(1, list.Count);
        }

        public virtual void QueryPolls()
        {
            // Act
            var list = _dataManager.QueryPolls();

            // Assert
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual("Ki a legjobb?", list[0].Title);
        }

        public virtual void QueryPollsWithNominationState()
        {
            // Act
            var list = _dataManager.QueryPolls(PollState.Nomination);

            // Assert
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Ki a legjobb?", list[0].Title);
        }

        public virtual void QueryPoll()
        {
            // Arrange
            var poll = _dataManager.QueryPolls(PollState.Nomination)[0];

            // Act
            var one = _dataManager.QueryPoll(poll.Id);

            // Assert
            Assert.AreNotEqual(null, one);
            Assert.AreEqual("Ki a legjobb?", one.Title);
        }

        public virtual void SavePoll()
        {
            // Arrange
            var list = _dataManager.QueryPolls();

            // Act
            // create
            var poll2 = new Poll
            {
                Id = Guid.NewGuid(),
                State = PollState.Nomination,
                Text = "Proba1",
                Title = "Proba1",
                PublicationDate = DateTime.Now,
                NominationDeadline = DateTime.Now.AddDays(+2),
                VotingStartDate = DateTime.Now.AddDays(+4),
                VotingDeadline = DateTime.Now.AddDays(+6),
                AnnouncementDate = DateTime.Now.AddDays(+8)
            };
            _dataManager.SavePoll(poll2);

            // update
            list[0].Text = "Valami";
            _dataManager.SavePoll(list[0]);

            // Assert
            list = _dataManager.QueryPolls();
            Assert.AreEqual(5, list.Count);
            Assert.AreNotEqual(Guid.Empty, list[0].Id); // new id should have been assigned
            Assert.AreEqual("Proba1", list[0].Text);
            Assert.AreEqual("Valami", list[1].Text);
        }

        public virtual void QueryPollSubject()
        {
            // Act
            var subject = _dataManager.QueryPollSubject(1);

            // Assert
            Assert.AreNotEqual(null, subject);
            Assert.AreEqual("Ehezok viadala", subject.Title);
        }

        public virtual void SearchPollSubjects()
        {
            // Act
            var list = _dataManager.SearchPollSubjects("Valami");

            // Assert
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Valami Amerika", list[0].Title);
            Assert.AreEqual("Valami Amerika 2", list[1].Title);
        }

        public virtual void SavePollSubject()
        {
            // Arrange
            var one = _dataManager.QueryPollSubject(1);

            // Act
            // create
            var subject = new PollSubject { Id = 5, Title = "Titanic", Year = 1976 };
            _dataManager.SavePollSubject(subject);

            // update
            one.Title = "Ehezok viadala 2";
            _dataManager.SavePollSubject(one);

            // Assert
            one = _dataManager.QueryPollSubject(1);
            Assert.AreNotEqual(null, one);
            Assert.AreEqual("Ehezok viadala 2", one.Title);
            one = _dataManager.QueryPollSubject(5);
            Assert.AreNotEqual(null, one);
            Assert.AreEqual("Titanic", one.Title);
        }

        public virtual void SavePollSubjectsBatch()
        {
            // Act
            // create
            var subjects = new List<PollSubject>
            {
                new PollSubject {Id = 5, Title = "Titanic", Year = 1976},
                new PollSubject {Id = 6, Title = "Egy", Year = 2001},
                new PollSubject {Id = 7, Title = "Ketto", Year = 2003}
            };

            // event a lot
            for (var i = 0; i < 1500; i++)
            {
                subjects.Add(new PollSubject { Id = 1000 + i, Title = "z Film #" + i, Year = 1980 + i / 50 });
            }

            _dataManager.SavePollSubjectsBatch(subjects);

            // Assert
            var one = _dataManager.QueryPollSubject(6);
            Assert.AreNotEqual(null, one);
            Assert.AreEqual("Egy", one.Title);

            one = _dataManager.QueryPollSubject(7);
            Assert.AreNotEqual(null, one);
            Assert.AreEqual("Ketto", one.Title);

            var list = _dataManager.SearchPollSubjects("z Film #");
            Assert.AreEqual(1500, list.Count);
        }

        public virtual void QueryBannedUsers()
        {
            // Act
            var list = _dataManager.QueryBannedUsers();

            // Assert
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Noemi", list[0].Name);
        }

        public virtual void QueryUser()
        {
            // Act
            var one = _dataManager.QueryUser(1);
            var none = _dataManager.QueryUser(-1);

            // Assert
            Assert.AreNotEqual(null, one);
            Assert.AreEqual("Lali", one.Name);

            Assert.AreEqual(null, none);
        }

        public virtual void SearchUsers()
        {
            // Act
            var list = _dataManager.SearchUsers("A");

            // Assert
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Admin", list[0].Name);
            Assert.AreEqual("Agi", list[1].Name);
        }

        public virtual void SaveUser()
        {
            // Arrange
            var one = _dataManager.QueryUser(1);

            // Act
            // create
            var user = new User { Id = 5, Name = "Valaki", IsBanned = false };
            _dataManager.SaveUser(user);

            // update
            one.Name = "La";
            _dataManager.SaveUser(one);

            // Assert
            one = _dataManager.QueryUser(1);
            Assert.AreNotEqual(null, one);
            Assert.AreEqual("La", one.Name);
            one = _dataManager.QueryUser(5);
            Assert.AreNotEqual(null, one);
            Assert.AreEqual("Valaki", one.Name);
        }

        public virtual void QueryVoteUserVoted()
        {
            // Act
            var poll = _dataManager.QueryPolls(PollState.Voting)[0];
            var user = _dataManager.QueryUser(2);
            var vote = _dataManager.QueryVote(poll, user);

            // Assert
            Assert.AreNotEqual(null, vote);
            Assert.AreEqual("Valami Amerika", vote.Nomination.Subject.Title);
        }

        public virtual void QueryVoteUserNoVote()
        {
            // Act
            var poll = _dataManager.QueryPolls(PollState.Voting)[0];
            var user = _dataManager.QueryUser(4);
            var vote = _dataManager.QueryVote(poll, user);

            // Assert
            Assert.AreEqual(null, vote);
        }

        public virtual void QueryVoteNotExistingPoll()
        {
            // Act
            var poll = new Poll
            {
                Id = Guid.NewGuid(),
                State = PollState.Nomination,
                Text = "Proba1",
                Title = "Proba1",
                PublicationDate = DateTime.Now,
                NominationDeadline = DateTime.Now.AddDays(+1),
                VotingStartDate = DateTime.Now.AddDays(+3),
                VotingDeadline = DateTime.Now.AddDays(+6),
                AnnouncementDate = DateTime.Now.AddDays(+8)
            };
            var user = _dataManager.QueryUser(4);
            _dataManager.QueryVote(poll, user);

            // Assert
            // expected exception
        }

        public virtual void QueryVoteNotExistingUser()
        {
            // Act
            var poll = _dataManager.QueryPolls(PollState.Voting)[0];
            var user = new User { Id = 6, Name = "v", IsBanned = true };
            _dataManager.QueryVote(poll, user);

            // Assert
            // expected exception
        }

        public virtual void SaveVote()
        {
            // Arrange
            var poll = _dataManager.QueryPolls(PollState.Voting)[0];
            var user = _dataManager.QueryUser(2);
            var user3 = _dataManager.QueryUser(3);
            var user4 = _dataManager.QueryUser(4);
            var nom = _dataManager.QueryNominations(poll)[0];
            var vote = _dataManager.QueryVote(poll, user);

            // Act
            // create
            var newVote = new Vote { Date = DateTime.Now, User = user4, Nomination = nom };
            _dataManager.SaveVote(newVote);

            // update
            vote.User = user3;
            _dataManager.SaveVote(vote);

            // Assert
            var one = _dataManager.QueryVote(poll, user4);
            Assert.AreNotEqual(null, one);
            one = _dataManager.QueryVote(poll, user4);
            Assert.AreNotEqual(null, one);
        }
    }
}