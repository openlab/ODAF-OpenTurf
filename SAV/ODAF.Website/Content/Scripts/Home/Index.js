/*
** OnLoad
*/
$(function () {

    // Handle New Landmark modals
    $('#newlandmark').click(LANDMARK.Add);
    $('#validAddLandmark').click(LANDMARK.Save);
    $('#validCancelLandmark').click(LANDMARK.Cancel);
});


/*
** Document Loaded
*/
$(document).ready(function () {

    MapObject.Load();
    GetRegions();
    GetFeeds();
    LANDMARK.Load();
});


/*
** Data object
*/
var DATA = {
    MapZommLevel: 12,
    Regions: null,
    Feeds: null,
    SelectedFeed: -1,
    FeedCollectionTab: [],
    SharedLandmarks: null
}

/*
** User object
*/
var USER = {
    Logged: false,
    Id: null,
    SocialId: null,
    ScreeName: null,
}


/*
** Map object
*/
var MapObject = {
    Loaded: false,
    map: null,
    Load: function () {
        var tmp = document.getElementById("bmc");
        MapObject.map = new Microsoft.Maps.Map(document.getElementById("myMap"),
         {
             credentials: tmp.textContent,
             zoom: DATA.MapZommLevel,
             center: new Microsoft.Maps.Location(48.85952, 2.352421),
             mapTypeId: Microsoft.Maps.MapTypeId.aerial,
             labelOverlay: Microsoft.Maps.LabelOverlay.hidden
         });
        MapObject.Loaded = true;
        INFOBOX.View = new Microsoft.Maps.Infobox(new Microsoft.Maps.Location(0, 0), { visible: false, offset: new Microsoft.Maps.Point(0, 20) });
        INFOBOX.Layer = new Microsoft.Maps.EntityCollection();
    },
    UpdateCenter: function () {
        var ridx = parseInt($('#regionDropdown').val());
        var locationtab = DATA.Regions[ridx].BoundaryPolygon.replace("\"latitude\":", "").replace("\"longitude\":", "").split(",");
        MapObject.map.setView({ zoom: DATA.MapZommLevel, center: new Microsoft.Maps.Location(locationtab[0], locationtab[1]) })
    },
}



var INFOBOX = {
    StringInfo: "",
    StringTag: "",
    StringComments: "",
    StringRating: "",
    StringDelete: "",
    StringAddTag: "",
    StringAddComment: "",
    StringAddRating: "",
    StringAlreadyRated: "",
    StringSignInToRate: "",
    View: null,
    Layer: null,
    Data: null,
    Rated: false,

    GetContent: function (data) {
        if (data == null) {
            INFOBOX.Data = null;
            return null;
        }
        INFOBOX.Data = data;

        var content = "<div id='custominfobox'>";
        content += "<span id='customInfoBoxCloseBt'" + "onclick='INFOBOX.Close()'" + ">x</span>";
        content += "<ul id='custominfoboxhead'><li class='infoboxtitles'><a href='javascript:;'>" + INFOBOX.StringInfo + "</a></li><li class='infoboxtitles'><a href='javascript:;'>" + INFOBOX.StringTag + "</a></li><li class='infoboxtitles'><a href='javascript:;'>" + INFOBOX.StringComments + "</a></li><li class='infoboxtitles'><a href='javascript:;'>" + INFOBOX.StringRating + "</a></li></ul>"
        content += "<p id='landmarktitle'>" + data.summary.Name + "</p>";

        content += "<div id='custominfoboxcontent'>";
        content += "<div id='tab1' class='tab'><span id='landmarkdescription'>" + data.summary.Description + "</span>";
        //content += "<div class='custominfoboxbt' ><a href='#'>Edit</a></div>";
        if (USER.Logged == "True" && data.summary.CreatedById == USER.Id) {
            content += "<div class='custominfoboxbt' onClick='INFOBOX.DeleteSummary()' ><a href='#'>" + INFOBOX.StringDelete + "</a></div>";
        }
        content += "</div>";

        content += "<div id='tab2' class='tab'><span id='landmarktags'>" + ((data.summary.Tag != null) ? data.summary.Tag : "") + "</span>";
        if (USER.Logged == "True") {

            content += "<input name='tag' id='landmarktagbox'  onclick='this.select()' />  <div class='custominfoboxbt' " + "onclick='INFOBOX.AddTag()'" + "><a href='#'>" + INFOBOX.StringAddTag + "</a></div>";
        }
        content += "</div>";
        content += "<div id='tab3' class='tab'>";

        if (USER.Logged == "True") {
            content += "<input name='comment' id='landmarkcommentbox'  onclick='this.select()' />";
            content += "<div class='custominfoboxbt' " + "onclick='INFOBOX.AddComment()'" + "><a href='#' >" + INFOBOX.StringAddComment + "</a></div>";
        }

        content += "<ul id='commentlist'>";
        $.each(data.comments, function (ndx, comment) {
            content += "<li><span class='commenttext'>" + comment.Text + "</span><div><span class='commentuser'>" + comment.User + "</span><span class='commenttime'>" + comment.Date + "</span></div></li>";
        });
        content += "</ul></div>";



        content += "<div id='tab4' class='tab'>";
        content += "<div id='ratingcontrol'>";
        content += "<!----><a id='start5' href='#' onClick='javascript:INFOBOX.AddRating(5)' title='Give 5 stars'>★</a>";
        content += "<!----><a id='start4' href='#' onClick='javascript:INFOBOX.AddRating(4)' title='Give 4 stars'>★</a>";
        content += "<!----><a id='start3' href='#' onClick='javascript:INFOBOX.AddRating(3)' title='Give 3 stars'>★</a>";
        content += "<!----><a id='start2' href='#' onClick='javascript:INFOBOX.AddRating(2)' title='Give 2 stars'>★</a>";
        content += "<!----><a id='start1' href='#' onClick='javascript:INFOBOX.AddRating(1)' title='Give 1 star'>★</a>";
        content += "</div>";

        content += "<span id='landmarkdratingcontroltext'>" + INFOBOX.StringAddRating + "</span></div>";


        content += "</div></div>";

        return content;
    },
    Show: function (e) {
        if (e.targetType == 'pushpin') {
            INFOBOX.Rated = false;
            var loc = e.target.getLocation();
            INFOBOX.View.setLocation(loc);

            var region_index = $('#regionDropdown').val();

            var url = "/Home/GetSummaryByGuid?guid=" + e.target.Description
                + "&name=" + e.target.Title + "&latitude=" + loc.latitude + "&longitude=" + loc.longitude
                + "&createdby=" + USER.Id + "&Region_Index=" + region_index + '&Feed_Index=' + DATA.SelectedFeed;

            var data = "";
            jQuery.ajax({
                url: url, success: function (target) {
                    data = target;
                }, async: false
            });

            INFOBOX.View.setOptions({
                visible: true,
                //title: e.target.Title,
                //description: e.target.Description
                htmlContent: INFOBOX.GetContent(data)
            });
            MapObject.map.entities.remove(INFOBOX.Layer);
            MapObject.map.entities.push(INFOBOX.Layer);
            INFOBOX.Layer.push(INFOBOX.View);

            $(".infoboxtitles li:first").addClass("active");
            $(".tab:not(:first)").hide();
            $(".infoboxtitles").click(function () {
                var index = $(this).index();
                $(".infoboxtitles").removeClass("active");
                $(this).addClass("active");
                $(".tab").hide();
                $(".tab").eq(index).finish().fadeIn();
            });
        }
    },
    AddTag: function () {
        var tag = $('#landmarktagbox').val()
        if (tag.length > 0) {

            var url = "/Home/AddTagToSummary?guid=" + INFOBOX.Data.summary.Guid + "&name=" + tag;

            var data = "";
            jQuery.ajax({
                url: url, success: function (target) {
                    data = target;
                }, async: false
            });

            document.getElementById("landmarktags").innerHTML = data;
            $('#landmarktagbox').val("");

        }
    },
    AddComment: function () {
        var comment = $('#landmarkcommentbox').val()
        if (comment.length > 0) {

            var url = "/Home/AddCommentToSummary?Text=" + comment + "&CreatedBy=" + USER.Id + "&SummaryId=" + INFOBOX.Data.summary.Id;

            var data = "";
            jQuery.ajax({
                url: url, success: function (target) {
                    data = target;
                }, async: false
            });


            var li = document.createElement('li');

            li.innerHTML = "<li><span class='commenttext'>" + data.Text + "</span><div><span class='commentuser'>" + USER.ScreeName + "</span><span class='commenttime'>" + data.Date + "</span></div></li>";



            document.getElementById("commentlist").appendChild(li);
            $('#landmarkcommentbox').val("");

        }
    },
    AddRating: function (rate) {
        if (USER.Logged != "True") {
            document.getElementById("landmarkdratingcontroltext").innerHTML = INFOBOX.StringSignInToRate;

            return;
        }
        if (INFOBOX.Rated == true) {

            document.getElementById("landmarkdratingcontroltext").innerHTML = INFOBOX.StringAlreadyRated;
        } else {

            var url = "/Home/AddRateToSummary?guid=" + INFOBOX.Data.summary.Guid + "&rate=" + rate;

            var data = "";
            jQuery.ajax({
                url: url, success: function (target) {
                    data = target;
                }, async: false
            });

            for (var i = 1; i <= rate; i++) {
                $("#start" + i).css({
                    color: "#00a6de"
                });
            }

            document.getElementById("landmarkdratingcontroltext").innerHTML = data;
            INFOBOX.Rated = true;
        }
    },
    DeleteSummary: function () {

        var url = "/Home/DeleteSummary?guid=" + INFOBOX.Data.summary.Guid + "&userId=" + USER.Id;

        var data = "";
        jQuery.ajax({
            url: url, success: function (target) {
                data = target;
            }, async: false
        });

        INFOBOX.Close();
        //GetFeeds();
        LANDMARK.Load();
    },
    Close: function () {
        MapObject.map.entities.remove(INFOBOX.Layer);
    },
}



/*
** Get and save the list of regions available
*/
function GetRegions() {
    var url = "/Home/Regions";

    $.ajax({
        url: url,
        success: function (target) {
            DATA.Regions = target;
        },
        type: "POST",
        async: false
    });
}

/*
** Get the list of feeds related to the selected regions
** then load the checkbox list clear the map and center the map
*/
function GetFeeds() {
    var region_index = parseInt($('#regionDropdown').val());
    var url = "/Home/Feeds?Region_Index=" + region_index;

    $.post(url, function (data) {
        DATA.Feeds = data;
        var list = $('#checklist');
        list.empty();

        for (var i = 0; i < DATA.Feeds.length; i++) {
            // Create the list item:
            var item = document.createElement('li');
            var checker = document.createElement("input");
            checker.setAttribute('id', 'checkbox' + i);
            checker.setAttribute('type', 'checkbox');
            checker.setAttribute('value', i);
            checker.setAttribute('onclick', 'GetPoints(this)');
            // Set its contents:
            item.appendChild(checker);
            item.appendChild(document.createTextNode(DATA.Feeds[i].Title));
            // Add it to the list:
            list.append(item);
            list.append('<br/>');
        }
        $.each(DATA.FeedCollectionTab, function (ndx, feed) {
            MapObject.map.entities.remove(feed);
        });


        DATA.FeedCollectionTab = [];

        MapObject.UpdateCenter();
    });
}


function GetPoints(checkbox) {
    var region_index = $('#regionDropdown').val();
    var feed_index = checkbox.value;
    DATA.SelectedFeed = feed_index;
    var url = "/Home/GetPointsFeedUrl?Region_Index=" + region_index + '&Feed_Index=' + feed_index;
    MapObject.map.entities.remove(INFOBOX.Layer);
    if (!checkbox.checked) {
        MapObject.map.entities.remove(DATA.FeedCollectionTab[DATA.SelectedFeed]);
        DATA.SelectedFeed = -1;
    }
    else {
        var datasource = "";
        jQuery.ajax({
            url: url, success: function (target) {
                datasource = target;
            }, async: false
        });

        $.getJSON(datasource + '&callback=?', null)
            .done(function (data) {
                if (DATA.SelectedFeed >= 0) {
                    var pinLayer = new Microsoft.Maps.EntityCollection();
                    var pushpinOptions = { icon: DATA.Feeds[DATA.SelectedFeed].ImageUrl };

                    for (var i = 0; i < data.d.length; i++) {
                        var loc = new Microsoft.Maps.Location(data.d[i].latitude, data.d[i].longitude);
                        var pushpin = new Microsoft.Maps.Pushpin(loc, pushpinOptions);
                        if (data.d[i].libelle != null)
                            pushpin.Title = data.d[i].libelle;
                        else if (data.d[i].raison_sociale != null)
                            pushpin.Title = data.d[i].raison_sociale;
                        else
                            pushpin.Title = data.d[i].entityid;
                        pushpin.Description = data.d[i].entityid;
                        Microsoft.Maps.Events.addHandler(pushpin, 'click', INFOBOX.Show);
                        pinLayer.push(pushpin);
                    };
                    DATA.FeedCollectionTab[DATA.SelectedFeed] = pinLayer;
                    MapObject.map.entities.push(DATA.FeedCollectionTab[DATA.SelectedFeed]);
                }
            });
    }
}


/*
** Landmark object
*/
var LANDMARK = {
    CurrentPin: null,
    Load: function () {
        var showlandmarks = false;
        var showuserlandmarks = false;

        if (document.getElementById('SharedLandmarks').checked == true)
            showlandmarks = true;
        if (document.getElementById('YourLandmarks') != null && document.getElementById('YourLandmarks').checked == true)
            showuserlandmarks = true;

        var url = "/Home/GetSharedLandmarks?showlandmarks=" + showlandmarks + "&showuserlandmarks=" + showuserlandmarks + "&userid=" + USER.SocialId;
        MapObject.map.entities.remove(INFOBOX.Layer);

        jQuery.ajax({
            url: url, success: function (data) {
                var pinLayer = new Microsoft.Maps.EntityCollection();
                MapObject.map.entities.remove(DATA.SharedLandmarks);

                for (var i = 0; i < data.length; i++) {
                    //Microsoft.Maps.Events.addHandler(pushpin, 'click', INFOBOX.Show);
                    var pushpinOptions = { htmlContent: "<div  style=' pointer-events: all; padding: 1px;background: #fff;border-radius: 200px;'><img  style='border-radius: 200px;'src='" + data[i].Avatar + "'></div>" };
                    //var pushpinOptions = { htmlContent: "<div class='row'><div class='three columns'><dl class='vertical tabs'><dd class='active'><a href='#adminMenu1'>menu1</a></dd><dd><a href='#adminMenu2'>menu2</a></dd><dd ><a href='#adminMenu3'>menu3</a></dd><dd><a href='#adminMenu4'>menu4</a></dd><dd ><a href='#adminMenu5'>menu5</a></dd></dl><input type='submit' id='submit' class='secondary success radius button' value='Sauvegarder' /></div><div class='nine columns'><ul class='tabs-content'><li class='active' id='adminMenu1Tab'></li><li id='adminMenu2Tab'></li><li  id='adminMenu3Tab'></li><li id='adminMenu4Tab'></li><li id='adminMenu5Tab'></li></ul></div></div>" };

                    var loc = new Microsoft.Maps.Location(data[i].Summary.Latitude, data[i].Summary.Longitude);
                    var pushpin = new Microsoft.Maps.Pushpin(loc, pushpinOptions);
                    pushpin.Title = data[i].Summary.Name + "";
                    pushpin.Description = data[i].Summary.Guid;
                    Microsoft.Maps.Events.addHandler(pushpin, 'click', INFOBOX.Show);
                    pinLayer.push(pushpin);
                };
                DATA.SharedLandmarks = pinLayer;
                MapObject.map.entities.push(DATA.SharedLandmarks);
            }, async: false
        });
    },
    Add: function () {
        if (LANDMARK.CurrentPin != null)
            return;
        var pushpinOptions = { draggable: true };
        LANDMARK.CurrentPin = new Microsoft.Maps.Pushpin(MapObject.map.getCenter(), pushpinOptions);
        Microsoft.Maps.Events.addHandler(LANDMARK.CurrentPin, 'click', LANDMARK.ShowForm);
        MapObject.map.entities.push(LANDMARK.CurrentPin);
    },
    ShowForm: function () {
        $("#addLandmarkModal").reveal();
    },
    Save: function () {

        var addModal = $(this).parents('#addLandmarkModal');
        var landmark = {};

        landmark.Name = $(addModal).find('.landmarkTitle').val();
        landmark.Description = $(addModal).find('.landmarkDescription').val();

        if (landmark.Name.length < 8) {
            $('#addLandmarkModal .alert-box').show();
            return;
        }

        LANDMARK.Create(landmark);
        $('#addLandmarkModal .alert-box').hide();
        $('#addLandmarkModal input[type=text]').val('');
        $('#addLandmarkModal').trigger('reveal:close');
    },
    Create: function (landmark) {
        var url = "/Home/CreateLandmark?";
        var loc = LANDMARK.CurrentPin.getLocation();
        url += "name=" + landmark.Name;
        url += "&description=" + landmark.Description;
        url += "&layerid=" + USER.SocialId;
        url += "&latitude=" + loc.latitude;
        url += "&longitude=" + loc.longitude;
        url += "&createdby=" + USER.Id;
        var actionResult = null;
        jQuery.ajax({
            url: url,
            success: function (result) {
                actionResult = result;
            },
            async: false
        });

        if (actionResult == 'success')
            LANDMARK.Load();
        MapObject.map.entities.remove(LANDMARK.CurrentPin);
        LANDMARK.CurrentPin = null;

    },
    Cancel: function () {
        MapObject.map.entities.remove(LANDMARK.CurrentPin);
        LANDMARK.CurrentPin = null;
        $("#addLandmarkModal").trigger('reveal:close');
    }
}
