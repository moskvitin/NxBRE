/*
 * Created by SharpDevelop.
 * User: David Dossot
 * Date: 6/9/2007
 * Time: 3:46 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using NxDSL;

using Antlr.Runtime;

namespace NxDSL.GUI
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		
		public MainForm()
		{
			// The InitializeComponent() call is required for Windows Forms designer support.
			InitializeComponent();
			
			Definitions definitions = new Definitions("../../../../Rulefiles/discount.nxdsl.defs");
			
			InferenceRules_ENLexer irl = new InferenceRules_ENLexer(
								new ANTLRFileStream("../../../../Rulefiles/discount.nxdsl"));
			
			StringBuilder htmlBuffer = new StringBuilder("<html><body><font face='courier' color='#000000'>");
			
			IToken token;
			bool inQuote = false;
			StringBuilder statementBuffer = new StringBuilder();
			
			while((token = irl.NextToken()) != Token.EOF_TOKEN) {
				Console.Write(token.Text + "/" + token.Type);
				
				if (token.Type == InferenceRules_ENLexer.NEWLINE) {
					string statement = statementBuffer.ToString();
						
					if (definitions.Contains(statement)) {
						htmlBuffer.Append("<font color='#006600'>");
					} else {
						htmlBuffer.Append("<font color='#FF0000'>");
					}
					
					htmlBuffer.Append(statement).Append("</font>");
					
					htmlBuffer.Append("<br/>");
					statementBuffer = new StringBuilder();
				}
				else if (token.Type == InferenceRules_ENLexer.TAB) {
					htmlBuffer.Append("&nbsp;&nbsp;");
				}
				else {
					if ((token.Type == InferenceRules_ENLexer.RULE)
					    || (token.Type == InferenceRules_ENLexer.FACT)
					    || (token.Type == InferenceRules_ENLexer.QUERY)){
						htmlBuffer.Append("<br/>");
					}
					
					if ((token.Type == InferenceRules_ENLexer.QUOTE) && (!inQuote)) {
						htmlBuffer.Append("<font color='#0000FF'>");
						htmlBuffer.Append(token.Text);
						inQuote = true;
					}
					else if ((token.Type == InferenceRules_ENLexer.QUOTE) && (inQuote)) {
						htmlBuffer.Append(token.Text);
						htmlBuffer.Append("</font>");
						inQuote = false;
					}
					else if (inQuote) {
						htmlBuffer.Append(token.Text);
					}
					else if ((token.Type == InferenceRules_ENLexer.CHAR)
					        || (token.Type == InferenceRules_ENLexer.SPACE)
					        || (token.Type == InferenceRules_ENLexer.NUMERIC)) {
						
						statementBuffer.Append(token.Text);
					}
					else if ((token.Type == InferenceRules_ENLexer.COUNT)
					        || (token.Type == InferenceRules_ENLexer.DEDUCT)
					        || (token.Type == InferenceRules_ENLexer.FORGET)
					        || (token.Type == InferenceRules_ENLexer.MODIFY)) {
						
						htmlBuffer.Append("<font color='#990066'><b>").Append(token.Text).Append(" ").Append("</b></font>");
					}
					else {
						htmlBuffer.Append("<b>").Append(token.Text).Append(" ").Append("</b>");
					}
				}
			}
			
			Html = htmlBuffer.Append("</font></body></html>").ToString();
		}
	}
}