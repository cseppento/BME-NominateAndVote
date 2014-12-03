﻿using NominateAndVote.DataModel;
using System.Web.Http;

namespace NominateAndVote.RestService.Controllers
{
    [RoutePrefix("api/Admin")]
    public class AdminController : BaseApiController
    {
        public AdminController()
        {
        }

        public AdminController(IDataManager dataManager)
            : base(dataManager)
        {
        }

        [Route("BanUser")]
        [HttpPost]
        public IHttpActionResult BanUser(string userId)
        {
            long id;
            if (long.TryParse(userId, out id))
            {
                var user = DataManager.QueryUser(id);
                if (user != null) {
                    user.IsBanned = true;
                    DataManager.SaveUser(user);

                    return Ok(user);
                }
                return NotFound();             
            }

            return NotFound();
        }

        [Route("UnBanUser")]
        [HttpPost]
        public IHttpActionResult UnBanUser(string userId)
        {
            long id;
            if (long.TryParse(userId, out id))
            {
                var user = DataManager.QueryUser(id);

                if (user != null)
                {
                    user.IsBanned = false;
                    DataManager.SaveUser(user);

                    return Ok(user);
                }
                else
                {
                    return NotFound();
                }
            }

            return NotFound();
        }
    }
}