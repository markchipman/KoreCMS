﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using Kore.Caching;
using Kore.Localization.Domain;
using Kore.Localization.Models;
using Kore.Localization.Services;
using Kore.Web.Http.OData;
using Kore.Web.Security.Membership.Permissions;

using Microsoft.OData.Core;
using Microsoft.OData.Core.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;

namespace Kore.Web.ContentManagement.Areas.Admin.Localization.Controllers.Api
{
    //[Authorize(Roles = KoreConstants.Roles.Administrators)]
    public class LocalizableStringApiController : GenericODataController<LocalizableString, Guid>
    {
        private readonly ICacheManager cacheManager;

        public LocalizableStringApiController(
            ILocalizableStringService service,
            ICacheManager cacheManager)
            : base(service)
        {
            this.cacheManager = cacheManager;
        }

        protected override Guid GetId(LocalizableString entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(LocalizableString entity)
        {
            entity.Id = Guid.NewGuid();
        }

        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public virtual IHttpActionResult GetComparitiveTable([FromODataUri] string cultureCode)
        {
            if (!CheckPermission(ReadPermission))
            {
                return Unauthorized();
            }
            else
            {
                var query = Service.Repository.Table
                        .Where(x => x.CultureCode == null || x.CultureCode == cultureCode)
                        .GroupBy(x => x.TextKey)
                        .Select(grp => new ComparitiveLocalizableString
                        {
                            Key = grp.Key,
                            InvariantValue = grp.FirstOrDefault(x => x.CultureCode == null).TextValue,
                            LocalizedValue = grp.FirstOrDefault(x => x.CultureCode == cultureCode) == null
                                ? string.Empty
                                : grp.FirstOrDefault(x => x.CultureCode == cultureCode).TextValue
                        });

                return Ok(query);
            }
        }

        //[EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
        //[HttpGet]
        //public virtual PageResult<ComparitiveLocalizableString> GetComparitiveTable(
        //    [FromODataUri] string cultureCode, ODataQueryOptions<ComparitiveLocalizableString> queryOptions)
        //{
        //    if (!CheckPermission(ReadPermission))
        //    {
        //        var query = Enumerable.Empty<ComparitiveLocalizableString>().AsQueryable();
        //        IQueryable results = queryOptions.ApplyTo(query);

        //        return new PageResult<ComparitiveLocalizableString>(
        //            results as IEnumerable<ComparitiveLocalizableString>,
        //            Request.ODataProperties().NextLink,
        //            Request.ODataProperties().TotalCount);
        //    }
        //    else
        //    {
        //        var query = Service.Repository.Table
        //                .Where(x => x.CultureCode == null || x.CultureCode == cultureCode)
        //                .GroupBy(x => x.TextKey)
        //                .Select(grp => new ComparitiveLocalizableString
        //                {
        //                    Key = grp.Key,
        //                    InvariantValue = grp.FirstOrDefault(x => x.CultureCode == null).TextValue,
        //                    LocalizedValue = grp.FirstOrDefault(x => x.CultureCode == cultureCode) == null
        //                        ? string.Empty
        //                        : grp.FirstOrDefault(x => x.CultureCode == cultureCode).TextValue
        //                });

        //        IQueryable results = queryOptions.ApplyTo(query);

        //        return new PageResult<ComparitiveLocalizableString>(
        //            results as IEnumerable<ComparitiveLocalizableString>,
        //            Request.ODataProperties().NextLink,
        //            Request.ODataProperties().TotalCount);
        //    }
        //}

        [HttpPost]
        public virtual IHttpActionResult PutComparitive(ODataActionParameters parameters)
        {
            if (!CheckPermission(WritePermission))
            {
                return new UnauthorizedResult(new AuthenticationHeaderValue[0], ActionContext.Request);
            }

            string cultureCode = (string)parameters["cultureCode"];
            string key = (string)parameters["key"];
            var entity = (ComparitiveLocalizableString)parameters["entity"];

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!key.Equals(entity.Key))
            {
                return BadRequest();
            }

            var localizedString = Service.Repository.Table.FirstOrDefault(x => x.CultureCode == cultureCode && x.TextKey == key);

            if (localizedString == null)
            {
                localizedString = new LocalizableString
                {
                    Id = Guid.NewGuid(),
                    CultureCode = cultureCode,
                    TextKey = key,
                    TextValue = entity.LocalizedValue
                };
                Service.Insert(localizedString);
            }
            else
            {
                localizedString.TextValue = entity.LocalizedValue;
                Service.Update(localizedString);
            }

            cacheManager.Remove(string.Concat("Kore_LocalizableStrings_", cultureCode));

            return Updated(entity);
        }

        [HttpPost]
        public virtual IHttpActionResult DeleteComparitive(ODataActionParameters parameters)
        {
            if (!CheckPermission(WritePermission))
            {
                return new UnauthorizedResult(new AuthenticationHeaderValue[0], ActionContext.Request);
            }

            string cultureCode = (string)parameters["cultureCode"];
            string key = (string)parameters["key"];

            var entity = Service.FindOne(x => x.CultureCode == cultureCode && x.TextKey == key);
            if (entity == null)
            {
                return NotFound();
            }

            entity.TextValue = null;
            Service.Update(entity);
            //Repository.Delete(entity);

            cacheManager.Remove(string.Concat("Kore_LocalizableStrings_", cultureCode));

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override Permission ReadPermission
        {
            get { return CmsPermissions.LocalizableStringsRead; }
        }

        protected override Permission WritePermission
        {
            get { return CmsPermissions.LocalizableStringsWrite; }
        }
    }
}