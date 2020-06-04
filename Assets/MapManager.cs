using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapzen;
using Nominatim.API.Geocoders;
using Nominatim.API.Models;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    
    [SerializeField] private float expanse = 0.001f;

    public InputField SearchField;
    public Text LoadingIndicator;
    public RegionMap map;
    public InputField apiKey;

    private bool isSearching = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateMap()
    {
        if(isSearching)
            return;

        try
        {
            isSearching = true;

            LoadingIndicator.text = "Resolving request...";
            var results = SearchLocationsAsync().GetAwaiter().GetResult();
            var result = results.FirstOrDefault();
            
            
            if (result != null && map != null)
            {
                var minLat = result.Latitude - expanse;
                var maxLat = result.Latitude + expanse;
                var minLon = result.Longitude - expanse;
                var maxLon = result.Longitude + expanse;

                var min = new LngLat(minLon, minLat);
                var max = new LngLat(maxLon, maxLat);
                map.Area.max = max;
                map.Area.min = min;

                map.ApiKey = apiKey.text;
                
                LoadingIndicator.text = "Downloading map...";
                map.DownloadTilesAsync();
                
            }
            
        }
        finally
        {
            isSearching = false;
        }
    }

    public void Update()
    {
        LoadingIndicator.enabled = isSearching;
    }


    private Task<GeocodeResponse[]> SearchLocationsAsync()
    {

        var geoCoder = new ForwardGeocoder();
        var request = new ForwardGeocodeRequest
        {
            queryString = SearchField.text,
            BreakdownAddressElements = true,
            ShowExtraTags = true,
            ShowAlternativeNames = true,
            ShowGeoJSON = true,
            LimitResults = 100,
        };

        return geoCoder.Geocode(request);
    }
    
}
