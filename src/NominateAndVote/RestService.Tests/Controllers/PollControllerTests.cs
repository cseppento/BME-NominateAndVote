﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NominateAndVote.DataModel;
using NominateAndVote.DataModel.Model;
using NominateAndVote.DataModel.Tests;
using NominateAndVote.RestService.Controllers;
using System.Collections.Generic;

namespace NominateAndVote.RestService.Tests.Controllers
{
    [TestClass]
    public class PollControllerTests
    {
        private PollController _controller;
        private IDataManager _dataManager;

        [TestInitialize]
        public void Initialize()
        {
            _dataManager = new SampleDataModel().CreateDataManager();
            _controller = new PollController(_dataManager);
        }

        [TestMethod]
        public void GetNomination()
        {
            // Arrange

            // Act
            var result = _controller.GetNominationPolls() as List<Poll>;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue((_dataManager.QueryPolls(PollState.Nomination)).Count == result.Count);
        }

        [TestMethod]
        public void GetVoting()
        {
            // Arrange

            // Act
            var result = _controller.GetVotingPolls() as List<Poll>;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue((_dataManager.QueryPolls(PollState.Voting)).Count == result.Count);
        }
    }
}