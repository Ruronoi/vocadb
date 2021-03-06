﻿using System.Web.Mvc;
using NLog;

namespace VocaDb.Web.Code.Security {

	/// <summary>
	/// Denies access to Authorized actions for IPs that are restricted.
	/// This filter is applied globally for all actions marked with the <see cref="AuthorizeAttribute"/>.
	/// </summary>
	public class RestrictBlockedIPAttribute : ActionFilterAttribute {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private readonly IPRuleManager ipRuleManager;

		public RestrictBlockedIPAttribute(IPRuleManager ipRuleManager) {
			this.ipRuleManager = ipRuleManager;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext) {

			var host = filterContext.HttpContext.Request.UserHostAddress;

			if (ipRuleManager.IsAllowed(host))
				return;

			if (filterContext.ActionDescriptor.IsDefined(typeof(AuthorizeAttribute), false)) {
				log.Warn("Restricting blocked IP {0}.", host);
				filterContext.Result = new HttpUnauthorizedResult();
			}

		}

	}

}