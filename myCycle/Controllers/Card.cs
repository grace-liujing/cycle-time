using System;
using System.Collections.Generic;
using myCycle.Utilities;
using System.Linq;

namespace myCycle.Controllers
{
    public class Card
    {
        public string Name { get; private set; }
        public string StoryNumber { get; private set; }
        public int Version { get; private set; }
        public DateTime ModifiedOn { get; private set; }
        public List<KeyValuePair<string, string>> Properties { get; private set; }
        private static readonly string[] propertyRange = new[]{"status"};

        private Card(string name, string storyNumber, int version, DateTime modifiedOn, List<KeyValuePair<string, string>> properties)
        {
            Name = name;
            StoryNumber = storyNumber;
            Version = version;
            ModifiedOn = modifiedOn;
            Properties = properties;
        }

        public static Card Create(dynamic card)
        {
            return new Card(GetCardName(card), GetNumber(card), GetVersion(card), GetModifiedTime(card), GetProperties(card));
        }

        private static List<KeyValuePair<string, string>> GetProperties(dynamic card)
        {
            var properties = (List<dynamic>)DynamicHelper.ToDList(card.properties.property);
            return properties
                .Where(p=>propertyRange.Contains(((string)p.name).ToLower()))
                .Select(p => new KeyValuePair<string, string>((string)(p.name), (string)(p.value))).ToList();
        }

        private static string GetCardName(dynamic card)
        {
            return (string)card.name;
        }

        private static int GetVersion(dynamic card)
        {
            return int.Parse((string)card.version);
        }

        private static string GetNumber(dynamic card)
        {
            return (string)card.number;
        }

        private static DateTime GetModifiedTime(dynamic card)
        {
            return DateTime.Parse((string)card.modified_on).ToUniversalTime();
        }
    }
}