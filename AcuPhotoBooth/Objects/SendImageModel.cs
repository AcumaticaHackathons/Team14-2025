using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshAPIUI
{
    public class SendImageModel
    {
        public string image_url { get; set; } = "https://bstevens.net/wp-content/uploads/2025/01/Brenda.png";
        public bool enable_pbr { get; set; } = true;
        public bool should_remesh { get; set; } = true;
        public bool should_texture { get; set; } = true;
        public SendImageModel(string urlImage)
        {
            image_url = urlImage;
        }
    }
    public class SendImageResultModel
    {
        // ex="01949f80-ef7d-7734-8446-26136b9aa11c"
        public string result { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ModelUrls
    {
        public string glb { get; set; }
        public string fbx { get; set; }
        public string usdz { get; set; }
        public string obj { get; set; }
        public string stl { get; set; }
    }

    public class ThreeDTaskModel
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
    public ModelUrls model_urls { get; set; }
        // Need this one
    public string thumbnail_url { get; set; }
    public List<ImageTextureUrl> texture_urls { get; set; }
}

public class ImageTextureUrl
{
    public string base_color { get; set; }
    public string metallic { get; set; }
    public string roughness { get; set; }
    public string normal { get; set; }
}
}
