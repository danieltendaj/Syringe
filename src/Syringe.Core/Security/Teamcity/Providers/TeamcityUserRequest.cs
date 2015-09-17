﻿using RestSharp;

namespace Syringe.Core.Security.Teamcity.Providers
{
    public class TeamcityUserRequest
    {
        private const string ResourceUrl = "/guestAuth/app/rest/users/{0}";

        public RestRequest GetRequest(string userName)
        {
            var request = new RestRequest(Method.GET)
                          {
                              Resource = string.Format(ResourceUrl, userName),
                              RootElement = "user"
                          };

            request.AddHeader("Accept", "application/json");

            return request;
        }
    }
}