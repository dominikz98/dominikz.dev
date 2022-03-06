using dominikz.Api.Models.Options;
using dominikz.Endpoints.ViewModels;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace dominikz.Api.Commands
{
    public class GetPodcast : IRequest<VMPodcast>
    {
    }

    public class GetPodcastHandler : IRequestHandler<GetPodcast, VMPodcast>
    {
        private readonly PodcastOptions _options;
        private readonly HttpClient _noobitClient;

        public GetPodcastHandler(IHttpClientFactory httpClientFactory, IOptions<PodcastOptions> options)
        {
            _options = options.Value;
            _noobitClient = httpClientFactory.CreateClient("noobit");
        }

        public async Task<VMPodcast> Handle(GetPodcast request, CancellationToken cancellationToken)
        {
            var feed = await _noobitClient.GetStringAsync(_options.RSSFeed, cancellationToken);

            XNamespace itunes = "http://www.itunes.com/dtds/podcast-1.0.dtd";
            var channel = XElement.Parse(feed).Element("channel");

            // Cast channel
            var podcast = new VMPodcast()
            {
                Description = channel.Element("description")?.Value,
                Email = channel.Element(itunes + "owner")?.Element(itunes + "email")?.Value,
                Categories = channel.Elements(itunes + "category").Select(x => x.Attribute("text").Value).ToList(),
                ImageUrl = channel.Element("image")?.Element("link")?.Value,
                RSS = "https://www.noobit.dev/feed/podcast",
                Instagram = "https://www.instagram.com/tapetenlasagne/",
                Episodes = new(),
                Spotify = "DUMMY",
                ITunes = "DUMMY"
            };

            // Cast items
            foreach (var item in channel.Elements("item"))
                podcast.Episodes.Add(new VMPodcastEpisode()
                {
                    Title = item.Element("title")?.Value,
                    Description = item.Element("description")?.Value,
                    Duration = int.TryParse(item.Element(itunes + "duration")?.Value, out var duration) ? duration : 0,
                    ImageUrl = item.Element(itunes + "image")?.Attribute("href")?.Value,
                    Publication = DateTime.TryParse(item.Element("pubDate")?.Value, out var pubdate) ? pubdate : DateTime.Now,
                    RSS = podcast.RSS,
                    Instagram = "DUMMY",
                    Spotify = "DUMMY",
                    ITunes = "DUMMY"
                });

            return podcast;
        }
    }
}
