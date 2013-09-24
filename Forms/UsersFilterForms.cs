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
							Id: "tagids", Name: "TagIds",
							Title: T("Tags"),
							Description: T("Select some users."),
							Size: 10,
							Multiple: true
							),
						_Exclusion: Shape.FieldSet(
							_OperatorOneOf: Shape.Radio(
								Id: "operator-is-one-of", Name: "Operator",
								Title: T("Is one of"), Value: "0", Checked: true
								),
							_OperatorIsAllOf: Shape.Radio(
								Id: "operator-is-all-of", Name: "Operator",
								Title: T("Is all of"), Value: "1"
								)
							));

					var users = _service.ContentManager.Query<UserPart, UserPartRecord>()
					                    .Where(u => u.RegistrationStatus == UserStatus.Approved)
					                    .List();
					
					foreach (var user in users) {
						f._Users.Add(new SelectListItem { Value = user.Id.ToString(), Text = user.UserName });
					}

					return f;
				};

			context.Form("SelectTags", form);

		}
	}
}