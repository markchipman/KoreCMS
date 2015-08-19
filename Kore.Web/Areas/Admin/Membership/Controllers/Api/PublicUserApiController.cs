﻿using System.Linq;
using System.Web.OData;
using System.Web.OData.Query;
using Kore.Security.Membership;

namespace Kore.Web.Areas.Admin.Membership.Controllers.Api
{
    public class PublicUserApiController : ODataController
    {
        protected IMembershipService Service { get; private set; }

        public PublicUserApiController(IMembershipService service)
        {
            this.Service = service;
        }

        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
        public virtual IQueryable<PublicUserInfo> Get()
        {
            return Service.GetAllUsersAsQueryable().Select(x => new PublicUserInfo
            {
                Id = x.Id,
                UserName = x.UserName
            });
        }

        public virtual PublicUserInfo Get([FromODataUri] string key)
        {
            var entity = Service.GetUserById(key);
            return new PublicUserInfo
            {
                Id = entity.Id,
                UserName = entity.UserName
            };
        }
    }

    public class PublicUserInfo
    {
        public string Id { get; set; }

        public string UserName { get; set; }
    }
}