using Microsoft.Office.Interop.Outlook;
using Exception = System.Exception;

namespace RiskConsult.Interop.Outlook;

public static class OutlookExtensions
{
	public static void SendMail( string subject, string body, string[] toEmails, string[]? attachments = null )
	{
		if ( toEmails == null || toEmails.Length == 0 )
		{
			throw new Exception( "Any e-mail address" );
		}

		try
		{
			Application application = ( Application ) ExMarshal.GetActiveObject( "Outlook.Application" ) ?? new Application();
			var mailItem = ( MailItem ) application.CreateItem( OlItemType.olMailItem );
			MailItem mailItem2 = mailItem;
			mailItem2.Subject = subject;
			mailItem2.Body = body;
			foreach ( var name in toEmails )
			{
				_ = mailItem2.Recipients.Add( name );
			}

			if ( attachments != null )
			{
				foreach ( var source in attachments )
				{
					_ = mailItem2.Attachments.Add( source );
				}
			}

			mailItem2.Save();
			mailItem2.Send();
		}
		catch ( Exception ex )
		{
			throw new Exception( $"Cannot send mail: {ex.Message}", ex );
		}
	}
}
