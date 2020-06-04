using System.Linq;
using System.Threading.Tasks;
using Mapzen;
using UnityEngine;
using Nominatim.API.Geocoders;
using Nominatim.API.Models;


public class NominatimLookup : MonoBehaviour
{
    [SerializeField] private RegionMap map;

    [SerializeField] private float expanse = 0.01f;
    [SerializeField] private string location;
    
    // Start is called before the first frame update
    async void  Start()
    {
        var results = await SearchLocationsAsync();
        var result = results.FirstOrDefault();

        if (result != null && map != null)
        {
            var minlat = result.Latitude - expanse;
            var maxlat = result.Latitude + expanse;
            var minLon = result.Longitude - expanse;
            var maxLon = result.Longitude + expanse;

            var min = new LngLat(minLon, minlat);
            var max = new LngLat(maxLon, maxlat);
            map.Area.max = max;
            map.Area.min = min;
        }
    }
    
    private Task<GeocodeResponse[]> SearchLocationsAsync()
    {

        var geoCoder = new ForwardGeocoder();
        var request = new ForwardGeocodeRequest
        {
            queryString = location,
            BreakdownAddressElements = true,
            ShowExtraTags = true,
            ShowAlternativeNames = true,
            ShowGeoJSON = true,
            LimitResults = 100,
        };

        return geoCoder.Geocode(request);
    }

}
