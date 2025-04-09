using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Mapping.Labeling;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Toolkit.UI;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;

namespace ArcGIS.Net.FeatureCollectionLayer.HitsIssue
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapViewModel : INotifyPropertyChanged
    {
        private const string ID = "id";

        public MapViewModel()
        {
            _map = new Map(SpatialReferences.WebMercator)
            {
                InitialViewpoint = new Viewpoint(
                    new Envelope(-180, -85, 180, 85, SpatialReferences.Wgs84)
                ),
            };

            // mock point graphics very close to each other
            var graphics = Enumerable.Range(1, 100).Select(CreateGraphic).ToList();

            var table = new FeatureCollectionTable(graphics, _fields) { Renderer = GetRenderer(), };

            var collectionLayer = new Esri.ArcGISRuntime.Mapping.FeatureCollectionLayer(
                new FeatureCollection([table])
            )
            {
                Id = "myFeatureCollectionLayer"
            };

            var layer = collectionLayer.Layers[0];
            layer.LabelsEnabled = true;
            layer.LabelDefinitions.Add(GetLabelDefinition());

            Map.OperationalLayers.Add(collectionLayer);
        }

        private Graphic CreateGraphic(int id)
        {
            return new(
                new MapPoint(id / 100d, id / 100d, SpatialReferences.Wgs84),
                [new KeyValuePair<string, object?>(ID, id)]
            );
        }

        public GeoViewController Controller { get; } = new GeoViewController();

        public ICommand GeoViewTappedCommand =>
            new RelayCommand(async param => await OnGeoViewTapped((GeoViewInputEventArgs)param));

        private async Task OnGeoViewTapped(GeoViewInputEventArgs args)
        {
            var resultsPerLayer = await Controller.IdentifyLayersAsync(args.Position, 100);

            var allGeoElements = resultsPerLayer.SelectMany(x =>
                x.SublayerResults.SelectMany(y => y.GeoElements)
            );

            // expecting 100 results
            var count = allGeoElements.Count();
            Debug.WriteLine("GeoViewController_IdentifyLayersAsync: " + count);
        }

        private LabelDefinition GetLabelDefinition()
        {
            var textSymbol = new TextSymbol
            {
                Color = System.Drawing.Color.Black,
                Size = 10,
                FontWeight = FontWeight.Bold,
                HaloColor = System.Drawing.Color.White,
                HaloWidth = 3,
            };

            string expressionScript = $"$feature.{ID}";
            var expression = new ArcadeLabelExpression(expressionScript);
            var def = new LabelDefinition(expression, textSymbol)
            {
                Placement = Esri.ArcGISRuntime.ArcGISServices.LabelingPlacement.PointCenterRight,
            };

            return def;
        }

        private Renderer GetRenderer()
        {
            return new SimpleRenderer(
                new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Red, 20)
            );
        }

        private readonly List<Field> _fields = [new Field(FieldType.Int32, ID, null, 20)];

        private Map _map;

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Map Map
        {
            get => _map;
            set
            {
                _map = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Raises the <see cref="MapViewModel.PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
