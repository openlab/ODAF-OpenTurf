<%@ Page Language="C#" MasterPageFile="~/Views/Shared/ODAdminMasterPage.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
ODAF Administration - Feeds
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OtherHeadContent" runat="server">

    <script type="text/javascript">

        jQuery(document).ready(function () {
            jQuery("#list").jqGrid({
                datatype: 'local',
                colNames: ['Data Source', 'Data Source', 'Unique Id', 'Title', 'Updated', 'Summary', 'KML Url', 'Image URL', 'Region', 'Active'],
                colModel: [
                  { name: 'DataSource', index: 'DataSource', stype: 'select', searchoptions: { value: "0:;<%= ViewData["DataSources"] %>", defaultValue: '<%= Request.Params["datasource"] %>'}, width: 180, editable: false },
                  { name: 'PointDataSourceId', index: 'PointDataSourceId', search:false, width: 180, hidden:true, editable: true, edittype: 'select', editrules: { edithidden: true }, editoptions: { value: "<%= ViewData["DataSources"] %>"} },
                  { name: 'UniqueId', index: 'UniqueId', width: 230, search:false, editable: false, edittype: 'text', editoptions: { size: 40, maxlength: 255} },
                  { name: 'Title', index: 'Title', width: 230, editable: true, search:false, edittype: 'text', editoptions: { size: 40, maxlength: 255} },
                  { name: 'UpdatedOn', index: 'UpdatedOn', align: 'center', search:false, width: 150, editable: true, hidden: true, editrules: { edithidden: true} },
                  { name: 'Summary', index: 'Summary', hidden: true, width: 200, editable: true, edittype: 'textarea', editrules: { edithidden: true }, editoptions: { rows: "3", cols: 35} },
                  { name: 'KMLFeedUrl', index: 'KMLFeedUrl', hidden: true, width: 180, editable: true, edittype: 'text', editrules: { edithidden: true }, editoptions: { size: 40, maxlength: 255} },
                  { name: 'ImageUrl', index: 'ImageUrl', hidden: true, width: 180, editable: true, edittype: 'text', editrules: { edithidden: true }, editoptions: { size: 40, maxlength: 255} },
                  { name: 'IsRegion', index: 'IsRegion', width: 70, search:false, align: 'center', editable: true, edittype: 'checkbox', editoptions: { value: "True:False"} },
                  { name: 'Active', index: 'Active', search:false, width: 70, align: 'center', editable: true, edittype: 'checkbox', editoptions: { value: "True:False"} },
                ],
                pager: '#pager',
                rowNum: 20,
                rowList: [20, 100, 200, 500],
                sortname: 'CreatedOn',
                sortorder: 'desc',
                viewrecords: true,
                caption: 'Data Feeds',
                editurl: '/admin/updatefeed',
                height: '100%',
                hidegrid: false,
                ondblClickRow: function (id) {
                    jQuery('#list').jqGrid('editGridRow', id, { width:'250px', closeAfterEdit: true, top: 150, left: 150, modal: true, viewPagerButtons:false });
                }
            });
            jQuery("#list").jqGrid('navGrid', '#pager', { add:true, edit:false, search:false });
            jQuery("#list").jqGrid('filterToolbar', { autosearch: true });
            jQuery('#list').jqGrid('setGridParam', { datatype: 'json', url: '/admin/getfeeds', mtype: 'GET' });
            jQuery('#list')[0].triggerToolbar();
        });
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table id="list"></table> 
    <div id="pager"></div>
</asp:Content>
       