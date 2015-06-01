namespace Our.Umbraco.Ditto.Tests.Models
{
    public class CurrentContentTestModel
    {
        [CurrentContent]
        public MyMetaDataModel MetaData1 { get; set; }

        public MyMetaDataModel MetaData2 { get; set; }
    }

    public class MyMetaDataModel
    {
        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }
    }
}