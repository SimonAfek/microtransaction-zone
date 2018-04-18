using System;
using Newtonsoft.Json;

namespace DIHMT.Models
{
    public class TumblrResponseModel
    {
        [JsonProperty("meta")] public TumblrResponseModelMeta Meta { get; set; }
        [JsonProperty("response")] public TumblrResponseModelResponse Response { get; set; }
    }

    public class TumblrResponseModelMeta
    {
        [JsonProperty("status")] public int Status { get; set; }
        [JsonProperty("msg")] public string Msg { get; set; }
    }

    public class TumblrResponseModelResponse
    {
        [JsonProperty("posts")]
        public TumblrResponseModelResponsePost[] Posts { get; set; }
    }

    public class TumblrResponseModelResponsePost
    {
        // text, quote, link, answer, video, audio, photo, chat
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("post_url")] public string PostUrl { get; set; }
        [JsonProperty("short_url")] public string ShortUrl { get; set; }
        [JsonProperty("summary")] public string Summary { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("caption")] public string Caption { get; set; }
        [JsonProperty("state")] public string State { get; set; }
        [JsonProperty("date")] public DateTime Date { get; set; }
        [JsonProperty("photos")] public TumblrResponseModelResponsePostPhoto[] Photos { get; set; }
        [JsonProperty("asking_name")] public string AskingName { get; set; }
        [JsonProperty("question")] public string Question { get; set; }
        [JsonProperty("answer")] public string Answer { get; set; }
        [JsonProperty("url")] public string Url { get; set; }
    }

    public class TumblrResponseModelResponsePostPhoto
    {
        [JsonProperty("caption")] public string Caption { get; set; }
        [JsonProperty("original_size")] public TumblrResponseModelResponsePostPhotoDetails OriginalSize { get; set; }
    }

    public class TumblrResponseModelResponsePostPhotoDetails
    {
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("width")] public int Width { get; set; }
        [JsonProperty("height")] public int Height { get; set; }
    }
}
