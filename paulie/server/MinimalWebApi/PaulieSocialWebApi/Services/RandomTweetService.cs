﻿using PaulieSocialWebApi.Extensions;
using PaulieSocialWebApi.Models;
using System.Web;

namespace PaulieSocialWebApi.Services
{
    public interface IRandomTweetService
    {
        Task<TweetModel> GetRandomVipTweet(string id);
    }

    public class RandomTweetService : IRandomTweetService
    {
        private readonly HttpClient _httpClient;

        public RandomTweetService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("TwitterClient");
        }

        public async Task<TweetModel> GetRandomVipTweet(string id)
        {
            var builder = new UriBuilder($"{_httpClient.BaseAddress}users/{id}/tweets");
            builder.Port = -1;
            var queryStrings = HttpUtility.ParseQueryString(builder.Query);
            queryStrings.AddStandardTwitterQueryStrings();
            queryStrings["exclude"] = "retweets,replies";
            builder.Query = queryStrings.ToString();
            var url = builder.ToString();

            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            var tweets = await response.DeserializeOrThrow();

            var getIndexOfRandomTweet = new Random().Next(0, tweets.Data.Length);

            return new TweetModel
            {
                Data = new[] { tweets.Data[getIndexOfRandomTweet] },
                Includes = tweets.Includes
            };
        }
    }
}
