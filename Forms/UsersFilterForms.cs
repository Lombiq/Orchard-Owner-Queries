using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
							Id: "users", Name: "Users",
							Title: T("Tags"),
							Description: T("Select some users."),
							Size: 10,
							Multiple: true
							)
						);

					var users = _service.ContentManager.Query<UserPart, UserPartRecord>()
					                    .Where(u => u.RegistrationStatus == UserStatus.Approved)
					                    .List();
					
					foreach (var user in users)
					{
						f._Users.Add(new SelectListItem { Value = String.Format("{{u:{0},n:'{1}'}}",user.Id, user.UserName), Text = user.UserName});
					}

					return f;
				};

			context.Form(FormId, form);

		}

		public static IList<String> GetUserIds(String userList) {
			return GetUserBits(userList, 1);
		}

		public static IList<String> GetUserNames(String userList) {
			return GetUserBits(userList, 2);
		}

		private static IList<String> GetUserBits(String userList, int place) {
			var userBits = new List<String>();
			var usersMatch = Regex.Matches(userList, @"{u:\d+,n:'.*?'},{0,1}");
			for (var i = 0; i < usersMatch.Count; i++)
			{
				var userMatch = Regex.Match(usersMatch[i].Value, @"{u:(\d+),n:'(.*?)'}");
				userBits.Add(userMatch.Groups[place].Value);
			}

			return userBits;
		}
	}
}