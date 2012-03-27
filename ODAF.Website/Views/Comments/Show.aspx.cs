using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ODAF.Data;
using vancouveropendata;
using vancouveropendata.Controllers;

public partial class Views_Comments_Show : System.Web.Mvc.ViewPage< AggregatedComment >
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public string GetUserImageUrl(long userId)
    {
         return this.BuildUrlFromExpression<UserController>(c => c.Image(userId, "png"));
    }
}
