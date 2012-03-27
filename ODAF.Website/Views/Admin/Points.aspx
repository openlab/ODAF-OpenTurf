<%@ Page Language="C#" MasterPageFile="~/Views/Shared/ODAdminMasterPage.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
ODAF Administration - Placemarks
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OtherHeadContent" runat="server">

    <script type="text/javascript">

        function formateadorLink(cellvalue, options, rowObject) {
            return "<a href=/admin/sources>" + cellvalue + "</a>";
        }

        jQuery(document).ready(function () {
            jQuery("#list").jqGrid({
                datatype: 'local',
                colNames: ['User', 'LayerId', 'Name', 'Description', 'Latitude', 'Longitude', 'Tags', 'Comments', 'Ratings', 'Modified', 'Created'],
                colModel: [
                  { name: 'screen_name', index: 'screen_name', width: 180, editable: false, search: true, searchoptions: { defaultValue: '<%= Request.Params["screen_name"] %>'} },
                  { name: 'LayerId', index: 'LayerId', hidden: true, width: 180, editable: true, edittype: 'text', editrules: { edithidden: true }, editoptions: { size: 40, maxlength: 255} },
                  { name: 'Name', index: 'Name', width: 230, editable: true, edittype: 'text', search: true, editoptions: { size: 40, maxlength: 255} },
                  { name: 'Description', index: 'Description', hidden: true, width: 200, editable: true, edittype: 'textarea', editrules: { edithidden: true }, editoptions: { rows: "3", cols: 35} },
                  { name: 'Latitude', index: 'Latitude', hidden: true, align: 'center', width: 100, editrules: { edithidden: true }, search: false, editable: true, edittype: 'text', editoptions: { size: 15, maxlength: 255} },
                  { name: 'Longitude', index: 'Longitude', hidden: true, align: 'center', width: 100, editrules: { edithidden: true }, search: false, editable: true, edittype: 'text', editoptions: { size: 15, maxlength: 255} },
                  { name: 'Tag', index: 'Tag', hidden:true, width: 200, editable: true, edittype: 'text', editrules: { edithidden: true }, editoptions: { size: 40, maxlength: 255} },
                  { name: 'CommentCount',firstsortorder:'desc', index: 'CommentCount', align: 'center', width: 90, editable: false, search:false },
                  { name: 'RatingTotal', firstsortorder: 'desc', index: 'RatingTotal', align: 'center', editable: false, width: 70, search: false },
                  { name: 'ModifiedOn', firstsortorder: 'desc', index: 'ModifiedOn', width: 150, align: 'center', editable: false, search: false },
                  { name: 'CreatedOn', firstsortorder: 'desc', index: 'CreatedOn', width: 150, align: 'center', editable: false, search: false }
                ],
                pager: '#pager',
                rowNum: 20,
                rowList: [20, 100, 200, 500],
                sortname: 'CreatedOn',
                sortorder: 'desc',
                viewrecords: true,
                caption: 'Placemarks',
                editurl: '/admin/updatepoint',
                height: '100%',
                hidegrid: false,
                ondblClickRow: function (id) {
                    jQuery('#list').jqGrid('editGridRow', id, { width:'250px', closeAfterEdit: true, top: 150, left: 150, modal: true, viewPagerButtons:false });
                }
            });
            jQuery("#list").jqGrid('navGrid', '#pager', { add: false, edit: false, search: false });
            jQuery("#list").jqGrid('filterToolbar', { autosearch: true });
            jQuery('#list').jqGrid('setGridParam', { datatype: 'json', url: '/admin/getpoints', mtype: 'GET' });
            jQuery('#list')[0].triggerToolbar();
        });
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table id="list"></table> 
    <div id="pager"></div>
</asp:Content>
       