using System;
using NogginBox.OwnerQueries.Accessors;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.Services;


namespace NogginBox.OwnerQueries.Filters
{
	[OrchardFeature("NogginBox.OwnerProfilePageQueries")]
	public class OwnerIsProfilePageUser : IFilterProvider
	{
		private readonly IProfilePageUserAccessor _currentProfilePageUserAccessor;

		public OwnerIsProfilePageUser(IProfilePageUserAccessor workContextAccessor)
		{
			_currentProfilePageUserAccessor = workContextAccessor;
		}

		public Localizer T { get; set; }

		public void Describe(DescribeFilterContext describe) {
			describe.For("ContentOwner", T("Content Owner"), T("The owner of the content item"))
				.Element("ProfilePageUserMatch", T("Profile page user"), T("Is owned by the current profile page user. Only works in widget on profile page."),
					ApplyFilter,
					context => T("Content is owned by profile page user")
				);
		}

		public void ApplyFilter(FilterContext context)
		{
			var profilePageUser = _currentProfilePageUserAccessor.CurrentProfilePageUser;
			if (profilePageUser == null) return;

			Action<IAliasFactory> selector = alias => alias.ContentPartRecord<CommonPartRecord>();
			Action<IHqlExpressionFactory> filter = x => x.Eq("OwnerId", profilePageUser.Id);
			context.Query.Where(selector, filter);
		}
	}
}