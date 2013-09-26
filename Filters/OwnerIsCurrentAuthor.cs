using System;
using NogginBox.OwnerQueries.Accessors;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.Services;


namespace NogginBox.OwnerQueries.Filters
{
	public class OwnerIsCurrentAuthor : IFilterProvider
	{
		private readonly ICurrentContentAccessor _currentContentAccessor;

		public OwnerIsCurrentAuthor(ICurrentContentAccessor currentContentAccessor)
		{
			_currentContentAccessor = currentContentAccessor;
		}

		public Localizer T { get; set; }

		public void Describe(DescribeFilterContext describe) {
			describe.For("ContentOwner", T("Content Owner"), T("The owner of the content item"))
				.Element("AuthorOwnerMatch", T("Author of current content"), T("Is owned by the author of the page's main document. Works best with widgets."),
					ApplyFilter,
					context => T("Content is owned by main document author")
				);
		}

		public void ApplyFilter(FilterContext context)
		{
			var currentContent = _currentContentAccessor.CurrentContentItem;
			if (currentContent == null) return;

			var commonPart = currentContent.As<CommonPart>();
			if (commonPart == null) return;

			Action<IAliasFactory> selector = alias => alias.ContentPartRecord<CommonPartRecord>();
			Action<IHqlExpressionFactory> filter = x => x.Eq("OwnerId", commonPart.Owner.Id);
			context.Query.Where(selector, filter);
		}
	}
}