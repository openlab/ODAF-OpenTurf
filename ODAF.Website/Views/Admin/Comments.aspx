<%@ Page Language="C#" MasterPageFile="~/Views/Shared/ODAdminMasterPage.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
ODAF Administration - Comments
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OtherHeadContent" runat="server">

    <script type="text/javascript">

        jQuery(document).ready(function () {
            jQuery("#list").jqGrid({
                datatype: "local",
                colNames: ['User', 'User Id', 'Placemark', 'Comment', 'Created'],
                colModel: [
                  { name: 'screen_name', index: 'screen_name', editable: false, searchoptions: { defaultValue: '<%= Request.Params["screen_name"] %>'} },
                  { name: 'CreatedById', index: 'CreatedById', hidden: true, editable: false },
                  { name: 'summary', index: 'Summary', editable: false, searchoptions: { defaultValue: '<%= Request.Params["summary"] %>'} },
                  { name: 'Text', index: 'Text', width: 350, editable: false },
                  { name: 'CreatedOn', index: 'CreatedOn', search: false, align: 'center', width: 150, editable: false },
                ],
                pager: '#pager',
                rowNum: 20,
                width: 700,
                rowList: [20, 100, 200, 500],
                sortname: 'CreatedOn',
                sortorder: 'desc',
                viewrecords: true,
                caption: 'Comments',
                editurl: '/admin/updatecomment',
                height: '100%',
                hidegrid: false
            });
            jQuery("#list").jqGrid('navGrid', '#pager', { add: false, edit: false, search: false });
            jQuery("#list").jqGrid('filterToolbar', { autosearch: true });
            jQuery('#list').jqGrid('setGridParam', { datatype: 'json', url: '/admin/getcomments', mtype: 'GET' });
            jQuery('#list')[0].triggerToolbar();
        });
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table id="list"></table> 
    <div id="pager"></div>
</asp:Content>
       