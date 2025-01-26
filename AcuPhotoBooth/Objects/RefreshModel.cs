using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MeshAPIUI
{
    public class RemeshRequestModel
    {
        public string input_task_id { get; set; } = "01948129-4e03-7675-82dd-8b64f3cf3b9e";
        public List<string> target_formats { get; set; } = new List<string> { "stl" };
        public string topology { get; set; } = "quad";
        public int target_polycount { get; set; } = 50000;
        public double resize_height { get; set; } = 1.0;
        public string origin_at { get; set; } = "bottom";


        public RemeshRequestModel(string imageID)
        {
            input_task_id = imageID;
        }
    }

    public class RemeshResultModel
    {
        // ex="01949f80-ef7d-7734-8446-26136b9aa11c"
        public string result { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class RemeshModelUrls
    {
        public string glb { get; set; }
        public string stl { get; set; }
    }

    public class RemeshModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string art_style { get; set; }
        public string object_prompt { get; set; }
        public string style_prompt { get; set; }
        public string negative_prompt { get; set; }
        public string status { get; set; }
        public long created_at { get; set; }
        public int progress { get; set; }
        public long started_at { get; set; }
        public long finished_at { get; set; }
        public long expires_at { get; set; }
        public object task_error { get; set; }
        public string model_url { get; set; }
            //get the url STL
        public ModelUrls model_urls { get; set; }
        public string thumbnail_url { get; set; }
        [JsonPropertyName("texture_urls")]
        public List<RemeshTextureUrl> texture_urls { get; set; }
    }
        
    public class RemeshTextureUrl
    {
        public string base_color { get; set; }
    }
}
