using Sitecore.XConnect;
using Sitecore.XConnect.Schema;

namespace RadioPollResult.Models
{
    public class PreferencesModel
    {
        public static XdbModel Model { get; } = PreferencesModel.BuildModel();

        private static XdbModel BuildModel()
        {
            XdbModelBuilder modelBuilder = new XdbModelBuilder("PreferencesModel", new XdbModelVersion(0, 1));

            modelBuilder.ReferenceModel(Sitecore.XConnect.Collection.Model.CollectionModel.Model);
            modelBuilder.DefineFacet<Contact, Preferences>(Preferences.DefaultFacetKey);

            return modelBuilder.BuildModel();
        }
    }
}