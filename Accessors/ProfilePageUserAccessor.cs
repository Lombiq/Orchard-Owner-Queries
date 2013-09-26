using System;
using System.Web.Routing;
using Orchard;
using Orchard.Core.Common.Utilities;
using Orchard.Environment.Extensions;
using Orchard.Security;

namespace NogginBox.OwnerQueries.Accessors
{
	public interface IProfilePageUserAccessor : IDependency
	{
		IUser CurrentProfilePageUser { get; }
	}

	[OrchardFeature("NogginBox.OwnerProfilePageQueries")]
	public class ProfilePageUserAccessor : IProfilePageUserAccessor
	{
		private readonly LazyField<IUser> _currentProfilePageUser = new LazyField<IUser>();
		private readonly IMembershipService _membershipService;
		private readonly RequestContext _requestContext;

		public ProfilePageUserAccessor(IMembershipService membershipService, RequestContext requestContext)
		{
			_membershipService = membershipService;
			_requestContext = requestContext;
			_currentProfilePageUser.Loader(GetCurrentProfilePageUser);
		}
 
		public IUser CurrentProfilePageUser 
		{
			get { return _currentProfilePageUser.Value; }
		}
 
		private IUser GetCurrentProfilePageUser()
		{
			var username = GetUserNameFromProfilePage();
			return String.IsNullOrEmpty(username) ? null : _membershipService.GetUser(username); ;
		}
 
		private String GetUserNameFromProfilePage()
		{
			object area;
			object userName;

			if (_requestContext.RouteData.Values.TryGetValue("username", out userName) && _requestContext.RouteData.Values.TryGetValue("area", out area))
			{
				if ((String) area == "Contrib.Profile") {
					return (String) userName;
				}
			}
			return null;
		}
	}
}