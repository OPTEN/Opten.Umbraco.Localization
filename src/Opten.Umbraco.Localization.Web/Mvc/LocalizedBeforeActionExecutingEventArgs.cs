using System;
using System.Web.Mvc;

namespace Opten.Umbraco.Localization.Web.Mvc
{
	/// <summary>
	/// EventArgs for the LocalizedRenderMvcController BeforeActionExecuting Event
	/// </summary>
	/// <seealso cref="System.EventArgs" />
	public class LocalizedBeforeActionExecutingEventArgs : EventArgs
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="LocalizedBeforeActionExecutingEventArgs"/> class.
		/// </summary>
		/// <param name="filterContext">OnActionExecuting filterContext</param>
		public LocalizedBeforeActionExecutingEventArgs(ActionExecutingContext filterContext)
		{
			ActionExecutingContext = filterContext;
			Cancel = false;
		}

		/// <summary>
		/// The filter context
		/// </summary>
		/// <value>
		/// The action executing context.
		/// </value>
		public ActionExecutingContext ActionExecutingContext { get; set; }

		/// <summary>
		/// Cancel Localization Code execution
		/// </summary>
		/// <value>
		///   <c>true</c> if cancel; otherwise, <c>false</c>.
		/// </value>
		public bool Cancel { get; set; }

	}
}