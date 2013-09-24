using System;
using NogginBox.OwnerQueries.Forms;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.Services;


namespace NogginBox.OwnerQueries.Filters
{
	public class OwnerIsLoggedInUser : IFilterProvider
	{
		private readonly IWorkContextAccessor _workContextAccessor;

		public OwnerIsLoggedInUser(IWorkContextAccessor workContextAccessor)
		{
			_workContextAccessor = workContextAccessor;
		}

		public Localizer T { get; set; }

		public void Describe(DescribeFilterContext describe) {
			describe.For("ContentOwner", T("Content Owner"), T("The owner of the content item"))
				.Element("LoggedInUserOwnerMatch", T("Logged in user"), T("Is owned by logged in user"),
					ApplyFilter,
					context => T("Content is owned by logged in user")
				);
		}

		public void ApplyFilter(FilterContext context)
		{
			var user = _workContextAccessor.GetContext().CurrentUser;
			if (user == null) return;

			Action<IAliasFactory> selector = alias => alias.ContentPartRecord<CommonPartRecord>();
			Action<IHqlExpressionFactory> filter = x => x.Eq("OwnerId", user.Id);
			context.Query.Where(selector, filter);
		}
	}
}