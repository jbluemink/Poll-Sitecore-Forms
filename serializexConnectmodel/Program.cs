using RadioPollResult.Models;
using Sitecore.XConnect.Serialization;
using System.IO;

namespace serializexConnectmodel
{
    /// <summary>
    /// A console app to generate the json model, run and it generate the "PreferencesModel, 0.1.json"
    /// deploy PreferencesModel, 0.1.json to 
    /// C:\<Path to xConnect>\root\App_Data\Models
    /// C:\<Path to indexer>\root\App_data\Models
    /// C:\<Path to marketing automation operations>\root\App_Data\Models
    /// see https://doc.sitecore.com/en/developers/101/sitecore-experience-platform/deploy-a-custom-model.html
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var json = XdbModelWriter.Serialize(PreferencesModel.Model);

            File.WriteAllText(RadioPollResult.Models.PreferencesModel.Model.FullName + ".json ", json);
        }
    }
}
