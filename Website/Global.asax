<%@ Application Language="C#" %>

<script RunAt="server">

    public void Application_BeginRequest(object sender, EventArgs e)
    {
        System.Web.WebPages.WebPageHttpHandler.DisableWebPagesResponseHeader = true;
    }
       
</script>
