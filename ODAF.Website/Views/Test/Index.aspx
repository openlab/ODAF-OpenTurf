<%@ Page Language="C#" AutoEventWireup="true" Inherits="Views_Test_Index" Codebehind="Index.aspx.cs" %>
<%@ Import Namespace="vancouveropendata.Controllers" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>


<%
    
 %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <script src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.min.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/IEprompt.js") %>" type="text/javascript"></script>
    <script type="text/javascript">

        function BuildUrl(action) {
            var scheme = window.location.protocol; //"<%= Request.Url.Scheme %>";
            var authority = window.location.host;  //"<%= Request.Url.Authority %>";
            var virtualdir = '<%= ResolveUrl("~") %>';
            var url = scheme + "//" + authority + virtualdir + action;
            return url;
        }

        function allSummaries() {
            var url = BuildUrl("Summaries/List.json/");

            $.ajax({
                type: "GET",
                url: url,
                cache: false,
                complete: allSummariesCompleted
            });
        }

        function allSummariesCompleted(xhr, textStatus) 
        {
            if (xhr.status == 200) {
                var json = eval('(' + xhr.responseText + ')');
                var header = "<h1>Summaries</h1><hr/>";
                var table_data = "<table border='1' cellspacing='0' style='border-collapse:collapse;width:100%;text-align:center;'>";

                table_data += "<tr>";
                table_data += "<th>Id</th>";
                table_data += "<th>Name</th>";
                table_data += "<th>Guid</th>";
                table_data += "<th>CreatedById</th>";
                table_data += "<th>Description</th>";
                table_data += "<th>Latitude</th>";
                table_data += "<th>Longitude</th>";
                table_data += "<th>LayerId</th>";
                table_data += "<th>CommentCount</th>";
                table_data += "<th>RatingTotal</th>";
                table_data += "<th>RatingCount</th>";
                table_data += "<th>Tag</th>";
                table_data += "<th>Summary Actions</th>";
                table_data += "<th>Comment Actions</th>";
                table_data += "</tr>";

                for (var i = 0; i < json.length; ++i) {

                    table_data += "<tr>";
                    table_data += "<td>" + json[i].Id + "</td>";
                    table_data += "<td>" + json[i].Name + "</td>";
                    table_data += "<td>" + json[i].Guid + "</td>";
                    table_data += "<td>" + json[i].CreatedById + "</td>";
                    table_data += "<td>" + json[i].Description + "</td>";
                    table_data += "<td>" + json[i].Latitude + "</td>";
                    table_data += "<td>" + json[i].Longitude + "</td>";
                    table_data += "<td>" + json[i].LayerId + "</td>";
                    table_data += "<td>" + json[i].CommentCount + "</td>";
                    table_data += "<td>" + json[i].RatingTotal + "</td>";
                    table_data += "<td>" + json[i].RatingCount + "</td>";
                    table_data += "<td>" + json[i].Tag + "</td>";
                    table_data += "<td>";
                        table_data += "<a href='#' onclick='deleteSummary(" + json[i].Id + ")'>del</a>" + "&nbsp;&nbsp;";
                        table_data += "<a href='#' onclick='editSummary(" + json[i].Id + ")'>edit</a>" + "&nbsp;&nbsp;";
                        table_data += "<a href='#' onclick='newRating(" + json[i].Id + ")'>rate</a>" + "&nbsp;&nbsp;";
                        table_data += "<a href='#' onclick='newTag(" + json[i].Id + ", \"" + json[i].Tag + "\")'>tag</a>" + "&nbsp;&nbsp;";
                        table_data += "</td>";
                    table_data += "<td>";
                        table_data += "<a href='#' onclick='allComments(" + json[i].Id + ")'>all</a>" + "&nbsp;&nbsp;";
                        table_data += "<a href='#' onclick='newComment(" + json[i].Id + ")'>new</a>" + "&nbsp;&nbsp;";
                    table_data += "</td>";
                    table_data += "</tr>";
                }

                table_data += "</table>"

                if (json.length > 0) {
                    $("#summaries").html(header + table_data);
                } else {
                    $("#summaries").html(header + "<br/>No summaries found.");
                }
            }
        }

        function newComment(summaryId)
        {
            var create_callback = function(xhr, textStatus) {
                var id = summaryId;
                if (xhr.status == 201) {
                    //alert(xhr.getResponseHeader("Location"));
                    allSummaries();
                    allComments(id);
                } else {
                    alert(textStatus + " : " + xhr.responseText);
                }
            };

            var prompt_callback = function(text) {
                if (text) {
                    createAComment(summaryId, text, create_callback);
                }
            };
                                    
            IEprompt("Enter your comment", "bla bla bla", prompt_callback);
        }

        function newRating(summaryId) {
            var create_callback = function(xhr, textStatus) {
                var id = summaryId;
                if (xhr.status == 200) {
                    allSummaries();
                } else {
                    alert(textStatus + " : " + xhr.responseText);
                }
            };

            var prompt_callback = function(rating) {
                if (rating) {
                    createARating(summaryId, parseInt(rating), create_callback);
                }
            };

            IEprompt("Enter your rating (0-100)", "50", prompt_callback);
        }


        function newTag(summaryId, existingTags) {

            var create_callback = function(xhr, textStatus) {
                var id = summaryId;
                if (xhr.status == 200) {
                    allSummaries();
                } else {
                    alert(textStatus + " : " + xhr.responseText);
                }
            };

            var prompt_callback = function(tag) {
                if (tag) {
                    createATag(summaryId, tag, create_callback);
                }
            };

            IEprompt("Enter your tag (current tags: " + existingTags + ")", "", prompt_callback);
        }
        
        function allComments(summaryId) 
        {
            var url = BuildUrl("Comments/List.json/" + summaryId);
            var callback = function(xhr, status) {
                var id = summaryId;
                allCommentsCompleted(xhr, status, id);
            };

            $.ajax({
                type: "GET",
                url: url,
                cache: false,
                complete: callback
            });
        }

        var g_last_summary_id = null;
        function allCommentsCompleted(xhr, textStatus, summaryId) 
        {
            if (xhr.status == 200) {
                var json = eval('(' + xhr.responseText + ')');
                var header = "<h1>Comments for Summary " + summaryId + "</h1><hr/>";
                var table_data = "<table border='1' cellspacing='0' style='border-collapse:collapse;width:50%;text-align:center;'>";

                g_last_summary_id = summaryId;
                var refresh_comments_func = function(xhr, textStatus) {
                    if (xhr.status == 200) {
                        allSummaries();
                        allComments(g_last_summary_id);
                        $("#edit_comment").hide();
                        $("#update_comment").hide();
                    } else {
                        alert(textStatus + " : " + xhr.responseText);
                    }
                };
                
                table_data += "<tr>";
                table_data += "<th>Id</th>";
                table_data += "<th>Text</th>";
                table_data += "<th>SummaryId</th>";
                table_data += "<th>CommentAuthor</th>";
                table_data += "<th>ServiceName</th>";
                table_data += "<th>Actions</th>";
                table_data += "</tr>";

                for (var i = 0; i < json.length; ++i) {
                    table_data += "<tr>";
                    table_data += "<td>" + json[i].Comment.Id + "</td>";
                    table_data += "<td>" + json[i].Comment.Text + "</td>";
                    table_data += "<td>" + json[i].Comment.SummaryId + "</td>";
                    table_data += "<td>" + json[i].CommentAuthor + "</td>";
                    table_data += "<td>" + json[i].ServiceName + "</td>";
                    table_data += "<td>";
                        table_data += "<a href='#' onclick='deleteComment(" + json[i].Comment.Id + ", " + refresh_comments_func.toString() + ")'>del</a>" + "&nbsp;&nbsp;";
                        table_data += "<a href='#' onclick='editComment(" + json[i].Comment.Id + ", " + refresh_comments_func.toString() + ")'>edit</a>" + "&nbsp;&nbsp;";
                    table_data += "</td>";
                    table_data += "</tr>";
                }

                table_data += "</table>"

                if (json.length > 0) {
                    $("#comments").html(header + table_data);
                } else {
                    $("#comments").html(header + "<br/>No comments found.");
                }
            }
        }

        function deleteComment(commentId, callback) {
            var url = BuildUrl("Comments/Remove.json/" + commentId);

            if (!confirm("Delete comment, are you sure?")) {
                return;
            }

            $.ajax({
                type: "POST",
                url: url,
                complete: callback
            });
        }

        function deleteSummary(summaryId) 
        {
            var url = BuildUrl("Summaries/Remove.json/" + summaryId);
            
            var del_callback = function(xhr, textStatus) {
                if (xhr.status == 200) {
                    allSummaries();
                    $("#comments").hide();
                } else {
                    alert(textStatus + " : " + xhr.responseText);
                }
            };

            if (!confirm("Delete summary, are you sure?")) {
                return;
            }

            $.ajax({
                type: "POST",
                url: url,
                complete: del_callback
            });
        }

        function showAComment(commentId) {
            var url = BuildUrl("Comments/Show.json/" + commentId);

            $.ajax({
                type: "GET",
                url: url,
                complete: completeCallback
            });
        }

        function createAComment(summaryId, comment_text, callback) {
            var url = BuildUrl("Comments/Add.json/" + summaryId);
            var parameters = "Text=" + encodeURIComponent(comment_text);

            $.ajax({
                type: "POST",
                url: url,
                data: parameters,
                complete: callback
            });
        }

        function createARating(summaryId, rating, callback) {
            var url = BuildUrl("Summaries/AddRating.json/" + summaryId);
            var parameters = "rating=" + encodeURIComponent(rating);

            $.ajax({
                type: "POST",
                url: url,
                data: parameters,
                complete: callback
            });
        }

        function createATag(summaryId, tag, callback) {
            var url = BuildUrl("Summaries/AddTag.json/" + summaryId);
            var parameters = "tag=" + encodeURIComponent(tag);

            $.ajax({
                type: "POST",
                url: url,
                data: parameters,
                complete: callback
            });
        }

        function completeCallback(xhr, textStatus) {
            if (xhr.status != 200) {
                alert(xhr.status + " --> " + xhr.responseText);
            }
        }

        function newSummary() {
            $("#new_summary").show();
        }

        function editSummary(summaryId) 
        {
            var url = BuildUrl("Summaries/ShowById.json/" + summaryId);
            var callback = function(xhr, textStatus) {
                if (xhr.status == 200) {
                    var json = eval('(' + xhr.responseText + ')');
                    $("#update_summary :input[name=Description]").val(json.Description);
                    $("#update_summary :input[name=Name]").val(json.Name);
                    $("#update_summary :input[name=Latitude]").val(json.Latitude);
                    $("#update_summary :input[name=Longitude]").val(json.Longitude);
                    $("#update_summary :input[name=LayerId]").val(json.LayerId);
                    $("#update_summary :input[name=Guid]").val(json.Guid);
                    $("#update_summary :input[name=id]").val(json.Id);
                    $("#update_summary").show();
                } else {
                    alert(textStatus + " : " + xhr.responseText);
                }
            };

            $.ajax({
                type: "GET",
                url: url,
                cache: false,
                complete: callback
            });
        }

        function updateASummary() 
        {
            var url = BuildUrl("Summaries/Edit.json/" + $("#update_summary :input[name=id]").val());
            var parameters = $("#update_summary form").serialize();

            var callback = function(xhr, textStatus) {
                if (xhr.status == 200) {
                    allSummaries();
                    $("#update_summary").hide();
                } else {
                    alert(textStatus + " : " + xhr.responseText);
                }
            };

            $.ajax({
                type: "POST",
                url: url,
                data: parameters,
                complete: callback
            });
        }

        function editComment(commentId) {
            var url = BuildUrl("Comments/Show.json/" + commentId);
            var callback = function(xhr, textStatus) {
                if (xhr.status == 200) {
                    var json = eval('(' + xhr.responseText + ')');
                    $("#update_comment :input[name=Text]").val(json.Comment.Text);
                    $("#update_comment :input[name=SummaryId]").val(json.Comment.SummaryId);
                    $("#update_comment :input[name=id]").val(json.Comment.Id);
                    $("#update_comment").show();
                } else {
                    alert(textStatus + " : " + xhr.responseText);
                }
            };

            $.ajax({
                type: "GET",
                url: url,
                cache: false,
                complete: callback
            });
        }

        function updateAComment() {
            var comment_id = $("#update_comment :input[name=id]").val();
            var summary_id = $("#update_comment :input[name=SummaryId]").val();
            
            var url = BuildUrl("Comments/Edit.json/" + comment_id);
            var parameters = $("#update_comment form").serialize();

            var callback = function(xhr, textStatus) {
                if (xhr.status == 200) {
                    allComments(summary_id);
                    $("#update_comment").hide();
                } else {
                    alert(textStatus + " : " + xhr.responseText);
                }
            };

            $.ajax({
                type: "POST",
                url: url,
                data: parameters,
                complete: callback
            });
        }

        function createASummary() 
        {
            var url = BuildUrl("Summaries/Add.json/");
            var parameters = $("#new_summary form").serialize();

            var create_callback = function(xhr, textStatus) {
                if (xhr.status == 201) {
                    //alert(xhr.getResponseHeader("Location"));
                    allSummaries();
                    $("#new_summary").hide();
                } else {
                    alert(textStatus + " : " + xhr.responseText);
                }
            };

            $.ajax({
                type: "POST",
                url: url,
                data: parameters,
                complete: create_callback
            });
        }

    </script>
    <title></title>
</head>
<body>

<hr />
<br />
<a href="#" onclick="allSummaries();">All Summaries</a>
<a href="#" onclick="newSummary();">New Summary</a>
<br />


<div id="summaries"></div>
<div id="comments"></div>


<div id="new_summary" style="display:none">
<h1>Create a new Summary</h1>
<hr />
    <form action="#">
        <label>Name</label>&nbsp;&nbsp;<input name="Name" type="text" value="namus"/><br />
        <label>Description</label>&nbsp;&nbsp;<input name="Description" type="text" value="something"/><br />
        <label>Latitude</label>&nbsp;&nbsp;<input name="Latitude" type="text" value="1.000"/><br />
        <label>Longitude</label>&nbsp;&nbsp;<input name="Longitude" type="text" value="2.000"/><br />
        <label>LayerId</label>&nbsp;&nbsp;<input name="LayerId" type="text" value="MyLayerId"/><br />
        <label>Guid</label>&nbsp;&nbsp;<input name="Guid" type="text" value="MyGuid"/><br />
        <button onclick="createASummary(); return false;">Create</button>
        <button onclick="$('#new_summary').hide(); return false;">Cancel</button>
    </form>
</div>

<div id="update_summary" style="display:none">
<h1>Update a Summary</h1>
<hr />
    <form action="#">
        <label>Name</label>&nbsp;&nbsp;<input name="Name" type="text" value=""/><br />
        <label>Description</label>&nbsp;&nbsp;<input name="Description" type="text" value=""/><br />
        <label>Latitude</label>&nbsp;&nbsp;<input name="Latitude" type="text" value=""/><br />
        <label>Longitude</label>&nbsp;&nbsp;<input name="Longitude" type="text" value=""/><br />
        <label>LayerId</label>&nbsp;&nbsp;<input name="LayerId" type="text" value=""/><br />
        <label>Guid</label>&nbsp;&nbsp;<input name="Guid" type="text" value=""/><br />
        <input name="id" type="hidden" value=""/><br />
        <button onclick="updateASummary(); return false;">Update</button>
        <button onclick="$('#update_summary').hide(); return false;">Cancel</button>
    </form>
</div>

<div id="update_comment" style="display:none">
<h1>Edit a Comment</h1>
<hr />
    <form action="#">
        <label>Text</label><br /><textarea cols="50" rows="10" name="Text"></textarea><br />
        <input name="id" type="hidden" value=""/><br />
        <input name="SummaryId" type="hidden" value=""/><br />
        <button onclick="updateAComment(); return false;">Update</button>
        <button onclick="$('#update_comment').hide(); return false;">Cancel</button>
    </form>
</div>


</body>
</html>
