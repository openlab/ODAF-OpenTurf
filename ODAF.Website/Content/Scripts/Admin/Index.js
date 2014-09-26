/*
** OnLoad
*/
$(function () {

    // Handle user modals
    $('#validEditUser').click(Users.Edit);

    // Handle source modals
    $('#validAddSource').click(Sources.Add);
    $('#validEditSource').click(Sources.Edit);

    // Handle feed modals
    $('#validAddFeed').click(Feeds.Add);
    $('#validEditFeed').click(Feeds.Edit);

    // Handle feed modals
    $('#validEditPlacemark').click(Placemarks.Edit);

    // Load user list
    Users.Load();

    // Load source list
    Sources.Load();

    // Load feed list
    Feeds.Load();

    // Load placemark list
    Placemarks.Load();

    // Load comment list
    Comments.Load();

});


/*
** User object
*/
var Users = {
    UserData: null,
    CurEditingRow: null,
    Load: function () {
        var UserData;

        var url = "/Admin/GetUsers";
        jQuery.ajax({
            url: url, success: function (target) {
                UserData = target;
            },
            async: false
        });

        if (UserData == null) {
            $('#userList').hide();
            return;
        }
        this.UserData = UserData;
        $('#userList tbody tr.user').remove();
        $.each(UserData, function (ndx, user) {
            Users.AddUserRow(user);
        });
    },
    AddUserRow: function (user) {
        $('#noUser').hide();
        $('#userList').show();

        var appRow = $('#userList tbody tr.model').clone();

        var access;
        switch (user.UserAccess) {
            case 0:
                access = 'Normal'
                break;
            case 1:
                access = 'Pending'
                break;
            case 2:
                access = 'Deleted'
                break;
            case 3:
                access = 'Banned'
                break;
            default:
                access = 'Deleted'
        }

        var role;
        switch (user.UserRole) {
            case 0:
                role = 'Member'
                break;
            case 1:
                role = 'Moderator'
                break;
            case 2:
                role = 'Administrator'
                break;
            default:
                role = 'Member'
        }

        var creationdate = new Date(parseInt(user.CreatedOn.substr(6)));
        var accessdate = new Date(parseInt(user.LastAccessedOn.substr(6)));

        appRow.find('.colAction.edit').click(Users.DisplayEditModal);
        appRow.find('.colName').html(user.screen_name);
        appRow.find('.colStatus').html(access);
        appRow.find('.colRole').html(role);
        appRow.find('.colLastLogin').html(accessdate.toDateString() + " - " + ((creationdate.getHours() > 9) ? creationdate.getHours() : "0" + creationdate.getHours()) + ":" + ((creationdate.getMinutes() > 9) ? creationdate.getMinutes() : "0" + creationdate.getMinutes()));
        appRow.find('.colJoined').html(accessdate.toDateString() + " - " + ((accessdate.getHours() > 9) ? accessdate.getHours() : "0" + accessdate.getHours()) + ":" + ((accessdate.getMinutes() > 9) ? accessdate.getMinutes() : "0" + accessdate.getMinutes()));
        appRow.find('.colPlacemarks').html(user.Summaries);
        appRow.find('.colComments').html(user.Comments);
        appRow.removeClass('model');
        appRow.addClass('user');

        $('#userList tbody').append(appRow);
    },

    Edit: function () {
        var editModal = $(this).parents('#editUserModal');
        var user = {};


        user.UserAccess = document.getElementById('editUserStatus').selectedIndex;
        user.UserRole = document.getElementById('editUserRole').selectedIndex;

        Users.Update(user);
        $('#editUserModal .alert-box').hide();
        $('#editUserModal input[type=text]').val('');
        $('#editUserModal').trigger('reveal:close');
    },
    DisplayEditModal: function () {
        Users.CurEditingRow = $(this).parents('.user')[0];
        var i = Users.CurEditingRow.rowIndex - 2;
        var data = Users.UserData[i];
        
        var $selectUserStatus = $('#editUserStatus');
        $selectUserStatus.html('');
        $selectUserStatus.append('<option>Normal</option><option>Pending</option><option>Deleted</option><option>Banned</option>');
        document.getElementById('editUserStatus').selectedIndex = data.UserAccess;

        var $selectUserRole = $('#editUserRole');
        $selectUserRole.html('');
        $selectUserRole.append('<option>Member</option><option>Moderator</option>');
        document.getElementById('editUserRole').selectedIndex = data.UserRole;


        $('#editUserModal').find('input.userName').val(data.screen_name);

        $('#editUserModal').reveal();
    },
    Update: function (user) {
        var i = Users.CurEditingRow.rowIndex - 2;

        var url = "/Admin/UpdateUser?";
        url += "id=" + Users.UserData[i].Id;
        url += "&useraccess=" + user.UserAccess;
        url += "&userrole=" + user.UserRole;
        var actionResult = null;
        jQuery.ajax({
            url: url,
            success: function (result) {
                actionResult = result;
            },
            async: false
        });

        if (actionResult == 'success') {
            Users.Load();
        } else
            alert(actionResult);
    },
}



/*
** Source object
*/
var Sources = {
    SourceData: null,
    CurEditingRow: null,
    Load: function () {

        var SourceData;

        var url = "/Admin/GetSources";
        jQuery.ajax({
            url: url, success: function (target) {
                SourceData = target;
            },
            async: false
        });

        if (SourceData == null) {
            $('#feedList').hide();
            return;
        }
        this.SourceData = SourceData;
        $('#sourceList tbody tr.source').remove();
        var appRow = $('#sourceList tbody tr.model').clone();
        $('#sourceList tbody').empty();
        $('#sourceList tbody').append(appRow);

        //cleaning the Feed source dropdown of the Add Source Modal
        var $selectAddFeed = $('#addFeedDataSource');
        $selectAddFeed.html('');
        //cleaning the Feed source dropdown of the Edit Source Modal
        var $selectEditFeed = $('#editFeedDataSource');
        $selectEditFeed.html('');


        $.each(SourceData, function (ndx, source) {
            Sources.AddSourceRow(source);
            $selectAddFeed.append('<option >' + source.Title + '</option>');
            $selectEditFeed.append('<option >' + source.Title + '</option>');
        });
    },
    AddSourceRow: function (source) {
        $('#noSource').hide();
        $('#sourceList').show();

        var appRow = $('#sourceList tbody tr.model').clone();


        appRow.find('.colAction.remove').click(Sources.Delete);
        appRow.find('.colAction.edit').click(Sources.DisplayEditModal);
        appRow.find('.colUniqueId').html(source.UniqueId);
        appRow.find('.colTitle').html(source.Title);
        appRow.find('.colActive').html(source.Active);
        appRow.find('.colFeeds').html(source.Feeds);
        appRow.find('.colLayers').html(source.Layers);
        appRow.removeClass('model');
        appRow.addClass('source');

        $('#sourceList tbody').append(appRow);
    },

    Add: function () {
        var addModal = $(this).parents('#addSourceModal');
        var source = {};

        source.Title = $(addModal).find('.sourceTitle').val();
        source.Updated = $(addModal).find('.sourceUpdated').val();
        source.Description = $(addModal).find('.sourceDescription').val();
        source.AuthorName = $(addModal).find('.sourceAuthorName').val();
        source.AuthorEmail = $(addModal).find('.sourceAuthorEmail').val();
        source.Boundary = $(addModal).find('.sourceBoundary').val();
        source.Active = $(addModal).find('.sourceActive').prop('checked');

        if (source.Title.length == 0 || source.Updated.length == 0 || source.Description.length == 0 || source.AuthorName.length == 0 || source.AuthorEmail.length == 0 || source.Boundary.length == 0) {
            $('#addSourceModal .alert-box').show();
            return;
        }

        Sources.Create(source);
        $('#addSourceModal .alert-box').hide();
        $('#addSourceModal input[type=text]').val('');
        $('#addSourceModal').trigger('reveal:close');
    },
    Create: function (source) {
        var url = "/Admin/CreateSource?";
        url += "title=" + source.Title;
        url += "&updated=" + source.Updated;
        url += "&description=" + source.Description;
        url += "&authorname=" + source.AuthorName;
        url += "&authoremail=" + source.AuthorEmail;
        url += "&boundary=" + source.Boundary;
        url += "&active=" + source.Active;
        var actionResult = null;
        jQuery.ajax({
            url: url,
            success: function (result) {
                actionResult = result;
            },
            async: false
        });

        if (actionResult == 'success')
            Sources.Load();
        else
            alert(actionResult);


    },

    Edit: function () {
        var editModal = $(this).parents('#editSourceModal');
        var source = {};

        source.Title = $(editModal).find('.sourceTitle').val();
        source.Updated = $(editModal).find('.sourceUpdated').val();
        source.Description = $(editModal).find('.sourceDescription').val();
        source.AuthorName = $(editModal).find('.sourceAuthorName').val();
        source.AuthorEmail = $(editModal).find('.sourceAuthorEmail').val();
        source.Boundary = $(editModal).find('.sourceBoundary').val();
        source.Active = $(editModal).find('.sourceActive').prop('checked');

        if (source.Title.length == 0 || source.Updated.length == 0 || source.Description.length == 0 || source.AuthorName.length == 0 || source.AuthorEmail.length == 0 || source.Boundary.length == 0) {
            $('#editSourceModal .alert-box').show();
            return;
        }

        Sources.Update(source);
        $('#editSourceModal .alert-box').hide();
        $('#editSourceModal input[type=text]').val('');
        $('#editSourceModal').trigger('reveal:close');
    },
    DisplayEditModal: function () {
        Sources.CurEditingRow = $(this).parents('.source')[0];
        var i = Sources.CurEditingRow.rowIndex - 2;
        var data = Sources.SourceData[i];

        $('#editSourceModal').find('input.sourceTitle').val(data.Title);
        var updated = new Date(parseInt(data.UpdatedOn.substr(6)));
        var updatedstring = updated.toDateString() + "  " + ((updated.getHours() > 9) ? updated.getHours() : "0" + updated.getHours()) + ":" + ((updated.getMinutes() > 9) ? updated.getMinutes() : "0" + updated.getMinutes());
        $('#editSourceModal').find('input.sourceUpdated').val(updatedstring);
        $('#editSourceModal').find('input.sourceDescription').val(data.Description);
        $('#editSourceModal').find('input.sourceAuthorName').val(data.AuthorName);
        $('#editSourceModal').find('input.sourceAuthorEmail').val(data.AuthorEmail);
        $('#editSourceModal').find('input.sourceBoundary').val(data.BoundaryPolygon);
        $('#editSourceModal').find('input.sourceActive').prop('checked', data.Active);

        $('#editSourceModal').reveal();
    },
    Update: function (source) {
        var i = Sources.CurEditingRow.rowIndex - 2;

        var url = "/Admin/UpdateSource?";
        url += "id=" + Sources.SourceData[i].PointDataSourceId;
        url += "&title=" + source.Title;
        url += "&updated=" + source.Updated;
        url += "&description=" + source.Description;
        url += "&authorname=" + source.AuthorName;
        url += "&authoremail=" + source.AuthorEmail;
        url += "&boundary=" + source.Boundary;
        url += "&active=" + source.Active;
        var actionResult = null;
        jQuery.ajax({
            url: url,
            success: function (result) {
                actionResult = result;
            },
            async: false
        });

        if (actionResult == 'success')
            Sources.Load();
        else
            alert(actionResult);
    },

    Delete: function () {

        var rowToDelete = $(this).parents('.source')[0];
        var idx = rowToDelete.rowIndex - 2;

        var url = "/Admin/DeleteSource?";
        url += "id=" + Sources.SourceData[idx].PointDataSourceId;
        var actionResult = null;

        jQuery.ajax({
            url: url,
            success: function (result) {
                actionResult = result;
            },
            error: function (result) {
                actionResult = "nooooooooope";
            },
            async: false
        });

        if (actionResult == 'success') {
            Sources.Load();
            Feeds.Load();
        }
        else
            alert(actionResult);

    },
    CompareRoutine: function (a, b) {
        if (a.Name.toLowerCase() < b.Name.toLowerCase()) {
            return -1;
        }

        if (a.Name.toLowerCase() > b.Name.toLowerCase()) {
            return 1;
        }

        return 0;
    }
}


/*
** Feed object
*/
var Feeds = {
    FeedData: null,
    CurEditingRow: null,
    Load: function () {

        var FeedData;

        var url = "/Admin/GetFeeds";
        jQuery.ajax({
            url: url, success: function (target) {
                FeedData = target;
            },
            async: false
        });

        if (FeedData == null) {
            $('#feedList').hide();
            return;
        }
        Feeds.FeedData = FeedData;
        $('#feedList tbody tr.source').remove();
        var appRow = $('#feedList tbody tr.model').clone();
        $('#feedList tbody').empty();
        $('#feedList tbody').append(appRow);
        $.each(FeedData, function (ndx, feed) {
            Feeds.AddFeedRow(feed);
        });
    },
    AddFeedRow: function (feed) {
        $('#noFeed').hide();
        $('#feedList').show();

        var feedRow = $('#feedList tbody tr.model').clone();

        feedRow.find('.colAction.remove').click(Feeds.Delete);
        feedRow.find('.colAction.edit').click(Feeds.DisplayEditModal);
        feedRow.find('.colDataSource').html(feed.DataSourceName);
        feedRow.find('.colUniqueId').html(feed.UniqueId);
        feedRow.find('.colTitle').html(feed.Title);
        feedRow.find('.colRegion').html(feed.IsRegion + "");
        feedRow.find('.colActive').html(feed.Active);
        feedRow.removeClass('model');
        feedRow.addClass('feed');

        $('#feedList tbody').append(feedRow);
    },

    Add: function () {
        var addModal = $(this).parents('#addFeedModal');
        var feed = {};

        var selector = document.getElementById('addFeedDataSource');

        feed.PointDataSourceId = Sources.SourceData[selector.selectedIndex].PointDataSourceId;
        feed.Title = $(addModal).find('.feedTitle').val();
        feed.Updated = $(addModal).find('.feedUpdated').val();
        feed.Summary = $(addModal).find('.feedSummary').val();
        feed.Kml = $(addModal).find('.feedKmlUrl').val();
        feed.Image = $(addModal).find('.feedImageUrl').val();
        feed.Region = $(addModal).find('.feedRegion').prop('checked');
        feed.Active = $(addModal).find('.feedActive').prop('checked');

        if (feed.Title.length == 0 || feed.Updated.length == 0 || feed.Kml.length == 0 || feed.Image.length == 0) {
            $('#addFeedModal .alert-box').show();
            return;
        }

        Feeds.Create(feed);
        $('#addFeedModal .alert-box').hide();
        $('#addFeedModal input[type=text]').val('');
        $('#addFeedModal').trigger('reveal:close');
    },
    Create: function (feed) {
        var url = "/Admin/CreateFeed?";
        url += "title=" + feed.Title;
        url += "&pointdatasourceid=" + feed.PointDataSourceId;
        url += "&updated=" + feed.Updated;
        url += "&summary=" + feed.Summary;
        url += "&kml=" + feed.Kml;
        url += "&image=" + feed.Image;
        url += "&region=" + feed.Region;
        url += "&active=" + feed.Active;
        var actionResult = null;
        jQuery.ajax({
            url: url,
            success: function (result) {
                actionResult = result;
            },
            async: false
        });

        if (actionResult == 'success')
            Feeds.Load();
        else
            alert(actionResult);


    },

    Edit: function () {
        var editModal = $(this).parents('#editFeedModal');
        var feed = {};

        var selector = document.getElementById('editFeedDataSource');

        feed.PointDataSourceId = Sources.SourceData[selector.selectedIndex].PointDataSourceId;
        feed.Title = $(editModal).find('.feedTitle').val();
        feed.Updated = $(editModal).find('.feedUpdated').val();
        feed.Summary = $(editModal).find('.feedSummary').val();
        feed.Kml = $(editModal).find('.feedKmlUrl').val();
        feed.Image = $(editModal).find('.feedImageUrl').val();
        feed.Region = $(editModal).find('.feedRegion').prop('checked');
        feed.Active = $(editModal).find('.feedActive').prop('checked');

        if (feed.Title.length == 0 || feed.Updated.length == 0 || feed.Kml.length == 0 || feed.Image.length == 0) {
            $('#editFeedModal .alert-box').show();
            return;
        }

        Feeds.Update(feed);
        $('#editFeedModal .alert-box').hide();
        $('#editFeedModal input[type=text]').val('');
        $('#editFeedModal').trigger('reveal:close');
    },
    DisplayEditModal: function () {
        Feeds.CurEditingRow = $(this).parents('.feed')[0];
        var i = Feeds.CurEditingRow.rowIndex - 2;
        var data = Feeds.FeedData[i];
        var idx = 0;
        for (var j = 0; j < Sources.SourceData.length; j++)
        {
            if (Sources.SourceData[j].PointDataSourceId == data.PointDataSourceId) {
                idx = j;
                break;
            }
        }
        var selector = document.getElementById('editFeedDataSource');
        selector.selectedIndex = idx;

        $('#editFeedModal').find('input.feedTitle').val(data.Title);
        var updated = new Date(parseInt(data.UpdatedOn.substr(6)));
        var updatedstring = updated.toDateString() + "  " + ((updated.getHours() > 9) ? updated.getHours() : "0" + updated.getHours()) + ":" + ((updated.getMinutes() > 9) ? updated.getMinutes() : "0" + updated.getMinutes());
        $('#editFeedModal').find('input.feedUpdated').val(updatedstring);
        $('#editFeedModal').find('input.feedSummary').val(data.Summary);

        $('#editFeedModal').find('input.feedKmlUrl').val(data.KMLFeedUrl);
        $('#editFeedModal').find('input.feedImageUrl').val(data.ImageUrl);

        $('#editFeedModal').find('input.feedRegion').prop('checked', data.IsRegion);
        $('#editFeedModal').find('input.feedActive').prop('checked', data.Active);

        $('#editFeedModal').reveal();
    },
    Update: function (feed) {
        var i = Feeds.CurEditingRow.rowIndex - 2;

        var url = "/Admin/UpdateFeed?";
        url += "id=" + Feeds.FeedData[i].PointDataFeedId;
        url += "&title=" + feed.Title;
        url += "&pointdatasourceid=" + feed.PointDataSourceId;
        url += "&updated=" + feed.Updated;
        url += "&summary=" + feed.Summary;
        url += "&kml=" + feed.Kml;
        url += "&image=" + feed.Image;
        url += "&region=" + feed.Region;
        url += "&active=" + feed.Active;
        var actionResult = null;
        jQuery.ajax({
            url: url,
            success: function (result) {
                actionResult = result;
            },
            async: false
        });

        if (actionResult == 'success')
        {
            Feeds.Load();
            Sources.Load();

        } else
            alert(actionResult);
    },

    Delete: function () {

        var rowToDelete = $(this).parents('.feed')[0];
        var idx = rowToDelete.rowIndex - 2;

        var url = "/Admin/DeleteFeed?";
        url += "id=" + Feeds.FeedData[idx].PointDataFeedId;
        var actionResult = null;

        jQuery.ajax({
            url: url,
            success: function (result) {
                actionResult = result;
            },
            error: function (result) {
                actionResult = "nooooooooope";
            },
            async: false
        });

        if (actionResult == 'success')
        {
            Feeds.Load();
            Sources.Load();

        }
        else
            alert(actionResult);

    },
    CompareRoutine: function (a, b) {
        if (a.Name.toLowerCase() < b.Name.toLowerCase()) {
            return -1;
        }

        if (a.Name.toLowerCase() > b.Name.toLowerCase()) {
            return 1;
        }

        return 0;
    }
}

/*
** Placemark object
*/
var Placemarks = {
    PlacemarkData: null,
    CurEditingRow: null,
    Load: function () {

        var PlacemarkData;

        var url = "/Admin/GetPlacemarks";
        jQuery.ajax({
            url: url, success: function (target) {
                PlacemarkData = target;
            },
            async: false
        });

        if (PlacemarkData == null) {
            $('#placemarkList').hide();
            return;
        }
        Placemarks.PlacemarkData = PlacemarkData;
        $('#placemarkList tbody tr.source').remove();
        var appRow = $('#placemarkList tbody tr.model').clone();
        $('#placemarkList tbody').empty();
        $('#placemarkList tbody').append(appRow);
        $.each(PlacemarkData, function (ndx, placemark) {
            Placemarks.AddPlacemarkRow(placemark);
        });
    },
    AddPlacemarkRow: function (placemark) {
        $('#noPlacemark').hide();
        $('#placemarkList').show();
        var placemarkRow = $('#placemarkList tbody tr.model').clone();

        placemarkRow.find('.colAction.remove').click(Placemarks.Delete);
        placemarkRow.find('.colAction.edit').click(Placemarks.DisplayEditModal);
        placemarkRow.find('.colUser').html(placemark.screen_name);
        placemarkRow.find('.colName').html(placemark.Name);
        placemarkRow.find('.colComment').html(placemark.CommentCount);
        placemarkRow.find('.colRatings').html(placemark.RatingTotal + " (" + placemark.RatingCount + ")");

        var modifiedDate = new Date(parseInt(placemark.ModifiedOn.substr(6)));
        var modified = modifiedDate.toDateString() + "  " + ((modifiedDate.getHours() > 9) ? modifiedDate.getHours() : "0" + modifiedDate.getHours()) + ":" + ((modifiedDate.getMinutes() > 9) ? modifiedDate.getMinutes() : "0" + modifiedDate.getMinutes());
        placemarkRow.find('.colModified').html(modified);

        var ceatedDate = new Date(parseInt(placemark.CreatedOn.substr(6)));
        var created = ceatedDate.toDateString() + "  " + ((ceatedDate.getHours() > 9) ? ceatedDate.getHours() : "0" + ceatedDate.getHours()) + ":" + ((ceatedDate.getMinutes() > 9) ? ceatedDate.getMinutes() : "0" + ceatedDate.getMinutes());
        placemarkRow.find('.colCreated').html(created);
        placemarkRow.removeClass('model');
        placemarkRow.addClass('placemark');


        $('#placemarkList tbody').append(placemarkRow);
    },

    Edit: function () {
        var editModal = $(this).parents('#editPlacemarkModal');
        var placemark = {};

        var selector = document.getElementById('editFeedDataSource');

        placemark.LayerId = $(editModal).find('.placemarkLayerId').val();
        placemark.Name = $(editModal).find('.placemarkName').val();
        placemark.Description = $(editModal).find('.placemarkDescription').val();
        placemark.Latitude = $(editModal).find('.placemarkLatitude').val();
        placemark.Longitude = $(editModal).find('.placemarkLongitude').val();
        placemark.Tag = $(editModal).find('.placemarkTags').val();

        if (placemark.LayerId.length == 0 || placemark.Name.length == 0 || placemark.Latitude.length == 0 || placemark.Longitude.length == 0) {
            $('#editPlacemarkModal .alert-box').show();
            return;
        }

        Placemarks.Update(placemark);
        $('#editPlacemarkModal .alert-box').hide();
        $('#editPlacemarkModal input[type=text]').val('');
        $('#editPlacemarkModal').trigger('reveal:close');
    },
    DisplayEditModal: function () {
        Placemarks.CurEditingRow = $(this).parents('.placemark')[0];
        var i = Placemarks.CurEditingRow.rowIndex - 2;
        var data = Placemarks.PlacemarkData[i];

        $('#editPlacemarkModal').find('input.placemarkLayerId').val(data.LayerId);
        $('#editPlacemarkModal').find('input.placemarkName').val(data.Name);
        $('#editPlacemarkModal').find('input.placemarkDescription').val(data.Description);
        $('#editPlacemarkModal').find('input.placemarkLatitude').val(data.Latitude);
        $('#editPlacemarkModal').find('input.placemarkLongitude').val(data.Longitude);
        $('#editPlacemarkModal').find('input.placemarkTags').val(data.Tag);

        $('#editPlacemarkModal').reveal();
    },
    Update: function (placemark) {
        var i = Placemarks.CurEditingRow.rowIndex - 2;

        var url = "/Admin/UpdatePlacemark?";
        url += "id=" + Placemarks.PlacemarkData[i].Id;
        url += "&layerid=" + placemark.LayerId;
        url += "&name=" + placemark.Name;
        url += "&description=" + placemark.Description;
        url += "&latitude=" + placemark.Latitude;
        url += "&longitude=" + placemark.Longitude;
        url += "&tag=" + placemark.Tag;
        var actionResult = null;
        jQuery.ajax({
            url: url,
            success: function (result) {
                actionResult = result;
            },
            async: false
        });

        if (actionResult == 'success') {
            {
                Placemarks.Load();
                Comments.Load();
            }
        } else
            alert(actionResult);
    },

    Delete: function () {

        var rowToDelete = $(this).parents('.placemark')[0];
        var idx = rowToDelete.rowIndex - 2;

        var url = "/Admin/DeletePlacemark?";
        url += "id=" + Placemarks.PlacemarkData[idx].Id;
        var actionResult = null;

        jQuery.ajax({
            url: url,
            success: function (result) {
                actionResult = result;
            },
            error: function (result) {
                actionResult = "nooooooooope";
            },
            async: false
        });

        if (actionResult == 'success') {
            Placemarks.Load();
            Comments.Load();
        }
        else
            alert(actionResult);

    },
    CompareRoutine: function (a, b) {
        if (a.Name.toLowerCase() < b.Name.toLowerCase()) {
            return -1;
        }

        if (a.Name.toLowerCase() > b.Name.toLowerCase()) {
            return 1;
        }

        return 0;
    }
}

/*
** Comment object
*/
var Comments = {
    CommentData: null,
    Load: function () {

        var CommentData;

        var url = "/Admin/GetComments";
        jQuery.ajax({
            url: url, success: function (target) {
                CommentData = target;
            },
            async: false
        });

        if (CommentData == null) {
            $('#commentList').hide();
            return;
        }
        Comments.CommentData = CommentData;
        $('#commentList tbody tr.source').remove();
        var appRow = $('#commentList tbody tr.model').clone();
        $('#commentList tbody').empty();
        $('#commentList tbody').append(appRow);
        $.each(CommentData, function (ndx, comment) {
            Comments.AddCommentRow(comment);
        });
    },
    AddCommentRow: function (comment) {
        $('#noComment').hide();
        $('#commentList').show();
        var commentRow = $('#commentList tbody tr.model').clone();

        commentRow.find('.colAction.remove').click(Comments.Delete);
        //commentRow.find('.colAction.edit').click(Comments.DisplayEditModal);
        commentRow.find('.colUser').html(comment.screen_name);
        commentRow.find('.colPlacemark').html(comment.summary);
        commentRow.find('.colComment').html(comment.Text);

        var ceatedDate = new Date(parseInt(comment.CreatedOn.substr(6)));
        var created = ceatedDate.toDateString() + "  " + ((ceatedDate.getHours() > 9) ? ceatedDate.getHours() : "0" + ceatedDate.getHours()) + ":" + ((ceatedDate.getMinutes() > 9) ? ceatedDate.getMinutes() : "0" + ceatedDate.getMinutes());
        commentRow.find('.colCreated').html(created);

        commentRow.removeClass('model');
        commentRow.addClass('comment');


        $('#commentList tbody').append(commentRow);
    },

    Delete: function () {

        var rowToDelete = $(this).parents('.comment')[0];
        var idx = rowToDelete.rowIndex - 2;

        var url = "/Admin/DeleteComment?";
        url += "id=" + Comments.CommentData[idx].Id;
        var actionResult = null;

        jQuery.ajax({
            url: url,
            success: function (result) {
                actionResult = result;
            },
            error: function (result) {
                actionResult = "nooooooooope";
            },
            async: false
        });

        if (actionResult == 'success') {
            Placemarks.Load();
            Comments.Load();
        }
        else
            alert(actionResult);

    },
    CompareRoutine: function (a, b) {
        if (a.Name.toLowerCase() < b.Name.toLowerCase()) {
            return -1;
        }

        if (a.Name.toLowerCase() > b.Name.toLowerCase()) {
            return 1;
        }

        return 0;
    }
}
