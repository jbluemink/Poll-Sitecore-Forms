using EPExpressTab.Data;
using EPExpressTab.Repositories;
using RadioPollResult.Models;
using System;

namespace RadioPollResult
{
	public class EpExpressPreferences : EpExpressViewModel
	{
		public override string Heading => "Custom preferences";
		public override string TabLabel => "preferences";
		public override object GetModel(Guid contactId)
		{
			var contact = EPRepository.GetContact(contactId, RadioPollResult.Preferences.DefaultFacetKey);
			var preferences = contact?.GetFacet<Preferences>(Preferences.DefaultFacetKey);


			return new EpExpressPreferencesModel
			{
				FavoritCarColor = preferences?.FavoritCarColor
			};
		}
		public override string GetFullViewPath(object model)
		{
			return "/views/RadioPollResult/EpExpressPreferences.cshtml";
		}
	}
}