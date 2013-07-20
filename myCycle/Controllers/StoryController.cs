using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using RestSharp;
using myCycle.Models;
using myCycle.Utilities;
using System.Linq;

namespace myCycle.Controllers
{
    public class StoryController : Controller
    {
        private readonly IRestClient restClient;

        public StoryController(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public ActionResult Index()
        {
            ViewBag.ProjectNames = GetProjectNames();
            return View(new StoryCycleModel());
        }

        [HttpPost]
        public ActionResult Index(StoryCycleModel model)
        {
            model.StoryName = GetStoryName(model.ProjectName, model.StoryNumber);
            model.StatusPieces = GetStatusPieces(model.ProjectName, model.StoryNumber).ToList();
            ViewBag.ProjectNames = GetProjectNames();
            return View(model);
        }

        private IEnumerable<KeyValuePair<string,string>> GetProjectNames()
        {
            var request = new RestRequest("projects.xml", Method.GET);
            var projects = (List<dynamic>) DynamicHelper.ToDList(restClient.ExecuteXml(request).project);
            return projects.Select(p => new KeyValuePair<string, string>(p.identifier, p.name)).ToList();
        }

        private string GetStoryName(string projectName,string storyNumber)
        {
            var request = new RestRequest("projects/{project}/cards/{card}.xml", Method.GET);
            request.AddUrlSegment("project", projectName).AddUrlSegment("card", storyNumber);
            var card = restClient.ExecuteXml(request);
            return card.name;
        }

        private IEnumerable<StatusPiece> GetStatusPieces(string projectName,string storyNumber)
        {
            var request = new RestRequest("projects/{project}/cards/{card}.xml");
            request.AddUrlSegment("project", projectName).AddUrlSegment("card", storyNumber);
            var card = restClient.ExecuteXml(request);
            var version = int.Parse(card.version);

            var pieces = new List<StatusMoment>();

            foreach (var index in Enumerable.Range(1, version))
            {
                var versionedRequest = new RestRequest("projects/{project}/cards/{card}.xml?version={version}");
                versionedRequest.AddUrlSegment("project", projectName).AddUrlSegment("card", storyNumber).AddUrlSegment("version", index.ToString(CultureInfo.InvariantCulture));
                var versionedCard = restClient.ExecuteXml(versionedRequest);
                var moment = DateTime.Parse((string) versionedCard.modified_on).ToUniversalTime();
                var properties = (List<dynamic>)DynamicHelper.ToDList(versionedCard.properties.property);
                var property = properties.FirstOrDefault(p => ((string) p.name).Equals("status", StringComparison.InvariantCultureIgnoreCase));
                if(property!=null)
                {
                    pieces.Add(new StatusMoment{Moment = moment, Status = property.value.ToString()});
                }
            }
            pieces.Add(new StatusMoment{Moment = DateTime.UtcNow, Status = "Current"});

            var dictionary = new Dictionary<string, TimeSpan>();
            for (var i = 0; i < pieces.Count - 1; i++)
            {
                var currentPiece = pieces[i];
                var nextPiece = pieces[i + 1];
                var span = nextPiece.Moment - currentPiece.Moment;
                if (!dictionary.ContainsKey(currentPiece.Status))
                {
                    dictionary.Add(currentPiece.Status, new TimeSpan(0));
                }
                dictionary[currentPiece.Status] += span;
            }

            var statusPieces = dictionary.Select(p => new StatusPiece{Status = p.Key, Duration = p.Value.TotalMinutes}).ToArray();

            return statusPieces;

        }
    }
}