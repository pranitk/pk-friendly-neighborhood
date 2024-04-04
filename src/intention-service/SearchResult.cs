using Newtonsoft.Json;

namespace intention_service
{
    public class SearchResult
    {
        [JsonProperty("@search.captions")]
        public string SearchCaptions { get; set; }

        [JsonProperty("@search.highlights")]
        public string SearchHighlights { get; set; }

        [JsonProperty("@search.reranker_score")]
        public string SearchRerankerScore { get; set; }

        [JsonProperty("@search.score")]
        public double SearchScore { get; set; }

        [JsonProperty("Autism_Diagnosis_Summary")]
        public string AutismDiagnosisSummary { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
