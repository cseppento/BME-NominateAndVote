﻿using NominateAndVote.DataModel.Model;
using System;

namespace NominateAndVote.DataModel
{
    public class SimpleDataModel : DefaultDataModel
    {
        public void LoadSampleData()
        {
            Clear();

            // News
            var news1 = new News { Id = Guid.NewGuid(), Title = "First", Text = "Blah blah", PublicationDate = DateTime.Now.AddDays(-2) };

            // Users
            var user1 = new User { Id = Guid.NewGuid(), IsBanned = false, Name = "Lali" };
            var user2 = new User { Id = Guid.NewGuid(), IsBanned = false, Name = "Agi" };
            var user3 = new User { Id = Guid.NewGuid(), IsBanned = true, Name = "Noemi" };
            var user4 = new User { Id = Guid.NewGuid(), IsBanned = false, Name = "Admin" };
            var admin = new Administrator { UserId = user4.Id };

            // Poll subjects
            var ps1 = new PollSubject { Id = 1, Title = "Ehezok viadala", Year = 2013 };
            var ps2 = new PollSubject { Id = 2, Title = "Valami Amerika", Year = 2005 };
            var ps3 = new PollSubject { Id = 3, Title = "Valami Amerika 2", Year = 2007 };

            // Poll in nomination state with 1 nomination
            var poll1 = new Poll
            {
                Id = Guid.NewGuid(),
                Text = "Ki a legjobb?",
                State = PollState.Nomination,
                PublicationDate = DateTime.Now.AddDays(-2),
                NominationDeadline = DateTime.Now.AddDays(+2),
                VotingStartDate = DateTime.Now.AddDays(+4),
                VotingDeadline = DateTime.Now.AddDays(+8),
                AnnouncementDate = DateTime.Now.AddDays(+12)
            };

            var poll1Nom = new Nomination
            {
                Id = Guid.NewGuid(),
                Text = "Mert en azt mondtam",
                User = user1,
                VoteCount = 0,
                Subject = ps1,
                Poll = poll1
            };

            // Poll with voting state with 2 nominations and 1 vote
            var poll2 = new Poll
            {
                Id = Guid.NewGuid(),
                Text =
                    "Melyik a legjobb magyar film?",
                State = PollState.Voting,
                PublicationDate = DateTime.Now.AddDays(-10),
                NominationDeadline = DateTime.Now.AddDays(-2),
                VotingStartDate = DateTime.Now.AddDays(-1),
                VotingDeadline = DateTime.Now.AddDays(+8),
                AnnouncementDate = DateTime.Now.AddDays(+12)
            };

            var poll2Nom1 = new Nomination
            {
                Id = Guid.NewGuid(),
                Text = "A kedvencem",
                User = user1,
                VoteCount = 0,
                Subject = ps2,
                Poll = poll2
            };

            var poll2Nom2 = new Nomination
            {
                Id = Guid.NewGuid(),
                Text = "Vicces",
                User = user2,
                VoteCount = 0,
                Subject = ps3,
                Poll = poll2
            };

            var vote = new Vote { Date = DateTime.Now.AddDays(-1), User = user2, Nomination = poll2Nom2 };
            poll2Nom2.VoteCount = 1;

            // Poll with closed state
            var poll3 = new Poll
            {
                Id = Guid.NewGuid(),
                Text = "Melyik a legjobb magyar film?",
                State = PollState.Voting,
                PublicationDate = DateTime.Now.AddDays(-10),
                NominationDeadline = DateTime.Now.AddDays(-8),
                VotingStartDate = DateTime.Now.AddDays(-6),
                VotingDeadline = DateTime.Now.AddDays(-4),
                AnnouncementDate = DateTime.Now.AddDays(-1)
            };

            var poll3Nom1 = new Nomination
            {
                Id = Guid.NewGuid(),
                Text = "Jok a szineszek",
                User = user1,
                VoteCount = 0,
                Subject = ps2,
                Poll = poll3
            };

            var poll3Nom2 = new Nomination
            {
                Id = Guid.NewGuid(),
                Text = "Jo a tortenet",
                User = user2,
                VoteCount = 0,
                Subject = ps3,
                Poll = poll3
            };

            var vote1 = new Vote { Date = DateTime.Now.AddDays(-2), User = user2, Nomination = poll3Nom1 };
            var vote2 = new Vote { Date = DateTime.Now.AddDays(-3), User = user1, Nomination = poll3Nom2 };
            poll3Nom1.VoteCount = 1;
            poll3Nom2.VoteCount = 1;

            // Poll with nominations on the same subject (to be merged)
            var poll4 = new Poll
            {
                Id = Guid.NewGuid(),
                Text = "Melyik a legjobb magyar film?",
                State = PollState.Nomination,
                PublicationDate = DateTime.Now.AddDays(-10),
                NominationDeadline = DateTime.Now.AddDays(-8),
                VotingStartDate = DateTime.Now.AddDays(+2),
                VotingDeadline = DateTime.Now.AddDays(+7),
                AnnouncementDate = DateTime.Now.AddDays(+12)
            };

            var poll4Nom1 = new Nomination
            {
                Id = Guid.NewGuid(),
                Text = "Valami",
                User = user1,
                VoteCount = 0,
                Subject = ps2,
                Poll = poll4
            };

            var poll4Nom2 = new Nomination
            {
                Id = Guid.NewGuid(),
                Text = "Csak",
                User = user2,
                VoteCount = 0,
                Subject = ps2,
                Poll = poll4
            };

            // Add objects to the lists
            Users.Add(user1);
            Users.Add(user2);
            Users.Add(user3);
            Users.Add(user4);

            Administrators.Add(admin);

            News.Add(news1);

            Nominations.Add(poll1Nom);
            Nominations.Add(poll2Nom1);
            Nominations.Add(poll2Nom2);
            Nominations.Add(poll3Nom1);
            Nominations.Add(poll3Nom2);
            Nominations.Add(poll4Nom1);
            Nominations.Add(poll4Nom2);

            PollSubjects.Add(ps1);
            PollSubjects.Add(ps2);
            PollSubjects.Add(ps3);

            Polls.Add(poll1);
            Polls.Add(poll2);
            Polls.Add(poll3);
            Polls.Add(poll4);

            Votes.Add(vote);
            Votes.Add(vote1);
            Votes.Add(vote2);

            // Refresh the relational lists
            RefreshPocoRelationalLists();
        }
    }
}