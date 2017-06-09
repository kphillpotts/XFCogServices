using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFCogServices.Models
{
    class SearchResult
    {
        [JsonProperty("readLink")]
        public string ReadLink { get; set; }
        [JsonProperty("webSearchUrl")]
        public string WebSearchUrl { get; set; }
        [JsonProperty("totalEstimatedMatches")]
        public int TotalEstimatedMatches { get; set; }
        [JsonProperty("value")]
        public List<Image> Images { get; set; }
        [JsonProperty("nextOffsetAddCount")]
        public int NextOffsetAddCount { get; set; }
        [JsonProperty("displayShoppingSourcesBadges")]
        public bool DisplayShoppingSourcesBadges { get; set; }
        [JsonProperty("displayRecipeSourcesBadges")]
        public bool DisplayRecipeSourcesBadges { get; set; }
    }
}
