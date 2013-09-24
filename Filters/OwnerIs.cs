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
	public class OwnerIs : IFilterProvider
	{
		private readonly IWorkContextAccessor _workContextAccessor;

		public OwnerIs(IWorkContextAccessor workContextAccessor)
		{
			_workContextAccessor = workContextAccessor;
		}

		public Localizer T { get; set; }

		public void Describe(DescribeFilterContext describe) {
			describe.For("ContentOwner", T("Content Owner"), T("The owner of the content item"))
				.Element("OwnerMatch", T("Specified user"), T("Is owned by the specified user"),
					ApplyFilter,
					(Func<dynamic, LocalizedString>)DisplayFilter,
					UsersFilterForms.FormId
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

		public LocalizedString DisplayFilter(dynamic context)
		{
			string userName = Convert.ToString(context.State.UserName);

			if (String.IsNullOrEmpty(userName))
			{
				userName = "any user";
			}

			return T("Content is owned by {0}", String.Join(", ", userName));
		}
	}
}