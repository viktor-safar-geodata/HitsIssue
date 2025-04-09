using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace ArcGIS.Net.FeatureCollectionLayer.HitsIssue
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void TheMapView_GeoViewTapped(
            object sender,
            Esri.ArcGISRuntime.UI.Controls.GeoViewInputEventArgs e
        )
        {
            var resultsPerLayer = await TheMapView.IdentifyLayersAsync(e.Position, 100, false, 200);

            var allGeoElements = resultsPerLayer.SelectMany(x =>
                x.SublayerResults.SelectMany(y => y.GeoElements)
            );

            // expecting 100 results
            var count = allGeoElements.Count();
            Debug.WriteLine("TheMapView_IdentifyLayersAsync: " + count);
        }
    }
}
