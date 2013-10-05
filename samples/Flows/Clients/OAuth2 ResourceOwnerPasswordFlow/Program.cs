﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using Thinktecture.IdentityModel.Client;
using Thinktecture.IdentityModel.Client.Extensions;

namespace Thinktecture.Samples
{
    class Program
    {
        //static Uri _baseAddress = new Uri(Constants.WebHostv1BaseAddress);
        static Uri _baseAddress = new Uri(Constants.WebHostv2BaseAddress);

        static void Main(string[] args)
        {
            var response = RequestToken();

            var token = response.AccessToken;
            //token = RefreshToken(response.RefreshToken);

            CallService(token);
        }

        private static TokenResponse RequestToken()
        {
            "Requesting token.".ConsoleYellow();

            var client = new OAuth2Client(
                new Uri(Constants.AS.OAuth2TokenEndpoint),
                Constants.Clients.ResourceOwnerClient,
                Constants.Clients.ResourceOwnerClientSecret);

            var response = client.RequestResourceOwnerPasswordAsync("bob", "abc!123", "read").Result;

            Console.WriteLine(" access token");
            response.AccessToken.ConsoleGreen();

            Console.WriteLine("\n refresh token");
            response.RefreshToken.ConsoleGreen();
            Console.WriteLine();

            return response;
        }

        private static void CallService(string token)
        {
            var client = new HttpClient {
                BaseAddress = _baseAddress
            };

            client.SetBearerToken(token);

            while (true)
            {
                "Calling service.".ConsoleYellow();

                Helper.Timer(() =>
                {
                    var response = client.GetAsync("identity").Result;
                    response.EnsureSuccessStatusCode();

                    var claims = response.Content.ReadAsAsync<IEnumerable<Tuple<string, string>>>().Result;
                    Helper.ShowConsole(claims);
                });

                Console.ReadLine();
            }            
        }

        private static string RefreshToken(string refreshToken)
        {
            "Refreshing token.".ConsoleYellow();

            var client = new OAuth2Client(
                new Uri(Constants.AS.OAuth2TokenEndpoint),
                Constants.Clients.ResourceOwnerClient,
                Constants.Clients.ResourceOwnerClientSecret);

            var response = client.RequestRefreshTokenAsync(refreshToken).Result;

            return response.AccessToken;
        }
    }
}