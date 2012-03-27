<%@ Page Language="C#" MasterPageFile="~/Views/Shared/ODAdminMasterPage.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
ODAF Administration - Users
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OtherHeadContent" runat="server">

    <script type="text/javascript">

        function commentsLink(cellvalue, options, rowObject) {
            if (cellvalue > 0)
                return "<a href=/admin/comments?screen_name=" + escape(rowObject[1]) + ">" + cellvalue + "</a>";
            else return 0;
        }

        function summaryLink(cellvalue, options, rowObject) {
            if (cellvalue > 0)
                return "<a href=/admin/points?screen_name=" + escape(rowObject[1]) + ">" + cellvalue + "</a>";
            else return 0;
        }

        jQuery(document).ready(function () {
            jQuery("#list").jqGrid({
                url: '/admin/getusers',
                datatype: 'json',
                mtype: 'GET',
                colNames: ['Id', 'Name', 'Status', 'Role', 'LastLogin', 'Joined', 'Placemarks', 'Comments'],
                colModel: [
                  { name: 'Id', index: 'Id', editable: false, hidden: true },
                  { name: 'Name', index: 'screen_name',width: 160, readonly: true, editable: true, edittype: 'text', editoptions: { readonly: true} },
                  { name: 'UserAccess', stype: 'select', searchoptions: { value: ":;0:Normal;1:Banned;2:Deleted;3:Banned" }, index: 'UserAccess', width: 100, align: 'center', editable: true, edittype: 'select', editoptions: { value: { 0: 'Normal', 1: 'Pending', 2: 'Deleted', 3: 'Banned'}} },
                  { name: 'UserRole', stype: 'select', searchoptions: { value: ":;0:Member;1:Moderator;2:Administrator" }, index: 'UserRole', width: 100, align: 'center', editable: true, edittype: 'select', editoptions: { value: { 0: 'Member', 1: 'Moderator', 2: 'Administrator'}} },
                  { name: 'LastLogin', search: false, align: 'center', index: 'LastAccessedOn', width: 150 },
                  { name: 'Joined', search: false, align: 'center', index: 'CreatedOn', width: 150 },
                  { name: 'Points', search: false, align: 'center', index: 'Summaries', width: 90, formatter: summaryLink },
                  { name: 'Comments', search: false, align: 'center', index: 'Comments', width: 90, formatter: commentsLink }
                ],
                pager: '#pager',
                rowNum: 20,
                rowList: [20, 100, 200, 500],
                sortname: 'CreatedOn',
                sortorder: 'desc',
                viewrecords: true,
                caption: 'Users',
                editurl: '/admin/updateuser',
                height: '100%',
                hidegrid: false,
                ondblClickRow: function (id) {
                    jQuery('#list').jqGrid('editGridRow', id, { editCaption: "Edit User", width: 250, closeAfterEdit: true, top: 150, left: 150, modal: true, viewPagerButtons: false });
                }
            });
            jQuery("#list").jqGrid('filterToolbar', { autosearch: true });
        });
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table id="list"></table> 
    <div id="pager"></div>
</asp:Content>