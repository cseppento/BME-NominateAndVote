﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NominateAndVote.DataModel;
using NominateAndVote.DataModel.Poco;
using NominateAndVote.DataModel.Tests;
using NominateAndVote.DataTableStorage.Tests;
using NominateAndVote.RestService.Controllers;
using NominateAndVote.RestService.Models;
using System;
using System.Linq;
using System.Web.Http.Results;

namespace NominateAndVote.RestService.Tests.Controllers
{
    public abstract class NominationControllerTests
    {
        private NominationController _controller;
        private IDataManager _dataManager;

        public abstract void Initialize();

        private void DoInitialize(IDataManager dataManager)
        {
            _dataManager = dataManager;
            _controller = new NominationController(_dataManager);
        }

        public abstract void Save();

        private void DoSave()
        {
            // Arrange
            var poll = _dataManager.QueryPolls()[0];

            var user = new User { Id = 888, IsBanned = false, Name = "Kis Bela" };
            _dataManager.SaveUser(user);
            var subject = _dataManager.QueryPollSubject(1);

            var bindingModel = new SaveNominationBindingModel
            {
                Text = "text",
                PollId = poll.Id.ToString(),
                UserId = user.Id,
                SubjectId = subject.Id
            };

            // Act
            var result = _controller.Save(bindingModel) as OkNegotiatedContentResult<Nomination>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(Guid.Empty, result.Content.Id);
            Assert.AreEqual("text", result.Content.Text);
        }

        public abstract void Save_Update();

        private void DoSave_Update()
        {
            // Arrange
            var nominations = _controller.GetForUser("1");
            var one = nominations.ElementAt(0);
            var bindingModel = new SaveNominationBindingModel
            {
                Id = one.Id.ToString(),
                Text = "proba",
                PollId = one.Poll.Id.ToString(),
                UserId = one.User.Id,
                SubjectId = one.Subject.Id
            };

            // Act
            var result = _controller.Save(bindingModel) as OkNegotiatedContentResult<Nomination>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(Guid.Empty, result.Content.Id);
            Assert.AreEqual("proba", result.Content.Text);
        }

        public abstract void Save_Null();

        private void DoSave_Null()
        {
            // Act
            var result = _controller.Save(null) as BadRequestErrorMessageResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Message, "No data");
        }

        public abstract void Delete();

        private void DoDelete()
        {
            // Arrange
            var poll = _dataManager.QueryPolls()[1];
            var q = _dataManager.QueryNominations(poll);
            var nomination = q.First();

            // Act
            var result = _controller.Delete(poll.Id.ToString(), nomination.Id.ToString());

            // Assert
            poll = _dataManager.QueryPolls()[1];

            Assert.IsTrue(result);
            Assert.IsFalse(poll.Nominations.Contains(nomination));
        }

        [TestClass]
        public class NominationControllerMemoryTests : NominationControllerTests
        {
            [TestInitialize]
            public override void Initialize()
            {
                DoInitialize(new SampleDataModel().CreateDataManager());
            }

            [TestMethod]
            [TestCategory("Integration/RestService/Memory/NominationController")]
            public override void Save()
            {
                DoSave();
            }

            [TestMethod]
            [TestCategory("Integration/RestService/Memory/NominationController")]
            public override void Save_Update()
            {
                DoSave_Update();
            }

            [TestMethod]
            [TestCategory("Integration/RestService/Memory/NominationController")]
            public override void Save_Null()
            {
                DoSave_Null();
            }

            [TestMethod]
            [TestCategory("Integration/RestService/Memory/NominationController")]
            public override void Delete()
            {
                DoDelete();
            }
        }

        [TestClass]
        public class NominationControllerTableStoregeTests : NominationControllerTests
        {
            private DataTableStorageTestHelper _helper;

            [TestInitialize]
            public override void Initialize()
            {
                _helper = new DataTableStorageTestHelper();
                _helper.Initialize(new SampleDataModel());
                DoInitialize(_helper.TableStorageDataManager);
            }

            [TestCleanup]
            public void Cleanup()
            {
                _helper.CleanUp();
            }

            [TestMethod]
            [TestCategory("Integration/RestService/TableStorage/NominationController")]
            public override void Save()
            {
                DoSave();
            }

            [TestMethod]
            [TestCategory("Integration/RestService/TableStorage/NominationController")]
            public override void Save_Update()
            {
                DoSave_Update();
            }

            [TestMethod]
            [TestCategory("Integration/RestService/TableStorage/NominationController")]
            public override void Save_Null()
            {
                DoSave_Null();
            }

            [TestMethod]
            [TestCategory("Integration/RestService/TableStorage/NominationController")]
            public override void Delete()
            {
                DoDelete();
            }
        }
    }
}