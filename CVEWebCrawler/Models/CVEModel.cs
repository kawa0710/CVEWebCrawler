using Sky.Data.Csv;

namespace CVEWebCrawler.Models
{
    public class CVEModel
    {
        public int SN { get; set; }

        public string? ID { get; set; }

        public string? URL { get; set; }

        public string? KB_ID { get; set; }

        public string? KB_URL { get; set; }

        public string? DownloadURL { get; set; }

        public bool? IsDownloadable { get; set; }
    }

    public class CVEModelResolver : AbstractDataResolver<CVEModel>
    {
        public override CVEModel Deserialize(List<String> data)
        {
            return new CVEModel
            {
                ID = data[0],
            };
        }

        public override List<String?> Serialize(CVEModel data)
        {
            return new List<String?>
            {
                data.SN.ToString(),
                data.ID,
                data.URL,
                data.KB_ID,
                data.KB_URL,
                data.IsDownloadable.HasValue ? (data.IsDownloadable.Value ? "Y" : "N") : null,
                data.DownloadURL,
            };
        }
    }
}
