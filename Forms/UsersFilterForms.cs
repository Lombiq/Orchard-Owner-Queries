using System;
using System.Web.Mvc;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Events;
using Orchard.Localization;
using Orchard.Users.Models;
using Orchard.ContentManagement;

namespace NogginBox.OwnerQueries.Forms
{
	public interface IFormProvider : IEventHandler
	{
		void Describe(dynamic context);
	}

	public class UsersFilterForms : IFormProvider
	{
		private readonly IOrchardServices _service;
		protected dynamic Shape { get; set; }
		public Localizer T { get; set; }

		public const String FormId = "OwnerIsUserMatchTypeForm";

		public UsersFilterForms(IShapeFactory shapeFactory, IOrchardServices orchardServices)
		{
			_service = orchardServices;
			Shape = shapeFactory;
		}

		public void Describe(dynamic context) {
			Func<IShapeFactory, dynamic> form =
				shape => {

					var f = Shape.Form(
						Id: "SelectTags",
						_Users: Shape.SelectList(
							Id: "userNames", Name: "UserNames",
							Title: T("Tags"),
							Description: T("Select some users."),
							Size: 10,
							Multiple: true
							)
						);

					var users = _service.ContentManager.Query<UserPart, UserPartRecord>()
					                    .Where(u => u.RegistrationStatus == UserStatus.Approved)
					                    .List();
					
					foreach (var user in users) {
						f._Users.Add(new SelectListItem { Value = user.UserName, Text = user.UserName });
					}

					return f;
				};

			context.Form(FormId, form);

		}
	}
}