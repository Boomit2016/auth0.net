﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Auth0.Core;
using Auth0.Core.Http;
using PortableRest;
using Auth0.ManagementApi.Client.Models;

namespace Auth0.ManagementApi.Client.Clients
{
    public class RulesClient : ClientBase, IRulesClient
    {
        public RulesClient(ApiConnection connection)
            : base(connection)
        {
        }

        public Task<Rule> Create(RuleCreateRequest request)
        {
            return Connection.PostAsync<Rule>("rules", ContentTypes.Json, request, null, null, null, null, null);
        }

        public Task Delete(string id)
        {
            return Connection.DeleteAsync<object>("rules/{id}", new Dictionary<string, string>
            {
                {"id", id}
            });
        }

        public Task<Rule> Get(string id, string fields = null, bool includeFields = true)
        {
            return Connection.GetAsync<Rule>("rules/{id}",
                new Dictionary<string, string>
                {
                    {"id", id}
                },
                new Dictionary<string, string>
                {
                    {"fields", fields},
                    {"include_fields", includeFields.ToString().ToLower()}
                }, null);
        }

        public Task<IList<Rule>> GetAll(bool? enabled = null, string fields = null, bool includeFields = true, string stage = null)
        {
            return Connection.GetAsync<IList<Rule>>("rules", null,
                new Dictionary<string, string>
                {
                    {"enabled", enabled.HasValue ? enabled.Value.ToString().ToLower() : null},
                    {"fields", fields},
                    {"include_fields", includeFields.ToString().ToLower()},
                    {"stage", stage}
                }, null);
        }

        // TODO: Look at making fields Nullable, otherwise default values are sent during PATCH
        public Task<Rule> Update(string id, RuleUpdateRequest request)
        {
            return Connection.PatchAsync<Rule>("rules/{id}", request, new Dictionary<string, string>
            {
                {"id", id}
            });
        }
    }
}