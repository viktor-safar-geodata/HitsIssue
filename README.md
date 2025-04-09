A PoC illustrating a bug in `GeoViewController.IdentifyLayersAsync()` of `FeatureCollectionLayer`

Setup:
- Esri.ArcGISRuntime.WPF 200.6
- Esri.ArcGISRuntime.Toolkit.WPF 200.6
- create a collection of Graphic => graphics
- add then into a FeatureCollection
- add the FeatureCollection to FeatureCollectionLayer
- add the FeatureCollectionLayer into Map.OperationalLayers
- run the app and click on the cluster of symbols in the middle of the map

Bug repro:
- in code behind, we have `TheMapView_GeoViewTapped` which calls `IdentifyLayersAsync` directly on the `MapView` object
- in VM, we have a `GeoViewController` and a handler method `OnGeoViewTapped`, which calls `IdentifyLayersAsync` on the `GeoViewController`
- in the output we can see the number of GeoElements returned from both methods
- the direct path `MapView.IdentifyLayersAsync` returns 100 GeoElements
- the path via the `GeoViewController.IdentifyLayersAsync` return only a single GeoElement
