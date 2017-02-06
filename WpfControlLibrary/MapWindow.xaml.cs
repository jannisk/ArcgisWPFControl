using System.Windows.Controls;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Http;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.ArcGISRuntime.Tasks.Offline;
using Esri.ArcGISRuntime.Symbology;
using System.Threading;
using System.Windows;
using System;
using System.Windows.Media;
using Esri.ArcGISRuntime.UI;
using System.Collections.Generic;
using System.Xml;

namespace WpfControlLibrary
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : UserControl
    {
        private const string basemapUrl = "http://sampleserver6.arcgisonline.com/arcgis/rest/services/World_Street_Map/MapServer";
        private const string operationalUrl = "http://sampleserver6.arcgisonline.com/arcgis/rest/services/Sync/SaveTheBaySync/FeatureServer/0";

        private string _localTileCachePath;

        private string _localGeodatabasePath;

        // Graphics overlay to host graphics
        private GraphicsOverlay _polygonOverlay;
        private GraphicsOverlay _situationalOverlay;

        private CancellationTokenSource _cancellationTokenSource;
        private bool _enabledSketching;

        public MapWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            // Create new Map with basemap

            MyMapView.NavigationCompleted += (s, e) => { GenerateLocalTilesButton.IsEnabled = MyMapView.MapScale < 6000000; };
            MyMapView.Loaded += MyMapView_Loaded;

        }

        private  async void MyMapView_Loaded(object sender, RoutedEventArgs e)
        {
            TryLoadOnlineLayers();
            MyMapView.ViewpointChanged += MyMapView_ViewpointChanged;
            MyMapView.GeoViewTapped += OnGeoViewTapped;
        }

        private async void OnGeoViewTapped(object sender, GeoViewInputEventArgs e)
        {
            var tolerance = 10d; // Use larger tolerance for touch
            var maximumResults = 1; // Only return one graphic  
            var onlyReturnPopups = false; // Return more than popups
            if (_enabledSketching) return;
            // Use the following method to identify graphics in a specific graphics overlay
            IdentifyGraphicsOverlayResult identifyResults = await MyMapView.IdentifyGraphicsOverlayAsync(
                 _situationalOverlay,
                 e.Position,
                 tolerance,
                 onlyReturnPopups,
                 maximumResults);

            // Check if we got results
            if (identifyResults.Graphics.Count > 0)
            {
                //  Display to the user the identify worked.
                MessageBox.Show("Tapped on graphic", "");
            }
        }

        private void MyMapView_ViewpointChanged(object sender, EventArgs e)
        {
            // Unhook the event
            MyMapView.ViewpointChanged -= MyMapView_ViewpointChanged;

            CreateOverlay();
            CreateMilOverlay();

        }

        private void DataOptionChecked(object sender, RoutedEventArgs e)
        {
            MyMapView.Map.Basemap.BaseLayers.Clear();
            if (UseOnlineDataOption.IsChecked == true)
            {
                TryLoadOnlineLayers();
            }
            else // offline
            {
                TryLoadLocalLayers();
            }
        }

        private async void GetTiles(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                var context = SynchronizationContext.Current;
                // show the status controls
                StatusPanel.Visibility = Visibility.Visible;
                StatusMessagesList.Items.Add("Requesting tile cache ...");

                // cancel if an earlier call was made
                _cancellationTokenSource?.Cancel();

                // get a cancellation token for this task
                _cancellationTokenSource = new CancellationTokenSource();
                var cancelToken = _cancellationTokenSource.Token;             
               
                // create a new ExportTileCacheTask to generate the tiles
                var exportTilesTask = ExportTileCacheTask.CreateAsync(new Uri(basemapUrl)).Result;
                //// define options for the new tiles (extent, scale levels, format
                var exportTileCacheParams =  exportTilesTask.CreateDefaultExportTileCacheParametersAsync(MyMapView.VisibleArea, 6000000.0, 1.0).Result;
             
                // download the tile package to the app's local folder
                var outFolder = AppDomain.CurrentDomain.BaseDirectory;

                var job =  exportTilesTask.ExportTileCache(exportTileCacheParams, outFolder + "asdf.tpk" );
                job.JobChanged += (s, e) =>
                {
                    switch (job.Status)
                    {
                        case Esri.ArcGISRuntime.Tasks.JobStatus.Succeeded:
                            context.Post(AddMessageToList, new Tuple<string, double>("Synchronization is complete!", 100));
                            break;

                        // StatusMessagesList.Items.Add("Synchronization is complete!");
                        case Esri.ArcGISRuntime.Tasks.JobStatus.Failed:
                            context.Post(AddMessageToList, new Tuple<string, double>(job.Error.Message, 0));
                            break;
                        default:
                            var magn = 0.0;
                            foreach (var jobMessage in job.Messages)
                            {
                                if (jobMessage.Message.Contains("Finished::"))
                                {
                                    // parse out the percentage complete and update the progress bar
                                    var numString = jobMessage.Message.Substring(jobMessage.Message.IndexOf("::") + 2, 3).Trim();
                                    var pct = 0.0;
                                    if (double.TryParse(numString, out pct))
                                    {
                                        magn = pct;
                                    }
                                }
                            }

                            context.Post(AddMessageToList, new Tuple<string, double>("Sync in progress ..." + magn + "%", magn));
                            break;
                    }

                };
               // report changes in the job status
                var result = await job.GetResultAsync();
                _localTileCachePath = result.Path;
            }
            catch (Exception exp)
            {
                StatusMessagesList.Items.Clear();
                StatusMessagesList.Items.Add("Unable to get local tiles: " + exp.Message);
            }
            finally
            {
                // reset the progress indicator
                StatusProgressBar.Value = 0;
                StatusMessagesList.Items.Add(string.Format("Local tiles created at: {0}", _localTileCachePath));
            }
        }

        private void AddMessageToList(object v)
        {
            var tuple = (Tuple<string, double>) v;
            StatusMessagesList.Items.Clear();
            StatusMessagesList.Items.Add(tuple.Item1);
            StatusProgressBar.Value = tuple.Item2;
        }

        private async void GetFeatures(object sender, RoutedEventArgs e)
        {

        }

        private async void TryLoadLocalLayers()
        {
            try
            {
                if (string.IsNullOrEmpty(_localTileCachePath))
                {
                    throw new Exception("Local features do not yet exist. Please generate them first.");
                }

                if (string.IsNullOrEmpty(_localTileCachePath))
                {
                    throw new Exception("Local tiles do not yet exist. Please generate them first.");
                }
                var basemapLayer = new ArcGISTiledLayer(new Uri(_localTileCachePath));
                await basemapLayer.LoadAsync();
                MyMapView.Map.Basemap.BaseLayers.Add(basemapLayer);
            }
            catch (Exception exp)
            {
                MessageBox.Show("Unable to load local layers: " + exp.Message, "Load Layers");

            }
        }

        private async void TryLoadOnlineLayers()
        {
            try
            {

                Map myMap = new Map(SpatialReferences.WebMercator);

                // create an online tiled map service layer, an online feature layer
                var basemapLayer = new ArcGISTiledLayer(new Uri(basemapUrl));
                var operationalLayer = new FeatureLayer(new Uri(operationalUrl));
           
                // give the feature layer an ID so it can be found later
                operationalLayer.Id = "Sightings";

                //var messageLayer = new Esri.ArcGISRuntime.Symbology.DictionarySymbolStyle
                // initialize the layers
                await basemapLayer.LoadAsync();
                await operationalLayer.LoadAsync();

                //// see if there was an exception when initializing the layers, if so throw an exception
                //if (basemapLayer.InitializationException != null ||
                //    operationalLayer.InitializationException != null)
                //{
                //    unable to load one or more of the layers, throw an exception
                //    throw new Exception("Could not initialize layers");
                //}

             

                // Assign the map to the MapView
                MyMapView.Map = myMap;

                // add layers
                MyMapView.Map.Basemap.BaseLayers.Add(basemapLayer);
                MyMapView.Map.OperationalLayers.Add(operationalLayer);
            }
            // handle a variety of possible exceptions
            catch (ArcGISWebException arcGISExp)
            {
                // token required?
                MessageBox.Show("Unable to load online layers: credentials may be required", "Load Error");


            }
            catch (System.Net.Http.HttpRequestException httpExp)
            {
                // not connected? server down? wrong URI?
                MessageBox.Show("Unable to load online layers: check your connection and verify service URLs", "Load Error");


            }
            catch (Exception exp)
            {
                // other problems ...
                MessageBox.Show("Unable to load online layers: " + exp.Message, "Load Error");


            }
        }

        private void CreateOverlay()

        {
            // Get area that is shown in a MapView
            Polygon visibleArea = MyMapView.VisibleArea;

            // Get extent of that area
            Envelope extent = visibleArea.Extent;

            // Get central point of the extent
            MapPoint centerPoint = extent.GetCenter();

            // Create values inside the visible extent for creating graphic
            var extentWidth = extent.Width / 5;
            var extentHeight = extent.Height / 10;

            // Create point collection
            Esri.ArcGISRuntime.Geometry.PointCollection points = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.WebMercator)
                {
                    new MapPoint(centerPoint.X - extentWidth * 2, centerPoint.Y - extentHeight * 2),
                    new MapPoint(centerPoint.X - extentWidth * 2, centerPoint.Y + extentHeight * 2),
                    new MapPoint(centerPoint.X + extentWidth * 2, centerPoint.Y + extentHeight * 2),
                    new MapPoint(centerPoint.X + extentWidth * 2, centerPoint.Y - extentHeight * 2)
                };

            // Create overlay to where graphics are shown
            GraphicsOverlay overlay = new GraphicsOverlay();
            // Add points to the graphics overlay
            foreach (var point in points)
            {
                // Create new graphic and add it to the overlay
                overlay.Graphics.Add(new Graphic(point));
            }

            // Create symbol for points
            SimpleMarkerSymbol pointSymbol = new SimpleMarkerSymbol()
            {
                Color = Colors.Yellow,
                Size = 30,
                Style = SimpleMarkerSymbolStyle.Square,
            };

            // Create simple renderer with symbol
            SimpleRenderer renderer = new SimpleRenderer(pointSymbol);

            // Set renderer to graphics overlay
            overlay.Renderer = renderer;

            // Add created overlay to the MapView
            MyMapView.GraphicsOverlays.Add(overlay);

        }

        /// <summary>
        /// Creates an overlay with mil symbols
        /// </summary>
        /// <remarks>
        /// ATTENTION Multipoint symbols throw exception they 
        /// do not appear in Mapview
        /// </remarks>
        private async void CreateMilOverlay()

        {
            // Creating graphics overlay
            _situationalOverlay = new GraphicsOverlay(); 
            var symbolStyle = await DictionarySymbolStyle.OpenAsync("mil2525d", @"C:\Users\jkotsis\Documents\Visual Studio 2015\Projects\WPF-WinForms\WFControlLibrary\WFHost\Resources\mil2525d.stylx");

            var messages = ParseMessages();
            //var symfieldOverides = new Dictionary<string, string>();
            //symfieldOverides.Add("sidc", "sic");
            foreach (var attributes in messages)
            {
                _situationalOverlay.Graphics.Add(CreateGraphic(attributes));
            }
            var fieldNames = symbolStyle.SymbologyFieldNames;
            var textFieldNames = symbolStyle.TextFieldNames;
            DictionaryRenderer renderer = new DictionaryRenderer(symbolStyle); //, symfieldOverides, new Dictionary<string, string>() );
            _situationalOverlay.Renderer = renderer;
            MyMapView.GraphicsOverlays.Add(_situationalOverlay);
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
   
        private void MySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        /// <summary>
        /// Sketch editor does not work, THROWS fatal exception
        /// program aborts when try to sketch other than point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button_Click(object sender, RoutedEventArgs e)
        {
            _enabledSketching = true;
            try
            {
                var fillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Cross, Colors.Black, new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Colors.Black, 1));

                var aStyle = new SketchStyle();
                aStyle.FillSymbol = fillSymbol;
                MyMapView.SketchEditor.Style = aStyle;
                var  geometry = await MyMapView.SketchEditor.StartAsync(SketchCreationMode.Circle, false);
                GraphicsOverlay graphicsOverlay;
                graphicsOverlay = new GraphicsOverlay();
                MyMapView.GraphicsOverlays.Add(graphicsOverlay);
                //graphicsOverlay = MyMapView.GraphicsOverlays[0];
                //var outlineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Colors.Black, 1.0);
                //var line = new Graphic(geometry, outlineSymbol);
                //var markerSymbol =  new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Square, Colors.AliceBlue, 12);
                //var aPoint = new Graphic(geometry, markerSymbol);
                var aGraphic = new Graphic(geometry, fillSymbol);
                graphicsOverlay.Graphics.Add(aGraphic);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }

        }
    }
}
