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
        private readonly ICardFactory cardFactory;

        public StoryController(IRestClient restClient, ICardFactory cardFactory)
        {
            this.restClient = restClient;
            this.cardFactory = cardFactory;
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

        private IEnumerable<KeyValuePair<string, string>> GetProjectNames()
        {
            var request = new RestRequest("projects.xml", Method.GET);
            var projects = (List<dynamic>)DynamicHelper.ToDList(restClient.ExecuteXml(request).project);
            return projects.Select(p => new KeyValuePair<string, string>(p.identifier, p.name)).ToList();
        }

        private string GetStoryName(string projectName, string storyNumber)
        {
            return cardFactory.Get(projectName, storyNumber).Name;
        }

        private static IEnumerable<StatusPiece> GetStatusPieces(IEnumerable<Card> versionedCards)
        {
            var moments = GetStatusMoments(versionedCards).ToList();
            moments.Add(new StatusMoment { Moment = DateTime.UtcNow, Status = "Current" });
            return moments.Squeeze((next, current) => new StatusPiece(current.Status, next.Moment - current.Moment))
                .GroupBy(s => s.Status)
                .Select(g => g.Aggregate((p, ap) => new StatusPiece(p.Status, p.Span + ap.Span)))
                .ToList();
        }

        private static IEnumerable<StatusMoment> GetStatusMoments(IEnumerable<Card> versionedCards)
        {
            return versionedCards.Select(c =>
                                                          {
                                                              var statusProperty = c.Properties.FirstOrDefault(p => p.Key.ToLower() == "status");
                                                              return !statusProperty.Equals(default(KeyValuePair<string, string>)) ? new StatusMoment{Moment = c.ModifiedOn, Status = statusProperty.Value} : null;
                                                          }).ToArray();
        }

        private IEnumerable<StatusPiece> GetStatusPieces(string projectName, string storyNumber)
        {
            var version = GetCardMaxVersion(projectName, storyNumber);
            var versionedCards = Enumerable.Range(1, version).Select(v => cardFactory.Get(projectName, storyNumber, v)).ToArray();
            return GetStatusPieces(versionedCards);
        }

        private int GetCardMaxVersion(string projectName, string storyNumber)
        {
            return cardFactory.Get(projectName, storyNumber).Version;
        }
    }
}