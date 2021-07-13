using Sitecore.XConnect;
using System;

namespace RadioPollResult
{
    [Serializable]
    [FacetKey(DefaultFacetKey)]
    public class Preferences : Sitecore.XConnect.Facet
    {
        public const string DefaultFacetKey = "Preferences";

        //All facets must declare an empty constructor - this is an oData restriction. The constructor can be private.
        private Preferences() { }

        public Preferences(string favoritCarColor)
        { 
            FavoritCarColor = favoritCarColor;
        }
        public string FavoritCarColor { get; set; } // Example: Red
    }
}