/// <summary>
/// The ImageCrop* models were introduced in Umbraco v7.0+.
/// Since we are compiling/testing Ditto against Umbraco v6.2.5,
/// we do not have access to the ImageCrop* models.
/// They have been reproduced here for testing purposes.
/// </summary>
namespace Our.Umbraco.Ditto.Tests.Models
{
    using System.Collections.Generic;
    using System.Web;

    public class ImageCropDataSet : IHtmlString
    {
        public string Src { get; set; }

        public ImageCropFocalPoint FocalPoint { get; set; }

        public IEnumerable<ImageCropData> Crops { get; set; }

        public string ToHtmlString()
        {
            return this.Src;
        }
    }

    public class ImageCropData
    {
        public string Alias { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public ImageCropCoordinates Coordinates { get; set; }
    }

    public class ImageCropFocalPoint
    {
        public decimal Left { get; set; }

        public decimal Top { get; set; }
    }

    public class ImageCropCoordinates
    {
        public decimal X1 { get; set; }

        public decimal Y1 { get; set; }

        public decimal X2 { get; set; }

        public decimal Y2 { get; set; }
    }
}