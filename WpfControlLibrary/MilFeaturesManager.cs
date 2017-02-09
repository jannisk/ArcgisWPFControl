using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;

namespace WpfControlLibrary
{
    public class MilFeaturesManager
    {
        private readonly MapWindow _mapWindow;

        public MilFeaturesManager(MapWindow mapWindow)
        {
            _mapWindow = mapWindow;
        }

        /// <summary>
        /// Creates an overlay with mil symbols
        /// </summary>
        /// <remarks>
        /// ATTENTION Multipoint symbols throw exception they 
        /// do not appear in Mapview
        /// </remarks>
        public async Task<GraphicsOverlay> CreateMilOverlay()

        {
            // Creating graphics overlay
            var situationalOverlay = new GraphicsOverlay(); 
            var symbolStyle = await DictionarySymbolStyle.OpenAsync("mil2525d", @"C:\Users\jkotsis\Documents\Visual Studio 2015\Projects\WPF-WinForms\WFControlLibrary\WFHost\Resources\mil2525d.stylx");

            var messages = ParseMessages();
            //var symfieldOverides = new Dictionary<string, string>();
            //symfieldOverides.Add("sidc", "sic");
            foreach (var attributes in messages)
            {
                situationalOverlay.Graphics.Add(CreateGraphic(attributes));
            }
            var fieldNames = symbolStyle.SymbologyFieldNames;
            var textFieldNames = symbolStyle.TextFieldNames;
            DictionaryRenderer renderer = new DictionaryRenderer(symbolStyle); //, symfieldOverides, new Dictionary<string, string>() );
            situationalOverlay.Renderer = renderer;
            return situationalOverlay;
        }

        private Graphic CreateGraphic(Dictionary<string, object> attributes)
        {
            // get spatial reference
            int wkid = int.Parse((string)attributes["_wkid"]);
            SpatialReference sr = SpatialReference.Create(wkid);

            // get points from coordinates' string
            var points = new Esri.ArcGISRuntime.Geometry.PointCollection(sr);

            var controlPoints= ((string)attributes["_control_points"]).Split(';');
            foreach(var controlPoint in controlPoints)
            {
                var coordinates = controlPoint.Split(',');
                var mapPoint = new MapPoint(double.Parse(coordinates[0]), double.Parse(coordinates[1]), 0, sr);
                points.Add(mapPoint);

            }
            return new Graphic(new Multipoint(points), attributes);
        }

        private List<Dictionary<string, object>> ParseMessages()
        {
            var messages = new List<Dictionary<string, object>>();
            new HashSet<string>();

            var xmlDoc = new XmlDocument();
            //Next xml contains only single point symbols
            //TODO: Multipoint symbols throw when drawing in the canvas 
            //
            xmlDoc.LoadXml(Properties.Resources.Mil2525DUnitMessages);
            foreach (XmlNode nd in xmlDoc.DocumentElement.SelectNodes("message"))
            {
                var message = new Dictionary<string, object>();
                foreach(XmlNode aNode in nd.ChildNodes)
                {
                    System.Diagnostics.Debug.Print(aNode.Name + ": "  + aNode.InnerText);
                    message.Add(aNode.Name, aNode.InnerText);
                }
                messages.Add(message);
            }
           
            return messages;

        }
    }
}