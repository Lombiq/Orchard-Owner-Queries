using NogginBox.OwnerQueries.Forms;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.Services;
using System;
using System.Linq;


namespace NogginBox.OwnerQueries.Filters
{
	public class OwnerIs : IFilterProvider
	{
		public Localizer T { get; set; }

		public void Describe(DescribeFilterContext describe) {
			describe.For("ContentOwner", T("Content Owner"), T("The owner of the content item"))
				.Element("OwnerMatch", T("Specified user"), T("Is owned by the specified user"),
					ApplyFilter,
					DisplayFilter,
					UsersFilterForms.FormId
				);
		}

		public void ApplyFilter(FilterContext context)
		{
			var userList = (String)context.State.Users;
			if (userList == null) return;

			var userIds = UsersFilterForms.GetUserIds(userList);
			if (!userIds.Any()) return;

			Action<IAliasFactory> selector = alias => alias.ContentPartRecord<CommonPartRecord>();
			Action<IHqlExpressionFactory> filter = x => x.InG("OwnerId", userIds);
			context.Query.Where(selector, filter);
		}

		public LocalizedString DisplayFilter(dynamic context)
		{
			var userNameList = UsersFilterForms.GetUserNames((String)context.State.Users);

			var users = (userNameList.Any())
				            ? String.Join(", ", userNameList)
				            : "any user";

			return T("Content is owned by {0}", users);
		}
	}
}