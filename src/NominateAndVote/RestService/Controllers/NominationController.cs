﻿using NominateAndVote.DataModel;
using NominateAndVote.DataModel.Model;
using NominateAndVote.RestService.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NominateAndVote.RestService.Controllers
{
    [RoutePrefix("api/Nomination")]
    public class NominationController : ApiController
    {
        private readonly IDataManager _dataManager;

        public NominationController()
            : base()
        {
            var model = new SimpleDataModel();
            model.LoadSampleData();
            _dataManager = new DataModelManager(model);
        }

        public NominationController(IDataManager dataManager)
            : base()
        {
            if (dataManager == null)
            {
                throw new ArgumentNullException("The data manager must not be null", "dataManager");
            }

            _dataManager = dataManager;
        }

        [Route("User")]
        [HttpGet]
        public List<Nomination> Get(string id)
        {
            Guid idGuid;
            if (Guid.TryParse(id, out idGuid))
            {
                var user = _dataManager.QueryUser(idGuid);
                return _dataManager.QueryNominations(user);
            }
            return null;
        }

        [Route("Save")]
        [HttpPost]
        public IHttpActionResult Save(NominationBindingModel newsBindingModel)
        {
            if (newsBindingModel == null)
            {
                return BadRequest("No data");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nomination = newsBindingModel.ToPoco();
            if (nomination.Id.Equals(Guid.Empty))
            {
                nomination.Id = Guid.NewGuid();
            }
            else
            {
                var poll = _dataManager.QueryPoll(nomination.Poll.Id);
                var user = _dataManager.QueryUser(nomination.User.Id);
                var nominations = _dataManager.QueryNominations(poll, user);
                Nomination oldNomination;
                foreach (var n in nominations)
                {
                    if (n.Id == nomination.Id)
                    {
                        oldNomination = nomination;
                        break;
                    }
                }
            }

            _dataManager.SaveNomination(nomination);

            return Ok(nomination);
        }

        [Route("Delete")]
        [HttpDelete]
        public bool Delete(string id)
        {
            Guid idGuid;
            if (Guid.TryParse(id, out idGuid))
            {
                _dataManager.DeleteNomination(idGuid);
                return true;
            }
            return false;
        }
    }
}