<%@ Page Language="C#" MasterPageFile="~/Views/Shared/ODAdminMasterPage.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
ODAF Administration - Data Sources
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OtherHeadContent" runat="server">

    <script type="text/javascript">

        function feedLink(cellvalue, options, rowObject) {
            return "<a href=/Admin/Feeds/?datasource=" + rowObject[0] + ">" + cellvalue + "</a>";
        }

        function layerLink(cellvalue, options, rowObject) {
            return "<a href=/Admin/Layers/?datasource=" + rowObject[0] + ">" + cellvalue + "</a>";
        }

        jQuery(document).ready(function () {
            jQuery("#list").jqGrid({
                url: '/admin/getsources',
                datatype: 'json',
                mtype: 'GET',
                colNames: ['Id', 'UniqueId', 'Title', 'Updated', 'Description', 'Author Name', 'Author Email', 'Boundary Polygon', 'Active', 'Feeds', 'Layers'],
                colModel: [
                  { name: 'Id', index: 'Id', editable: false, hidden: true },
                  { name: 'UniqueId', index: 'UniqueId', width: 240, editable: false, edittype: 'text', editoptions: { size: 40, maxlength: 255} },
                  { name: 'Title', index: 'Title', width: 100, editable: true, edittype: 'text', editoptions: { size: 40, maxlength: 255} },
                  { name: 'UpdatedOn', index: 'UpdatedOn', align: 'center', width: 150, editable: true, hidden: true, editrules: { edithidden: true} },
                  { name: 'Description', index: 'Description', hidden: true, width: 200, editable: true, edittype: 'textarea', editrules: { edithidden: true }, editoptions: { rows: "3", cols: 35} },
                  { name: 'AuthorName', index: 'AuthorName', hidden: true, width: 180, editable: true, edittype: 'text', editrules: { edithidden: true }, editoptions: { size: 40, maxlength: 255} },
                  { name: 'AuthorEmail', index: 'AuthorEmail', hidden: true, width: 180, editable: true, edittype: 'text', editrules: { edithidden: true }, editoptions: { size: 40, maxlength: 255} },
                  { name: 'BoundaryPolygon', index: 'BoundaryPolygon', hidden: true, width: 200, editable: true, edittype: 'textarea', editrules: { edithidden: true }, editoptions: { rows: "3", cols: 35} },
                  { name: 'Active', index: 'Active', width: 150, align: 'center', editable: true, edittype: 'checkbox', editoptions: { value:"True:False" } },
                  { name: 'Feeds', index: 'Feeds', align: 'center', width: 85, editable: false, formatter: feedLink },
                  { name: 'Layers', index: 'Layers', align: 'center', width: 85, editable: false, formatter: layerLink }
                ],
                pager: '#pager',
                pgbuttons:false,
                pginput:false,
                
                sortname: 'CreatedOn',
                sortorder: 'desc',
                viewrecords: true,
                caption: 'Data Sources',
                editurl: '/admin/updatesource',
                height: '300',
                hidegrid: false,
                ondblClickRow: function (id) {
                    jQuery('#list').jqGrid('editGridRow', id, { width: '250px', closeAfterEdit: true, top: 150, left: 150, modal: true, viewPagerButtons: false });
                }
            });
            jQuery("#list").jqGrid('navGrid', '#pager', { add:true, edit:false, search:false });
        });
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table id="list"></table> 
    <div id="pager"></div>
</asp:Content>
       